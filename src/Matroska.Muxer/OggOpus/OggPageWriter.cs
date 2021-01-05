using System.Collections.Generic;
using System.IO;
using System.Linq;
using Matroska.Muxer.Extensions;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Ogg_page
    /// </summary>
    internal class OggPageWriter
    {
        private readonly int _serial;
        private readonly BinaryWriter _writer;

        private ulong _granulePosition;
        private int _page;

        public OggPageWriter(Stream stream, int? serial = null)
        {
            _writer = new BinaryWriter(stream);
            _serial = serial ?? -1071269784;
        }

        public void WriteOggPage(OggHeaderType oggHeaderType, byte numberOfSegments, List<SegmentEntry> oggPages)
        {
            var data = oggPages.SelectMany(o => o.Data).ToArray();

            _granulePosition += (ulong)oggPages.Sum(op => op.NumberOfSamples * op.NumberOfFrames);

            var oggHeader = new OggHeader
            {
                StreamVersion = 0,
                TypeFlag = oggHeaderType,
                GranulePosition = _granulePosition,
                Serial = _serial,
                PageNumber = _page,
                Checksum = 0,
                TotalSegments = numberOfSegments,
                SegmentTable = oggPages.SelectMany(o => o.SegmentBytes).ToArray()
            };

            oggHeader.Checksum = CalculateCheckSum(oggHeader, data);

            _writer.Write(oggHeader);
            _writer.Write(data);
            _writer.Flush();

            _page++;
        }

        private static uint CalculateCheckSum(OggHeader oggHeader, byte[] data)
        {
            using var oggPageStream = new MemoryStream();
            using var oggPageWriter = new BinaryWriter(oggPageStream);
            oggPageWriter.Write(oggHeader);
            oggPageWriter.Write(data);
            oggPageWriter.Flush();

            return OggCRC32.CalculateCRC(0, oggPageStream.ToArray());
        }
    }
}