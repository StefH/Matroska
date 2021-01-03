using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Cues
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CuePoint, typeof(CuePoint))]
        public List<CuePoint>? CuePoints { get; set; }
    }
}