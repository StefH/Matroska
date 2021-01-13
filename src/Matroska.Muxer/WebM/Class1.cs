using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matroska.Muxer.WebM
{
	// https://gist.github.com/UCIS/983f2c16d36aee7f75cff4ac477d6805
	class WebmPacketizer //: IPacketizer
	{
		private UInt64 position = 0;

		static WebmPacketizer()
		{
		}

		public WebmPacketizer()
		{
		}

		private static Byte[] Concat(params Byte[][] parts)
		{
			return new byte[0]; // ArrayUtil.Merge(parts);
		}

		private static Byte[] EncodeID(UInt32 id)
		{
			if ((id & 0xFF000000) != 0) return new Byte[] { (Byte)(id >> 24), (Byte)(id >> 16), (Byte)(id >> 8), (Byte)id };
			if ((id & 0xFF0000) != 0) return new Byte[] { (Byte)(id >> 16), (Byte)(id >> 8), (Byte)id };
			if ((id & 0xFF00) != 0) return new Byte[] { (Byte)(id >> 8), (Byte)id };
			if ((id & 0xFF) != 0) return new Byte[] { (Byte)id };
			throw new InvalidOperationException();
		}

		private static Byte[] EncodeLength(UInt32 value)
		{
			if (value <= 0x7F) return new Byte[] { (Byte)(0x80 | value) };
			if (value <= 0x3FFF) return new Byte[] { (Byte)(0x40 | (value >> 8)), (Byte)value };
			if (value <= 0x1FFFFF) return new Byte[] { (Byte)(0x20 | (value >> 16)), (Byte)(value >> 8), (Byte)value };
			if (value <= 0xFFFFFFF) return new Byte[] { (Byte)(0x10 | (value >> 24)), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
			return new Byte[] { 0x08, (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
		}

		private static Byte[] EncodeUInt(UInt64 value)
		{
			if (value <= 0xFF) return new Byte[] { (Byte)value };
			if (value <= 0xFFFF) return new Byte[] { (Byte)(value >> 8), (Byte)value };
			if (value <= 0xFFFFFF) return new Byte[] { (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
			if (value <= 0xFFFFFFFF) return new Byte[] { (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
			if (value <= 0xFFFFFFFFFF) return new Byte[] { (Byte)(value >> 32), (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
			if (value <= 0xFFFFFFFFFFFF) return new Byte[] { (Byte)(value >> 40), (Byte)(value >> 32), (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
			if (value <= 0xFFFFFFFFFFFFFF) return new Byte[] { (Byte)(value >> 48), (Byte)(value >> 40), (Byte)(value >> 32), (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
			return new Byte[] { (Byte)(value >> 56), (Byte)(value >> 48), (Byte)(value >> 40), (Byte)(value >> 32), (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value }; return new Byte[] { (Byte)(value >> 56), (Byte)(value >> 48), (Byte)(value >> 40), (Byte)(value >> 32), (Byte)(value >> 24), (Byte)(value >> 16), (Byte)(value >> 8), (Byte)value };
		}

		private static Byte[] MakeElement(UInt32 id, Byte[] data)
		{
			return Concat(EncodeID(id), EncodeLength((UInt32)data.Length), data);
		}

		private static Byte[] MakeInfiniteElement(UInt32 id, Byte[] data)
		{
			return Concat(EncodeID(id), EncodeLength(0xFFFFFFFF), data);
		}

		private static Byte[] MakeMaster(UInt32 id, params Byte[][] parts)
		{
			return MakeElement(id, Concat(parts));
		}

		private static Byte[] MakeInfiniteMaster(UInt32 id, params Byte[][] parts)
		{
			return MakeInfiniteElement(id, Concat(parts));
		}

		private static Byte[] MakeUInt(UInt32 id, UInt64 value)
		{
			return MakeElement(id, EncodeUInt(value));
		}

		private static Byte[] MakeString(UInt32 id, String value)
		{
			return MakeElement(id, Encoding.ASCII.GetBytes(value));
		}

		private static Byte[] MakeUnicode(UInt32 id, String value)
		{
			return MakeElement(id, Encoding.UTF8.GetBytes(value));
		}

		private static Byte[] MakeFloat(UInt32 id, Single value)
		{
			Byte[] data = BitConverter.GetBytes(value);
			Array.Reverse(data);
			return MakeElement(id, data);
		}

		public Byte[] BuildHeader(Byte channels, UInt32 originalSampleRate)
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
								new Byte[] {
									(Byte)'O', (Byte)'p', (Byte)'u', (Byte)'s', (Byte)'H', (Byte)'e', (Byte)'a', (Byte)'d',
									1, (Byte)channels, 0x38, 0x01,
									(Byte)(originalSampleRate >> 0), (Byte)(originalSampleRate >> 8), (Byte)(originalSampleRate >> 16), (Byte)(originalSampleRate >> 24),
									0, 0, 0
								}
							)
						)
					)
				)
			);
		}

		public Byte[] GetDataPage(Byte[] packet, UInt32 samples)
		{
			UInt64 cposition = position;
			position += samples;
			//return new Byte[0];
			return MakeMaster(0x1F43B675, //Cluster
										  //MakeUInt(0xE7, position), //Timestamp
				MakeUInt(0xE7, cposition * 1000 / 48000), //Timestamp
				MakeElement(0xA3, //SimpleBlock
					Concat(
						EncodeLength(1), //Track Number
						new Byte[] { 0, 0 }, //Relative timestamp
						new Byte[] { 0x80 }, //Flags
						packet
					)
				)
			);
		}
	}
}
