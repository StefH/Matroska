using NEbml.Core;

namespace Matroska.Extensions
{
    internal static class ReaderExtensions
    {
		//public static bool LocateElement(this EbmlReader reader, ElementDescriptor descriptor)
		//{
		//	return reader.LocateElement(descriptor.Identifier.EncodedValue);
		//}

		public static bool LocateElement(this EbmlReader reader, ulong identifier)
		{
			while (reader.ReadNext())
			{
				if (reader.ElementId == identifier)
				{
					return true;
				}
			}

			return false;
		}
	}
}