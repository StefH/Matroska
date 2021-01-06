using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Cues : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CuePoint, typeof(CuePoint))]
        public List<CuePoint>? CuePoints { get; set; }
    }
}