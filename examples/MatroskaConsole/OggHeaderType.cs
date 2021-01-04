using System;

namespace Matroska
{
    [Flags]
    public enum OggHeaderType : byte
    {
        None = 0x00,

        /// <summary>
        /// The first packet on this page is a continuation of the previous packet in the logical bitstream.
        /// </summary>
        Continuation = 0x01,

        /// <summary>
        /// Beginning Of Stream. This page is the first page in the logical bitstream. The BOS flag must be set on the first page of every logical bitstream, and must not be set on any other page.
        /// </summary>
        BeginningOfStream = 0x02,

        /// <summary>
        /// This page is the last page in the logical bitstream. The EOS flag must be set on the final page of every logical bitstream, and must not be set on any other page.
        /// </summary>
        EndOfStream = 0x04
    }
}