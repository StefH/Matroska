using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Segment
    {
        [MatroskaElementDescriptor(MatroskaSpecification.SeekHead)]
        public SeekHead? SeekHead { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Info)]
        public Info? Info { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Cues)]
        public Cues? Cues { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Tracks)]
        public Tracks? Tracks { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Cluster, typeof(Cluster))]
        public List<Cluster>? Clusters { get; set; }
    }
}