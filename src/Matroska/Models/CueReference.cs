using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class CueReference
    {
        [MatroskaElementDescriptor(MatroskaSpecification.CueRefTime)]
        public ulong CueRefTime { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueRefCluster)]
        public ulong CueRefCluster { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueRefNumber)]
        public ulong CueRefNumber { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CueRefCodecState)]
        public ulong CueRefCodecState { get; set; }
    }
}