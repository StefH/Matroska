using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaTags : List<MatroskaTag>
   {
      public void AddTagEntry(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition == MatroskaSpecification.Tags)
         {
            foreach (var child in element.Children)
            {
               AddTagEntry(child as EBMLMasterElement);
            }
         }
         else if (element.Definition == MatroskaSpecification.Tag)
         {
            var entry = new MatroskaTag();
            entry.ReadFrom(element);
            Add(entry);
         }
      }

      public void CopyTo(MatroskaTags tags, bool shallow = false)
      {
         if (shallow)
         {
            tags.Clear();
            for (int i = 0, j = Count; i < j; i++) { tags.Add(this[i]); }
         }
         else
         {
            tags.Clear();
            for (int i = 0, j = Count; i < j; i++)
            {
               var tag = new MatroskaTag();
               this[i].CopyTo(tag);
               tags.Add(tag);
            }
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.Tags, cancellationToken);
         for (int i = 0; i < Count; i++) { await this[i].Write(writer, cancellationToken); }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var tracks = new EBMLMasterElement(MatroskaSpecification.Tags);
         foreach (var track in this) { tracks.AddChild(track.ToElement()); }
         return tracks;
      }
   }
}
