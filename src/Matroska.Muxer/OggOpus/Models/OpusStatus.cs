namespace Matroska.Muxer.OggOpus.Models
{
    internal static class OpusStatus
    {
        public const int OPUS_OK = 0;
        public const int OPUS_BAD_ARG = -1;
        public const int OPUS_BUFFER_TOO_SMALL = -2;
        public const int OPUS_INTERNAL_ERROR = -3;
        public const int OPUS_INVALID_PACKET = -4;
        public const int OPUS_UNIMPLEMENTED = -5;
        public const int OPUS_INVALID_STATE = -6;
        public const int OPUS_ALLOC_FAIL = -7;
    }
}