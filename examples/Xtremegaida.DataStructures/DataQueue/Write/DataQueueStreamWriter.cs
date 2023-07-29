using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueStreamWriter : IDataQueueWriter, IDisposable, IAsyncDisposable
   {
      private readonly Stream baseStream;
      private readonly bool keepStreamOpen;
      private volatile bool disposed;
      private long totalBytesWritten;
      private byte[] byteWrite;

      public bool IsWriteClosed => disposed;
      public long TotalBytesWritten => totalBytesWritten;

      public DataQueueStreamWriter(Stream baseStream, bool keepStreamOpen = false)
      {
         this.baseStream = baseStream;
         this.keepStreamOpen = keepStreamOpen;
      }

      public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
      {
         Interlocked.Add(ref totalBytesWritten, buffer.Length);
         return baseStream.WriteAsync(buffer, cancellationToken);
      }

      public ValueTask WriteByteAsync(byte data, CancellationToken cancellationToken = default)
      {
         if (byteWrite == null) { byteWrite = new byte[1]; }
         byteWrite[0] = data;
         Interlocked.Increment(ref totalBytesWritten);
         return baseStream.WriteAsync(byteWrite, cancellationToken);
      }

      public void CloseWrite()
      {
         Dispose();
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
