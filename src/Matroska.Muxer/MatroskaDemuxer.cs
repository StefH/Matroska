using System.IO;
using Matroska.Models;
using Matroska.Muxer.OggOpus;
using Matroska.Muxer.OggOpus.Settings;

namespace Matroska.Muxer
{
    public static class MatroskaDemuxer
    {
        public static void ExtractOggOpusAudio(Stream inputStream, Stream outputStream, OggOpusAudioStreamDemuxerSettings? settings = null)
        {
            var doc = MatroskaSerializer.Deserialize(inputStream);
            ExtractOggOpusAudio(doc, outputStream, settings);
        }

        public static void ExtractOggOpusAudio(MatroskaDocument doc, Stream outputStream, OggOpusAudioStreamDemuxerSettings? settings = null)
        {
            new OggOpusAudioStreamDemuxer(doc).CopyTo(outputStream, settings);
        }
    }
}