using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Tracks
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackEntry)]
        public TrackEntry? TrackEntry { get; set; }
    }
}