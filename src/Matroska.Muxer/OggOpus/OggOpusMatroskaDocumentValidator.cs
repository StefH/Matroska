using System.Linq;
using FluentValidation;
using Matroska.Models;

namespace Matroska.Muxer.OggOpus
{
    internal class OggOpusMatroskaDocumentValidator : AbstractValidator<MatroskaDocument>
    {
        public OggOpusMatroskaDocumentValidator()
        {
            RuleFor(_ => _.Segment).NotNull();
            RuleFor(_ => _.Segment.Tracks).NotNull();
            RuleFor(_ => _.Segment.Tracks.TrackEntries).NotEmpty();
            RuleFor(_ => _.Segment.Tracks.TrackEntries).Must(t => t.Any(te => te.CodecID == OggOpusConstants.CodecID))
                .WithMessage($"At least 1 Audio Stream with CodecID = '{OggOpusConstants.CodecID}' should be present.");

            RuleForEach(_ => _.Segment.Tracks.TrackEntries).SetValidator(new OggOpusTrackEntryValidator());
        }
    }

    internal class OggOpusTrackEntryValidator : AbstractValidator<TrackEntry>
    {
        public OggOpusTrackEntryValidator()
        {
            // RuleFor(_ => _.Audio).NotNull();
        }
    }
}