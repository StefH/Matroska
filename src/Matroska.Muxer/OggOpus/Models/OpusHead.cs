using System.IO;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
    internal struct OpusHead
    {
        private const string OpusHeadID = "OpusHead";

        public string ID { get; set; }
        public byte Version { get; set; }
        public byte OutputChannelCount { get; set; }
        public ushort PreSkip { get; set; }
        public uint InputSampleRate { get; set; }
        public short OutputGain { get; set; }
        public byte ChannelMappingFamily { get; set; }

        public void Write(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(OpusHeadID));
            w.Write(Version);
            w.Write(OutputChannelCount);
            w.Write(PreSkip);
            w.Write(InputSampleRate);
            w.Write(OutputGain);
            w.Write(ChannelMappingFamily);
        }

        public void Read(BinaryReader r)
        {
            ID = Encoding.ASCII.GetString(r.ReadBytes(8));
            Version = r.ReadByte();
            OutputChannelCount = r.ReadByte();
            PreSkip = r.ReadUInt16();
            InputSampleRate = r.ReadUInt32();
            OutputGain = r.ReadInt16();
            ChannelMappingFamily = r.ReadByte();
        }
    }
}