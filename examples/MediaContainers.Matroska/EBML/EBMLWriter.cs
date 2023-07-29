using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public sealed class EBMLWriter : IDisposable, IAsyncDisposable
   {
      private readonly DataBufferCache cache;
      private readonly IDataQueueWriter writer;
      private readonly Stream stream;
      private readonly bool keepWriterOpen;
      private readonly DataBuffer writeBuffer = new(null, new byte[4096]);
      private int writeBusy;
      private long startOffset;
      private bool rewindOnDispose;
      private volatile bool disposed;
      private Level level;

      const int maxMasterElementBufferSize = (1 << (24 - 3)) - 1;

      private class Level : IDisposable
      {
         public readonly Level Parent;
         public readonly EBMLElementDefiniton Definition;
         public readonly DataQueue Buffer;
         public readonly IDataQueueWriter ParentWriter;
         public readonly long RelativeDataOffset;
         public readonly bool CanSeek;
         public bool NeedSizeWritten;
         public int? DataSizeWidth;
         public Task ForwardTask;

         public Level(Level parent, EBMLElementDefiniton def, DataQueue buffer, IDataQueueWriter parentWriter, bool canSeek)
         {
            Parent = parent;
            Definition = def;
            Buffer = buffer;
            ParentWriter = parentWriter;
            CanSeek = canSeek;
            RelativeDataOffset = parentWriter.TotalBytesWritten;
            buffer.OnBlockWrite += OnBlockWriteEvent;
         }

         public Task ForwardBuffer(CancellationToken cancellationToken = default)
         {
            lock (this)
            {
               if (ForwardTask == null) { ForwardTask = ForwardAsync(); }
               if (cancellationToken.CanBeCanceled && !ForwardTask.IsCompleted) { return ForwardTask.WaitAsync(cancellationToken); }
               return ForwardTask;
            }
         }

         private void OnBlockWriteEvent()
         {
            ForwardBuffer();
         }

         private async Task ForwardAsync()
         {
            EBMLVInt sizeVInt;
            if (Buffer.IsWriteClosed)
            {
               sizeVInt = new EBMLVInt((ulong)Buffer.UnreadLength);
            }
            else
            {
               NeedSizeWritten = true;
               if (!CanSeek && Definition.AllowUnknownSize) { NeedSizeWritten = false; }
               sizeVInt = EBMLVInt.CreateUnknown(NeedSizeWritten ? 8 : 1);
            }
            DataSizeWidth = sizeVInt.WidthBytes;
            await sizeVInt.Write(ParentWriter);
            await Buffer.ForwardTo(ParentWriter);
         }

         public void Dispose()
         {
            Buffer.Dispose();
         }
      }

      public bool StreamingMode { get; set; }
      public bool WindowsCompatibilityMode { get; set; } = true;
      public EBMLElementDefiniton CurrentContainer => level?.Definition;
      public long CurrentContainerSize => currentLevelWriter.TotalBytesWritten + writeBuffer.UnreadSize;
      public bool CanSeek => !StreamingMode && (stream?.CanSeek ?? false);

      private IDataQueueWriter currentLevelWriter => level?.Buffer ?? writer;

      public EBMLWriter(IDataQueueWriter writer, bool keepWriterOpen = false, DataBufferCache cache = null)
      {
         this.writer = writer;
         this.keepWriterOpen = keepWriterOpen;
         this.cache = cache ?? DataBufferCache.DefaultCache;
      }

      public EBMLWriter(Stream stream, bool keepStreamOpen = false, DataBufferCache cache = null, bool rewindOnDispose = false)
         : this(new DataQueueStreamWriter(stream, keepStreamOpen), false, cache)
      {
         this.stream = stream;
         if (stream.CanSeek && rewindOnDispose)
         {
            startOffset = stream.Position;
            this.rewindOnDispose = true;
         }
      }

      public static int CalculateWidth(long value)
      {
         if (value == 0) { return 0; }
         for (int size = 1; size < 8; size++)
         {
            var shift = 64 - (size << 3);
            if (((value << shift) >> shift) == value) { return size; }
         }
         return 8;
      }

      public static int CalculateWidth(ulong value)
      {
         if (value == 0) { return 0; }
         for (int size = 1; size < 8; size++)
         {
            var mask = (ulong)(-1L << (size << 3));
            if ((mask & value) == 0) { return size; }
         }
         return 8;
      }

      public ValueTask FlushInternalBuffer(CancellationToken cancellationToken = default)
      {
         if (writeBuffer.UnreadSize == 0) { return ValueTask.CompletedTask; }
         var memory = new ReadOnlyMemory<byte>(writeBuffer.Buffer, writeBuffer.ReadOffset, writeBuffer.WriteOffset - writeBuffer.ReadOffset);
         writeBuffer.ReadOffset = 0; writeBuffer.WriteOffset = 0;
         return currentLevelWriter.WriteAsync(memory, cancellationToken);
      }

      public async ValueTask WriteSignedInteger(EBMLElementDefiniton def, long value, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.SignedInteger) { throw new ArgumentException("Element type mismatch", nameof(def)); }
         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var size = (value == 0 && WindowsCompatibilityMode) ? 1 : CalculateWidth(value);
            var sizeVInt = new EBMLVInt((ulong)size);
            if (writeBuffer.SpaceLeft < (def.Id.WidthBytes + sizeVInt.WidthBytes + size)) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            if (size == 0) { return; }
            for (int i = (size - 1) << 3; i >= 0; i -= 8)
            {
               writeBuffer.Buffer[writeBuffer.WriteOffset++] = (byte)((value >> i) & 0xff);
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteUnsignedInteger(EBMLElementDefiniton def, ulong value, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.UnsignedInteger) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var size = (value == 0 && WindowsCompatibilityMode) ? 1 : CalculateWidth(value);
            var sizeVInt = new EBMLVInt((ulong)size);
            if (writeBuffer.SpaceLeft < (def.Id.WidthBytes + sizeVInt.WidthBytes + size)) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            if (size == 0) { return; }
            for (int i = (size - 1) << 3; i >= 0; i -= 8)
            {
               writeBuffer.Buffer[writeBuffer.WriteOffset++] = (byte)((value >> i) & 0xff);
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteFloat(EBMLElementDefiniton def, float value, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.Float) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var sizeVInt = new EBMLVInt(4);
            if (writeBuffer.SpaceLeft < (def.Id.WidthBytes + sizeVInt.WidthBytes + 4)) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            System.Buffers.Binary.BinaryPrimitives.WriteSingleBigEndian(writeBuffer.WriteSpan, value);
            writeBuffer.WriteOffset += 4;
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteFloat(EBMLElementDefiniton def, double value, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.Float) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var sizeVInt = new EBMLVInt(8);
            if (writeBuffer.SpaceLeft < (def.Id.WidthBytes + sizeVInt.WidthBytes + 8)) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            System.Buffers.Binary.BinaryPrimitives.WriteDoubleBigEndian(writeBuffer.WriteSpan, value);
            writeBuffer.WriteOffset += 8;
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteDate(EBMLElementDefiniton def, DateTime date, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.Date) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var sizeVInt = new EBMLVInt(8);
            if (writeBuffer.SpaceLeft < (def.Id.WidthBytes + sizeVInt.WidthBytes + 8)) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            var value = (date.Ticks - EBMLDateElement.Epoch.Ticks) * 100;
            System.Buffers.Binary.BinaryPrimitives.WriteInt64BigEndian(writeBuffer.WriteSpan, value);
            writeBuffer.WriteOffset += 8;
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteString(EBMLElementDefiniton def, string str, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.String && def.Type != EBMLElementType.UTF8) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            if (str == null) { str = string.Empty; }
            var estimatedMaxSize = def.Id.WidthBytes + 8 + (def.Type != EBMLElementType.String ? str.Length : (str.Length * 4));
            if (writeBuffer.SpaceLeft < estimatedMaxSize) { await FlushInternalBuffer(cancellationToken); }
            if (writeBuffer.SpaceLeft >= estimatedMaxSize)
            {
               var originalOffset = writeBuffer.WriteOffset;
               writeBuffer.WriteOffset += def.Id.WidthBytes + 1;
               var size = (def.Type == EBMLElementType.UTF8 ? System.Text.Encoding.UTF8 : System.Text.Encoding.ASCII).GetBytes(str, writeBuffer.WriteSpan);
               var sizeVInt = new EBMLVInt((ulong)size);
               if (sizeVInt.WidthBytes > 1) { writeBuffer.WriteSpan.Slice(0, size).CopyTo(writeBuffer.WriteSpan.Slice(sizeVInt.WidthBytes - 1)); }
               writeBuffer.WriteOffset = originalOffset;
               def.Id.Write(writeBuffer);
               sizeVInt.Write(writeBuffer);
               writeBuffer.WriteOffset += size;
            }
            else
            {
               if (writeBuffer.UnreadSize > 0) { await FlushInternalBuffer(cancellationToken); }
               var writer = currentLevelWriter;
               using (var tmp = cache.Pop(str.Length * 4))
               {
                  var size = (def.Type == EBMLElementType.UTF8 ? System.Text.Encoding.UTF8 : System.Text.Encoding.ASCII).GetBytes(str, tmp.Buffer);
                  var sizeVInt = new EBMLVInt((ulong)size);
                  await def.Id.Write(writer, cancellationToken);
                  await sizeVInt.Write(writer, cancellationToken);
                  await writer.WriteAsync(new ReadOnlyMemory<byte>(tmp.Buffer, 0, size), cancellationToken);
               }
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteVoid(int bytes, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            if (bytes < 0) { bytes = 0; }
            var sizeVInt = new EBMLVInt((ulong)bytes);
            if (writeBuffer.SpaceLeft < (EBMLElementDefiniton.Void.Id.WidthBytes + sizeVInt.WidthBytes + 16)) { await FlushInternalBuffer(cancellationToken); }
            EBMLElementDefiniton.Void.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            while (bytes > 0)
            {
               if (writeBuffer.SpaceLeft < 16) { await FlushInternalBuffer(cancellationToken); }
               var canWrite = writeBuffer.SpaceLeft;
               if (canWrite > bytes) { canWrite = bytes; }
               writeBuffer.WriteSpan.Slice(0, canWrite).Clear();
               writeBuffer.WriteOffset += canWrite;
               bytes -= canWrite;
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteBinary(EBMLElementDefiniton def, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.Binary && def.Type != EBMLElementType.Unknown) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var sizeVInt = new EBMLVInt((ulong)bytes.Length);
            var estimatedMaxSize = def.Id.WidthBytes + sizeVInt.WidthBytes + bytes.Length;
            if (writeBuffer.SpaceLeft < estimatedMaxSize) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            if (bytes.Length == 0) { return; }

            var canWrite = writeBuffer.SpaceLeft;
            if (canWrite > bytes.Length) { canWrite = bytes.Length; }
            bytes.Slice(0, canWrite).Span.CopyTo(writeBuffer.WriteSpan);
            writeBuffer.WriteOffset += canWrite;
            if (bytes.Length > canWrite)
            {
               await FlushInternalBuffer(cancellationToken);
               await currentLevelWriter.WriteAsync(bytes.Slice(canWrite), cancellationToken);
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteBinary(EBMLElementDefiniton def, ReadOnlyMemory<byte> header, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.Binary && def.Type != EBMLElementType.Unknown) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            var sizeVInt = new EBMLVInt((ulong)(header.Length + bytes.Length));
            var estimatedMaxSize = def.Id.WidthBytes + sizeVInt.WidthBytes + header.Length + bytes.Length;
            if (writeBuffer.SpaceLeft < estimatedMaxSize) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            sizeVInt.Write(writeBuffer);
            if (header.Length == 0 && bytes.Length == 0) { return; }

            var canWrite = writeBuffer.SpaceLeft;
            if (canWrite > header.Length) { canWrite = header.Length; }
            header.Slice(0, canWrite).Span.CopyTo(writeBuffer.WriteSpan);
            writeBuffer.WriteOffset += canWrite;
            if (header.Length > canWrite)
            {
               await FlushInternalBuffer(cancellationToken);
               await currentLevelWriter.WriteAsync(header.Slice(canWrite), cancellationToken);
            }

            if (writeBuffer.UnreadSize == 0) { canWrite = 0; }
            else
            {
               canWrite = writeBuffer.SpaceLeft;
               if (canWrite > bytes.Length) { canWrite = bytes.Length; }
               bytes.Slice(0, canWrite).Span.CopyTo(writeBuffer.WriteSpan);
               writeBuffer.WriteOffset += canWrite;
            }
            if (bytes.Length > canWrite)
            {
               await FlushInternalBuffer(cancellationToken);
               await currentLevelWriter.WriteAsync(bytes.Slice(canWrite), cancellationToken);
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteBinary(EBMLElementDefiniton def, Stream bytes, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (bytes == null) { throw new ArgumentNullException(nameof(bytes)); }
         if (def.Type != EBMLElementType.Binary && def.Type != EBMLElementType.Unknown) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            if (writeBuffer.UnreadSize > 0) { await FlushInternalBuffer(cancellationToken); }
            var writer = currentLevelWriter;
            await def.Id.Write(writer, cancellationToken);
            if (bytes.CanSeek)
            {
               var size = new EBMLVInt((ulong)(bytes.Length - bytes.Position));
               await size.Write(writer, cancellationToken);
               if (writer is IDataQueue dq) { await bytes.CopyToAsync(dq.WriteStream, cancellationToken); }
               else
               {
                  using (var target = new DataQueueWriteStream(writer, true))
                  {
                     await bytes.CopyToAsync(target, cancellationToken);
                  }
               }
            }
            else
            {
               using (var mem = new MemoryStream())
               {
                  await bytes.CopyToAsync(mem, cancellationToken);
                  await new EBMLVInt((ulong)mem.Length).Write(writer, cancellationToken);
                  mem.Position = 0;
                  using (var target = new DataQueueWriteStream(writer, true)) { await mem.CopyToAsync(target, cancellationToken); }
               }
            }
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask WriteElement(EBMLElement element, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (element == null) { throw new ArgumentNullException(nameof(element)); }
         switch (element.ElementType)
         {
            case EBMLElementType.SignedInteger:
               await WriteSignedInteger(element.Definition, element.IntValue, cancellationToken); 
               break;
            case EBMLElementType.UnsignedInteger:
               await WriteUnsignedInteger(element.Definition, element.UIntValue, cancellationToken);
               break;
            case EBMLElementType.Float:
               if (element.DataSize.Value == 8) { await WriteFloat(element.Definition, element.DoubleValue, cancellationToken); }
               else { await WriteFloat(element.Definition, element.FloatValue, cancellationToken); }
               break;
            case EBMLElementType.Date:
               await WriteDate(element.Definition, element.DateValue ?? EBMLDateElement.Epoch, cancellationToken); 
               break;
            case EBMLElementType.String:
            case EBMLElementType.UTF8:
               await WriteString(element.Definition, element.StringValue, cancellationToken);
               break;
            case EBMLElementType.Binary:
            case EBMLElementType.Unknown:
               if (element is EBMLVoidElement)
               {
                  await WriteVoid((int)element.DataSize.Value, cancellationToken);
               }
               else
               {
                  var bin = (EBMLBinaryElement)element;
                  if (bin.Reader == null) { await WriteBinary(element.Definition, bin.Value, cancellationToken); }
                  else { await WriteBinary(element.Definition, new DataQueueReadStream(bin.Reader), cancellationToken); }
               }
               break;
            case EBMLElementType.Master:
               var master = (EBMLMasterElement)element;
               await BeginMasterElement(master.Definition, cancellationToken);
               foreach (var child in master.Children) { await WriteElement(child, cancellationToken); }
               await EndMasterElement(cancellationToken);
               break;
         }
      }

      public async ValueTask BeginMasterElement(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(EBMLWriter)); }
         if (def == null) { throw new ArgumentNullException(nameof(def)); }
         if (def.Type != EBMLElementType.Master) { throw new ArgumentException("Element type mismatch", nameof(def)); }

         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            if (writeBuffer.SpaceLeft < def.Id.WidthBytes) { await FlushInternalBuffer(cancellationToken); }
            def.Id.Write(writeBuffer);
            await FlushInternalBuffer(cancellationToken);
            level = new Level(level, def, new DataQueue(maxMasterElementBufferSize, cache), currentLevelWriter, CanSeek);
         }
         finally { Interlocked.Decrement(ref writeBusy); }
      }

      public async ValueTask EndMasterElement(CancellationToken cancellationToken = default)
      {
         if (level == null) { throw new InvalidOperationException(); }
         Level endedLevel = null;
         try
         {
            if (Interlocked.Increment(ref writeBusy) != 1) { throw new InvalidOperationException("Concurrent writes not allowed."); }
            await FlushInternalBuffer(cancellationToken);
            endedLevel = level;
            level = level.Parent;
            endedLevel.Buffer.CloseWrite();
            await endedLevel.ForwardBuffer(cancellationToken);
            if (endedLevel.NeedSizeWritten)
            {
               if (CanSeek)
               {
                  await FlushAsync(cancellationToken);
                  var blockSize = endedLevel.Buffer.TotalBytesWritten;
                  stream.Seek(-blockSize - 8, SeekOrigin.Current);
                  if ((blockSize & ~((1L << 56) - 1)) != 0) { throw new InvalidOperationException("Block size too large"); }
                  System.Buffers.Binary.BinaryPrimitives.WriteInt64BigEndian(writeBuffer.WriteSpan, blockSize | (1L << 56));
                  await stream.WriteAsync(writeBuffer.Buffer, writeBuffer.WriteOffset, 8, cancellationToken);
                  stream.Seek(blockSize, SeekOrigin.Current);
               }
               else if (!endedLevel.Definition.AllowUnknownSize)
               {
                  throw new InvalidOperationException("Block size unknown without seekable stream");
               }
            }
         }
         finally
         {
            Interlocked.Decrement(ref writeBusy);
            endedLevel?.Dispose();
         }
      }

      public struct RelativeOffset
      {
         private readonly EBMLElementDefiniton relativeTo;
         private readonly Level parent;
         private readonly long relOffset;
         private readonly bool inited;

         public bool Marked => inited;

         public RelativeOffset(EBMLWriter writer, EBMLElementDefiniton relativeTo)
         {
            this.relativeTo = relativeTo;
            parent = writer.level;
            relOffset = writer.currentLevelWriter.TotalBytesWritten + writer.writeBuffer.UnreadSize;
            inited = true;
         }

         public long? Resolve()
         {
            if (!inited) { return null; }
            var offset = relOffset;
            var current = parent;
            while (current != null && current.Definition != relativeTo)
            {
               if (current.DataSizeWidth == null)
               {
                  current.ForwardBuffer();
                  if (current.DataSizeWidth == null) { return null; }
               }
               offset += current.DataSizeWidth.Value + current.RelativeDataOffset;
               current = current.Parent;
            }
            if (current?.Definition != relativeTo)
            {
               throw new ArgumentException("Element not found in current hierarchy.", "relativeTo");
            }
            return offset;
         }
      }

      public RelativeOffset MarkRelativeOffset(EBMLElementDefiniton relativeTo = null)
      {
         return new RelativeOffset(this, relativeTo);
      }

      public async ValueTask FlushAsync(CancellationToken cancellationToken = default)
      {
         if (level != null)
         {
            var current = level; do { _ = current.ForwardBuffer(); } while ((current = current.Parent) != null);
            current = level; do { await current.Buffer.WaitForEmpty(cancellationToken); } while ((current = current.Parent) != null);
         }
         if (stream != null) { await stream.FlushAsync(cancellationToken); }
      }

      public async ValueTask<EBMLWriter> SeekWriter(long offset, SeekOrigin origin, CancellationToken cancellationToken = default)
      {
         if (!CanSeek) { throw new InvalidOperationException(); }
         await FlushAsync(cancellationToken);
         var writer = new EBMLWriter(stream, true, cache, true);
         stream.Seek(offset, origin);
         return writer;
      }

      public async ValueTask DisposeAsync()
      {
         if (disposed) { return; }
         disposed = true;
         while (level != null) { await EndMasterElement(); }
         await FlushInternalBuffer();
         if (rewindOnDispose) { stream.Seek(startOffset, SeekOrigin.Begin); }
         if (!keepWriterOpen)
         {
            writer.CloseWrite();
            if (writer is IAsyncDisposable a) { await a.DisposeAsync(); }
            else if (writer is IDisposable b) { b.Dispose(); }
         }
      }

      public void Dispose()
      {
         var task = DisposeAsync();
         if (!task.IsCompletedSuccessfully) { task.AsTask().Wait(); }
      }
   }
}
