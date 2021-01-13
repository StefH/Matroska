using System;
using System.Text;

namespace Matroska.Muxer.WebM
{
    // https://gist.github.com/UCIS/983f2c16d36aee7f75cff4ac477d6805
    class WebmPacketizer //: IPacketizer
    {
        private ulong position = 0;

        static WebmPacketizer()
        {
        }

        public WebmPacketizer()
        {
        }

        private static byte[] Concat(params byte[][] parts)
        {
            return new byte[0]; // ArrayUtil.Merge(parts);
        }

        private static byte[] EncodeID(uint id)
        {
            if ((id & 0xFF000000) != 0) return new byte[] { (byte)(id >> 24), (byte)(id >> 16), (byte)(id >> 8), (byte)id };
            if ((id & 0xFF0000) != 0) return new byte[] { (byte)(id >> 16), (byte)(id >> 8), (byte)id };
            if ((id & 0xFF00) != 0) return new byte[] { (byte)(id >> 8), (byte)id };
            if ((id & 0xFF) != 0) return new byte[] { (byte)id };
            throw new InvalidOperationException();
        }

        private static byte[] EncodeLength(uint value)
        {
            if (value <= 0x7F) return new byte[] { (byte)(0x80 | value) };
            if (value <= 0x3FFF) return new byte[] { (byte)(0x40 | (value >> 8)), (byte)value };
            if (value <= 0x1FFFFF) return new byte[] { (byte)(0x20 | (value >> 16)), (byte)(value >> 8), (byte)value };
            if (value <= 0xFFFFFFF) return new byte[] { (byte)(0x10 | (value >> 24)), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
            return new byte[] { 0x08, (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
        }

        private static byte[] EncodeUInt(ulong value)
        {
            if (value <= 0xFF) return new byte[] { (byte)value };
            if (value <= 0xFFFF) return new byte[] { (byte)(value >> 8), (byte)value };
            if (value <= 0xFFFFFF) return new byte[] { (byte)(value >> 16), (byte)(value >> 8), (byte)value };
            if (value <= 0xFFFFFFFF) return new byte[] { (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
            if (value <= 0xFFFFFFFFFF) return new byte[] { (byte)(value >> 32), (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
            if (value <= 0xFFFFFFFFFFFF) return new byte[] { (byte)(value >> 40), (byte)(value >> 32), (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
            if (value <= 0xFFFFFFFFFFFFFF) return new byte[] { (byte)(value >> 48), (byte)(value >> 40), (byte)(value >> 32), (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
            return new byte[] { (byte)(value >> 56), (byte)(value >> 48), (byte)(value >> 40), (byte)(value >> 32), (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value }; return new byte[] { (byte)(value >> 56), (byte)(value >> 48), (byte)(value >> 40), (byte)(value >> 32), (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value };
        }

        private static byte[] MakeElement(uint id, byte[] data)
        {
            return Concat(EncodeID(id), EncodeLength((uint)data.Length), data);
        }

        private static byte[] MakeInfiniteElement(uint id, byte[] data)
        {
            return Concat(EncodeID(id), EncodeLength(0xFFFFFFFF), data);
        }

        private static byte[] MakeMaster(uint id, params byte[][] parts)
        {
            return MakeElement(id, Concat(parts));
        }

        private static byte[] MakeInfiniteMaster(uint id, params byte[][] parts)
        {
            return MakeInfiniteElement(id, Concat(parts));
        }

        private static byte[] MakeUInt(uint id, ulong value)
        {
            return MakeElement(id, EncodeUInt(value));
        }

        private static byte[] MakeString(uint id, string value)
        {
            return MakeElement(id, Encoding.ASCII.GetBytes(value));
        }

        private static byte[] MakeUnicode(uint id, string value)
        {
            return MakeElement(id, Encoding.UTF8.GetBytes(value));
        }

        private static byte[] MakeFloat(uint id, float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            return MakeElement(id, data);
        }

        public byte[] BuildHeader(byte channels, uint originalSampleRate)
        {
            originalSampleRate = 48000;
            return Concat(
                MakeMaster(0x1A45DFA3, //EBML
                    MakeUInt(0x4286, 1), //EBMLVersion
                    MakeUInt(0x42F7, 1), //EBMLReadVersion
                    MakeUInt(0x42F2, 4), //EBMLMaxIDLength
                    MakeUInt(0x42F3, 8), //EBMLMaxSizeLength
                    MakeString(0x4282, "webm"), //DocType
                    MakeUInt(0x4287, 4), //DocTypeVersion
                    MakeUInt(0x4285, 2) //DocTypeReadVersion
                ),
                MakeInfiniteMaster(0x18538067, //Segment
                    MakeMaster(0x114D9B74, //SeekHead
                        MakeMaster(0x4DBB, //Seek
                            MakeElement(0x53AB, EncodeID(0x1549A966)), //SeekID - Info
                            MakeUInt(0x53AC, 78 - 45) //SeekPosition - bytes offset from SeekHead
                        ),
                        MakeMaster(0x4DBB, //Seek
                            MakeElement(0x53AB, EncodeID(0x1654AE6B)), //SeekID - Tracks
                            MakeUInt(0x53AC, 127 - 45) //SeekPosition - bytes offset from SeekHead
                        )
                    ),
                    MakeMaster(0x1549A966, //Info
                                           //MakeUInt(0x2AD7B1, 20833), //TimestampScale = 1000000000/48000=20833 nS per sample
                        MakeUInt(0x2AD7B1, 1000000), //TimestampScale = 1000000000/1000000=1ms
                        MakeUnicode(0x4D80, "URadioServer"), //MuxingApp
                        MakeUnicode(0x5741, "URadioServer"), //WritingApp
                        MakeFloat(0x4489, 10000000)//, //Duration
                                                   //MakeElement(0x73A4, UCIS.NaCl.randombytes.generate(16)) //SegmentUID
                    ),
                    MakeMaster(0x1654AE6B, //Tracks
                        MakeMaster(0xAE, //TrackEntry
                            MakeUInt(0xD7, 1), //TrackNumber
                            MakeUInt(0x73C5, 1), //TrackUID
                            MakeUInt(0x9C, 0), //FlagLacing
                            MakeString(0x22B59C, "und"), //Language
                            MakeString(0x86, "A_OPUS"), //CodecID
                            MakeUInt(0x56AA, 6500000), //CodecDelay
                            MakeUInt(0x56BB, 80000000), //SeekPreRoll
                            MakeUInt(0x83, 2), //TrackType
                            MakeMaster(0xE1, //Audio
                                MakeFloat(0xB5, 48000), //SamplingFrequency
                                MakeUInt(0x9F, channels) //Channels
                            ),
                            MakeElement(0x63A2, //CodecPrivate
                                new byte[] {
                                    (byte)'O', (byte)'p', (byte)'u', (byte)'s', (byte)'H', (byte)'e', (byte)'a', (byte)'d',
                                    1, (byte)channels, 0x38, 0x01,
                                    (byte)(originalSampleRate >> 0), (byte)(originalSampleRate >> 8), (byte)(originalSampleRate >> 16), (byte)(originalSampleRate >> 24),
                                    0, 0, 0
                                }
                            )
                        )
                    )
                )
            );
        }

        public byte[] GetDataPage(byte[] packet, uint samples)
        {
            ulong cposition = position;
            position += samples;
            //return new Byte[0];
            return MakeMaster(0x1F43B675, //Cluster
                                          //MakeUInt(0xE7, position), //Timestamp
                MakeUInt(0xE7, cposition * 1000 / 48000), //Timestamp
                MakeElement(0xA3, //SimpleBlock
                    Concat(
                        EncodeLength(1), //Track Number
                        new byte[] { 0, 0 }, //Relative timestamp
                        new byte[] { 0x80 }, //Flags
                        packet
                    )
                )
            );
        }
    }
}
