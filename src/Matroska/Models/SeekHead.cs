using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class SeekHead
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Seek)]
        public Seek? Seek { get; set; }
    }
}