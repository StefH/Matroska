using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Seek
    {
        [MatroskaElementDescriptor(MatroskaSpecification.SeekID)]
        public byte[]? SeekID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SeekPosition)]
        public ulong SeekPosition { get; set; }
    }
}