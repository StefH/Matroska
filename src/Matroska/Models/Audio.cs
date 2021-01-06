using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Audio : BaseCRC32
    {
        [MatroskaElementDescriptor(MatroskaSpecification.SamplingFrequency)]
        public double SamplingFrequency { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.OutputSamplingFrequency)]
        public double? OutputSamplingFrequency { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.Channels)]
        public ulong Channels { get; private set; }

        [MatroskaElementDescriptor(MatroskaSpecification.BitDepth)]
        public ulong BitDepth { get; private set; }
    }
}