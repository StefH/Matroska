namespace Matroska.Muxer.OggOpus.Settings
{
    public class OggOpusAudioStreamDemuxerSettings
    {
        private const byte MaxSegmentParts = 0x64;

        public byte MaxSegmentPartsPerOggPage { get; set; } = MaxSegmentParts;

        public int AudioTrackNumber { get; set; } = 1;
    }
}