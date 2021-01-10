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