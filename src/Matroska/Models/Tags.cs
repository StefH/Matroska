using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Tags : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Tag, typeof(TagEntry))]
        public List<TagEntry>? TagEntries { get; private set; }
    }
}