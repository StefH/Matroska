using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class SeekHead : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Seek)]
        public Seek? Seek { get; set; }
    }
}