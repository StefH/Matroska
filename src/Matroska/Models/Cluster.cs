using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Cluster : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Timecode)]
        public ulong Timecode { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Position)]
        public ulong? Position { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.PrevSize)]
        public ulong? PrevSize { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SimpleBlock, typeof(SimpleBlock))]
        public List<SimpleBlock>? SimpleBlocks { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.BlockGroup, typeof(BlockGroup))]
        public List<BlockGroup>? BlockGroups { get; set; }
    }
}