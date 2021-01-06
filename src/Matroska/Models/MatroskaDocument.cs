namespace Matroska.Models
{
    public sealed class MatroskaDocument
    {
        public Ebml Ebml { get; set; } = null!;

        public Segment Segment { get; set; } = null!;
    }
}