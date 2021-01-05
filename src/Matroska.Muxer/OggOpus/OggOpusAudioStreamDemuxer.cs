using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Matroska.Models;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    public class OggOpusAudioStreamDemuxer
    {
        private const byte MaxSegmentParts = 0x64;

        private readonly MatroskaDocument _doc;
        private readonly int _sampleRate;
        private readonly int _channels;

        public OggOpusAudioStreamDemuxer(MatroskaDocument doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));

            if (doc?.Segment?.Tracks?.TrackEntry?.Audio == null)
            {
                throw new ArgumentException("Audio");
            }

            _sampleRate = (int)doc.Segment.Tracks.TrackEntry.Audio.SamplingFrequency;
            _channels = (int)doc.Segment.Tracks.TrackEntry.Audio.Channels;
        }

        public void CopyTo(Stream outputStream, byte MaxSegmentPartsPerOggPage = MaxSegmentParts)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (_doc.Segment == null)
            {
                throw new ArgumentNullException("Segment");
            }

            if (_doc.Segment.Clusters == null)
            {
                throw new ArgumentNullException("Segment.Clusters");
            }

            var oggPageWriter = new OggPageWriter(outputStream);
            var oggOpusHeaderWriter = new OggOpusHeaderWriter(oggPageWriter);

            // Write OpusHeader (+ OpusTags)
            oggOpusHeaderWriter.WriteHeaders(_channels, _sampleRate);

            // Loop OggSegments
            var oggHeaderType = OggHeaderType.None;
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

            // Loop
            foreach (var oggSegmentTable in _doc.Segment.Clusters.Select(ConvertClusterToSegmentTable))
            {
                foreach (var oggSegmentEntry in oggSegmentTable)
                {
                    oggPageWithSegments.Add(oggSegmentEntry);
                    segmentParts = (byte)(segmentParts + oggSegmentEntry.SegmentBytes.Length);

                    if (segmentParts >= MaxSegmentPartsPerOggPage)
                    {
                        WriteOggPageAndResetParts(oggHeaderType);
                    }
                }

                if (segmentParts > 0)
                {
                    WriteOggPageAndResetParts(oggHeaderType);
                }
            }
        }

        private List<SegmentEntry> ConvertClusterToSegmentTable(Cluster cluster)
        {
            var list = new List<SegmentEntry>();

            if (cluster.SimpleBlocks == null)
            {
                return list;
            }

            foreach (var block in cluster.SimpleBlocks)
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
                    NumberOfSamples = OpusPacketInfoParser.GetNumSamples(block.Data, 0, dataLength, _sampleRate),
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

            int numberOfSegmentTableBytes = dataSegmentLength / 255;
            var segmentTable = new byte[1 + numberOfSegmentTableBytes];
            segmentTable.AsSpan().Fill(255);
            segmentTable[numberOfSegmentTableBytes] = (byte)(dataSegmentLength - (numberOfSegmentTableBytes * 255));

            return segmentTable;
        }
    }
}