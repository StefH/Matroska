using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public sealed class EBMLReader : IDisposable
   {
      private readonly Dictionary<ulong, EBMLElementDefiniton> elementTypes = new Dictionary<ulong, EBMLElementDefiniton>();
      private readonly DataBufferCache cache;
      private readonly IDataQueueReader reader;
      private readonly Stream stream;
      private readonly long startOffset;
      private readonly bool keepReaderOpen;
      private readonly byte[] readTmp = new byte[8];
      private readonly List<ElementLevel> masterBlocks = new();
      private readonly DataQueueLimitedReaderMutable blockLimitedReader = new();
      private volatile bool disposed;
      private int maxInlineBinarySize = 4096;
      private ElementLevel currentLevel;
      private EBMLElement lastCachedElement;

      public EBMLMasterElement CurrentCachedContainer => currentLevel?.CachedElement;
      public EBMLElement LastCachedElement => lastCachedElement;
      public long CurrentReaderOffset => reader.TotalBytesRead + startOffset;
      public bool CanSeek => stream?.CanSeek ?? false;

      public int MaxInlineBinarySize
      {
         get { return maxInlineBinarySize; }
         set { maxInlineBinarySize = Math.Max(value, 0); }
      }

      private IDataQueueReader currentLevelReader => currentLevel?.BlockLimitedReader ?? reader;

      private class ElementLevel
      {
         public readonly int Level;
         public readonly DataQueueLimitedReaderMutable BlockLimitedReader = new();
         public EBMLElementDefiniton Definition;
         public EBMLMasterElement CachedElement;
         public EBMLVInt DataSize;
         public long DataOffset;

         public ElementLevel(int level) { Level = level; }

         public void Clear()
         {
            CachedElement?.MarkAsFullyRead();
            Definition = null;
         }
      }

      public EBMLReader(IDataQueueReader reader, bool keepReaderOpen = false, DataBufferCache cache = null)
      {
         this.reader = reader;
         this.keepReaderOpen = keepReaderOpen;
         this.cache = cache ?? DataBufferCache.DefaultCache;
         EBMLElementDefiniton.AddGlobalElements(this);
      }

      public EBMLReader(Stream stream, bool keepStreamOpen = false, DataBufferCache cache = null)
         : this(new DataQueueStreamReader(stream, keepStreamOpen), false, cache)
      {
         this.stream = stream;
         if (stream.CanSeek) { startOffset = stream.Position; }
      }

      public void AddElementDefinition(EBMLElementDefiniton def)
      {
         elementTypes.Add(def.Id.ValueWithMarker, def);
      }

      public EBMLElementDefiniton GetElementDefinition(ulong id)
      {
         if (elementTypes.TryGetValue(id, out var def)) { return def; }
         return null;
      }

      private async ValueTask<EBMLElementStruct> ReadSignedInteger(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue || size.Value > 8) { return default; }
         var offset = CurrentReaderOffset;
         long value = 0;
         for (int i = (int)size.Value; i > 0; i--)
         {
            var read = await reader.ReadByteAsync(cancellationToken);
            if (read < 0) { return default; }
            value = (value << 8) | (byte)read;
         }
         var shift = 64 - ((int)size.Value << 3);
         value = (value << shift) >> shift;
         return new EBMLElementStruct(def, size, offset)
         {
            Value = new EBMLElementStructValue() { SignedInteger = value }
         };
      }

      private async ValueTask<EBMLElementStruct> ReadUnsignedInteger(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue || size.Value > 8) { return default; }
         var offset = CurrentReaderOffset;
         ulong value = 0;
         for (int i = (int)size.Value; i > 0; i--)
         {
            var read = await reader.ReadByteAsync(cancellationToken);
            if (read < 0) { return default; }
            value = (value << 8) | (byte)read;
         }
         return new EBMLElementStruct(def, size, offset)
         {
            Value = new EBMLElementStructValue() { UnsignedInteger = value }
         };
      }

      private async ValueTask<EBMLElementStruct> ReadFloat(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue) { return default; }
         var offset = CurrentReaderOffset;
         if (size.Value == 8)
         {
            await reader.ReadAsync(new Memory<byte>(readTmp, 0, 8), true, cancellationToken);
            var value = System.Buffers.Binary.BinaryPrimitives.ReadDoubleBigEndian(readTmp);
            return new EBMLElementStruct(def, size, offset)
            {
               Value = new EBMLElementStructValue() { Float64 = value }
            };
         }
         if (size.Value == 4)
         {
            await reader.ReadAsync(new Memory<byte>(readTmp, 0, 4), true, cancellationToken);
            var value = System.Buffers.Binary.BinaryPrimitives.ReadSingleBigEndian(readTmp);
            return new EBMLElementStruct(def, size, offset)
            {
               Value = new EBMLElementStructValue() { Float32 = value },
               UseFloat32 = true
            };
         }
         return default;
      }
      
      private async ValueTask<EBMLElementStruct> ReadString(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue) { return default; }
         var offset = CurrentReaderOffset;
         int readBytes = (int)size.Value;
         if (readBytes == 0) { return new EBMLElementStruct(def, size, offset) { String = string.Empty }; }
         using (var buffer = cache.Pop(readBytes))
         {
            readBytes = await reader.ReadAsync(new Memory<byte>(buffer.Buffer, 0, readBytes), true, cancellationToken);
            var str = (def.Type == EBMLElementType.UTF8 ? Encoding.UTF8 : Encoding.ASCII).GetString(buffer.Buffer, 0, readBytes);
            return new EBMLElementStruct(def, size, offset) { String = str };
         }
      }

      private async ValueTask<EBMLElementStruct> ReadDate(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue) { return default; }
         var offset = CurrentReaderOffset;
         if (size.Value == 0)
         {
            return new EBMLElementStruct(def, size, offset)
            {
               Value = new EBMLElementStructValue() { Date = EBMLDateElement.Epoch }
            };
         }
         if (size.Value == 8)
         {
            await reader.ReadAsync(new Memory<byte>(readTmp, 0, 8), true, cancellationToken);
            var value = System.Buffers.Binary.BinaryPrimitives.ReadInt64BigEndian(readTmp);
            return new EBMLElementStruct(def, size, offset)
            {
               Value = new EBMLElementStructValue() { Date = new DateTime(EBMLDateElement.Epoch.Ticks + (value / 100), DateTimeKind.Utc) }
            };
         }
         return default;
      }

      private async ValueTask<EBMLElementStruct> ReadBinary(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue) { return default; }
         var offset = CurrentReaderOffset;
         if (size.Value == 0) { return new EBMLElementStruct(def, size, offset); }
         if (size.Value <= (ulong)maxInlineBinarySize)
         {
            var bytes = new byte[(int)size.Value];
            await reader.ReadAsync(bytes, true, cancellationToken);
            return new EBMLElementStruct(def, size, offset) { Binary = bytes };
         }
         blockLimitedReader.SetReadSource(reader, (long)size.Value);
         return new EBMLElementStruct(def, size, offset) { Reader = blockLimitedReader };
      }

      private async ValueTask<EBMLElementStruct> ReadVoid(CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         if (size.IsUnknownValue) { return default; }
         var offset = CurrentReaderOffset;
         if (size.Value == 0) { return new EBMLElementStruct(EBMLElementDefiniton.Void, size, offset); }
         if (size.Value <= (ulong)maxInlineBinarySize)
         {
            await reader.ReadAsync((int)size.Value, cancellationToken);
            return new EBMLElementStruct(EBMLElementDefiniton.Void, size, offset);
         }
         blockLimitedReader.SetReadSource(reader, (long)size.Value);
         return new EBMLElementStruct(EBMLElementDefiniton.Void, size, offset) { Reader = blockLimitedReader };
      }

      private async ValueTask<EBMLElementStruct> ReadMaster(EBMLElementDefiniton def, CancellationToken cancellationToken = default)
      {
         var reader = currentLevelReader;
         var size = await EBMLVInt.Read(reader, cancellationToken);
         if (size.IsEmpty) { return default; }
         return new EBMLElementStruct(def, size, CurrentReaderOffset) { Reader = reader };
      }

      public async ValueTask<EBMLElementStruct> ReadNextElementRaw(bool cacheElement = true, CancellationToken cancellationToken = default)
      {
         if (blockLimitedReader.CanReadLength > 0)
         {
            await blockLimitedReader.ReadAsync((int)blockLimitedReader.CanReadLength, cancellationToken);
         }
         while (true)
         {
            var id = await EBMLVInt.Read(currentLevelReader, cancellationToken);
            if (id.IsEmpty) { if (currentLevel == null) { return default; } PopLevel(); continue; }
            elementTypes.TryGetValue(id.ValueWithMarker, out var def);
            EBMLElementStruct element;
            if (def != null)
            {
               while (currentLevel != null && currentLevel.BlockLimitedReader.IsUnknownSize && !def.IsDirectChildOf(currentLevel.Definition)) { PopLevel(); }
               if (def == EBMLElementDefiniton.Void) { element = await ReadVoid(cancellationToken); }
               else switch (def.Type)
               {
                  case EBMLElementType.SignedInteger: element = await ReadSignedInteger(def, cancellationToken); break;
                  case EBMLElementType.UnsignedInteger: element = await ReadUnsignedInteger(def, cancellationToken); break;
                  case EBMLElementType.Float: element = await ReadFloat(def, cancellationToken); break;
                  case EBMLElementType.String: case EBMLElementType.UTF8: element = await ReadString(def, cancellationToken); break;
                  case EBMLElementType.Date: element = await ReadDate(def, cancellationToken); break;
                  case EBMLElementType.Master: element = await ReadMaster(def, cancellationToken); break;
                  case EBMLElementType.Binary: default: element = await ReadBinary(def, cancellationToken); break;
               }
            }
            else
            {
               element = await ReadBinary(EBMLElementDefiniton.Unknown, cancellationToken);
            }
            if (element.Definition == null) { return element; }
            lastCachedElement = null;
            if (cacheElement)
            {
               lastCachedElement = element.ToElement();
               if (currentLevel != null)
               {
                  if (currentLevel.CachedElement == null) { CacheCurrentContainerTree(); }
                  currentLevel.CachedElement.AddChild(lastCachedElement);
               }
            }
            if (element.Definition.Type == EBMLElementType.Master)
            {
               var newLevel = (currentLevel?.Level ?? -1) + 1;
               if (masterBlocks.Count <= newLevel) { masterBlocks.Add(new ElementLevel(masterBlocks.Count)); }
               currentLevel = masterBlocks[newLevel];
               currentLevel.BlockLimitedReader.SetReadSource(element.Reader, element.DataSize.IsUnknownValue ? -1 : (long)element.DataSize.Value);
               currentLevel.Definition = element.Definition;
               currentLevel.DataSize = element.DataSize;
               currentLevel.DataOffset = element.DataOffset;
               currentLevel.CachedElement = cacheElement ? (EBMLMasterElement)lastCachedElement : null;
            }
            while (currentLevel != null && currentLevel.BlockLimitedReader.IsReadClosed) { PopLevel(); }
            return element;
         }
      }

      public async ValueTask<EBMLElement> ReadNextElement(bool cacheElement = true, CancellationToken cancellationToken = default)
      {
         var element = await ReadNextElementRaw(cacheElement, cancellationToken);
         if (element.Definition == null) { return null; }
         return lastCachedElement;
      }

      public void CacheCurrentContainerTree()
      {
         int level = (currentLevel?.Level ?? -1) + 1;
         for (int i = 0; i < level; i++)
         {
            var obj = masterBlocks[i];
            if (obj.CachedElement == null)
            {
               obj.CachedElement = new EBMLMasterElement(obj.Definition, obj.DataSize, obj.DataOffset);
               if (i > 0) { masterBlocks[i - 1].CachedElement.AddChild(obj.CachedElement); }
            }
         }
      }

      private void PopLevel()
      {
         currentLevel.Clear();
         if (currentLevel.Level == 0) { currentLevel = null; }
         else { currentLevel = masterBlocks[currentLevel.Level - 1]; }
      }

      public void Dispose()
      {
         if (disposed) { return; }
         disposed = true;
         while (currentLevel != null) { PopLevel(); }
         masterBlocks.Clear();
         if (!keepReaderOpen) { reader.Dispose(); }
      }
   }
}
