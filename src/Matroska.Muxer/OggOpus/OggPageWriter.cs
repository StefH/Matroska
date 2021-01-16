using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public OggPageWriter(Stream stream, int serial)
        {
            _writer = new BinaryWriter(stream);
            _serial = serial;
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

            CalculateChecksumAndWriteOggHeaderToBinaryWriter(_writer, oggHeader, data);

            _page++;
        }

        public void Flush()
        {
            _writer.Flush();
        }

        private static void CalculateChecksumAndWriteOggHeaderToBinaryWriter(BinaryWriter writer, OggHeader oggHeader, byte[] data)
        {
            var oggHeaderBytes = ArrayPool<byte>.Shared.Rent(oggHeader.Size);
            try
            {
                var spanWriter = new SpanWriter(oggHeaderBytes);
                spanWriter.WriteOggHeader(oggHeader);

                var checkSum = OggCRC32.CalculateCRC(0, oggHeaderBytes.AsSpan().Slice(0, oggHeader.Size), data);
                spanWriter.Write(checkSum, OggHeader.CheckSumLocation);

                writer.Write(oggHeaderBytes, 0, oggHeader.Size);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(oggHeaderBytes);
            }

            writer.Write(data);
        }
    }
}