using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public class DataQueueWriteStream : Stream
   {
      private readonly IDataQueueWriter owner;
      private readonly bool keepWriterOpen;

      public override bool CanRead => false;
      public override bool CanSeek => false;
      public override bool CanWrite => true;
      public override long Length => owner.TotalBytesWritten;
      public override long Position { get => owner.TotalBytesWritten; set => throw new NotSupportedException(); }

      public DataQueueWriteStream(IDataQueueWriter owner, bool keepWriterOpen = false)
      {
         this.owner = owner;
         this.keepWriterOpen = keepWriterOpen;
      }

      public override void Flush() { }
      public override int Read(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
      public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }
      public override void SetLength(long value) { throw new NotSupportedException(); }

      public override void Write(byte[] buffer, int offset, int count)
      {
         var task = owner.WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count));
         if (task.IsCompletedSuccessfully) { return; }
         task.AsTask().Wait();
      }

      public override void WriteByte(byte value)
      {
         var task = owner.WriteByteAsync(value);
         if (task.IsCompletedSuccessfully) { return; }
         task.AsTask().Wait();
      }

      public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
      {
         var task = owner.WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken);
         if (task.IsCompletedSuccessfully) { return Task.CompletedTask; }
         return task.AsTask();
      }

      public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
      {
         return owner.WriteAsync(buffer, cancellationToken);
      }

      public override void Close()
      {
         if (!keepWriterOpen) { owner.CloseWrite(); }
      }
   }
}
