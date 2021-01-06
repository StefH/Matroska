using System.Collections.Generic;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Tracks
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackEntry, typeof(TrackEntry))]
        public List<TrackEntry> TrackEntries { get; set; } = null!;
    }
}