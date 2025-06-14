using System.Linq;

namespace MediaContainers
{
   public class EBMLElementDefiniton
   {
      public static readonly EBMLElementDefiniton EBML = new EBMLElementDefiniton(0x1A45DFA3, EBMLElementType.Master, @"\EBML");
      public static readonly EBMLElementDefiniton EBMLVersion = new EBMLElementDefiniton(0x4286, EBMLElementType.UnsignedInteger, @"\EBML\EBMLVersion", defaultVal: "1");
      public static readonly EBMLElementDefiniton EBMLReadVersion = new EBMLElementDefiniton(0x42F7, EBMLElementType.UnsignedInteger, @"\EBML\EBMLReadVersion");
      public static readonly EBMLElementDefiniton EBMLMaxIDLength = new EBMLElementDefiniton(0x42F2, EBMLElementType.UnsignedInteger, @"\EBML\EBMLMaxIDLength", defaultVal: "4");
      public static readonly EBMLElementDefiniton EBMLMaxSizeLength = new EBMLElementDefiniton(0x42F3, EBMLElementType.UnsignedInteger, @"\EBML\EBMLMaxSizeLength", defaultVal: "8");
      public static readonly EBMLElementDefiniton DocType = new EBMLElementDefiniton(0x4282, EBMLElementType.String, @"\EBML\DocType");
      public static readonly EBMLElementDefiniton DocTypeVersion = new EBMLElementDefiniton(0x4287, EBMLElementType.UnsignedInteger, @"\EBML\DocTypeVersion");
      public static readonly EBMLElementDefiniton DocTypeReadVersion = new EBMLElementDefiniton(0x4285, EBMLElementType.UnsignedInteger, @"\EBML\DocTypeReadVersion");
      public static readonly EBMLElementDefiniton DocTypeExtension = new EBMLElementDefiniton(0x4281, EBMLElementType.Master, @"\EBML\DocTypeExtension");
      public static readonly EBMLElementDefiniton DocTypeExtensionName = new EBMLElementDefiniton(0x4283, EBMLElementType.String, @"\EBML\DocTypeExtension\DocTypeExtensionName");
      public static readonly EBMLElementDefiniton DocTypeExtensionVersion = new EBMLElementDefiniton(0x4284, EBMLElementType.UnsignedInteger, @"\EBML\DocTypeExtension\DocTypeExtensionVersion");
      public static readonly EBMLElementDefiniton Crc32 = new EBMLElementDefiniton(0xBF, EBMLElementType.Binary, "CRC-32");
      public static readonly EBMLElementDefiniton Void = new EBMLElementDefiniton(0xEC, EBMLElementType.Binary, "Void");
      public static readonly EBMLElementDefiniton Unknown = new EBMLElementDefiniton(0, EBMLElementType.Binary, "Unknown");

      private static readonly EBMLElementDefiniton[] headerElements = new EBMLElementDefiniton[]
      {
         EBML, EBMLVersion, EBMLReadVersion, EBMLMaxIDLength, EBMLMaxSizeLength,
         DocType, DocTypeVersion, DocTypeReadVersion, DocTypeExtension, DocTypeExtensionName, DocTypeExtensionVersion,
      };

      private static readonly EBMLElementDefiniton[] globalElements = new EBMLElementDefiniton[]
      {
         Crc32, Void
      };

      public readonly EBMLVInt Id;
      public readonly EBMLElementType Type;
      public readonly string Name;
      public readonly string Path;
      public readonly string FullPath;
      public readonly string DefaultValue;
      public readonly bool AllowUnknownSize;
      public readonly bool IsGlobal;

      public EBMLElementDefiniton(ulong id, EBMLElementType type, string fullPath, bool allowUnknownSize = false, string defaultVal = null)
      {
         Id = EBMLVInt.CreateWithMarker(id);
         Type = type;
         FullPath = fullPath;
         var components = fullPath.Split('\\', System.StringSplitOptions.RemoveEmptyEntries);
         Name = components[^1];
         if (Name.StartsWith('+')) { Name = Name.Substring(1); }
         if (!fullPath.StartsWith('\\') && components.Length == 1) { IsGlobal = true; Path = "\\"; }
         else { Path = "\\" + string.Join("\\", components.Take(components.Length - 1)); }
         AllowUnknownSize = allowUnknownSize;
         DefaultValue = defaultVal;
      }

      public static void AddHeaderElements(EBMLReader reader)
      {
         for (int i = 0; i < headerElements.Length; i++)
         {
            reader.AddElementDefinition(headerElements[i]);
         }
      }

      public static void AddGlobalElements(EBMLReader reader)
      {
         for (int i = 0; i < globalElements.Length; i++)
         {
            reader.AddElementDefinition(globalElements[i]);
         }
      }

      public bool IsDirectChildOf(EBMLElementDefiniton parent)
      {
         return IsGlobal || Path == parent.FullPath;
      }
   }
}
