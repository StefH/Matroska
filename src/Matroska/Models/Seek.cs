using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Seek : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.SeekID)]
        public byte[]? SeekID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SeekPosition)]
        public ulong SeekPosition { get; set; }
    }
}