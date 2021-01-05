namespace Matroska.Muxer.OggOpus.Models
{
    internal struct SegmentEntry
    {
        public byte[] SegmentBytes { get; set; }

        public byte[] Data { get; set; }

        public short TimeCode { get; set; }

        public int NumberOfFrames { get; set; }

        public int NumberOfSamples { get; set; }
    }
}