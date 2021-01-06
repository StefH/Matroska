using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class BlockGroup
    {
        [MatroskaElementDescriptor(MatroskaSpecification.ReferenceBlock)]
        public long? ReferenceBlock { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.DiscardPadding)]
        public long DiscardPadding { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.BlockDuration)]
        public ulong BlockDuration { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Block, typeof(Block))]
        public List<Block>? Blocks { get; private set; } = null!;
    }
}