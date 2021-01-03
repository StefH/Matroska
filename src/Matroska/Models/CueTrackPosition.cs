using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class CueTrackPosition
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CueTrack)]
        public ulong CueTrack { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueClusterPosition)]
        public ulong CueClusterPosition { get; set; }
    }
}