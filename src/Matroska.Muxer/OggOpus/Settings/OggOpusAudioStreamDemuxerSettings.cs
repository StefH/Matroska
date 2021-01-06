namespace Matroska.Muxer.OggOpus.Settings
{
    public class OggOpusAudioStreamDemuxerSettings
    {
        private const byte MaxSegmentParts = 0x64;

        public byte MaxSegmentPartsPerOggPage { get; set; } = MaxSegmentParts;

        public ulong AudioTrackNumber { get; set; } = 1;

        public int AudioStreamSerial { get; set; } = -1071269784;
    }
}