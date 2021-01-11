using System.IO;
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.Extensions
{
    internal static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, OggHeader oggHeader)
        {
            oggHeader.Write(writer);
        }
    }
}