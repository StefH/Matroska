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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public int WriteByte(byte b)
        //{
        //    var length = sizeof(byte);
        //    Span[Position] = b;
        //    Position += length;
        //    return length;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public int Write(int value)
        //{
        //    var length = sizeof(int);

        //    Span<int> typedSpan = stackalloc int[1] { value };
        //    var byteSpan = MemoryMarshal.Cast<int, byte>(typedSpan);
        //    byteSpan.CopyTo(Span.Slice(Position));

        //    Position += length;

        //    return sizeof(int);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Write<T>(T value) where T : unmanaged
        {
            var length = Unsafe.SizeOf<T>();

            Span<T> typedSpan = stackalloc T[1] { value };
            var byteSpan = MemoryMarshal.Cast<T, byte>(typedSpan);
            byteSpan.CopyTo(Span.Slice(Position));

            Position += length;

            return length;
        }



        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public int Write<T>(T value)
        //{
        //    var len = Unsafe.SizeOf<T>();

        //    Span<int> a = stackalloc T[1] { value };
        //    var ab = MemoryMarshal.Cast<int, byte>(a);
        //    ab.CopyTo(Span.Slice(Position));

        //    Position += len;

        //    return sizeof(int);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public byte ReadSByte()
        //{
        //    var result = _current.Slice(Position)[0];
        //    Position += sizeof(sbyte);
        //    return result;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public char ReadChar() => Read<char>();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public short ReadShort() => Read<short>();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public short ReadInt16() => ReadShort();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public ushort ReadUShort() => Read<ushort>();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public uint ReadUInt() => ReadUInt32();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public uint ReadUInt32() => Read<uint>();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public byte[] ReadBytes(int length)
        //{
        //    var result = _current.Slice(Position, Position + length).ToArray();
        //    Position += length;

        //    return result;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public string ReadString()
        //{
        //    var stringLength = Read7BitEncodedInt();
        //    var stringBytes = ReadBytes(stringLength);

        //    return Encoding.UTF8.GetString(stringBytes);
        //}

        #region VInt
        public int WriteVInt(ulong encodedValue, int length)
        {
            Span<byte> buffer = stackalloc byte[length];

            int p = length;
            for (var data = encodedValue; --p >= 0; data >>= 8)
            {
                buffer[p] = (byte)(data & 0xff);
            }

            buffer.CopyTo(Span);
            Position += length;

            return length;
        }

        public int WriteVInt(ulong value)
        {
            int length = 1;
            while ((value + 1) >> length * 7 != 0)
            {
                ++length;
            }
            Span<byte> buffer = stackalloc byte[length];

            int p = 77;
            for (var data = value; --p >= 0; data >>= 8)
            {
                buffer[p] = (byte)(data & 0xff);
            }

            buffer.CopyTo(Span);
            Position += length;

            return length;
        }

        public static int GetSize(ulong value)
        {
            int octets = 1;
            while ((value + 1) >> octets * 7 != 0)
            {
                ++octets;
            }
            return octets;
        }
        #endregion

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public int Write<T>(T value) where T : struct
        //{
        //    var len = Unsafe.SizeOf<T>();


        //    Span<T> numbers = stackalloc[] { value };

        //    Span<byte> a = stackalloc [1] { value };
        //    var ab = MemoryMarshal.Cast<T, byte>(a);
        //    ab.CopyTo(span);
        //    return sizeof(double);



        //    //var newSpan = _current.Slice(Position);
        //    var result = MemoryMarshal.Cast<byte, T>(Span)[0];
        //    Position += len;

        //    return len;
        //}
    }
}