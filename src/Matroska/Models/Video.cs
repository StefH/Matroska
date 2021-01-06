using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Video
    {
        [MatroskaElementDescriptor(MatroskaSpecification.FlagInterlaced)]
        public ulong FlagInterlaced { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.FieldOrder)]
        public ulong FieldOrder { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.StereoMode)]
        public ulong StereoMode { get; private set; }
        
        [MatroskaElementDescriptor(MatroskaSpecification.AlphaMode)]
        public ulong AlphaMode { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.PixelWidth)]
        public ulong PixelWidth { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.PixelHeight)]
        public ulong PixelHeight { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.DisplayWidth)]
        public ulong DisplayWidth { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.DisplayHeight)]
        public ulong DisplayHeight { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.AspectRatioType)]
        public ulong AspectRatioType { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Color)]
        public Color? Color { get; private set; }

    }
}