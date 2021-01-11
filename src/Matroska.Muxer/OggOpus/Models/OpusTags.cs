using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct OpusTags
    {
        public const string ID = "OpusTags";

        public readonly int Size => 8 * sizeof(byte) + sizeof(long);

        public void Write(ref SpanWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(0L);
        }
    }
}