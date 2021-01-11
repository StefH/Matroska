using System.IO;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.Extensions
{
    internal static class SpanWriterExtensions
    {
        public static void WriteOpusHead(this ref SpanWriter writer, OpusHead opusHead)
        {
            opusHead.Write(ref writer);
        }

        public static void WriteOpusTags(this ref SpanWriter writer, OpusTags opusTags)
        {
            opusTags.Write(ref writer);
        }
    }
}