using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueLimitedReader : IDataQueueReader
   {
      private readonly IDataQueueReader reader;
      private readonly bool keepStreamOpen;
      private readonly long maxReadBytes;
      private long totalBytesRead;
      private volatile bool disposed;

      public long UnreadLength
      {
         get
         {
            var canRead = reader.UnreadLength;
            var maxRead = maxReadBytes - totalBytesRead;
            if (maxRead < canRead) { canRead = maxRead; }
            return canRead;
         }
      }

      public long CanReadLength => maxReadBytes - totalBytesRead;
      public bool IsReadClosed => disposed || (totalBytesRead >= maxReadBytes);
      public long TotalBytesRead => totalBytesRead;

      public DataQueueLimitedReader(IDataQueueReader reader, long maxReadBytes, bool keepStreamOpen = false)
      {
         if (maxReadBytes < 0) { throw new ArgumentOutOfRangeException(nameof(maxReadBytes)); }
         this.reader = reader;
         this.keepStreamOpen = keepStreamOpen;
         this.maxReadBytes = maxReadBytes;
      }

      public async ValueTask<int> ReadAsync(Memory<byte> buffer, bool waitUntilFull = false, CancellationToken cancellationToken = default)
      {
         if (disposed) { return 0; }
         if (maxReadBytes >= 0)
         {
            var canRead = maxReadBytes - totalBytesRead;
            if (canRead < buffer.Length) { buffer = buffer.Slice(0, (int)canRead); }
         }
         int readBytes = await reader.ReadAsync(buffer, waitUntilFull, cancellationToken);
         Interlocked.Add(ref totalBytesRead, readBytes);
         return readBytes;
      }

      public async ValueTask<int> ReadAsync(int skipBytes, CancellationToken cancellationToken = default)
      {
         if (disposed) { return 0; }
         if (maxReadBytes >= 0)
         {
            var canRead = maxReadBytes - totalBytesRead;
            if (canRead < skipBytes) { skipBytes = (int)canRead; }
         }
         if (skipBytes <= 0) { return 0; }
         var readBytes = await reader.ReadAsync(skipBytes, cancellationToken);
         Interlocked.Add(ref totalBytesRead, readBytes);
         return readBytes;
      }

      public async ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default)
      {
         if (disposed || (maxReadBytes >= 0 && totalBytesRead >= maxReadBytes)) { return -1; }
         var read = await reader.ReadByteAsync(cancellationToken);
         Interlocked.Increment(ref totalBytesRead);
         return read;
      }

      public void Dispose()
      {
         if (disposed) { return; }
         disposed = true;
         if (!keepStreamOpen) { reader.Dispose(); }
      }
   }
}
