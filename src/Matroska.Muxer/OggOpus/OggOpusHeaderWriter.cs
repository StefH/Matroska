using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Matroska.Muxer.Extensions;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    internal class OggOpusHeaderWriter
    {
        private readonly OggPageWriter _oggPageWriter;

        public OggOpusHeaderWriter(OggPageWriter oggPageWriter)
        {
            _oggPageWriter = oggPageWriter;
        }

        public void WriteHeaders(int channels, int sampleRate, ushort preSkip)
        {
            Span<byte> span = stackalloc byte[Unsafe.SizeOf<OpusHead>()];
            var opusHeadSpanWriter = new SpanWriter(span);
            var opusHead = new OpusHead
            {
                Version = 1,
                OutputChannelCount = (byte)channels,
                PreSkip = preSkip,
                InputSampleRate = (uint)sampleRate,
                OutputGain = 0,
                ChannelMappingFamily = 0
            };
            opusHeadSpanWriter.Write(opusHead);
            WriteOggPage(OggHeaderType.BeginningOfStream, span.ToArray());

            //using var opusHeadStream = new MemoryStream();
            //using var opusHeadWriter = new BinaryWriter(opusHeadStream);
            //var opusHead = new OpusHead
            //{
            //    Version = 1,
            //    OutputChannelCount = (byte) channels,
            //    PreSkip = preSkip,
            //    InputSampleRate = (uint) sampleRate,
            //    OutputGain = 0,
            //    ChannelMappingFamily = 0
            //};
            //opusHeadWriter.Write(opusHead);
            //WriteOggPage(OggHeaderType.BeginningOfStream, opusHeadStream.ToArray());

            using var opusTagsStream = new MemoryStream();
            using var opusTagsWriter = new BinaryWriter(opusTagsStream);
            opusTagsWriter.Write(new OpusTags());
            WriteOggPage(OggHeaderType.None, opusTagsStream.ToArray());
        }

        private void WriteOggPage(OggHeaderType oggHeaderType, byte[] data)
        {
            _oggPageWriter.WriteOggPage(oggHeaderType, 1, new List<SegmentEntry> {
                new SegmentEntry
                {
                    Data = data,
                    SegmentBytes = new byte[] { (byte)data.Length }
                }
            });
        }
    }
}
