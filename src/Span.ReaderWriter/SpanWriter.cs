using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Span.ReaderWriter.Ebml;

namespace System.IO
{
    public ref struct SpanWriter
    {
        private const int BufferLength = 16;
        private const int LargeByteBufferSize = 256; // Size should be around the max number of chars/string * Encoding's max bytes/char

        public readonly Span<byte> Span;
        private readonly Encoding _encoding;
        private readonly Encoder _encoder;

        //private readonly int _maxChars;
        private readonly char[] _singleChar;
        private readonly byte[] _largeByteBuffer; // temp space for writing chars.
        private readonly byte[] _buffer; // temp space for writing primitives to.

        public int Length;
        public int Position;

        public SpanWriter(Span<byte> span) : this(span, new UTF8Encoding())
        {
        }

        public SpanWriter(Span<byte> span, Encoding encoding)
        {
            Span = span;
            _encoding = encoding;
            _encoder = encoding.GetEncoder();
            Length = span.Length;
            Position = 0;

            _largeByteBuffer = new byte[LargeByteBufferSize];
            //_maxChars = _largeByteBuffer.Length / _encoding.GetMaxByteCount(1);
            _singleChar = new char[1];
            _buffer = new byte[BufferLength];
        }

        public byte[] ToArray()
        {
            return Span.Slice(0, Position).ToArray();
        }

        public int Write(byte value)
        {
            Span[Position] = value;
            Position += 1;
            return 1;
        }

        public int Write(string value)
        {
            int len = _encoding.GetByteCount(value);
            Write7BitEncodedInt(len);

            var bytes = _encoding.GetBytes(value);
            return len + Write(bytes);
        }

        public int Write(byte[] value) => Write(value, value.Length);

        public int Write(byte[] value, int length)
        {
            Span<byte> byteSpan = value;
            byteSpan.CopyTo(Span.Slice(Position));

            Position += length;
            return length;
        }

        public int Write(char value)
        {
            _singleChar[0] = value;

            var numBytes = _encoder.GetBytes(_singleChar, 0, 1, _buffer, 0, true);
            return Write(_buffer, numBytes);
        }

        public int Write(char[] chars)
        {
            byte[] bytes = _encoding.GetBytes(chars, 0, chars.Length);
            return Write(bytes);
        }

        public int Write(decimal value) => Write(DecimalToBytes(value));

        public int Write(DateTime value) => Write(value.ToBinary());

        public int Write<T>(T value) where T : unmanaged
        {
            var length = Unsafe.SizeOf<T>();

            Span<T> typedSpan = stackalloc T[1] { value };
            var byteSpan = MemoryMarshal.Cast<T, byte>(typedSpan);
            byteSpan.CopyTo(Span.Slice(Position));

            Position += length;
            return length;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte[] DecimalToBytes(decimal number)
        {
            var decimalBits = decimal.GetBits(number);

            var lo = decimalBits[0];
            var mid = decimalBits[1];
            var hi = decimalBits[2];
            var flags = decimalBits[3];

            _buffer[0] = (byte)lo;
            _buffer[1] = (byte)(lo >> 8);
            _buffer[2] = (byte)(lo >> 16);
            _buffer[3] = (byte)(lo >> 24);

            _buffer[4] = (byte)mid;
            _buffer[5] = (byte)(mid >> 8);
            _buffer[6] = (byte)(mid >> 16);
            _buffer[7] = (byte)(mid >> 24);

            _buffer[8] = (byte)hi;
            _buffer[9] = (byte)(hi >> 8);
            _buffer[10] = (byte)(hi >> 16);
            _buffer[11] = (byte)(hi >> 24);

            _buffer[12] = (byte)flags;
            _buffer[13] = (byte)(flags >> 8);
            _buffer[14] = (byte)(flags >> 16);
            _buffer[15] = (byte)(flags >> 24);

            return _buffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // Copied from https://referencesource.microsoft.com/#mscorlib/system/io/binarywriter.cs,414
        private int Write7BitEncodedInt(int value)
        {
            int bytesWritten = 0;

            // Write out an int 7 bits at a time.  The high bit of the byte, when on, tells reader to continue reading more bytes.
            uint v = (uint)value; // support negative numbers
            while (v >= 0x80)
            {
                bytesWritten += Write((byte)(v | 0x80));
                v >>= 7;
            }

            return bytesWritten + Write((byte)v);
        }
    }
}