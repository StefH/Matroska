using Matroska.Attributes;

namespace Matroska.Models
{
    public abstract class BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CRC32)]
        public byte[]? CRC32 { get; set; }
    }
}