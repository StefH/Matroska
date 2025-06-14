using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public interface IDataQueueReader : IDisposable
   {
      long UnreadLength { get; }
      bool IsReadClosed { get; }
      long TotalBytesRead { get; }
      ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default);
      ValueTask<int> ReadAsync(Memory<byte> buffer, bool waitUntilFull = false, CancellationToken cancellationToken = default);
      ValueTask<int> ReadAsync(int skipBytes, CancellationToken cancellationToken = default);
   }
}
