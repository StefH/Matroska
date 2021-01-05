using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class CuePoint
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CueTime)]
        public ulong CueTime { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueTrackPositions, typeof(CueTrackPosition))]
        public List<CueTrackPosition>? CueTrackPositions { get; set; }
    }
}
