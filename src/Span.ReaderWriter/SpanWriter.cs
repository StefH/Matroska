using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public ref struct SpanWriter
    {
        public readonly Span<byte> Span;

        public int Length;
        public int Position;

        public SpanWriter(Span<byte> span)
        {
            Span = span;
            Length = span.Length;
            Position = 0;
        }

        public int Write(byte value)
        {
            Span[Position] = value;

            Position += 1;

            return 1;
        }

        public int Write<T>(T value) where T : unmanaged
        {
            var length = Unsafe.SizeOf<T>();

            Span<T> typedSpan = stackalloc T[1] { value };
            var byteSpan = MemoryMarshal.Cast<T, byte>(typedSpan);
            byteSpan.CopyTo(Span.Slice(Position));

            Position += length;

            return length;
        }

        public int Write(byte[] value)
        {
            var length = value.Length;

            Span<byte> byteSpan = value;
            byteSpan.CopyTo(Span.Slice(Position));

            Position += length;

            return length;
        }

        private byte[] DecimalToBytes(decimal number)
        {
            var decimalBuffer = new byte[16];

            var decimalBits = decimal.GetBits(number);

            var lo = decimalBits[0];
            var mid = decimalBits[1];
            var hi = decimalBits[2];
            var flags = decimalBits[3];

            decimalBuffer[0] = (byte)lo;
            decimalBuffer[1] = (byte)(lo >> 8);
            decimalBuffer[2] = (byte)(lo >> 16);
            decimalBuffer[3] = (byte)(lo >> 24);

            decimalBuffer[4] = (byte)mid;
            decimalBuffer[5] = (byte)(mid >> 8);
            decimalBuffer[6] = (byte)(mid >> 16);
            decimalBuffer[7] = (byte)(mid >> 24);

            decimalBuffer[8] = (byte)hi;
            decimalBuffer[9] = (byte)(hi >> 8);
            decimalBuffer[10] = (byte)(hi >> 16);
            decimalBuffer[11] = (byte)(hi >> 24);

            decimalBuffer[12] = (byte)flags;
            decimalBuffer[13] = (byte)(flags >> 8);
            decimalBuffer[14] = (byte)(flags >> 16);
            decimalBuffer[15] = (byte)(flags >> 24);

            return decimalBuffer;
        }

        #region VInt
        public int WriteVInt(ulong encodedCalue, int length)
        {
            int p = length;
            for (var data = encodedCalue; --p >= 0; data >>= 8)
            {
                Span[Position + p] = (byte)(data & 0xff);
            }

            Position += length;

            return length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVInt(ulong value)
        {
            var (encodedValue, length) = VIntUtils.Encode(value);
            return WriteVInt(encodedValue, length);
        }
        #endregion
    }
}