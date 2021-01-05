using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    internal class OggPageWriter
    {
        private readonly int _serial;
        private readonly Stream _stream;

        private ulong _granulePosition;
        private int _page;

        public OggPageWriter(Stream stream, int? serial = null)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
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

            using var oggPageStream = new MemoryStream();
            using var oggPageWriter = new BinaryWriter(oggPageStream);

            oggHeader.WriteToStream(oggPageWriter); // TODO ext
            oggPageWriter.Write(data);
            oggPageWriter.Flush();

            //var oggPageBytes = oggPageStream.ToArray();

            oggHeader.Checksum = OggCRC32.CalculateCRC(0, oggPageStream.ToArray());

            var oggPageWriterFinal = new BinaryWriter(_stream);
            oggHeader.WriteToStream(oggPageWriterFinal);
            oggPageWriterFinal.Write(data);
            oggPageWriterFinal.Flush();

            //oggPageWriterFinal.Write(data);
            //oggPageWriterFinal.Flush();

            //_stream.Write(data);

            _page++;
        }
    }
}
