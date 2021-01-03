using Matroska.Attributes;

namespace Matroska.Models
{
    public sealed class Ebml
    {
		[MatroskaElementDescriptor(MatroskaSpecification.EBMLVersion)]
		public ulong EBMLVersion { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.EBMLReadVersion)]
		public ulong EBMLReadVersion { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.EBMLMaxIDLength)]
		public ulong EBMLMaxIDLength { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.EBMLMaxSizeLength)]
		public ulong EBMLMaxSizeLength { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.DocType)]
		public string? DocType { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.DocTypeVersion)]
		public ulong DocTypeVersion { get; set; }

		[MatroskaElementDescriptor(MatroskaSpecification.DocTypeReadVersion)]
		public ulong DocTypeReadVersion { get; set; }
	}
}