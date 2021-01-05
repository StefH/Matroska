using System.IO;
using Matroska.Models;
using Matroska.Muxer.OggOpus;

namespace Matroska.Muxer
{
    public static class MatroskaDemuxer
    {
        public static void ExtractOggOpusAudio(MatroskaDocument doc, Stream stream)
        {
            new OggOpusDemuxer(doc).CopyTo(stream);
        }
    }
}