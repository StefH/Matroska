using System;
using Matroska.Muxer.OggOpus.Models;
using Tedd;

namespace Matroska.Muxer.Extensions
{
    internal static class SpanStreamExtensions
    {
        public static OpusHead ReadOpusHead(this Span<byte> span)
        {
            var h = new OpusHead();
            h.Read(span);
            return h;
        }
    }
}