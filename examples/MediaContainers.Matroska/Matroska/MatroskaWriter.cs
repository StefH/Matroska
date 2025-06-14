using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers.Matroska
{
   public class MatroskaWriter : IDisposable, IAsyncDisposable
   {
      public static readonly EBMLDocType DocTypeMatroska = new EBMLDocType("matroska", 4, 2);
      public static readonly EBMLDocType DocTypeWebM = new EBMLDocType("webm", 4, 2);

      private static readonly EBMLDocType[] docTypes = new EBMLDocType[]
      {
         DocTypeMatroska,
         DocTypeWebM,
      };

      private readonly DataBufferCache cache;
      private readonly EBMLDocumentWriter document;
      private readonly MatroskaInfo info = new();
      private readonly MatroskaTracks tracks = new();
      private readonly MatroskaTags tags = new();
      private readonly DataQueueMemoryReaderMutable memoryReader = new();
      private readonly DataBuffer writeBuffer = new(null, new byte[32]);
      private MatroskaTrackEntry[] trackByNumber;
      private MatroskaCues cues;
      private bool writtenHeaders;
      private bool writtenSeekInfo;
      private EBMLWriter.RelativeOffset seekHeadOffset;
      private EBMLWriter.RelativeOffset infoOffset;
      private EBMLWriter.RelativeOffset tracksOffset;
      private EBMLWriter.RelativeOffset tagsOffset;
      private EBMLWriter.RelativeOffset clusterOffset;
      private EBMLWriter.RelativeOffset durationOffset;
      private Queue<CueMark> clusterOffsets;
      private bool streamingMode;
      private int minTrackNumber;
      private int cueTrackNumber;
      private bool hasClusterCue;
      private bool firstCluster;
      private long firstClusterTimestampOffset;
      private bool firstBlock;
      private long lastClusterTimestamp;
      private long maxFrameTimestamp;
      private long lastVideoTimestamp;
      private long defaultDurationTimestamp;
      private int maxClusterSize = (1 << 21) - 1;
      private double timestampScaleSec;
      private volatile bool disposed;

      public virtual EBMLDocType[] SupportedDocTypes => docTypes;
      public EBMLDocumentWriter Document => document;
      public MatroskaInfo Info => info;
      public MatroskaTracks Tracks => tracks;
      public MatroskaTags Tags => tags;

      public long CalculatedDurationTimestamp => maxFrameTimestamp + defaultDurationTimestamp;

      public bool StreamingMode { get { return streamingMode; } set { document.Writer.StreamingMode = streamingMode = value; } }
      public bool AutoCalculateDuration { get; set; }
      public bool DropAudioToSyncVideo { get; set; }
      public double DropAudioMaxDelaySeconds { get; set; } = 1;

      private bool useCalculatedDuration => AutoCalculateDuration || DropAudioToSyncVideo;

      private struct CueMark
      {
         public readonly long Timestamp;
         public readonly EBMLWriter.RelativeOffset ClusterOffset;
         public readonly EBMLWriter.RelativeOffset BlockOffset;

         public CueMark(long timestamp, EBMLWriter.RelativeOffset clusterOffset, EBMLWriter.RelativeOffset blockOffset)
         {
            Timestamp = timestamp;
            ClusterOffset = clusterOffset;
            BlockOffset = blockOffset;
         }
      }

      public MatroskaWriter(EBMLDocumentWriter doc, DataBufferCache cache = null)
      {
         if (doc == null) { throw new ArgumentNullException(nameof(doc)); }
         var docTypes = SupportedDocTypes;
         var canWrite = false;
         for (int i = docTypes.Length - 1; i >= 0; i--)
         {
            if (doc.CanBeWrittenBy(docTypes[i])) { canWrite = true; break; }
         }
         if (!canWrite)
         {
            throw new ArgumentException("Unsupported DocType: " + doc.Header.DocType + " " + doc.Header.DocTypeVersion, nameof(doc));
         }
         document = doc;
         this.cache = cache ?? DataBufferCache.DefaultCache;
      }

      public static async ValueTask<MatroskaWriter> Write(IDataQueueWriter writer, bool keepWriterOpen = false,
         DataBufferCache cache = null, EBMLDocType format = default, CancellationToken cancellationToken = default)
      {
         if (format.DocType == null) { format = DocTypeMatroska; }
         var header = new EBMLHeader()
         {
            DocType = format.DocType,
            DocTypeVersion = format.WriteVersion,
            DocTypeReadVersion = format.ReadVersion
         };
         var doc = await EBMLDocumentWriter.Write(header, writer, keepWriterOpen, cache, cancellationToken);
         return new MatroskaWriter(doc, cache);
      }

      public static async ValueTask<MatroskaWriter> Write(Stream stream, bool keepStreamOpen = false,
         DataBufferCache cache = null, EBMLDocType format = default, CancellationToken cancellationToken = default)
      {
         if (format.DocType == null) { format = DocTypeMatroska; }
         var header = new EBMLHeader()
         {
            DocType = format.DocType,
            DocTypeVersion = format.WriteVersion,
            DocTypeReadVersion = format.ReadVersion
         };
         var doc = await EBMLDocumentWriter.Write(header, stream, keepStreamOpen, cache, cancellationToken);
         return new MatroskaWriter(doc, cache);
      }

      public static bool CanWriteDocument(EBMLDocumentWriter doc)
      {
         for (int i = docTypes.Length - 1; i >= 0; i--)
         {
            if (doc.CanBeWrittenBy(docTypes[i])) { return true; }
         }
         return false;
      }

      public async ValueTask WriteTrackInfo(CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(MatroskaWriter)); }
         if (writtenHeaders) { return; }
         if (tracks.Count == 0) { throw new InvalidOperationException("Need to add at least one track."); }
         writtenHeaders = true;
         var canSeek = document.Writer.CanSeek && !streamingMode;
         await document.Writer.BeginMasterElement(MatroskaSpecification.Segment, cancellationToken);
         if (canSeek)
         {
            seekHeadOffset = document.Writer.MarkRelativeOffset();
            await document.Writer.WriteVoid(126, cancellationToken);
            infoOffset = document.Writer.MarkRelativeOffset(MatroskaSpecification.Segment);
            if (AutoCalculateDuration && info.Duration <= 0) { info.Duration = 1; }
         }
         if (streamingMode) { info.Duration = 0; }
         durationOffset = await info.Write(document.Writer, cancellationToken);
         if (canSeek) { tracksOffset = document.Writer.MarkRelativeOffset(MatroskaSpecification.Segment); }
         await tracks.Write(document.Writer, cancellationToken);
         if (canSeek) { tagsOffset = document.Writer.MarkRelativeOffset(MatroskaSpecification.Segment); }
         await tags.Write(document.Writer, cancellationToken);
         if (!streamingMode) { await document.Writer.WriteVoid(126, cancellationToken); }
         timestampScaleSec = info.TimestampScale / 1000000000.0;

         int maxTrackNumber = tracks[0].TrackNumber;
         defaultDurationTimestamp = 0;
         minTrackNumber = maxTrackNumber;
         cueTrackNumber = -1;
         for (int i = tracks.Count - 1; i >= 0; i--)
         {
            if (tracks[i].HasAudio || tracks[i].TrackType == MatroskaTrackType.Video) { cueTrackNumber = tracks[i].TrackNumber; }
            if (maxTrackNumber < tracks[i].TrackNumber) { maxTrackNumber = tracks[i].TrackNumber; }
            if (minTrackNumber > tracks[i].TrackNumber) { minTrackNumber = tracks[i].TrackNumber; }
            if (defaultDurationTimestamp < (long)tracks[i].DefaultDuration) { defaultDurationTimestamp = (long)tracks[i].DefaultDuration; }
         }
         defaultDurationTimestamp = defaultDurationTimestamp / (long)info.TimestampScale;
         if (defaultDurationTimestamp <= 0) { defaultDurationTimestamp = 33333333 / (long)info.TimestampScale; }
         if (cueTrackNumber < 0) { cueTrackNumber = tracks[0].TrackNumber; }
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

      public async ValueTask WriteSeekInfo(CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(MatroskaWriter)); }
         if (writtenSeekInfo) { return; }
         writtenSeekInfo = true;
         while (document.Writer.CurrentContainer != MatroskaSpecification.Segment)
         {
            await document.Writer.EndMasterElement(cancellationToken);
         }
         if (document.Writer.CanSeek && !streamingMode)
         {
            EBMLWriter.RelativeOffset cuesOffset = default;
            WriteClusterOffset();
            if (cues != null)
            {
               cuesOffset = document.Writer.MarkRelativeOffset(MatroskaSpecification.Segment);
               await cues.Write(document.Writer, cancellationToken);
            }
            var seekHeadResolved = seekHeadOffset.Resolve();
            if (seekHeadResolved != null)
            {
               var seekHead = new MatroskaSeekHead();
               var offset = infoOffset.Resolve(); if (offset != null) { seekHead.AddSeekIndex(MatroskaSpecification.Info, offset.Value); }
               offset = tracksOffset.Resolve(); if (offset != null) { seekHead.AddSeekIndex(MatroskaSpecification.Tracks, offset.Value); }
               offset = tagsOffset.Resolve(); if (offset != null) { seekHead.AddSeekIndex(MatroskaSpecification.Tags, offset.Value); }
               offset = cuesOffset.Resolve(); if (offset != null) { seekHead.AddSeekIndex(MatroskaSpecification.Cues, offset.Value); }
               using (var seeked = await document.Writer.SeekWriter(seekHeadResolved.Value, SeekOrigin.Begin, cancellationToken))
               {
                  var written = seeked.CurrentContainerSize;
                  await seekHead.Write(seeked, cancellationToken);
                  written = seeked.CurrentContainerSize - written;
                  await seeked.WriteVoid(126 - (int)written, cancellationToken);
                  await seeked.FlushInternalBuffer(cancellationToken);
               }
            }
            if (useCalculatedDuration)
            {
               var durationOffsetResolved = durationOffset.Resolve();
               if (durationOffsetResolved != null)
               {
                  using (var seeked = await document.Writer.SeekWriter(durationOffsetResolved.Value, SeekOrigin.Begin, cancellationToken))
                  {
                     await seeked.WriteFloat(MatroskaSpecification.Duration, (double)CalculatedDurationTimestamp, cancellationToken);
                     await seeked.FlushInternalBuffer(cancellationToken);
                  }
               }
            }
         }
      }

      private void WriteClusterOffset()
      {
         if (clusterOffsets == null) { return; }
         while (clusterOffsets.Count > 0)
         {
            var mark = clusterOffsets.Peek();
            var offset = mark.ClusterOffset.Resolve();
            if (offset == null) { return; }
            clusterOffsets.Dequeue();
            if (cues == null) { cues = new MatroskaCues(); }
            var point = new MatroskaCuePoint() { Timestamp = (ulong)mark.Timestamp };
            point.Add(new MatroskaCueTrackPosition()
            {
               CueTrack = cueTrackNumber,
               CueClusterPosition = (ulong)offset.Value,
               CueRelativePosition = (ulong)(mark.BlockOffset.Resolve() ?? 0)
            });
            cues.Add(point);
         }
      }

      public async ValueTask WriteFrame(MatroskaFrame frame, CancellationToken cancellationToken = default)
      {
         if (disposed) { throw new ObjectDisposedException(nameof(MatroskaWriter)); }
         if (!writtenHeaders) { await WriteTrackInfo(cancellationToken); }
         MatroskaTrackEntry trackEntry = frame.Track;
         if (trackEntry == null)
         {
            if (trackByNumber != null)
            {
               var idx = frame.TrackIndex - minTrackNumber;
               if (idx < trackByNumber.Length) { trackEntry = trackByNumber[idx]; }
            }
            else
            {
               for (int idx = tracks.Count - 1; idx >= 0; idx--)
               {
                  if (tracks[idx].TrackNumber == frame.TrackIndex) { trackEntry = tracks[idx]; break; }
               }
            }
            if (trackEntry == null) { throw new ArgumentNullException("Track"); }
         }
         var trackIndex = trackEntry.TrackNumber;
         var delta = frame.Timestamp - lastClusterTimestamp;

         if (document.Writer.CurrentContainer == MatroskaSpecification.Cluster)
         {
            if (delta < -32768 || delta >= 32768 ||
               (document.Writer.CurrentContainerSize + frame.Buffer.UnreadSize + 16) > maxClusterSize ||
               (delta * timestampScaleSec) >= 5)
            {
               await document.Writer.EndMasterElement(cancellationToken);
               WriteClusterOffset();
            }
         }

         if (DropAudioToSyncVideo)
         {
            if (trackEntry.HasVideo || trackEntry.TrackType == MatroskaTrackType.Video) { lastVideoTimestamp = frame.Timestamp; }
            else if ((frame.Timestamp - lastVideoTimestamp) * timestampScaleSec >= DropAudioMaxDelaySeconds) { return; }
         }

         if (document.Writer.CurrentContainer != MatroskaSpecification.Cluster)
         {
            if (!firstCluster) { firstCluster = true; firstClusterTimestampOffset = frame.Timestamp; }
            lastClusterTimestamp = frame.Timestamp;
            delta = frame.Timestamp - lastClusterTimestamp;
            if (!streamingMode)
            {
               clusterOffset = document.Writer.MarkRelativeOffset(MatroskaSpecification.Segment);
               hasClusterCue = false;
            }
            firstBlock = true;
            await document.Writer.BeginMasterElement(MatroskaSpecification.Cluster, cancellationToken);
            await document.Writer.WriteUnsignedInteger(MatroskaSpecification.Timestamp,
               (ulong)(lastClusterTimestamp - firstClusterTimestampOffset), cancellationToken);
         }

         if (!streamingMode && !hasClusterCue && frame.IsKeyFrame && trackIndex == cueTrackNumber)
         {
            hasClusterCue = true;
            if (clusterOffsets == null) { clusterOffsets = new Queue<CueMark>(); }
            EBMLWriter.RelativeOffset blockOffset = default;
            if (!firstBlock) { blockOffset = document.Writer.MarkRelativeOffset(MatroskaSpecification.Cluster); }
            clusterOffsets.Enqueue(new CueMark(frame.Timestamp - firstClusterTimestampOffset, clusterOffset, blockOffset));
         }

         if (maxFrameTimestamp < frame.Timestamp) { maxFrameTimestamp = frame.Timestamp; }
         writeBuffer.ReadOffset = 0; writeBuffer.WriteOffset = 0;
         if (trackIndex <= 126) { writeBuffer.Buffer[writeBuffer.WriteOffset++] = (byte)(trackIndex | 0x80); }
         else { new EBMLVInt((ulong)trackIndex).Write(writeBuffer); }
         writeBuffer.Buffer[writeBuffer.WriteOffset++] = (byte)(delta >> 8);
         writeBuffer.Buffer[writeBuffer.WriteOffset++] = (byte)(delta & 0xff);
         writeBuffer.Buffer[writeBuffer.WriteOffset++] = (byte)(frame.IsKeyFrame ? 0x80 : 0x00);
         await document.Writer.WriteBinary(MatroskaSpecification.SimpleBlock, writeBuffer.ReadMemory, frame.Buffer.ReadMemory, cancellationToken);
         if (firstBlock && streamingMode) { await document.Writer.FlushAsync(cancellationToken); }
         firstBlock = false;
      }

      public ValueTask DisposeAsync()
      {
         if (disposed) { return ValueTask.CompletedTask; }
         disposed = true;
         return document.DisposeAsync();
      }

      public virtual void Dispose()
      {
         if (disposed) { return; }
         disposed = true;
         document.Dispose();
      }
   }
}
