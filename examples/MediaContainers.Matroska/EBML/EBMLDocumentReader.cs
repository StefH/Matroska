using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public sealed class EBMLDocumentReader : IDisposable
   {
      private readonly EBMLReader reader;

      public EBMLHeader Header { get; private set; }
      public EBMLMasterElement Body { get; private set; }
      public EBMLReader Reader => reader;

      private EBMLDocumentReader(EBMLReader reader, EBMLHeader header, EBMLMasterElement body)
      {
         this.reader = reader;
         Header = header;
         Body = body;
      }

      public bool CanBeReadBy(EBMLDocType type)
      {
         return type.CanReadDocument(Header);
      }

      private static async ValueTask<EBMLDocumentReader> Read(EBMLHeader header, EBMLReader reader, CancellationToken cancellationToken = default)
      {
         if (!EBMLDocTypeLookup.HandleEBMLDocType(header, reader))
         {
            throw new Exception("Unrecognised EBML doctype: " + header.DocType + " version " + header.DocTypeVersion);
         }
         var body = await reader.ReadNextElement(true, cancellationToken) as EBMLMasterElement;
         if (body == null) { throw new Exception("Expected a master element as document body"); }
         return new EBMLDocumentReader(reader, header, body);
      }

      public static async ValueTask<EBMLDocumentReader> Read(IDataQueueReader reader, bool keepReaderOpen = false,
         DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         var header = await EBMLHeader.Read(reader, cache, cancellationToken);
         var ebml = new EBMLReader(reader, keepReaderOpen, cache);
         return await Read(header, ebml, cancellationToken);
      }

      public static async ValueTask<EBMLDocumentReader> Read(Stream stream, bool keepStreamOpen = false,
         DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         var header = await EBMLHeader.Read(stream, cache, cancellationToken);
         var ebml = new EBMLReader(stream, keepStreamOpen, cache);
         return await Read(header, ebml, cancellationToken);
      }

      public void Dispose()
      {
         reader.Dispose();
      }
   }
}
