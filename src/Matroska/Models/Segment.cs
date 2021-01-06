using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Segment : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.Void)]
        public byte[]? Void { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SeekHead)]
        public SeekHead SeekHead { get; set; } = null!;

        [MatroskaElementDescriptor(MatroskaSpecification.Info)]
        public Info Info { get; set; } = null!;

        [MatroskaElementDescriptor(MatroskaSpecification.Cues)]
        public Cues Cues { get; set; } = null!;

        [MatroskaElementDescriptor(MatroskaSpecification.Tracks)]
        public Tracks Tracks { get; set; } = null!;

        [MatroskaElementDescriptor(MatroskaSpecification.Cluster, typeof(Cluster))]
        public List<Cluster> Clusters { get; set; } = null!;
    }
}