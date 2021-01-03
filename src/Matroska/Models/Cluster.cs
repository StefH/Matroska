using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Cluster
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Timecode)]
        public ulong Timecode { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SimpleBlock, typeof(SimpleBlock))]
        public List<SimpleBlock>? SimpleBlocks { get; set; }
    }
}