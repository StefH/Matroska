namespace Matroska.Models
{
    public sealed class MatroskaDocument
    {
        public Ebml? Ebml { get; set; }

        public Segment? Segment { get; set; }
    }
}