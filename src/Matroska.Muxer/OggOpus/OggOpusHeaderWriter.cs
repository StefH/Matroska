using System;
using System.Collections.Generic;
using System.IO;
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
            var opusHead = new OpusHead
            {
                Version = 1,
                OutputChannelCount = (byte)channels,
                PreSkip = preSkip,
                InputSampleRate = (uint)sampleRate,
                OutputGain = 0,
                ChannelMappingFamily = 0
            };
            Span<byte> span = stackalloc byte[opusHead.Size];
            var opusHeadSpanWriter = new SpanWriter(span);
            opusHeadSpanWriter.WriteOpusHead(opusHead);
            WriteOggPage(OggHeaderType.BeginningOfStream, opusHeadSpanWriter.ToArray());


            var opusTags = new OpusTags();
            var opusTagsSpanWriter = new SpanWriter(span);
            opusTagsSpanWriter.WriteOpusTags(opusTags);
            WriteOggPage(OggHeaderType.None, opusTagsSpanWriter.ToArray());
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
