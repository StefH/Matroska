using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
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

        public void Write(SpanWriter w)
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
        public void Read(SpanReader r)
        {
            ID = Encoding.ASCII.GetString(r.ReadBytes(8));
            Version = r.ReadByte();
            OutputChannelCount = r.ReadByte();
            PreSkip = r.ReadUShort();
            InputSampleRate = r.ReadUInt32();
            OutputGain = r.ReadShort();
            ChannelMappingFamily = r.ReadByte();
        }

        //public void Read(Span<byte> r)
        //{
        //    ID = Encoding.ASCII.GetString(r.MoveReadBytes(8));
        //    Version = r.MoveReadByte();
        //    OutputChannelCount = r.MoveReadByte();

        //    PreSkip = MemoryMarshal.Cast<byte, UInt16>(r)[0];
        //    r = r.Slice(sizeof(UInt16));

        //  //  PreSkip = r.MoveReadUInt16();
        //    InputSampleRate = MemoryMarshal.Cast<byte, UInt32>(r)[0]; //r.MoveReadUInt32();
        //    r = r.Slice(sizeof(UInt32));

        //    OutputGain = r.MoveReadInt16();
        //    ChannelMappingFamily = r.MoveReadByte();
        //}
    }
}