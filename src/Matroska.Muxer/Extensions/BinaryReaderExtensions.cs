using System.IO;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.Extensions
{
    internal static class BinaryReaderExtensions
    {
        public static OpusHead ReadOpusHead(this BinaryReader reader)
        {
            var h = new OpusHead();
            h.Read(reader);
            return h;
        }
    }
}