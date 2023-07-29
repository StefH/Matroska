using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaTag : List<MatroskaSimpleTag>
   {
      public int TargetTypeValue { get; set; } = 50;
      public string TargetType { get; set; }
      public List<ulong> TagTrackUID { get; set; }
      public List<ulong> TagEditionUID { get; set; }
      public List<ulong> TagChapterUID { get; set; }
      public List<ulong> TagAttachmentUID { get; set; }

      public void ReadFrom(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition != MatroskaSpecification.Tag) { return; }
         foreach (var child in element.Children)
         {
            if (child.Definition == MatroskaSpecification.Targets)
            {
               foreach (var sub in child.Children)
               {
                  if (sub.Definition == MatroskaSpecification.TargetTypeValue) { TargetTypeValue = (int)sub.IntValue; }
                  else if (sub.Definition == MatroskaSpecification.TargetType) { TargetType = sub.StringValue; }
                  else if (sub.Definition == MatroskaSpecification.TagTrackUID)
                  {
                     if (TagTrackUID == null) { TagTrackUID = new List<ulong>(); }
                     TagTrackUID.Add(sub.UIntValue);
                  }
                  else if (sub.Definition == MatroskaSpecification.TagEditionUID)
                  {
                     if (TagEditionUID == null) { TagEditionUID = new List<ulong>(); }
                     TagEditionUID.Add(sub.UIntValue);
                  }
                  else if (sub.Definition == MatroskaSpecification.TagChapterUID)
                  {
                     if (TagChapterUID == null) { TagChapterUID = new List<ulong>(); }
                     TagChapterUID.Add(sub.UIntValue);
                  }
                  else if (sub.Definition == MatroskaSpecification.TagAttachmentUID)
                  {
                     if (TagAttachmentUID == null) { TagAttachmentUID = new List<ulong>(); }
                     TagAttachmentUID.Add(sub.UIntValue);
                  }
               }
            }
            else if (child.Definition == MatroskaSpecification.SimpleTag)
            {
               var tag = new MatroskaSimpleTag();
               tag.ReadFrom((EBMLMasterElement)child);
               Add(tag);
            }
         }
      }

      public void CopyTo(MatroskaTag tag, bool shallow = false)
      {
         tag.TargetTypeValue = TargetTypeValue;
         tag.TargetType = TargetType;
         if (shallow)
         {
            tag.Clear();
            tag.TagTrackUID = TagTrackUID;
            tag.TagEditionUID = TagEditionUID;
            tag.TagChapterUID = TagChapterUID;
            tag.TagAttachmentUID = TagAttachmentUID;
            for (int i = 0, j = Count; i < j; i++) { tag.Add(this[i]); }
         }
         else
         {
            tag.Clear();
            tag.TagTrackUID = TagTrackUID?.ToList();
            tag.TagEditionUID = TagEditionUID?.ToList();
            tag.TagChapterUID = TagChapterUID?.ToList();
            tag.TagAttachmentUID = TagAttachmentUID?.ToList();
            for (int i = 0, j = Count; i < j; i++)
            {
               var simple = new MatroskaSimpleTag();
               this[i].CopyTo(simple);
               tag.Add(simple);
            }
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.Tag, cancellationToken);
         await writer.BeginMasterElement(MatroskaSpecification.Targets, cancellationToken);
         if (TargetTypeValue != 50) { await writer.WriteUnsignedInteger(MatroskaSpecification.TargetTypeValue, (ulong)TargetTypeValue, cancellationToken); }
         if (TargetType != null) { await writer.WriteString(MatroskaSpecification.TargetType, TargetType, cancellationToken); }
         if (TagTrackUID != null)
         {
            for (int i = 0; i < TagTrackUID.Count; i++)
            {
               await writer.WriteUnsignedInteger(MatroskaSpecification.TagTrackUID, TagTrackUID[i], cancellationToken);
            }
         }
         if (TagEditionUID != null)
         {
            for (int i = 0; i < TagEditionUID.Count; i++)
            {
               await writer.WriteUnsignedInteger(MatroskaSpecification.TagEditionUID, TagEditionUID[i], cancellationToken);
            }
         }
         if (TagChapterUID != null)
         {
            for (int i = 0; i < TagChapterUID.Count; i++)
            {
               await writer.WriteUnsignedInteger(MatroskaSpecification.TagChapterUID, TagChapterUID[i], cancellationToken);
            }
         }
         if (TagAttachmentUID != null)
         {
            for (int i = 0; i < TagAttachmentUID.Count; i++)
            {
               await writer.WriteUnsignedInteger(MatroskaSpecification.TagAttachmentUID, TagAttachmentUID[i], cancellationToken);
            }
         }
         await writer.EndMasterElement(cancellationToken);
         for (int i = 0; i < Count; i++) { await this[i].Write(writer, cancellationToken); }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var tracks = new EBMLMasterElement(MatroskaSpecification.Tag);
         var target = new EBMLMasterElement(MatroskaSpecification.Targets);
         if (TargetTypeValue != 50) { target.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TargetTypeValue, (ulong)TargetTypeValue)); }
         if (TargetType != null) { target.AddChild(new EBMLStringElement(MatroskaSpecification.TargetType, TargetType)); }
         if (TagTrackUID != null)
         {
            for (int i = 0; i < TagTrackUID.Count; i++)
            {
               target.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TagTrackUID, TagTrackUID[i]));
            }
         }
         if (TagEditionUID != null)
         {
            for (int i = 0; i < TagEditionUID.Count; i++)
            {
               target.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TagEditionUID, TagEditionUID[i]));
            }
         }
         if (TagChapterUID != null)
         {
            for (int i = 0; i < TagChapterUID.Count; i++)
            {
               target.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TagChapterUID, TagChapterUID[i]));
            }
         }
         if (TagAttachmentUID != null)
         {
            for (int i = 0; i < TagAttachmentUID.Count; i++)
            {
               target.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TagAttachmentUID, TagAttachmentUID[i]));
            }
         }
         tracks.AddChild(target);
         foreach (var track in this) { tracks.AddChild(track.ToElement()); }
         return tracks;
      }
   }
}
