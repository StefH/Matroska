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

        //public static void Write(this BinaryWriter writer, OpusHead opusHead)
        //{
        //    opusHead.Write(writer);
        //}

        //public static void Write(this BinaryWriter writer, OpusTags opusTags)
        //{
        //    opusTags.Write(writer);
        //}
    }
}