using System;
using Xtremegaida.DataStructures;

namespace MediaContainers.Matroska
{
   public struct MatroskaFrame : IDisposable
   {
      public readonly MatroskaTrackEntry Track;
      public readonly long Timestamp;
      public readonly DataBuffer Buffer;
      public readonly int TrackIndex;
      public readonly bool IsKeyFrame;

      public MatroskaFrame(MatroskaTrackEntry track, int trackIndex, long timestamp, DataBuffer buffer, bool keyFrame)
      {
         Track = track;
         TrackIndex = trackIndex;
         Timestamp = timestamp;
         Buffer = buffer;
         IsKeyFrame = keyFrame;
      }

      public void Dispose()
      {
         Buffer?.Dispose();
      }
   }
}
