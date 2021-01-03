using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class CueTrackPosition
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CueTrack)]
        public ulong CueTrack { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueClusterPosition)]
        public ulong CueClusterPosition { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueRelativePosition)]
        public ulong CueRelativePosition { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueDuration)]
        public ulong CueDuration { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueBlockNumber)]
        public ulong CueBlockNumber { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueCodecState)]
        public ulong CueCodecState { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueReference)]
        public List<CueReference>? CueReferences { get; set; }
    }
}