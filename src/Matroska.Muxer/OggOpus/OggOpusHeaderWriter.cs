﻿using System.Collections.Generic;
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

        public void WriteHeaders(int channels, int sampleRate)
        {
            using var opusHeadStream = new MemoryStream();
            using var opusHeadWriter = new BinaryWriter(opusHeadStream);
            var opusHead = new OpusHead
            {
                Version = 1,
                OutputChannelCount = (byte) channels,
                PreSkip = 0,
                InputSampleRate = (uint) sampleRate,
                OutputGain = 0,
                ChannelMappingFamily = 0
            };
            opusHeadWriter.Write(opusHead);
            opusHeadWriter.Flush();
            WriteOggPage(OggHeaderType.BeginningOfStream, opusHeadStream.ToArray());

            using var opusTagsStream = new MemoryStream();
            using var opusTagsWriter = new BinaryWriter(opusTagsStream);
            opusTagsWriter.Write(new OpusTags());
            opusTagsWriter.Flush();

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
