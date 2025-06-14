using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueStreamReader : IDataQueueReader, IAsyncDisposable
   {
      private readonly Stream baseStream;
      private readonly bool keepStreamOpen;
      private readonly long maxReadBytes = -1;
      private volatile bool disposed;
      private long totalBytesRead;
      private byte[] byteRead;

      public long UnreadLength
      {
         get
         {
            var canRead = !baseStream.CanSeek ? (disposed ? 0 : long.MaxValue) : (baseStream.Length - baseStream.Position);
            if (maxReadBytes >= 0)
            {
               var maxRead = maxReadBytes - totalBytesRead;
               if (maxRead < canRead) { canRead = maxRead; }
            }
            return canRead;
         }
      }

      public bool IsReadClosed => disposed;

      public long TotalBytesRead => totalBytesRead;

      public DataQueueStreamReader(Stream baseStream, bool keepStreamOpen = false)
      {
         this.baseStream = baseStream;
         this.keepStreamOpen = keepStreamOpen;
      }

      public DataQueueStreamReader(Stream baseStream, long maxReadBytes, bool keepStreamOpen = false) : this(baseStream, keepStreamOpen)
      {
         if (maxReadBytes < 0) { throw new ArgumentOutOfRangeException(nameof(maxReadBytes)); }
         this.maxReadBytes = maxReadBytes;
      }

      public async ValueTask<int> ReadAsync(Memory<byte> buffer, bool waitUntilFull = false, CancellationToken cancellationToken = default)
      {
         int readBytes = 0;
         if (disposed) { return 0; }
         if (maxReadBytes >= 0)
         {
            var canRead = maxReadBytes - totalBytesRead;
            if (canRead < buffer.Length) { buffer = buffer.Slice(0, (int)canRead); }
         }
         if (!waitUntilFull)
         {
            readBytes = await baseStream.ReadAsync(buffer, cancellationToken);
         }
         else
         {
            while (!buffer.IsEmpty)
            {
               var read = await baseStream.ReadAsync(buffer, cancellationToken);
               if (read <= 0) { break; }
               buffer = buffer.Slice(read);
               readBytes += read;
            };
         }
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
         if (baseStream.CanSeek)
         {
            var length = baseStream.Length;
            var pos = baseStream.Position;
            var canRead = length - pos;
            if (canRead > skipBytes) { canRead = skipBytes; }
            if (canRead <= 0) { canRead = 0; }
            else { baseStream.Seek(canRead, SeekOrigin.Current); }
            Interlocked.Add(ref totalBytesRead, canRead);
            return (int)canRead;
         }
         int readBytes = 0;
         if (skipBytes <= 4)
         {
            while (readBytes < skipBytes)
            {
               if (await ReadByteAsync(cancellationToken) < 0) { break; }
               readBytes++;
            }
         }
         else
         {
            var buffer = new byte[Math.Min(skipBytes, 4096)];
            while (readBytes < skipBytes)
            {
               var canRead = skipBytes - readBytes;
               if (canRead > buffer.Length) { canRead = buffer.Length; }
               var read = await baseStream.ReadAsync(buffer, 0, canRead, cancellationToken);
               if (read <= 0) { break; }
               readBytes += read;
            };
            Interlocked.Add(ref totalBytesRead, readBytes);
         }
         return readBytes;
      }

      public async ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default)
      {
         if (disposed || (maxReadBytes >= 0 && totalBytesRead >= maxReadBytes)) { return -1; }
         if (byteRead == null) { byteRead = new byte[1]; }
         var read = await baseStream.ReadAsync(byteRead, cancellationToken);
         if (read == 0) { return -1; }
         Interlocked.Increment(ref totalBytesRead);
         return byteRead[0];
      }

      public void Dispose()
      {
         if (disposed) { return; }
         disposed = true;
         if (!keepStreamOpen) { baseStream.Dispose(); }
      }

      public async ValueTask DisposeAsync()
      {
         if (disposed) { return; }
         disposed = true;
         if (!keepStreamOpen) { await baseStream.DisposeAsync(); }
      }
   }
}
