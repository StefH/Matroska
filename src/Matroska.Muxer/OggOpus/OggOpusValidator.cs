using System.Linq;
using FluentValidation;
using Matroska.Models;
using Matroska.Muxer.OggOpus.Settings;

namespace Matroska.Muxer.OggOpus
{
    internal class OggOpusValidator : AbstractValidator<(OggOpusAudioStreamDemuxerSettings settings, MatroskaDocument doc)>
    {
        public OggOpusValidator()
        {
            RuleFor(_ => _.settings).NotNull();
            RuleFor(_ => _.settings.MaxSegmentPartsPerOggPage).LessThan((byte) 255); // TODO

            RuleFor(_ => _.doc).NotNull();
            RuleFor(_ => _.doc.Segment).NotNull();
            RuleFor(_ => _.doc.Segment.Tracks).NotNull();
            RuleFor(_ => _.doc.Segment.Tracks.TrackEntries).NotEmpty();
            RuleFor(_ => _.doc.Segment.Tracks.TrackEntries).Must(t => t.Any(te => te.CodecID == OggOpusConstants.CodecID))
                .WithMessage($"At least 1 Audio Stream with CodecID = '{OggOpusConstants.CodecID}' should be present.");

            RuleFor(_ => _).Must(_ => _.doc.Segment.Tracks.TrackEntries.Any(te => te.TrackNumber == _.settings.AudioTrackNumber))
                .WithMessage(_ => $"Audio Stream with TrackNumber = '{_.settings.AudioTrackNumber}' is not found.");

            // RuleForEach(_ => _.doc.Segment.Tracks.TrackEntries).SetValidator(new OggOpusTrackEntryValidator());
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