namespace MediaContainers
{
   public struct EBMLDocType
   {
      public readonly string DocType;
      public readonly int WriteVersion;
      public readonly int ReadVersion;

      public EBMLDocType(string type, int writeVersion, int readVersion)
      {
         DocType = type;
         WriteVersion = writeVersion;
         ReadVersion = readVersion;
      }

      public bool CanReadDocument(EBMLHeader header)
      {
         if (header.DocType != DocType) { return false; }
         if (header.DocTypeReadVersion > ReadVersion) { return false; }
         return true;
      }

      public bool CanWriteDocument(EBMLHeader header)
      {
         if (header.DocType != DocType) { return false; }
         if (header.DocTypeVersion > WriteVersion) { return false; }
         return true;
      }
   }
}
