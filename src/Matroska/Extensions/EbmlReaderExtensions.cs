using System.Globalization;
using NEbml.Core;

namespace Matroska.Extensions;

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
        var name = "?";
        var dump = "?";

        if (MatroskaSpecification.ElementDescriptors.TryGetValue(r.ElementId, out var el))
        {
            name = el.Name;

            if (dumpValue)
            {
                dump = el.Type switch
                {
                    ElementType.AsciiString => r.ReadAscii(),
                    ElementType.Binary => "'Binary Data'",
                    ElementType.Date => r.ReadDate().ToString(CultureInfo.CurrentCulture),
                    ElementType.Float => r.ReadFloat().ToString(CultureInfo.CurrentCulture),
                    ElementType.SignedInteger => r.ReadInt().ToString(),
                    ElementType.UnsignedInteger => r.ReadUInt().ToString(),
                    ElementType.Utf8String => r.ReadUtf(),
                    ElementType.MasterElement => "'MasterElement'",
                    _ => $"unknown (id:{r})"
                };
            }
        }

        return $"0x{r.ElementId.Value:X8} {name} [{r.ElementSize} bytes]" + (dumpValue ? " Value: " + dump : "");
    }
}