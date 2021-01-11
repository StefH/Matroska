using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct OpusHead
    {
        public const string OpusHeadID = "OpusHead";

        public string ID { get; set; }
        public byte Version { get; set; }
        public byte OutputChannelCount { get; set; }
        public ushort PreSkip { get; set; }
        public uint InputSampleRate { get; set; }
        public short OutputGain { get; set; }
        public byte ChannelMappingFamily { get; set; }

        public int Size => 8 * sizeof(byte) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + +sizeof(uint) + sizeof(short) + sizeof(byte);

        public void Write(ref SpanWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(OpusHeadID));
            w.Write(Version);
            w.Write(OutputChannelCount);
            w.Write(PreSkip);
            w.Write(InputSampleRate);
            w.Write(OutputGain);
            w.Write(ChannelMappingFamily);
        }

        public void Read(ref SpanReader r)
        {
            ID = Encoding.ASCII.GetString(r.ReadBytes(8));
            Version = r.ReadByte();
            OutputChannelCount = r.ReadByte();
            PreSkip = r.ReadUShort();
            InputSampleRate = r.ReadUInt32();
            OutputGain = r.ReadShort();
            ChannelMappingFamily = r.ReadByte();
        }
    }
}