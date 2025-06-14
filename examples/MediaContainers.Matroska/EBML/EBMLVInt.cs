using System;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public struct EBMLVInt
   {
      public static readonly EBMLVInt Empty = new();

      public readonly byte WidthBytes;
      public readonly ulong Value;

      public ulong ValueMask => (1UL << ((WidthBytes << 3) - WidthBytes)) - 1;
      public ulong ValueWithMarker => Value | ((0x100UL >> WidthBytes) << ((WidthBytes - 1) << 3));
      public long SignedValue { get { var shift = 64 - ((WidthBytes << 3) - WidthBytes); return (long)(Value << shift) >> shift; } }
      public bool IsUnknownValue => Value == ValueMask;
      public bool IsMinWidth => WidthBytes == CalculateWidth(Value);
      public bool IsValidValue => WidthBytes != 0;
      public bool IsEmpty => WidthBytes == 0;

      public EBMLVInt(byte width, ulong value)
      {
         WidthBytes = width;
         Value = value;
         if (width <= 0 || width > 8) { throw new ArgumentOutOfRangeException(nameof(width)); }
         if ((value & ~ValueMask) != 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
      }

      public EBMLVInt(ulong value) : this(CalculateWidth(value), value) { }

      public static byte CalculateWidth(ulong value)
      {
         byte width = 1;
         do
         {
            var mask = (1UL << ((width << 3) - width)) - 1;
            if ((value & ~mask) == 0 && value != mask) { break; }
         }
         while (++width < 8);
         return width;
      }

      public static EBMLVInt CreateUnknown(int width = 1)
      {
         if (width <= 0) { width = 1; }
         if (width > 8) { width = 8; }
         var mask = (1UL << ((width << 3) - width)) - 1;
         return new EBMLVInt((byte)width, mask);
      }

      public static EBMLVInt CreateWithMarker(ulong value)
      {
         if (value == 0) { return Empty; }
         byte width = 1;
         do
         {
            var mask = (1UL << ((width << 3) + 1 - width)) - 1;
            if ((value & ~mask) == 0) { break; }
         }
         while (++width < 8);
         var widthMask = (ulong)(-1L << ((width << 3) + 1 - width));
         if ((value & widthMask) != 0) { throw new ArgumentException(nameof(value)); }
         var markerBit = ((0x100UL >> width) << ((width - 1) << 3));
         if ((value & markerBit) != markerBit) { throw new ArgumentException(nameof(value)); }
         return new EBMLVInt(width, value & ~markerBit);
      }

      public async ValueTask Write(IDataQueueWriter buffer, CancellationToken cancellationToken = default)
      {
         await buffer.WriteByteAsync((byte)((0x100 >> WidthBytes) | (byte)(Value >> ((WidthBytes - 1) << 3))), cancellationToken);
         for (int i = 2; i <= WidthBytes; i++)
         {
            await buffer.WriteByteAsync((byte)((Value >> ((WidthBytes - i) << 3)) & 0xff), cancellationToken);
         }
      }

      public void Write(DataBuffer buffer)
      {
         buffer.Buffer[buffer.WriteOffset++] = (byte)((0x100 >> WidthBytes) | (byte)(Value >> ((WidthBytes - 1) << 3)));
         for (int i = 2; i <= WidthBytes; i++) { buffer.Buffer[buffer.WriteOffset++] = (byte)((Value >> ((WidthBytes - i) << 3)) & 0xff); }
      }

      public static async ValueTask<EBMLVInt> Read(IDataQueueReader buffer, CancellationToken cancellationToken = default)
      {
         var prefix = await buffer.ReadByteAsync(cancellationToken);
         if (prefix <= 0) { return Empty; }
         byte width = 1;
         if ((prefix & 0x80) == 0)
         {
            width++;
            if ((prefix & 0x40) == 0)
            {
               width++;
               while (width < 8 && (prefix & (0x100 >> width)) == 0) { width++; }
            }
         }
         if (width == 1) { return new EBMLVInt(1, (ulong)(prefix & 0x7F)); }
         if (width == 2)
         {
            var next = await buffer.ReadByteAsync(cancellationToken);
            if (next < 0) { return Empty; }
            return new EBMLVInt(2, ((ulong)(prefix & 0x3F) << 8) | (byte)next);
         }
         ulong value = (ulong)(prefix & ((0x100 >> width) - 1));
         for (int i = 1; i < width; i++)
         {
            var next = await buffer.ReadByteAsync(cancellationToken);
            if (next < 0) { return Empty; }
            value = (value << 8) | (byte)next;
         }
         return new EBMLVInt(width, value);
      }
   }
}
