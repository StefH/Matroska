using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public sealed class EBMLDocumentWriter : IDisposable, IAsyncDisposable
   {
      private readonly EBMLWriter writer;

      public EBMLHeader Header { get; private set; }
      public EBMLWriter Writer => writer;

      private EBMLDocumentWriter(EBMLWriter writer, EBMLHeader header)
      {
         this.writer = writer;
         Header = header;
      }

      public bool CanBeWrittenBy(EBMLDocType type)
      {
         return type.CanWriteDocument(Header);
      }

      public static async ValueTask<EBMLDocumentWriter> Write(EBMLHeader header, IDataQueueWriter writer, bool keepWriterOpen = false,
         DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         var ebml = new EBMLWriter(writer, keepWriterOpen, cache);
         await header.Write(ebml, cancellationToken);
         return new EBMLDocumentWriter(ebml, header);
      }

      public static async ValueTask<EBMLDocumentWriter> Write(EBMLHeader header, Stream stream, bool keepStreamOpen = false,
         DataBufferCache cache = null, CancellationToken cancellationToken = default)
      {
         var ebml = new EBMLWriter(stream, keepStreamOpen, cache);
         await header.Write(ebml, cancellationToken);
         return new EBMLDocumentWriter(ebml, header);
      }

      public ValueTask DisposeAsync()
      {
         return writer.DisposeAsync();
      }

      public void Dispose()
      {
         writer.Dispose();
      }
   }
}
