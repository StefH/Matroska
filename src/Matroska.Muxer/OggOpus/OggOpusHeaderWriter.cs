using System.Collections.Generic;
using System.IO;
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

        public void WriteHeaders()
        {
            using var opusHeadStream = new MemoryStream();
            using var opusHeadWriter = new BinaryWriter(opusHeadStream);
            var opusHeader = new OpusHead
            {
                Version = 1,
                OutputChannelCount = 2,
                PreSkip = 0,
                InputSampleRate = 48000,
                OutputGain = 0,
                ChannelMappingFamily = 0
            };
            opusHeader.Write(opusHeadWriter);
            opusHeadWriter.Flush();

            var opusHeaderData = opusHeadStream.ToArray();

            _oggPageWriter.WriteOggPage(OggHeaderType.BeginningOfStream, 1, new List<SegmentEntry> {
                new SegmentEntry
                {
                    Data = opusHeaderData,
                    SegmentBytes = new byte[] { (byte) opusHeaderData.Length }
                }
            });

            using var opusTagsStream = new MemoryStream();
            using var opusTagsWriter = new BinaryWriter(opusTagsStream);
            var opusTags = new OpusTags
            {
            };
            opusTags.Write(opusTagsWriter);
            opusTagsWriter.Flush();

            var opustagsData = opusTagsStream.ToArray();

            _oggPageWriter.WriteOggPage(OggHeaderType.None, 1, new List<SegmentEntry> {
                new SegmentEntry
                {
                    Data = opustagsData,
                    SegmentBytes = new byte[] { (byte)opustagsData.Length }
                }
            });
        }
    }
}
