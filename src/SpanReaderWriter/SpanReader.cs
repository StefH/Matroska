using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            var result = _current.Slice(Position)[0];
            Position += sizeof(byte);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadSByte()
        {
            var result = _current.Slice(Position)[0];
            Position += sizeof(sbyte);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char ReadChar() => Read<char>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort() => Read<short>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16() => ReadShort();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort() => Read<ushort>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt() => ReadUInt32();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32() => Read<uint>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadBytes(int length)
        {
            var result = Span.Slice(Position, Position + length).ToArray();
            Position += length;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : struct
        {
            var newSpan = Span.Slice(Position);
            var result = MemoryMarshal.Cast<byte, T>(newSpan)[0];
            Position += Unsafe.SizeOf<T>();

            return result;
        }
    }
}