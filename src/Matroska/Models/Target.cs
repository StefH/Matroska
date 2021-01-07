using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Target
    {
        [MatroskaElementDescriptor(MatroskaSpecification.TargetTypeValue)]
        public ulong? TargetTypeValue { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TargetType)]
        public string? TargetType { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TagTrackUID)]
        public ulong? TagTrackUID { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TagEditionUID)]
        public ulong? TagEditionUID { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TagChapterUID)]
        public ulong? TagChapterUID { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.TagAttachmentUID)]
        public ulong? TagAttachmentUID { get; private set; }
    }
}