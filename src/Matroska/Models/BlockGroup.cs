using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class BlockGroup
    {
        [MatroskaElementDescriptor(MatroskaSpecification.DiscardPadding)]
        public long DiscardPadding { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Block, typeof(Block))]
        public List<Block>? Blocks { get; set; }
    }
}