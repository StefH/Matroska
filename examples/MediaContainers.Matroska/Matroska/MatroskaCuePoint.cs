using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaCuePoint : List<MatroskaCueTrackPosition>
   {
      public ulong SegmentOffset { get; set; }
      public ulong Timestamp { get; set; }

      public void ReadFrom(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition != MatroskaSpecification.CuePoint) { return; }
         foreach (var child in element.Children)
         {
            if (child.Definition == MatroskaSpecification.CueTime) { Timestamp = child.UIntValue; }
            else if (child.Definition == MatroskaSpecification.CueTrackPositions)
            {
               var track = new MatroskaCueTrackPosition();
               foreach (var sub in ((EBMLMasterElement)child).Children)
               {
                  if (sub.Definition == MatroskaSpecification.CueTrack) { track.CueTrack = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.CueClusterPosition) { track.CueClusterPosition = sub.UIntValue + SegmentOffset; }
                  else if (sub.Definition == MatroskaSpecification.CueRelativePosition) { track.CueRelativePosition = sub.UIntValue; }
                  else if (sub.Definition == MatroskaSpecification.CueDuration) { track.CueDuration = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.CueBlockNumber) { track.CueBlockNumber = (int)sub.IntValue; }
               }
               Add(track);
            }
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.CuePoint, cancellationToken);
         await writer.WriteUnsignedInteger(MatroskaSpecification.CueTime, Timestamp, cancellationToken);
         for (int i = 0; i < Count; i++)
         {
            var track = this[i];
            await writer.BeginMasterElement(MatroskaSpecification.CueTrackPositions, cancellationToken);
            await writer.WriteUnsignedInteger(MatroskaSpecification.CueTrack, (ulong)track.CueTrack, cancellationToken);
            await writer.WriteUnsignedInteger(MatroskaSpecification.CueClusterPosition, track.CueClusterPosition - SegmentOffset, cancellationToken);
            if (track.CueRelativePosition > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.CueRelativePosition, track.CueRelativePosition, cancellationToken); }
            if (track.CueDuration > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.CueDuration, (ulong)track.CueDuration, cancellationToken); }
            if (track.CueBlockNumber > 0) { await writer.WriteUnsignedInteger(MatroskaSpecification.CueBlockNumber, (ulong)track.CueBlockNumber, cancellationToken); }
            await writer.EndMasterElement(cancellationToken);
         }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var cue = new EBMLMasterElement(MatroskaSpecification.CuePoint);
         cue.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CueTime, Timestamp));
         foreach (var track in this)
         {
            var pos = new EBMLMasterElement(MatroskaSpecification.CueTrackPositions);
            pos.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CueTrack, (ulong)track.CueTrack));
            pos.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CueClusterPosition, track.CueClusterPosition - SegmentOffset));
            if (track.CueRelativePosition > 0) { pos.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CueRelativePosition, track.CueRelativePosition)); }
            if (track.CueDuration > 0) { pos.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CueDuration, (ulong)track.CueDuration)); }
            if (track.CueBlockNumber > 0) { pos.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.CueBlockNumber, (ulong)track.CueBlockNumber)); }
            cue.AddChild(pos);
         }
         return cue;
      }
   }

   public class MatroskaCueTrackPosition
   {
      public int CueTrack { get; set; }
      public ulong CueClusterPosition { get; set; }
      public ulong CueRelativePosition { get; set; }
      public int CueDuration { get; set; }
      public int CueBlockNumber { get; set; }
   }
}
