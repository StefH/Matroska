using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   public sealed class DataQueue : IDataQueue, IDataQueueReader, IDataQueueWriter, IDisposable
   {
      private readonly DataBufferCache cache;
      private readonly ConcurrentQueue<DataBuffer> bufferQueue = new();
      private EventWaitLight readWait;
      private EventWaitLight writeWait;
      private EventWaitLight closedWait;
      private EventWaitLight emptyWait;
#if DEBUG
      private SpinLock readLock = new(true);
      private SpinLock writeLock = new(true);
#else
      private SpinLock readLock = new(false);
      private SpinLock writeLock = new(false);
#endif
      private long currentCapacity;
      private long maxCapacity;
      private DataBuffer headRead;
      private DataBuffer tailWrite;
      private DataQueueReadStream readStream;
      private DataQueueWriteStream writeStream;
      private IDataQueueWriter forward;
      private volatile bool closedRead;
      private volatile bool closedWrite;
      private volatile bool disposed;
      private long bytesWritten;
      private long bytesRead;

      const int maxPacketSize = 65536;

      public long MaxCapacityBytes
      {
         get { return maxCapacity; }
         set { maxCapacity = value; writeWait.Trigger(); }
      }

      public long UnreadLength => currentCapacity;
      public bool IsReadClosed => closedRead;
      public bool IsWriteClosed => closedWrite;
      public long TotalBytesWritten => bytesWritten;
      public long TotalBytesRead => bytesRead;

      public IDataQueueReader Reader => this;
      public IDataQueueWriter Writer => this;
      public Stream ReadStream => readStream ??= new DataQueueReadStream(this);
      public Stream WriteStream => writeStream ??= new DataQueueWriteStream(this);

      public event Action OnBlockWrite;
      public event Action OnBlockRead;

      public DataQueue() : this(maxPacketSize * 4, null) { }

      public DataQueue(int maxCapacity, DataBufferCache cache = null)
      {
         this.maxCapacity = maxCapacity;
         this.cache = cache ?? DataBufferCache.DefaultCache;
      }

#region Read

      public async ValueTask<int> ReadByteAsync(CancellationToken cancellationToken = default)
      {
         bool locked = false;
         try
         {
            readLock.Enter(ref locked);
            while (true)
            {
               if (closedRead) { return -1; }
               if (headRead == null || headRead.ReadOffset >= headRead.Buffer.Length)
               {
                  if (headRead != null) { RecycleHead(); }
                  bufferQueue.TryDequeue(out headRead);
               }
               if (headRead == null || headRead.ReadOffset >= headRead.WriteOffset)
               {
                  if (closedWrite) { return -1; }
                  var task = readWait.WaitAsync(cancellationToken);
                  if (locked) { locked = false; readLock.Exit(); }
                  writeWait.Trigger();
                  emptyWait.Trigger();
                  OnBlockRead?.Invoke();
                  await task;
                  readLock.Enter(ref locked);
               }
               else
               {
                  var result = headRead.Buffer[headRead.ReadOffset++];
                  if (headRead.ReadOffset >= headRead.Buffer.Length) { RecycleHead(); }
                  Interlocked.Decrement(ref currentCapacity);
                  bytesRead++;
                  if (locked) { locked = false; readLock.Exit(); }
                  writeWait.Trigger();
                  return result;
               }
            }
         }
         finally
         {
            if (locked) { readLock.Exit(); }
         }
      }

      public async ValueTask<int> ReadAsync(Memory<byte> buffer, bool waitUntilFull = false, CancellationToken cancellationToken = default)
      {
         int readBytes = 0;
         bool locked = false;
         try
         {
            readLock.Enter(ref locked);
            while (!buffer.IsEmpty && !closedRead)
            {
               if (headRead == null || headRead.ReadOffset >= headRead.Buffer.Length)
               {
                  if (headRead != null) { RecycleHead(); }
                  bufferQueue.TryDequeue(out headRead);
               }
               if (headRead == null || headRead.ReadOffset >= headRead.WriteOffset)
               {
                  if ((!waitUntilFull && readBytes > 0) || closedWrite) { break; }
                  var task = readWait.WaitAsync(cancellationToken);
                  if (locked) { locked = false; readLock.Exit(); }
                  writeWait.Trigger();
                  emptyWait.Trigger();
                  OnBlockRead?.Invoke();
                  await task;
                  readLock.Enter(ref locked);
               }
               else
               {
                  var canRead = headRead.WriteOffset - headRead.ReadOffset;
                  if (canRead > buffer.Length) { canRead = buffer.Length; }
                  if (canRead > 0)
                  {
                     new Memory<byte>(headRead.Buffer, headRead.ReadOffset, canRead).CopyTo(buffer);
                     buffer = buffer.Slice(canRead);
                     headRead.ReadOffset += canRead;
                     readBytes += canRead;
                     Interlocked.Add(ref currentCapacity, -canRead);
                     bytesRead += canRead;
                     if (headRead.ReadOffset >= headRead.Buffer.Length) { RecycleHead(); }
                  }
               }
            }
         }
         finally
         {
            if (locked) { readLock.Exit(); }
         }
         if (readBytes > 0) { writeWait.Trigger(); }
         return readBytes;
      }

      public async ValueTask<int> ReadAsync(int skipBytes, CancellationToken cancellationToken = default)
      {
         int readBytes = 0;
         bool locked = false;
         try
         {
            readLock.Enter(ref locked);
            while (skipBytes > 0 && !closedRead)
            {
               if (headRead == null || headRead.ReadOffset >= headRead.Buffer.Length)
               {
                  if (headRead != null) { RecycleHead(); }
                  bufferQueue.TryDequeue(out headRead);
               }
               if (headRead == null || headRead.ReadOffset >= headRead.WriteOffset)
               {
                  if (closedWrite) { break; }
                  var task = readWait.WaitAsync(cancellationToken);
                  if (locked) { locked = false; readLock.Exit(); }
                  writeWait.Trigger();
                  emptyWait.Trigger();
                  OnBlockRead?.Invoke();
                  await task;
                  readLock.Enter(ref locked);
               }
               else
               {
                  var canRead = headRead.WriteOffset - headRead.ReadOffset;
                  if (canRead > skipBytes) { canRead = skipBytes; }
                  if (canRead > 0)
                  {
                     skipBytes -= canRead;
                     readBytes += canRead;
                     headRead.ReadOffset += canRead;
                     Interlocked.Add(ref currentCapacity, -canRead);
                     bytesRead += canRead;
                     if (headRead.ReadOffset >= headRead.Buffer.Length) { RecycleHead(); }
                  }
               }
            }
         }
         finally
         {
            if (locked) { readLock.Exit(); }
         }
         if (readBytes > 0) { writeWait.Trigger(); }
         return readBytes;
      }

      private void RecycleHead()
      {
         // Already in readLock
         if (tailWrite == headRead)
         {
            bool writeLocked = false;
            try
            {
               writeLock.Enter(ref writeLocked);
               if (tailWrite == headRead) { tailWrite = null; }
            }
            finally
            {
               if (writeLocked) { writeLock.Exit(); }
            }
         }
         headRead.Dispose();
         headRead = null;
      }

#endregion

#region Write

      public async ValueTask WriteByteAsync(byte data, CancellationToken cancellationToken = default)
      {
         if (forward != null)
         {
            await forward.WriteByteAsync(data, cancellationToken);
            bytesRead++; bytesWritten++;
            return;
         }
         bool locked = false;
         try
         {
            writeLock.Enter(ref locked);
            while (true)
            {
               if (closedWrite) { throw new InvalidOperationException(); }
               if (currentCapacity >= maxCapacity)
               {
                  var task = writeWait.WaitAsync(cancellationToken);
                  if (locked) { locked = false; writeLock.Exit(); }
                  readWait.Trigger();
                  OnBlockWrite?.Invoke();
                  await task;
                  writeLock.Enter(ref locked);
                  if (forward != null)
                  {
                     if (locked) { locked = false; writeLock.Exit(); }
                     await forward.WriteByteAsync(data, cancellationToken);
                     bytesRead++; bytesWritten++;
                     return;
                  }
               }
               else
               {
                  if (tailWrite == null || tailWrite.WriteOffset >= tailWrite.Buffer.Length)
                  {
                     tailWrite = cache.Pop(256);
                     bufferQueue.Enqueue(tailWrite);
                  }
                  tailWrite.Buffer[tailWrite.WriteOffset++] = data;
                  Interlocked.Increment(ref currentCapacity);
                  bytesWritten++;
                  if (locked) { locked = false; writeLock.Exit(); }
                  readWait.Trigger();
                  return;
               }
            }
         }
         finally
         {
            if (locked) { writeLock.Exit(); }
         }
      }

      public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
      {
         if (forward != null)
         {
            await forward.WriteAsync(buffer, cancellationToken);
            bytesRead += buffer.Length;
            bytesWritten += buffer.Length;
            return;
         }
         bool written = false;
         bool locked = false;
         try
         {
            writeLock.Enter(ref locked);
            while (!buffer.IsEmpty)
            {
               if (closedWrite) { throw new InvalidOperationException(); }
               if (currentCapacity >= maxCapacity)
               {
                  var task = writeWait.WaitAsync(cancellationToken);
                  if (locked) { locked = false; writeLock.Exit(); }
                  readWait.Trigger();
                  OnBlockWrite?.Invoke();
                  await task;
                  writeLock.Enter(ref locked);
                  if (forward != null)
                  {
                     if (locked) { locked = false; writeLock.Exit(); }
                     await forward.WriteAsync(buffer, cancellationToken);
                     bytesRead += buffer.Length;
                     bytesWritten += buffer.Length;
                     return;
                  }
               }
               else
               {
                  if (tailWrite == null || tailWrite.WriteOffset >= tailWrite.Buffer.Length)
                  {
                     var capacityLeft = maxCapacity - currentCapacity;
                     if (capacityLeft > buffer.Length) { capacityLeft = buffer.Length; }
                     tailWrite = cache.Pop((int)Math.Min(capacityLeft, maxPacketSize));
                     bufferQueue.Enqueue(tailWrite);
                  }
                  var canWrite = tailWrite.Buffer.Length - tailWrite.WriteOffset;
                  if (canWrite > buffer.Length) { canWrite = buffer.Length; }
                  if (canWrite > 0)
                  {
                     buffer.Slice(0, canWrite).CopyTo(new Memory<byte>(tailWrite.Buffer, tailWrite.WriteOffset, canWrite));
                     buffer = buffer.Slice(canWrite);
                     tailWrite.WriteOffset += canWrite;
                     Interlocked.Add(ref currentCapacity, canWrite);
                     bytesWritten += canWrite;
                     written = true;
                  }
               }
            }
         }
         finally
         {
            if (locked) { writeLock.Exit(); }
         }
         if (written) { readWait.Trigger(); }
      }

#endregion

      public void Clear(bool resetStats = true)
      {
         bool lockedRead = false, lockedWrite = false;
         try
         {
            readLock.Enter(ref lockedRead);
            writeLock.Enter(ref lockedWrite);
            if (resetStats)
            {
               bytesRead = 0;
               bytesWritten = 0;
            }
            tailWrite = null;
            if (headRead != null)
            {
               Interlocked.Add(ref currentCapacity, headRead.ReadOffset - headRead.WriteOffset);
               headRead.Dispose();
               headRead = null;
            }
            while (bufferQueue.TryDequeue(out var buffer))
            {
               Interlocked.Add(ref currentCapacity, buffer.ReadOffset - buffer.WriteOffset);
               buffer.Dispose();
            }
         }
         finally
         {
            if (lockedRead) { readLock.Exit(); }
            if (lockedWrite) { writeLock.Exit(); }
         }
         writeWait.Trigger();
         emptyWait.Trigger();
      }

      public byte[] ToArray(bool clear = false)
      {
         byte[] snapshot;
         bool lockedRead = false, lockedWrite = false;
         try
         {
            readLock.Enter(ref lockedRead);
            writeLock.Enter(ref lockedWrite);
            snapshot = new byte[currentCapacity];
            var snapshotSpan = new Span<byte>(snapshot);
            if (headRead != null)
            {
               var read = headRead.ReadSpan;
               read.CopyTo(snapshotSpan);
               snapshotSpan = snapshotSpan.Slice(read.Length);
               if (clear)
               {
                  Interlocked.Add(ref currentCapacity, headRead.ReadOffset - headRead.WriteOffset);
                  headRead.Dispose();
                  headRead = null;
               }
            }
            if (clear)
            {
               bytesRead = 0;
               bytesWritten = 0;
               tailWrite = null;
               while (bufferQueue.TryDequeue(out var buffer))
               {
                  var read = buffer.ReadSpan;
                  read.CopyTo(snapshotSpan);
                  snapshotSpan = snapshotSpan.Slice(read.Length);
                  Interlocked.Add(ref currentCapacity, buffer.ReadOffset - buffer.WriteOffset);
                  buffer.Dispose();
               }
            }
            else
            {
               foreach (var buffer in bufferQueue)
               {
                  var read = buffer.ReadSpan;
                  read.CopyTo(snapshotSpan);
                  snapshotSpan = snapshotSpan.Slice(read.Length);
               }
            }
         }
         finally
         {
            if (lockedRead) { readLock.Exit(); }
            if (lockedWrite) { writeLock.Exit(); }
         }
         if (clear) { emptyWait.Trigger(); }
         return snapshot;
      }

      public async ValueTask ForwardTo(IDataQueueWriter writer, CancellationToken cancellationToken = default)
      {
         if (closedRead) { return; }
         bool locked = false;
         readLock.Enter(ref locked);
         closedRead = true;
         if (locked) { locked = false; readLock.Exit(); }
         while (!disposed)
         {
            if (headRead == null || headRead.ReadOffset >= headRead.Buffer.Length)
            {
               if (headRead != null) { RecycleHead(); }
               bufferQueue.TryDequeue(out headRead);
            }
            if (headRead == null || headRead.ReadOffset >= headRead.WriteOffset)
            {
               if (closedWrite) { break; }
               try
               {
                  writeLock.Enter(ref locked);
                  if ((headRead == null || headRead.ReadOffset >= headRead.WriteOffset) && bufferQueue.Count == 0)
                  {
                     forward = writer;
                     if (headRead != null)
                     {
                        headRead.Dispose();
                        headRead = null;
                     }
                     tailWrite = null;
                     break;
                  }
               }
               finally
               {
                  if (locked) { locked = false; writeLock.Exit(); }
               }
               writeWait.Trigger();
               emptyWait.Trigger();
               OnBlockRead?.Invoke();
               await readWait.WaitAsync(cancellationToken);
            }
            else
            {
               var canRead = headRead.WriteOffset - headRead.ReadOffset;
               await writer.WriteAsync(new ReadOnlyMemory<byte>
                  (headRead.Buffer, headRead.ReadOffset, canRead), cancellationToken);
               headRead.ReadOffset += canRead;
               Interlocked.Add(ref currentCapacity, -canRead);
               bytesRead += canRead;
               if (headRead.ReadOffset >= headRead.Buffer.Length) { RecycleHead(); }
            }
         }
         writeWait.Trigger();
         emptyWait.Trigger();
         await WaitForClose(cancellationToken);
      }

      public async ValueTask WaitForEmpty(CancellationToken cancellationToken = default)
      {
         bool locked = false;
         try
         {
            readLock.Enter(ref locked);
            while (!disposed)
            {
               if (forward != null) { return; }
               if ((headRead == null || headRead.ReadOffset >= headRead.WriteOffset) && bufferQueue.Count == 0) { return; }
               if (locked) { locked = false; readLock.Exit(); }
               await emptyWait.WaitAsync(cancellationToken);
               readLock.Enter(ref locked);
            }
         }
         finally
         {
            if (locked) { readLock.Exit(); }
         }
      }

      public Task WaitForClose(CancellationToken cancellationToken = default)
      {
         return closedWait.WaitAsync(cancellationToken);
      }

      public void CloseWrite()
      {
         if (closedWrite) { return; }
         closedWrite = true;
         writeWait.Trigger();
         bool locked = false;
         writeLock.Enter(ref locked);
         tailWrite = null;
         if (locked) { writeLock.Exit(); }
         readWait.Trigger();
         closedWait.Trigger(null, true);
      }

      public void Dispose()
      {
         if (disposed) { return; }
         disposed = true;
         closedRead = true;
         CloseWrite();
         Clear(false);
      }
   }
}
