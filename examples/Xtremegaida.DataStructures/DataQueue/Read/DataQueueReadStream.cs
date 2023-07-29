using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueReadStream : Stream
   {
      private readonly IDataQueueReader owner;
      private readonly bool keepReaderOpen;

      public override bool CanRead => true;
      public override bool CanSeek => false;
      public override bool CanWrite => false;
      public override long Length => owner.TotalBytesRead + owner.UnreadLength;
      public override long Position { get => owner.TotalBytesRead; set => throw new NotSupportedException(); }

      public DataQueueReadStream(IDataQueueReader owner, bool keepReaderOpen = false)
      {
         this.owner = owner;
         this.keepReaderOpen = keepReaderOpen;
      }

      public override void Flush() { }
      public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
      public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
      public override void SetLength(long value) { throw new NotSupportedException(); }

      public override int Read(byte[] buffer, int offset, int count)
      {
         var task = owner.ReadAsync(new Memory<byte>(buffer, offset, count));
         if (task.IsCompletedSuccessfully) { return task.Result; }
         return task.AsTask().Result;
      }

      public override int ReadByte()
      {
         var task = owner.ReadByteAsync();
         if (task.IsCompletedSuccessfully) { return task.Result; }
         return task.AsTask().Result;
      }

      public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
      {
         var task = owner.ReadAsync(new Memory<byte>(buffer, offset, count), false, cancellationToken);
         if (task.IsCompletedSuccessfully) { return Task.FromResult(task.Result); }
         return task.AsTask();
      }

      public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
      {
         return owner.ReadAsync(buffer, false, cancellationToken);
      }

      public override void Close()
      {
         if (!keepReaderOpen) { owner.Dispose(); }
      }
   }
}
