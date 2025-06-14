using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaCues : List<MatroskaCuePoint>
   {
      public void AddCueEntry(EBMLMasterElement element, long segmentOffset)
      {
         if (element == null) { return; }
         if (element.Definition == MatroskaSpecification.Cues)
         {
            foreach (var child in element.Children)
            {
               AddCueEntry(child as EBMLMasterElement, segmentOffset);
            }
         }
         else if (element.Definition == MatroskaSpecification.CuePoint)
         {
            var entry = new MatroskaCuePoint() { SegmentOffset = (ulong)segmentOffset };
            entry.ReadFrom(element);
            Add(entry);
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.Cues, cancellationToken);
         for (int i = 0; i < Count; i++) { await this[i].Write(writer, cancellationToken); }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var tracks = new EBMLMasterElement(MatroskaSpecification.Cues);
         foreach (var track in this) { tracks.AddChild(track.ToElement()); }
         return tracks;
      }
   }
}
