using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class ContentEncoding
    {
        [MatroskaElementDescriptor(MatroskaSpecification.ContentCompression, typeof(ContentCompression))]
        public List<ContentCompression>? ContentCompressions { get; private set; }
    }
}