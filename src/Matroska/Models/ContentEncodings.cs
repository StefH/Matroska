using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class ContentEncodings
    {
        [MatroskaElementDescriptor(MatroskaSpecification.ContentEncoding, typeof(ContentEncoding))]
        public List<ContentEncoding>? ContentEncoding { get; private set; }
    }
}