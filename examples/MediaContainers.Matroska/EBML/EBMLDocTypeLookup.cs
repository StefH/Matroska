using System;
using System.Collections.Generic;

namespace MediaContainers
{
   public static class EBMLDocTypeLookup
   {
      private static readonly List<Func<EBMLHeader, EBMLReader, bool>> handlers = new List<Func<EBMLHeader, EBMLReader, bool>>();

      public static void AddEBMLDocType(Func<EBMLHeader, EBMLReader, bool> handler)
      {
         lock (handlers) { handlers.Add(handler); }
      }

      public static bool HandleEBMLDocType(EBMLHeader header, EBMLReader reader)
      {
         for (int i = handlers.Count - 1; i >= 0; i--)
         { 
            if (handlers[i](header, reader)) { return true; }
         }
         return false;
      }
   }
}
