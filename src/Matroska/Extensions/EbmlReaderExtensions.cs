using NEbml.Core;

namespace Matroska.Extensions
{
    internal static class EbmlReaderExtensions
    {
        public static bool LocateElement(this EbmlReader reader, ElementDescriptor descriptor)
        {
            return reader.LocateElement(descriptor.Identifier.EncodedValue);
        }

        public static bool LocateElement(this EbmlReader reader, ulong identifier)
        {
            while (reader.ReadNext())
            {
                if (reader.ElementId == identifier)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsKnownMasterElement(this EbmlReader reader)
        {
            return MatroskaSpecification.ElementDescriptors.TryGetValue(reader.ElementId, out var descriptor) && descriptor.Type == ElementType.MasterElement;
        }

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
                            dump = "'Binary Data'";
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
                            dump = "'MasterElement'";
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
