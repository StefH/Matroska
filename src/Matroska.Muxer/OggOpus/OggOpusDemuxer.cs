using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Matroska.Models;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    public class OggOpusDemuxer
    {
        private const byte MaxSegmentParts = 0x64;

        private readonly MatroskaDocument _doc;
        private readonly int _sampleRate;
        private readonly int _channels;

        public OggOpusDemuxer(MatroskaDocument doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));

            if (doc?.Segment?.Tracks?.TrackEntry?.Audio?.SamplingFrequency == null)
            {
                throw new ArgumentException("SamplingFrequency");
            }

            _sampleRate = (int)doc.Segment.Tracks.TrackEntry.Audio.SamplingFrequency;
            _channels = (int)doc.Segment.Tracks.TrackEntry.Audio.Channels;
        }

        public void CopyTo(Stream outputStream)
        {
            if (outputStream == null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (_doc?.Segment?.Clusters == null)
            {
                throw new ArgumentException();
            }

            var oggPageWriter = new OggPageWriter(outputStream);
            var oggOpusHeaderWriter = new OggOpusHeaderWriter(oggPageWriter);

            // Write OpusHeader (+ OpusTags)
            oggOpusHeaderWriter.WriteHeaders(_channels, _sampleRate);



            var oggHeaderType = OggHeaderType.None;
            byte segmentParts = 0;
            var oggPageWithSegments = new List<SegmentEntry>();

            void WriteOggPageAndResetParts(OggHeaderType oggHeaderType)
            {
                oggPageWriter.WriteOggPage(oggHeaderType, segmentParts, oggPageWithSegments);

                segmentParts = 0;
                oggPageWithSegments.Clear();
            }

            foreach (var oggSegmentTable in _doc.Segment.Clusters.Select(ConvertClusterToSegmentTable))
            {
                foreach (var oggSegmentEntry in oggSegmentTable)
                {
                    oggPageWithSegments.Add(oggSegmentEntry);
                    segmentParts = (byte)(segmentParts + oggSegmentEntry.SegmentBytes.Length);

                    if (segmentParts >= MaxSegmentParts)
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

                int len = block.Data.Length;

                byte[] segmentTable;
                if (len == 0)
                {
                    segmentTable = new byte[] { 255 };
                }
                else if (block.Data.Length < 255)
                {
                    segmentTable = new byte[] { (byte)len };
                }
                else if (block.Data.Length < 2 * 255)
                {
                    segmentTable = new byte[] { 255, (byte)(block.Data.Length - 255) };
                }
                else
                {
                    int numberOfSegmentTableBytes = len / 255;
                    segmentTable = new byte[1 + numberOfSegmentTableBytes];
                    segmentTable.AsSpan().Fill(255);
                    segmentTable[numberOfSegmentTableBytes] = (byte)(len - (numberOfSegmentTableBytes * 255));
                }

                list.Add(new SegmentEntry
                {
                    SegmentBytes = segmentTable,
                    Data = block.Data,
                    TimeCode = block.TimeCode,
                    NumberOfSamples = OpusPacketInfoParser.GetNumSamples(block.Data, 0, len, _sampleRate),
                    NumberOfFrames = OpusPacketInfoParser.GetNumFrames(block.Data, 0, len)
                });
            }

            return list;
        }
    }
}
