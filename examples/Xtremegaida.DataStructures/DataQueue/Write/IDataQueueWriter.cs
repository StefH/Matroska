using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public interface IDataQueueWriter
   {
      bool IsWriteClosed { get; }
      long TotalBytesWritten { get; }
      ValueTask WriteByteAsync(byte data, CancellationToken cancellationToken = default);
      ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
      void CloseWrite();
   }
}
