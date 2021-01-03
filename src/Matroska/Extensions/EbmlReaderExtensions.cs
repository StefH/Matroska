using System.Collections.Generic;
using NEbml.Core;

namespace Matroska.Extensions
{
    internal static class EbmlReaderExtensions
    {
        public static bool IsKnownMasterElement(this EbmlReader reader)
        {
            return MatroskaSpecification.ElementDescriptors.TryGetValue(reader.ElementId, out var descriptor) && descriptor.Type == ElementType.MasterElement;
        }

        //public static ElementDescriptor? GetElementDescriptor(this EbmlReader reader)
        //{
        //    if (MatroskaSpecification.ElementDescriptors.TryGetValue(reader.ElementId, out var descriptor))
        //    {
        //        return descriptor;
        //    }

        //    throw new KeyNotFoundException();
        //}

        public static string GetName(this EbmlReader r, bool dumpValue = false)
        {
            string name = "?";
            string dump = "?";

            if (MatroskaSpecification.ElementDescriptors.TryGetValue(r.ElementId, out var el))
            {
                name = el.Name;

                if (dumpValue)
                {
                    switch (el.Type)
                    {
                        case ElementType.AsciiString:
                            dump = r.ReadAscii();
                            break;
                        case ElementType.Binary:
                            dump = "binary data";
                            break;
                        case ElementType.Date:
                            dump = r.ReadDate().ToString();
                            break;
                        case ElementType.Float:
                            dump = r.ReadFloat().ToString();
                            break;
                        case ElementType.SignedInteger:
                            dump = r.ReadInt().ToString();
                            break;
                        case ElementType.UnsignedInteger:
                            dump = r.ReadUInt().ToString();
                            break;
                        case ElementType.Utf8String:
                            dump = r.ReadUtf();
                            break;
                        case ElementType.MasterElement:
                            dump = "'Master'";
                            break;
                        default:
                            dump = $"unknown (id:{r})";
                            break;
                    }
                }
            }

            return $"0x{r.ElementId.Value:X8} {name} [{r.ElementSize} bytes]" + (dumpValue ? " Value: " + dump : "");
        }
    }
}
