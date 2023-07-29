using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers.Matroska
{
   public class MatroskaReader : IDisposable
   {
      private static readonly EBMLDocType[] docTypes = new EBMLDocType[]
      {
         new EBMLDocType("matroska", 4, 4),
         new EBMLDocType("webm", 4, 4),
      };

      private readonly DataBufferCache cache;
      private readonly EBMLDocumentReader document;
      private readonly MatroskaSeekHead seekHead = new();
      private readonly MatroskaInfo info = new();
      private readonly MatroskaTracks tracks = new();
      private readonly MatroskaTags tags = new();
      private readonly MatroskaCues cues = new();
      private readonly DataQueueMemoryReaderMutable memoryReader = new();
      private readonly byte[] readBytes = new byte[8];
      private MatroskaTrackEntry[] trackByNumber;
      private Queue<MatroskaFrame> frameQueue;
      private long clusterTimestamp;
      private int scanIndex;
      private int minTrackNumber;
      private int[] lacingSizes;
      private bool readTrackInfo;
      private volatile bool disposed;

      public virtual EBMLDocType[] SupportedDocTypes => docTypes;
      public EBMLDocumentReader Document => document;
      public MatroskaSeekHead SeekHead => seekHead;
      public MatroskaInfo Info => info;
      public MatroskaTracks Tracks => tracks;
      public MatroskaTags Tags => tags;
      public MatroskaCues Cues => cues;

      static MatroskaReader()
      {
         MatroskaSpecification.RegisterFormat();
      }

      public MatroskaReader(EBMLDocumentReader doc, DataBufferCache cache = null)
      {
         if (doc == null) { throw new ArgumentNullException(nameof(doc)); }
         var docTypes = SupportedDocTypes;
         var canRead = false;
         for (int i = docTypes.Length - 1; i >= 0; i--)
         {
            if (doc.CanBeReadBy(docTypes[i])) { canRead = true; break; }
         }
         if (!canRead)
         {
            throw new ArgumentException("Unsupported DocType: " + doc.Header.DocType + " " + doc.Header.DocTypeReadVersion, nameof(doc));
         }
         document = doc;
         if (doc.Body.Definition != MatroskaSpecification.Segment)
         {
            throw new ArgumentException("Expected Segment as document body", nameof(doc));
         }
         this.cache = cache ?? DataBufferCache.DefaultCache;
      }

      public static async ValueTask<MatroskaReader> Read(IDataQueueReader reader, bool keepReaderOpen = false,
         DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         var doc = await EBMLDocumentReader.Read(reader, keepReaderOpen, cache, cancellationToken);
         var matroska = new MatroskaReader(doc, cache);
         await matroska.ReadTrackInfo(cancellationToken);
         return matroska;
      }

      public static async ValueTask<MatroskaReader> Read(Stream stream, bool keepStreamOpen = false,
         DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         var doc = await EBMLDocumentReader.Read(stream, keepStreamOpen, cache, cancellationToken);
         var matroska = new MatroskaReader(doc, cache);
         await matroska.ReadTrackInfo(cancellationToken);
         return matroska;
      }

      public static bool CanReadDocument(EBMLDocumentReader doc)
      {
         for (int i = docTypes.Length - 1; i >= 0; i--)
         {
            if (doc.CanBeReadBy(docTypes[i])) { return true; }
         }
         return false;
      }

      public void ScanTrackInfo()
      {
         var body = document.Body;
         for  (; scanIndex < body.Children.Length; scanIndex++)
         {
            var element = body.Children[scanIndex];
            if (element.Definition == MatroskaSpecification.SeekHead)
            {
               seekHead.AddSeekIndex(document.Reader, element as EBMLMasterElement, document.Body.DataOffset);
            }
            else if (element.Definition == MatroskaSpecification.Info)
            {
               info.ReadFrom(element as EBMLMasterElement);
            }
            else if (element.Definition == MatroskaSpecification.Tracks)
            {
               tracks.AddTrackEntry(element as EBMLMasterElement);
            }
            else if (element.Definition == MatroskaSpecification.Tags)
            {
               tags.AddTagEntry(element as EBMLMasterElement);
            }
            else if (element.Definition == MatroskaSpecification.Cues)
            {
               cues.AddCueEntry(element as EBMLMasterElement, document.Body.DataOffset);
            }
         }
         if (tracks.Count > 0)
         {
            int maxTrackNumber = tracks[0].TrackNumber;
            minTrackNumber = maxTrackNumber;
            for (int i = tracks.Count - 1; i >= 0; i--)
            {
               if (maxTrackNumber < tracks[i].TrackNumber) { maxTrackNumber = tracks[i].TrackNumber; }
               if (minTrackNumber > tracks[i].TrackNumber) { minTrackNumber = tracks[i].TrackNumber; }
            }
            var range = maxTrackNumber + 1 - minTrackNumber;
            if (range > 256) { trackByNumber = null; }
            else
            {
               trackByNumber = new MatroskaTrackEntry[range];
               for (int i = tracks.Count - 1; i >= 0; i--)
               {
                  trackByNumber[tracks[i].TrackNumber - minTrackNumber] = tracks[i];
               }
            }
         }
      }

      public async ValueTask ReadTrackInfo(CancellationToken cancellationToken = default)
      {
         if (readTrackInfo) { return; }
         readTrackInfo = true;
         document.Reader.MaxInlineBinarySize = 4096;
         while (document.Reader.LastCachedElement?.Definition != MatroskaSpecification.Cluster)
         {
            if (await document.Reader.ReadNextElement(true, cancellationToken) == null) { break; }
         }
         ScanTrackInfo();
      }

      public async ValueTask<MatroskaFrame> ReadFrame(CancellationToken cancellationToken = default)
      {
         if (frameQueue != null && frameQueue.Count > 0) { return frameQueue.Dequeue(); }
         if (!readTrackInfo) { await ReadTrackInfo(cancellationToken); }
         while (true)
         {
            document.Reader.MaxInlineBinarySize = 0;
            var element = await document.Reader.ReadNextElementRaw(false, cancellationToken);
            if (element.Definition == null) { return default; }
            if (element.Definition == MatroskaSpecification.Timestamp)
            {
               clusterTimestamp = (long)element.Value.UnsignedInteger;
            }
            else if (element.Definition == MatroskaSpecification.SimpleBlock ||
                     element.Definition == MatroskaSpecification.Block)
            {
               var reader = element.Reader;
               if (reader == null) { memoryReader.SetBuffer(element.Binary); reader = memoryReader; }
               MatroskaTrackEntry trackEntry = null;
               var trackIndexVint = await EBMLVInt.Read(reader, cancellationToken);
               var trackIndex = (int)trackIndexVint.Value;
               if (trackByNumber != null)
               {
                  var idx = trackIndex - minTrackNumber;
                  if (idx < trackByNumber.Length) { trackEntry = trackByNumber[idx]; }
               }
               else
               {
                  for (int idx = tracks.Count - 1; idx >= 0; idx--)
                  {
                     if (tracks[idx].TrackNumber == trackIndex) { trackEntry = tracks[idx]; break; }
                  }
               }
               if (await reader.ReadAsync(new Memory<byte>(readBytes, 0, 3), true, cancellationToken) < 3) { return default; }
               var timestampOffset = (short)((readBytes[0] << 8) | readBytes[1]);
               var timestamp = clusterTimestamp + timestampOffset;
               var flags = readBytes[2];
               MatroskaFrame firstFrame = default;
               int frameCount = 0;
               if ((flags & 0x06) != 0) // Has lacing
               {
                  if (lacingSizes == null) { lacingSizes = new int[256]; }
                  frameCount = await reader.ReadByteAsync(cancellationToken);
                  if ((flags & 0x06) == 0x02) // Xiph
                  {
                     for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                     {
                        lacingSizes[frameIndex] = await XiphLacingSize.Read(reader, cancellationToken);
                     }
                  }
                  else if ((flags & 0x06) == 0x04) // EBML
                  {
                     for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                     {
                        var val = await EBMLVInt.Read(reader, cancellationToken);
                        if (frameIndex == 0) { lacingSizes[frameIndex] = (int)val.Value; }
                        else { lacingSizes[frameIndex] = (int)val.SignedValue + lacingSizes[frameIndex - 1]; }
                     }
                  }
                  else // Fixed
                  {
                     var fixedSize = (int)reader.UnreadLength / (frameCount + 1);
                     for (int frameIndex = 0; frameIndex < frameCount; frameIndex++)
                     {
                        lacingSizes[frameIndex] = fixedSize;
                     }
                  }
               }
               for (int frameIndex = 0; frameIndex <= frameCount; frameIndex++)
               {
                  DataBuffer buffer = null;
                  try
                  {
                     var size = frameIndex == frameCount ? (int)reader.UnreadLength : lacingSizes[frameIndex];
                     buffer = cache.Pop(size);
                     buffer.WriteOffset = await reader.ReadAsync(new Memory<byte>(buffer.Buffer, 0, size), true, cancellationToken);
                     var frame = new MatroskaFrame(trackEntry, trackIndex, timestamp, buffer, (flags & 0x80) != 0);
                     if (frameIndex == 0) { firstFrame = frame; }
                     else
                     {
                        if (frameQueue == null) { frameQueue = new Queue<MatroskaFrame>(); }
                        frameQueue.Enqueue(frame);
                     }
                     buffer = null;
                  }
                  finally { buffer?.Dispose(); }
               }
               return firstFrame;
            }
            else if (element.Definition != MatroskaSpecification.Cluster &&
                     element.Definition.Type == EBMLElementType.Master &&
                    !element.Definition.Path.StartsWith(@"\Segment\Cluster"))
            {
               readTrackInfo = false;
               await ReadTrackInfo(cancellationToken);
            }
         }
      }

      public virtual void Dispose()
      {
         if (disposed) { return; }
         disposed = true;
         document.Dispose();
         if (frameQueue != null)
         {
            while (frameQueue.Count > 0) { frameQueue.Dequeue().Buffer.Dispose(); }
            frameQueue = null;
         }
      }
   }
}
