using System;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers.Matroska
{
   public static class XiphLacingSize
   {
      public static async ValueTask<int> Read(IDataQueueReader reader, CancellationToken cancellationToken = default)
      {
         int size = 0;
         do
         {
            var val = await reader.ReadByteAsync(cancellationToken);
            if (val < 0) { return size; }
            if (val < 255) { return size + val; }
            size += 255;
         }
         while (true);
      }

      public static int Read(DataBuffer buffer)
      {
         int size = 0;
         do
         {
            if (buffer.ReadOffset >= buffer.WriteOffset) { return -1; }
            var val = buffer.Buffer[buffer.ReadOffset++];
            if (val < 0) { return size; }
            if (val < 255) { return size + val; }
            size += 255;
         }
         while (true);
      }

      public static async ValueTask Write(IDataQueueWriter writer, int value, CancellationToken cancellationToken = default)
      {
         if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
         while (value >= 255) { value -= 255; await writer.WriteByteAsync(255, cancellationToken); }
         await writer.WriteByteAsync((byte)value, cancellationToken);
      }

      public static void Write(DataBuffer buffer, int value)
      {
         if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
         while (value >= 255) { value -= 255; buffer.Buffer[buffer.WriteOffset++] = 255; }
         buffer.Buffer[buffer.WriteOffset++] = (byte)value;
      }
   }
}
