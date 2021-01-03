using System;
using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Info
    {
        [MatroskaElementDescriptor(MatroskaSpecification.SegmentUID)]
        public byte[]? SegmentUID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SegmentFilename)]
        public string? SegmentFilename { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.PrevUID)]
        public byte[]? PrevUID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.PrevFilename)]
        public string? PrevFilename { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.NextUID)]
        public byte[]? NextUID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.NextFilename)]
        public string? NextFilename { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SegmentFamily)]
        public byte[]? SegmentFamily { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.ChapterTranslate)]
        public ChapterTranslate? ChapterTranslate { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TimestampScale)]
        public ulong TimestampScale { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Duration)]
        public double Duration { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.DateUTC)]
        public DateTime DateUTC { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Title)]
        public string? Title { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.MuxingApp)]
        public string? MuxingApp { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.WritingApp)]
        public string? WritingApp { get; set; }
    }
}