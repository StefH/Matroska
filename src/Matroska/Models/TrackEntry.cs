using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class TrackEntry
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackNumber)]
        public ulong TrackNumber { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TrackUID)]
        public ulong TrackUID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TrackType)]
        public TrackType TrackType { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagEnabled)]
        public ulong FlagEnabled { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagDefault)]
        public ulong FlagDefault { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagForced)]
        public ulong FlagForced { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagLacing)]
        public ulong FlagLacing { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Name)]
        public string? Name { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Language)]
        public string? Language { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecDelay)]
        public ulong CodecDelay { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SeekPreRoll)]
        public ulong SeekPreRoll { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecID)]
        public string? CodecID { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecPrivate)]
        public byte[]? CodecPrivate { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecName)]
        public string? CodecName { get; set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Audio)]
        public Audio? Audio { get; set; }
    }
}