namespace System
{
    internal static class VIntUtils
    {
        public static (ulong EncodedValue, int Length) Encode(ulong value, int length = 0)
        {
            if (length == 0)
            {
                while (DataBitsMask[++length] <= value) { }
            }

            var sizeMarker = 1UL << (7 * length);
            return (value | sizeMarker, length);
        }

        /// <summary>
        /// Maps length to data bits mask
        /// </summary>
        private static readonly ulong[] DataBitsMask =
        {
            (1L << 0) - 1,
            (1L << 7) - 1,
            (1L << 14) - 1,
            (1L << 21) - 1,
            (1L << 28) - 1,
            (1L << 35) - 1,
            (1L << 42) - 1,
            (1L << 49) - 1,
            (1L << 56) - 1
        };
    }
}