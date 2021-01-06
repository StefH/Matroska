using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentValidation;
using Matroska.Models;
using Matroska.Muxer.Extensions;
using Matroska.Muxer.OggOpus.Models;
using Matroska.Muxer.OggOpus.Settings;

namespace Matroska.Muxer.OggOpus
{
    internal static class OggOpusAudioStreamDemuxer
    {
        public static void CopyTo(MatroskaDocument doc, Stream outputStream, OggOpusAudioStreamDemuxerSettings? settings = null)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            settings ??= new OggOpusAudioStreamDemuxerSettings();

            var validator = new OggOpusValidator();
            validator.ValidateAndThrow((settings, doc));

            var opusAudio = doc.Segment.Tracks.TrackEntries.First(t => t.TrackNumber == settings.AudioTrackNumber);
            ushort preSkip = GetPreSkipFromCodecPrivate(opusAudio);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var sampleRate = (int)opusAudio.Audio.SamplingFrequency;
            var channels = (int)opusAudio.Audio.Channels;
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var oggPageWriter = new OggPageWriter(outputStream, settings.AudioStreamSerial);
            var oggOpusHeaderWriter = new OggOpusHeaderWriter(oggPageWriter);

            // Write OpusHeader (+ OpusTags)
            oggOpusHeaderWriter.WriteHeaders(channels, sampleRate, preSkip);

            // Loop OggSegments
            byte segmentParts = 0;
            var oggPageWithSegments = new List<SegmentEntry>();

            void WriteOggPageAndResetParts(OggHeaderType oggHeaderType)
            {
                // Write Ogg Page
                oggPageWriter.WriteOggPage(oggHeaderType, segmentParts, oggPageWithSegments);

                // Reset data
                segmentParts = 0;
                oggPageWithSegments.Clear();
            }

            // Loop SegmentEntries
            foreach (var oggSegmentEntry in ConvertClustersToSegmentTable(doc.Segment.Clusters, sampleRate, settings.AudioTrackNumber))
            {
                oggPageWithSegments.Add(oggSegmentEntry);
                segmentParts = (byte)(segmentParts + oggSegmentEntry.SegmentBytes.Length);

                if (segmentParts >= settings.MaxSegmentPartsPerOggPage)
                {
                    WriteOggPageAndResetParts(oggPageWithSegments.Any(o => o.IsLast) ? OggHeaderType.EndOfStream : OggHeaderType.None);
                }
            }

            if (segmentParts > 0)
            {
                WriteOggPageAndResetParts(OggHeaderType.EndOfStream);
            }
        }

        private static List<SegmentEntry> ConvertClustersToSegmentTable(List<Cluster> clusters, int sampleRate, ulong trackNumber)
        {
            var simpleBlocks = clusters.SelectMany(c => c.SimpleBlocks.Where(sb => sb?.TrackNumber == trackNumber)).Where(sb => sb != null);

            var segmentEntries = ConvertSimpleBlocksToSegmentEntries(simpleBlocks, sampleRate);

            if (segmentEntries.Count > 0)
            {
                var last = segmentEntries.Last();
                last.IsLast = true;
            }

            return segmentEntries;
        }

        private static List<SegmentEntry> ConvertSimpleBlocksToSegmentEntries(IEnumerable<SimpleBlock> blocks, int sampleRate)
        {
            var list = new List<SegmentEntry>();

            if (blocks == null)
            {
                return list;
            }

            foreach (var block in blocks)
            {
                if (block.Data == null)
                {
                    continue;
                }

                int dataLength = block.Data.Length;

                list.Add(new SegmentEntry
                {
                    SegmentBytes = GetSegmentBytes(dataLength),
                    Data = block.Data,
                    TimeCode = block.TimeCode,
                    NumberOfSamples = OpusPacketInfoParser.GetNumSamples(block.Data, 0, dataLength, sampleRate),
                    NumberOfFrames = OpusPacketInfoParser.GetNumFrames(block.Data, 0, dataLength)
                });
            }

            return list;
        }

        /// <summary>
        /// The segment table is an array of 8-bit values, each indicating the length of the corresponding segment within the page body. The number of segments is determined from the preceding Page Segments field. Each segment is between 0 and 255 bytes in length.
        /// The segments provide a way to group segments into packets, which are meaningful units of data for the decoder.When the segment's length is indicated to be 255, this indicates that the following segment is to be concatenated to this one and is part of the same packet. When the segment's length is 0–254, this indicates that this segment is the final segment in this packet.Where a packet's length is a multiple of 255, the final segment is length 0.
        /// Where the final packet continues on the next page, the final segment value is 255, and the continuation flag is set on the following page to indicate that the start of the new page is a continuation of last page.
        /// </summary>
        private static byte[] GetSegmentBytes(int dataSegmentLength)
        {
            if (dataSegmentLength == 0)
            {
                return new byte[] { 255 };
            }

            if (dataSegmentLength < 255)
            {
                return new byte[] { (byte)dataSegmentLength };
            }

            if (dataSegmentLength < 2 * 255)
            {
                return new byte[] { 255, (byte)(dataSegmentLength - 255) };
            }

            if (dataSegmentLength < 3 * 255)
            {
                return new byte[] { 255, 255, (byte)(dataSegmentLength - 2 * 255) };
            }

            int numberOfSegmentTableBytes = dataSegmentLength / 255;
            var segmentTable = new byte[1 + numberOfSegmentTableBytes];
            segmentTable.AsSpan().Fill(255);
            segmentTable[numberOfSegmentTableBytes] = (byte)(dataSegmentLength - (numberOfSegmentTableBytes * 255));

            return segmentTable;
        }

        private static ushort GetPreSkipFromCodecPrivate(TrackEntry opusAudio)
        {
            try
            {
                using var br = new BinaryReader(new MemoryStream(opusAudio.CodecPrivate));
                var opusHead = br.ReadOpusHead();

                new OggOpusOpusHeadValidator().ValidateAndThrow(opusHead);
                return br.ReadOpusHead().PreSkip;
            }
            catch
            {
                return 0;
            }
        }
    }
}