using System.IO;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
    internal struct OpusHead
    {
        public const string ID = "OpusHead";

        public byte Version;
        public byte OutputChannelCount;
        public ushort PreSkip;
        public uint InputSampleRate;
        public short OutputGain;
        public byte ChannelMappingFamily;
                
        public void Write(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(Version);
            w.Write(OutputChannelCount);
            w.Write(PreSkip);
            w.Write(InputSampleRate);
            w.Write(OutputGain);
            w.Write(ChannelMappingFamily);
        }
    }
}