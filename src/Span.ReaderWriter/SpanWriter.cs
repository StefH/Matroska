using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
    public ref struct SpanWriter
    {
        private const int DecimalLength = 16;
        private readonly byte[] _decimalBuffer;

        public readonly Span<byte> Span;
        private readonly Encoding _encoding;

        public int Length;
        public int Position;

        public SpanWriter(Span<byte> span) : this(span, new UTF8Encoding())
        {
        }

        public SpanWriter(Span<byte> span, Encoding encoding)
        {
            Span = span;
            _encoding = encoding;
            Length = span.Length;
            Position = 0;

            _decimalBuffer = new byte[DecimalLength];
        }

        public byte[] ToArray()
        {
            return Span.Slice(Position).ToArray();
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

        public int Write(decimal value) => Write(DecimalToBytes(value));

        public int Write(DateTime value) => Write(value.ToBinary());

        private byte[] DecimalToBytes(decimal number)
        {
            var decimalBits = decimal.GetBits(number);

            var lo = decimalBits[0];
            var mid = decimalBits[1];
            var hi = decimalBits[2];
            var flags = decimalBits[3];

            _decimalBuffer[0] = (byte)lo;
            _decimalBuffer[1] = (byte)(lo >> 8);
            _decimalBuffer[2] = (byte)(lo >> 16);
            _decimalBuffer[3] = (byte)(lo >> 24);

            _decimalBuffer[4] = (byte)mid;
            _decimalBuffer[5] = (byte)(mid >> 8);
            _decimalBuffer[6] = (byte)(mid >> 16);
            _decimalBuffer[7] = (byte)(mid >> 24);

            _decimalBuffer[8] = (byte)hi;
            _decimalBuffer[9] = (byte)(hi >> 8);
            _decimalBuffer[10] = (byte)(hi >> 16);
            _decimalBuffer[11] = (byte)(hi >> 24);

            _decimalBuffer[12] = (byte)flags;
            _decimalBuffer[13] = (byte)(flags >> 8);
            _decimalBuffer[14] = (byte)(flags >> 16);
            _decimalBuffer[15] = (byte)(flags >> 24);

            return _decimalBuffer;
        }

        #region VInt
        public int Write(VInt vint)
        {
            int p = vint.Length;
            for (var data = vint.EncodedValue; --p >= 0; data >>= 8)
            {
                Span[Position + p] = (byte)(data & 0xff);
            }

            Position += vint.Length;

            return vint.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int WriteVInt(ulong value)
        {
            var vint = new VInt(value);
            return Write(vint);
        }
        #endregion
    }
}