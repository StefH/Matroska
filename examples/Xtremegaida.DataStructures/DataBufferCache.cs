using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Xtremegaida.DataStructures
{
   public class DataBufferCache
   {
      public static readonly DataBufferCache DefaultCache = new DataBufferCache();

      private readonly ConcurrentQueue<DataBuffer> bucket0 = new();
      private readonly ConcurrentQueue<DataBuffer> bucket1 = new();
      private readonly ConcurrentQueue<DataBuffer> bucket2 = new();
      private readonly ConcurrentQueue<DataBuffer>[] buckets;
      private long memReserve;
      private long memReserveMax = 32 << 20; // 32MiB
      private long memUsed;
      private int memMinSize = 256;
      private int memMaxSize = 262144;
      private int bucketMaxSize0 = 1024;
      private int bucketMaxSize1 = 32768;

      public int BufferMinSize
      {
         get { return memMinSize; }
         set
         {
            var size = value;
            if (size <= 16) { size = 16; }
            if (size > 65536) { size = 65536; }
            memMinSize = size;
            for (int i = buckets.Length - 1; i >= 0; i--)
            {
               var bucket = buckets[i];
               var itemCount = bucket.Count;
               while (--itemCount >= 0 && bucket.TryDequeue(out var buffer))
               {
                  if (buffer.Buffer.Length >= size) { bucket.Enqueue(buffer); }
                  else { Interlocked.Add(ref memReserve, -buffer.Buffer.Length); }
               }
            }
         }
      }

      public int BufferMaxSize
      {
         get { return memMaxSize; }
         set
         {
            var size = value;
            if (size <= 256) { size = 256; }
            memMaxSize = size;
            for (int i = buckets.Length - 1; i >= 0; i--)
            {
               var bucket = buckets[i];
               var itemCount = bucket.Count;
               while (--itemCount >= 0 && bucket.TryDequeue(out var buffer))
               {
                  if (buffer.Buffer.Length <= size) { bucket.Enqueue(buffer); }
                  else { Interlocked.Add(ref memReserve, -buffer.Buffer.Length); }
               }
            }
         }
      }

      public long BufferMemoryReserveBytes => Interlocked.Read(ref memReserve);
      public long BufferMemoryUsedBytes => Interlocked.Read(ref memUsed);
      public long BufferMemoryMaxBytes
      {
         get { return Interlocked.Read(ref memReserveMax); }
         set
         {
            Interlocked.Exchange(ref memReserveMax, value);
            if (BufferMemoryUsedBytes > BufferMemoryMaxBytes)
            {
               for (int i = buckets.Length - 1; i >= 0 && BufferMemoryUsedBytes > BufferMemoryMaxBytes; i--)
               {
                  while (BufferMemoryUsedBytes > BufferMemoryMaxBytes && buckets[i].TryDequeue(out var buffer))
                  {
                     Interlocked.Add(ref memReserve, -buffer.Buffer.Length);
                  }
               }
            }
         }
      }

      public DataBufferCache()
      {
         buckets = new ConcurrentQueue<DataBuffer>[] { bucket0, bucket1, bucket2 };
      }

      internal void _PushInternal(DataBuffer buffer)
      {
         if (buffer.References != 0) { throw new InvalidOperationException(); }
         var length = buffer.Buffer.Length;
         if (length < memMinSize || length > memMaxSize) { return; }
         if ((BufferMemoryUsedBytes + length) > BufferMemoryMaxBytes) { return; }
         buffer.ReadOffset = 0;
         buffer.WriteOffset = 0;
         if (length > bucketMaxSize1) { bucket2.Enqueue(buffer); }
         else if (length > bucketMaxSize0) { bucket1.Enqueue(buffer); }
         else { bucket0.Enqueue(buffer); }
         Interlocked.Add(ref memReserve, length);
         Interlocked.Add(ref memUsed, -length);
      }

      public DataBuffer Pop(int minLength)
      {
         var bucket = minLength >= bucketMaxSize1 ? bucket2 : minLength >= bucketMaxSize0 ? bucket1 : bucket0;
         var itemCount = bucket.Count;
         while (--itemCount >= 0 && bucket.TryDequeue(out var buffer))
         {
            var length = buffer.Buffer.Length;
            if (length < minLength)
            {
#if DEBUG
               System.Diagnostics.Debug.Assert(buffer.WriteOffset == 0);
               System.Diagnostics.Debug.Assert(buffer.ReadOffset == 0);
               System.Diagnostics.Debug.Assert(buffer.References == 0);
#endif
               bucket.Enqueue(buffer);
            }
            else
            {
               Interlocked.Add(ref memReserve, -length);
               Interlocked.Add(ref memUsed, length);
#if DEBUG
               System.Diagnostics.Debug.Assert(buffer.WriteOffset == 0);
               System.Diagnostics.Debug.Assert(buffer.ReadOffset == 0);
               System.Diagnostics.Debug.Assert(buffer.References == 0);
#endif
               buffer._ResetInternal();
               return buffer;
            }
         }
         if (minLength < memMinSize) { minLength = memMinSize; }
         Interlocked.Add(ref memUsed, minLength);
         return new DataBuffer(this, new byte[minLength]);
      }

      public void Clear()
      {
         for (int i = buckets.Length - 1; i >= 0; i--)
         {
            var bucket = buckets[i];
            while (bucket.TryDequeue(out var buffer))
            {
               Interlocked.Add(ref memReserve, -buffer.Buffer.Length);
            }
         }
      }
   }

   public sealed class DataBuffer : IDisposable
   {
      public readonly DataBufferCache Owner;
      public readonly byte[] Buffer;
      public int ReadOffset;
      public int WriteOffset;

      private int refCount;

      public int References => refCount;
      public int UnreadSize => WriteOffset - ReadOffset;
      public int SpaceLeft => Buffer.Length - WriteOffset;
      public Span<byte> ReadSpan => new Span<byte>(Buffer, ReadOffset, WriteOffset - ReadOffset);
      public Span<byte> WriteSpan => new Span<byte>(Buffer, WriteOffset, Buffer.Length - WriteOffset);
      public ReadOnlyMemory<byte> ReadMemory => new ReadOnlyMemory<byte>(Buffer, ReadOffset, WriteOffset - ReadOffset);
      public Memory<byte> WriteMemory => new Memory<byte>(Buffer, WriteOffset, Buffer.Length - WriteOffset);

      public DataBuffer(DataBufferCache owner, byte[] buffer)
      {
         Owner = owner;
         Buffer = buffer;
         refCount = 1;
      }

      public void AddReference()
      {
         Interlocked.Increment(ref refCount);
      }

      internal void _ResetInternal()
      {
         ReadOffset = 0;
         WriteOffset = 0;
         refCount = 1;
      }

      public void Dispose()
      {
         if (Interlocked.Decrement(ref refCount) <= 0)
         {
            ReadOffset = 0;
            WriteOffset = 0;
            Owner?._PushInternal(this);
         }
      }
   }
}
