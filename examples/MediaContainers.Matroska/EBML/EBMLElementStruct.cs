using System;
using System.Runtime.InteropServices;
using Xtremegaida.DataStructures;

namespace MediaContainers
{
   public struct EBMLElementStruct
   {
      public readonly EBMLElementDefiniton Definition;
      public readonly EBMLVInt DataSize;
      public readonly long DataOffset;
      public EBMLElementStructValue Value;
      public string String;
      public byte[] Binary;
      public IDataQueueReader Reader;
      public bool UseFloat32;

      public EBMLElementStruct(EBMLElementDefiniton definition, EBMLVInt dataSize, long dataOffset)
      {
         Definition = definition;
         DataSize = dataSize;
         DataOffset = dataOffset;
         Value = default;
         String = null;
         Binary = null;
         Reader = null;
         UseFloat32 = false;
      }

      public EBMLElement ToElement()
      {
         switch (Definition.Type)
         {
            case EBMLElementType.SignedInteger:
               return new EBMLSignedIntegerElement(Definition, DataSize, DataOffset, Value.SignedInteger);
            case EBMLElementType.UnsignedInteger:
               return new EBMLUnsignedIntegerElement(Definition, DataSize, DataOffset, Value.UnsignedInteger);
            case EBMLElementType.Float:
               if (UseFloat32) { return new EBMLFloatElement(Definition, DataSize, DataOffset, Value.Float32); }
               return new EBMLFloatElement(Definition, DataSize, DataOffset, Value.Float64);
            case EBMLElementType.Date:
               return new EBMLDateElement(Definition, DataSize, DataOffset, Value.Date);
            case EBMLElementType.String:
            case EBMLElementType.UTF8:
               return new EBMLStringElement(Definition, DataSize, DataOffset, String);
            case EBMLElementType.Master:
               return new EBMLMasterElement(Definition, DataSize, DataOffset);
            case EBMLElementType.Binary:
            default:
               if (Definition == EBMLElementDefiniton.Void)
               {
                  if (Reader != null) { return new EBMLVoidElement(DataSize, DataOffset, Reader, true); }
                  return new EBMLVoidElement(DataSize, DataOffset);
               }
               if (Reader != null) { return new EBMLBinaryElement(Definition, DataSize, DataOffset, Reader, true); }
               return new EBMLBinaryElement(Definition, DataSize, DataOffset, Binary);
         }
      }
   }

   [StructLayout(LayoutKind.Explicit)]
   public struct EBMLElementStructValue
   {
      [FieldOffset(0)] public long SignedInteger;
      [FieldOffset(0)] public ulong UnsignedInteger;
      [FieldOffset(0)] public double Float64;
      [FieldOffset(0)] public float Float32;
      [FieldOffset(0)] public DateTime Date;
   }
}
