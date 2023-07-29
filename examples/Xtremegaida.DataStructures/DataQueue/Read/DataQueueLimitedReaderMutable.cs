using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueLimitedReaderMutable : IDataQueueReader
   {
      private IDataQueueReader reader;
      private long maxReadBytes;
      private long totalBytesRead;
      private bool isUnknownSize;
      private volatile bool disposed;

      public long UnreadLength
      {
         get
         {
            var canRead = reader?.UnreadLength ?? 0;
            if (!isUnknownSize)
            {
               var maxRead = maxReadBytes - totalBytesRead;
               if (maxRead < canRead) { canRead = maxRead; }
            }
            return canRead;
         }
      }

      public long CanReadLength => isUnknownSize ? 0 : (maxReadBytes - totalBytesRead);
      public bool IsReadClosed => disposed || (!isUnknownSize && totalBytesRead >= maxReadBytes);
      public long TotalBytesRead => totalBytesRead;
      public bool IsUnknownSize => isUnknownSize;

      public DataQueueLimitedReaderMutable() { }

      public void SetReadSource(IDataQueueReader reader, long maxReadBytes)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(DataQueueLimitedReaderMutable)); }
         isUnknownSize = false;
         if (maxReadBytes < 0) { isUnknownSize = true; maxReadBytes = 0; }
         this.reader = reader;
         this.maxReadBytes = maxReadBytes;
         totalBytesRead = 0;
      }

      public async ValueTask<int> ReadAsync(Memory<byte> buffer, bool waitUntilFull = false, CancellationToken cancellationToken = default)
      {
         if (disposed) { return 0; }
         if (!isUnknownSize)
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
         if (!isUnknownSize)
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
         if (disposed || (!isUnknownSize && totalBytesRead >= maxReadBytes)) { return -1; }
         var read = await reader.ReadByteAsync(cancellationToken);
         Interlocked.Increment(ref totalBytesRead);
         return read;
      }

      public void Dispose()
      {
         disposed = true;
      }
   }
}
