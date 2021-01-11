using System.IO;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.Extensions
{
    internal static class SpanReaderExtensions
    {
        public static OpusHead ReadOpusHead(this ref SpanReader reader)
        {
            var h = new OpusHead();
            h.Read(ref reader);
            return h;
        }
    }
}