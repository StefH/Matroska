using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaContainers.Matroska
{
   public class MatroskaSeekHead
   {
      private Dictionary<EBMLElementDefiniton, long> seekIndices = new();

      public KeyValuePair<EBMLElementDefiniton, long>[] Indices => seekIndices.ToArray();

      public void AddSeekIndex(EBMLElementDefiniton def, long offset)
      {
         seekIndices[def] = offset;
      }

      public void AddSeekIndex(EBMLReader ebml, EBMLMasterElement element, long segmentOffset)
      {
         if (element == null) { return; }
         if (element.Definition == MatroskaSpecification.SeekHead)
         {
            foreach (var child in element.Children)
            {
               AddSeekIndex(ebml, child as EBMLMasterElement, segmentOffset);
            }
         }
         else if (element.Definition == MatroskaSpecification.Seek)
         {
            EBMLElementDefiniton def = null; long position = -1;
            foreach (var child in element.Children)
            {
               if (child.Definition == MatroskaSpecification.SeekID)
               {
                  var bin = (EBMLBinaryElement)child;
                  var span = bin.Value.Span;
                  ulong id = 0;
                  for (int i = 0; i < span.Length; i++) { id = (id << 8) | span[i]; }
                  if (id != 0) { def = ebml.GetElementDefinition(id); }
               }
               else if (child.Definition == MatroskaSpecification.SeekPosition)
               {
                  position = child.IntValue;
               }
            }
            if (def != null && position >= 0)
            {
               AddSeekIndex(def, position + segmentOffset);
            }
         }
      }

      public long GetSeekOffset(EBMLElementDefiniton def)
      {
         if (seekIndices.TryGetValue(def, out var idx)) { return idx; }
         return -1;
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(MatroskaSpecification.SeekHead, cancellationToken);
         foreach (var index in seekIndices)
         {
            var buffer = new byte[index.Key.Id.WidthBytes];
            ulong id = index.Key.Id.ValueWithMarker;
            for (int i = buffer.Length - 1; i >= 0; i--) { buffer[i] = (byte)(id & 0xff); id >>= 8; }
            await writer.BeginMasterElement(MatroskaSpecification.Seek, cancellationToken);
            await writer.WriteBinary(MatroskaSpecification.SeekID, buffer, cancellationToken);
            await writer.WriteUnsignedInteger(MatroskaSpecification.SeekPosition, (ulong)index.Value, cancellationToken);
            await writer.EndMasterElement(cancellationToken);
         }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var seekHead = new EBMLMasterElement(MatroskaSpecification.SeekHead);
         foreach (var index in seekIndices)
         {
            var buffer = new byte[index.Key.Id.WidthBytes];
            ulong id = index.Key.Id.ValueWithMarker;
            for (int i = buffer.Length - 1; i >= 0; i--) { buffer[i] = (byte)(id & 0xff); id >>= 8; }
            var seek = new EBMLMasterElement(MatroskaSpecification.Seek);
            seek.AddChild(new EBMLBinaryElement(MatroskaSpecification.SeekID, buffer));
            seek.AddChild(new EBMLUnsignedIntegerElement(MatroskaSpecification.SeekPosition, (ulong)index.Value));
            seekHead.AddChild(seek);
         }
         return seekHead;
      }

      public override string ToString()
      {
         var str = new StringBuilder();
         foreach (var idx in seekIndices)
         {
            str.AppendLine(idx.Key.FullPath + ":0x" + Convert.ToString(idx.Value, 16));
         }
         return str.ToString();
      }
   }
}
