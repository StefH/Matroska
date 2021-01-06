using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class TrackEntry
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TrackNumber)]
        public ulong TrackNumber { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TrackUID)]
        public ulong TrackUID { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TrackType)]
        public TrackType TrackType { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagEnabled)]
        public ulong FlagEnabled { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagDefault)]
        public ulong FlagDefault { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagForced)]
        public ulong FlagForced { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FlagLacing)]
        public ulong FlagLacing { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Name)]
        public string? Name { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Language)]
        public string? Language { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecDelay)]
        public ulong CodecDelay { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.SeekPreRoll)]
        public ulong SeekPreRoll { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecID)]
        public string? CodecID { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecPrivate)]
        public byte[]? CodecPrivate { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.CodecName)]
        public string? CodecName { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.MinCache)]
        public ulong? MinCache { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.DefaultDuration)]
        public ulong? DefaultDuration { get; private set; } = null!;

        [MatroskaElementDescriptor(MatroskaSpecification.Audio)]
        public Audio? Audio { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Video)]
        public Video? Video { get; private set; }
    }
}