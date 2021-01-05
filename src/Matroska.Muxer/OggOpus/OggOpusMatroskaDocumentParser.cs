using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Matroska.Models;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    public class OggOpusMatroskaDocumentParser
    {
        private readonly MatroskaDocument _doc;
        private readonly Stream _outputStream;
        private readonly OggPageWriter _oggPageWriter;
        private readonly OggOpusHeaderWriter _oggOpusHeaderWriter;
        private readonly int _sampleRate;

        public OggOpusMatroskaDocumentParser(MatroskaDocument doc, Stream outputStream)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
            _outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));

            _oggPageWriter = new OggPageWriter(outputStream);
            _oggOpusHeaderWriter = new OggOpusHeaderWriter(_oggPageWriter);

            if (doc?.Segment?.Tracks?.TrackEntry?.Audio?.SamplingFrequency == null)
            {
                throw new ArgumentException();
            }


            _sampleRate = (int)doc.Segment.Tracks.TrackEntry.Audio.SamplingFrequency;
        }

        public void Parse()
        {
            if (_doc?.Segment?.Clusters == null)
            {
                throw new ArgumentException();
            }

            _oggOpusHeaderWriter.WriteHeaders();

            var oggHeaderType = OggHeaderType.None;

            var oggPageWithSegments = new List<SegmentEntry>();
            byte segmentParts = 0;

            foreach (var oggSegmentTable in _doc.Segment.Clusters.Select(ClusterToOggOpusSegmentTable))
            {
                foreach (var oggSegmentEntry in oggSegmentTable)
                {
                    oggPageWithSegments.Add(oggSegmentEntry);
                    segmentParts = (byte)(segmentParts + oggSegmentEntry.SegmentBytes.Length);

                    if (segmentParts >= 0x64)
                    {
                        _oggPageWriter.WriteOggPage(oggHeaderType, segmentParts, oggPageWithSegments);

                        segmentParts = 0;
                        oggPageWithSegments.Clear();
                    }
                }

                if (segmentParts > 0)
                {
                    _oggPageWriter.WriteOggPage(oggHeaderType, segmentParts, oggPageWithSegments);

                    segmentParts = 0;
                    oggPageWithSegments.Clear();
                }
            }
        }

        private List<SegmentEntry> ClusterToOggOpusSegmentTable(Cluster cluster)
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
