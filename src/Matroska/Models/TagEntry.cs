using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class TagEntry : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Targets, typeof(Target))]
        public List<Target>? Targets { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SimpleTag, typeof(SimpleTag))]
        public List<SimpleTag>? SimpleTags { get; private set; }
    }
}