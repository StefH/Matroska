using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public class EBMLHeader
   {
      public EBMLMasterElement OriginalElement { get; private set; }
      public int EBMLVersion { get; set; } = 1;
      public int EBMLReadVersion { get; set; } = 1;
      public int EBMLMaxIDLength { get; set; } = 4;
      public int EBMLMaxSizeLength { get; set; } = 8;
      public string DocType { get; set; }
      public int DocTypeVersion { get; set; } = 1;
      public int DocTypeReadVersion { get; set; } = 1;
      public List<KeyValuePair<string, ulong>> DocTypeExtensions { get; set; }

      public EBMLHeader() { }

      public EBMLHeader(EBMLMasterElement element)
      {
         if (element == null) { throw new ArgumentNullException(nameof(element)); }
         if (element.Definition != EBMLElementDefiniton.EBML) { throw new ArgumentException("Element must be an EBML header section", nameof(element)); }
         if (!element.IsFullyRead) { throw new ArgumentException("Element must be fully read", nameof(element)); }
         OriginalElement = element;
         foreach (var item in element.Children)
         {
            if (item.Definition == EBMLElementDefiniton.EBMLVersion) { EBMLVersion = (int)item.IntValue; }
            else if (item.Definition == EBMLElementDefiniton.EBMLReadVersion) { EBMLReadVersion = (int)item.IntValue; }
            else if (item.Definition == EBMLElementDefiniton.EBMLMaxIDLength) { EBMLMaxIDLength = (int)item.IntValue; }
            else if (item.Definition == EBMLElementDefiniton.EBMLMaxSizeLength) { EBMLMaxSizeLength = (int)item.IntValue; }
            else if (item.Definition == EBMLElementDefiniton.DocType) { DocType = item.StringValue; }
            else if (item.Definition == EBMLElementDefiniton.DocTypeVersion) { DocTypeVersion = (int)item.IntValue; }
            else if (item.Definition == EBMLElementDefiniton.DocTypeReadVersion) { DocTypeReadVersion = (int)item.IntValue; }
            else if (item.Definition == EBMLElementDefiniton.DocTypeExtension)
            {
               string name = string.Empty;
               ulong version = 0;
               foreach (var ext in ((EBMLMasterElement)item).Children)
               {
                  if (item.Definition == EBMLElementDefiniton.DocTypeExtensionName) { name = item.StringValue ?? name; }
                  else if (item.Definition == EBMLElementDefiniton.DocTypeExtensionVersion) { version = item.UIntValue; }
               }
               if (name.Length > 0 && version > 0)
               {
                  if (DocTypeExtensions == null) { DocTypeExtensions = new List<KeyValuePair<string, ulong>>(); }
                  DocTypeExtensions.Add(new KeyValuePair<string, ulong>(name, version));
               }
            }
         }
      }

      public static async ValueTask<EBMLHeader> Read(EBMLReader reader, CancellationToken cancellationToken = default)
      {
         var header = await reader.ReadNextElement(true, cancellationToken);
         if (header == null) { throw new Exception("Unexpected end of stream"); }
         if (header.Definition != EBMLElementDefiniton.EBML) { throw new Exception("Expected EBML header; read different element"); }
         while (!header.IsFullyRead) { if ((await reader.ReadNextElement(true, cancellationToken)) == null) { break; } }
         return new EBMLHeader(header as EBMLMasterElement);
      }

      public static async ValueTask<EBMLHeader> Read(IDataQueueReader reader, DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         using (var ebml = new EBMLReader(reader, true, cache))
         {
            EBMLElementDefiniton.AddHeaderElements(ebml);
            return await Read(ebml, cancellationToken);
         }
      }

      public static async ValueTask<EBMLHeader> Read(Stream stream, DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         using (var ebml = new EBMLReader(stream, true, cache))
         {
            EBMLElementDefiniton.AddHeaderElements(ebml);
            return await Read(ebml, cancellationToken);
         }
      }

      public async ValueTask Write(EBMLWriter writer, CancellationToken cancellationToken = default)
      {
         await writer.BeginMasterElement(EBMLElementDefiniton.EBML, cancellationToken);
         await writer.WriteUnsignedInteger(EBMLElementDefiniton.EBMLVersion, (ulong)EBMLVersion, cancellationToken);
         await writer.WriteUnsignedInteger(EBMLElementDefiniton.EBMLReadVersion, (ulong)EBMLReadVersion, cancellationToken);
         await writer.WriteUnsignedInteger(EBMLElementDefiniton.EBMLMaxIDLength, (ulong)EBMLMaxIDLength, cancellationToken);
         await writer.WriteUnsignedInteger(EBMLElementDefiniton.EBMLMaxSizeLength, (ulong)EBMLMaxSizeLength, cancellationToken);
         await writer.WriteString(EBMLElementDefiniton.DocType, DocType, cancellationToken);
         await writer.WriteUnsignedInteger(EBMLElementDefiniton.DocTypeVersion, (ulong)DocTypeVersion, cancellationToken);
         await writer.WriteUnsignedInteger(EBMLElementDefiniton.DocTypeReadVersion, (ulong)DocTypeReadVersion, cancellationToken);
         if (DocTypeExtensions != null)
         {
            foreach (var ext in DocTypeExtensions)
            {
               await writer.BeginMasterElement(EBMLElementDefiniton.DocTypeExtension, cancellationToken);
               await writer.WriteString(EBMLElementDefiniton.DocTypeExtensionName, ext.Key, cancellationToken);
               await writer.WriteUnsignedInteger(EBMLElementDefiniton.DocTypeExtensionVersion, ext.Value, cancellationToken);
               await writer.EndMasterElement(cancellationToken);
            }
         }
         await writer.EndMasterElement(cancellationToken);
      }

      public EBMLMasterElement ToElement()
      {
         var header = new EBMLMasterElement(EBMLElementDefiniton.EBML);
         header.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.EBMLVersion, (ulong)EBMLVersion));
         header.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.EBMLReadVersion, (ulong)EBMLReadVersion));
         header.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.EBMLMaxIDLength, (ulong)EBMLMaxIDLength));
         header.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.EBMLMaxSizeLength, (ulong)EBMLMaxSizeLength));
         header.AddChild(new EBMLStringElement(EBMLElementDefiniton.DocType, DocType));
         header.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.DocTypeVersion, (ulong)DocTypeVersion));
         header.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.DocTypeReadVersion, (ulong)DocTypeReadVersion));
         if (DocTypeExtensions != null)
         {
            var extMaster = new EBMLMasterElement(EBMLElementDefiniton.DocTypeExtension);
            foreach (var ext in DocTypeExtensions)
            {
               extMaster.AddChild(new EBMLStringElement(EBMLElementDefiniton.DocTypeExtensionName, ext.Key));
               extMaster.AddChild(new EBMLUnsignedIntegerElement(EBMLElementDefiniton.DocTypeExtensionVersion, ext.Value));
            }
            header.AddChild(extMaster);
         }
         return header;
      }

      public override string ToString()
      {
         return (DocType ?? string.Empty) + " " + DocTypeVersion;
      }
   }
}
