using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueMemoryReaderMutable : IDataQueueReader
   {
      private ReadOnlyMemory<byte> memory;
      private int totalBytesRead;
      private volatile bool disposed;

      public long UnreadLength => memory.Length - totalBytesRead;
      public bool IsReadClosed => disposed || totalBytesRead >= memory.Length;
      public long TotalBytesRead => totalBytesRead;
      public ReadOnlyMemory<byte> Memory => memory;

      public DataQueueMemoryReaderMutable() { }

      public void SetBuffer(ReadOnlyMemory<byte> memory)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(DataQueueMemoryReaderMutable)); }
         this.memory = memory;
         totalBytesRead = 0;
      }

      public void SetReadOffset(int offset)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(DataQueueMemoryReaderMutable)); }
         totalBytesRead = offset;
      }

      public ValueTask<int> ReadAsync(Memory<byte> buffer, bool waitUntilFull = false, CancellationToken cancellationToken = default)
      {
         if (disposed) { return ValueTask.FromResult(0); }
         var canRead = memory.Length - totalBytesRead;
         if (canRead > buffer.Length) { canRead = buffer.Length; }
         memory.Slice(totalBytesRead, canRead).CopyTo(buffer);
         Interlocked.Add(ref totalBytesRead, canRead);
         return ValueTask.FromResult(canRead);
      }

      public ValueTask<int> ReadAsync(int skipBytes, CancellationToken cancellationToken = default)
      {
         if (disposed) { return ValueTask.FromResult(0); }
         var canRead = memory.Length - totalBytesRead;
         if (canRead > skipBytes) { canRead = skipBytes; }
         Interlocked.Add(ref totalBytesRead, canRead);
         return ValueTask.FromResult(canRead);
      }

      public ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default)
      {
         if (disposed || totalBytesRead >= memory.Length) { return ValueTask.FromResult(-1); }
         var result = memory.Span[totalBytesRead];
         Interlocked.Increment(ref totalBytesRead);
         return ValueTask.FromResult((int)result);
      }

      public void Dispose()
      {
         disposed = true;
      }
   }
}
