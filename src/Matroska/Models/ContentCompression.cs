using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class ContentCompression
    {
        [MatroskaElementDescriptor(MatroskaSpecification.ContentCompAlgo)]
        public ulong ContentCompAlgo { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.ContentCompSettings)]
        public byte[]? ContentCompSettings { get; private set; }
    }
}