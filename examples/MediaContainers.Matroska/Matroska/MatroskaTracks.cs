using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaTracks : List<MatroskaTrackEntry>
   {
      public void AddTrackEntry(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition == MatroskaSpecification.Tracks)
         {
            foreach (var child in element.Children)
            {
               AddTrackEntry(child as EBMLMasterElement);
            }
         }
         else if (element.Definition == MatroskaSpecification.TrackEntry)
         {
            var entry = new MatroskaTrackEntry();
            entry.ReadFrom(element);
            Add(entry);
         }
      }

      public void CopyTo(MatroskaTracks tracks, bool shallow = false)
      {
         if (shallow)
         {
            tracks.Clear();
            for (int i = 0, j = Count; i < j; i++) { tracks.Add(this[i]); }
         }
         else
         {
            tracks.Clear();
            for (int i = 0, j = Count; i < j; i++)
            {
               var track = new MatroskaTrackEntry();
               this[i].CopyTo(track);
               tracks.Add(track);
            }
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.Tracks, cancellationToken);
         for (int i = 0; i < Count; i++) { await this[i].Write(writer, cancellationToken); }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var tracks = new EBMLMasterElement(MatroskaSpecification.Tracks);
         foreach (var track in this) { tracks.AddChild(track.ToElement()); }
         return tracks;
      }
   }
}
