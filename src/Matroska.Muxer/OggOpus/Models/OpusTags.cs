using System.IO;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
    internal struct OpusTags
    {
        public const string ID = "OpusTags";

        public void Write(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(0L);
        }
    }
}