﻿using System;
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

            _writer.Write(CalculateChecksumAndGetOggHeaderBytes(oggHeader, data));
            _writer.Write(data);
            _writer.Flush();

            _page++;
        }

        private static byte[] CalculateChecksumAndGetOggHeaderBytes(OggHeader oggHeader, byte[] data)
        {
            Span<byte> span = stackalloc byte[oggHeader.Size];

            var spanWriter = new SpanWriter(span);
            spanWriter.WriteOggHeader(oggHeader);

            var checkSum = OggCRC32.CalculateCRC(0, spanWriter.Span, data);

            spanWriter.Position = OggHeader.CheckSumLocation;
            spanWriter.Write(checkSum);

            return spanWriter.Span.ToArray();
        }
    }
}