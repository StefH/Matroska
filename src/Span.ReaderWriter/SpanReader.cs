using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    public ref struct SpanReader
    {
        public readonly ReadOnlySpan<byte> Span;
        private ReadOnlySpan<byte> _current;

        public int Length;
        public int Position;

        public SpanReader(ReadOnlySpan<byte> span)
        {
            Span = span;
            Length = span.Length;
            Position = 0;
            _current = span;
        }

        public bool ReadBool() => ReadByte() != 0;

        public byte ReadByte()
        {
            var result = _current.Slice(Position)[0];
            Position += sizeof(byte);
            return result;
        }

        public sbyte ReadSByte()
        {
            var result = _current.Slice(Position)[0];
            Position += sizeof(sbyte);
            return (sbyte) result;
        }

        public char ReadChar() => Read<char>();

        public short ReadShort() => Read<short>();

        public short ReadInt16() => ReadShort();

        public int ReadInt() => ReadInt32();

        public ushort ReadUShort() => Read<ushort>();

        public uint ReadUInt() => ReadUInt32();

        public uint ReadUInt32() => Read<uint>();

        public int ReadInt32() => Read<int>();

        public long ReadLong() => Read<long>();

        public ulong ReadULong() => Read<ulong>();

        public decimal ReadDecimal() => Read<decimal>();

        public float ReadFloat() => Read<float>();

        public double ReadDouble() => Read<double>();

        public byte[] ReadBytes(int length)
        {
            var result = _current.Slice(Position, Position + length).ToArray();
            Position += length;

            return result;
        }

        public string ReadString()
        {
            var stringLength = Read7BitEncodedInt();
            var stringBytes = ReadBytes(stringLength);

            return Encoding.UTF8.GetString(stringBytes);
        }

        #region VInt
        public (ulong Value, int Length, ulong EncodedValue) ReadVInt(int maxLength)
        {
            uint b1 = ReadByte();
            ulong raw = b1;
            uint mask = 0xFF00;

            for (int i = 0; i < maxLength; ++i)
            {
                mask >>= 1;

                if ((b1 & mask) != 0)
                {
                    ulong value = raw & ~mask;

                    for (int j = 0; j < i; ++j)
                    {
                        byte b = ReadByte();

                        raw = (raw << 8) | b;
                        value = (value << 8) | b;
                    }

                    return (value, i + 1, raw);
                }
            }

            throw new EndOfStreamException("Invalid Variable Int.");
        }
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : struct
        {
            var newSpan = _current.Slice(Position);
            var result = MemoryMarshal.Cast<byte, T>(newSpan)[0];
            Position += Unsafe.SizeOf<T>();

            return result;
        }

        private int Read7BitEncodedInt()
        {
            // Read out an Int32 7 bits at a time.
            // The high bit of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;
            do
            {
                // Check for a corrupted stream. Read a max of 5 bytes.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                {
                    throw new FormatException("Format_Bad7BitInt32");
                }

                // ReadByte handles end of stream cases for us.
                b = ReadByte();
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            return count;
        }
    }
}