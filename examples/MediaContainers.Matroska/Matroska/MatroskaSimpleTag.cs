using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaSimpleTag : List<MatroskaSimpleTag>
   {
      public string TagName { get; set; }
      public string TagLanguage { get; set; }
      public string TagLanguageIETF { get; set; }
      public bool TagDefault { get; set; } = true;
      public string TagString { get; set; }
      public byte[] TagBinary { get; set; }

      public void ReadFrom(EBMLMasterElement element)
      {
         if (element == null) { return; }
         if (element.Definition != MatroskaSpecification.SimpleTag) { return; }
         foreach (var child in element.Children)
         {
            if (child.Definition == MatroskaSpecification.TagName) { TagName = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.TagLanguage) { TagLanguage = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.TagLanguageIETF) { TagLanguageIETF = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.TagDefault) { TagDefault = child.IntValue != 0; }
            else if (child.Definition == MatroskaSpecification.TagString) { TagString = child.StringValue; }
            else if (child.Definition == MatroskaSpecification.TagBinary) { TagBinary = ((EBMLBinaryElement)child).Value.ToArray(); }
            else if (child.Definition == MatroskaSpecification.SimpleTag)
            {
               var tag = new MatroskaSimpleTag();
               tag.ReadFrom((EBMLMasterElement)child);
               Add(tag);
            }
         }
      }

      public void CopyTo(MatroskaSimpleTag simple, bool shallow = false)
      {
         simple.TagName = TagName;
         simple.TagLanguage = TagLanguage;
         simple.TagLanguageIETF = TagLanguageIETF;
         simple.TagDefault = TagDefault;
         simple.TagString = TagString;
         simple.TagBinary = TagBinary;
         if (shallow)
         {
            simple.Clear();
            for (int i = 0, j = Count; i < j; i++) { simple.Add(this[i]); }
         }
         else
         {
            simple.Clear();
            for (int i = 0, j = Count; i < j; i++)
            {
               var sub = new MatroskaSimpleTag();
               this[i].CopyTo(sub);
               simple.Add(sub);
            }
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.SimpleTag, cancellationToken);
         await writer.WriteString(MatroskaSpecification.TagName, TagName ?? string.Empty, cancellationToken);
         if (TagLanguage != null) { await writer.WriteString(MatroskaSpecification.TagLanguage, TagLanguage, cancellationToken); }
         if (TagLanguageIETF != null) { await writer.WriteString(MatroskaSpecification.TagLanguageIETF, TagLanguageIETF, cancellationToken); }
         if (!TagDefault) { await writer.WriteUnsignedInteger(MatroskaSpecification.TagDefault, 0, cancellationToken); }
         if (TagString != null) { await writer.WriteString(MatroskaSpecification.TagString, TagString, cancellationToken); }
         if (TagBinary != null) { await writer.WriteBinary(MatroskaSpecification.TagBinary, TagBinary, cancellationToken); }
         for (int i = 0; i < Count; i++) { await this[i].Write(writer, cancellationToken); }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var tracks = new EBMLMasterElement(MatroskaSpecification.SimpleTag);
         tracks.AddChild(new EBMLStringElement(MatroskaSpecification.TagName, TagName ?? string.Empty));
         if (TagLanguage != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.TagLanguage, TagLanguage)); }
         if (TagLanguageIETF != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.TagLanguageIETF, TagLanguageIETF)); }
         if (!TagDefault) { tracks.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.TagDefault, 0)); }
         if (TagString != null) { tracks.AddChild(new EBMLStringElement(MatroskaSpecification.TagString, TagString)); }
         if (TagBinary != null) { tracks.AddChild(new EBMLBinaryElement(MatroskaSpecification.TagBinary, TagBinary)); }
         foreach (var track in this) { tracks.AddChild(track.ToElement()); }
         return tracks;
      }
   }
}
