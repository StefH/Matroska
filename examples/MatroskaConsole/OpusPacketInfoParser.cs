/* Copyright (c) 2007-2008 CSIRO
   Copyright (c) 2007-2011 Xiph.Org Foundation
   Originally written by Jean-Marc Valin, Gregory Maxwell, Koen Vos,
   Timothy B. Terriberry, and the Opus open-source contributors
   Ported to C# by Logan Stromberg

   Redistribution and use in source and binary forms, with or without
   modification, are permitted provided that the following conditions
   are met:

   - Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

   - Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.

   - Neither the name of Internet Society, IETF or IETF Trust, nor the
   names of specific contributors, may be used to endorse or promote
   products derived from this software without specific prior written
   permission.

   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
   ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
   LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
   A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER
   OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
   EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
   PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
   LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
   NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
   SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matroska
{
    public static class OpusError
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

    internal static class OpusPacketInfoParser
    {
        /// <summary>
        /// Gets the number of frames in an Opus packet.
        /// </summary>
        /// <param name="packet">An Opus packet</param>
        /// <param name="len">The packet's length (must be at least 1)</param>
        /// <returns>The number of frames in the packet</returns>
        public static int GetNumFrames(byte[] packet, int packet_offset, int len)
        {
            int count;
            if (len < 1)
            {
                return OpusError.OPUS_BAD_ARG;
            }

            count = packet[packet_offset] & 0x3;
            if (count == 0)
            {
                return 1;
            }
            else if (count != 3)
            {
                return 2;
            }
            else if (len < 2)
            {
                return OpusError.OPUS_INVALID_PACKET;
            }
            else
            {
                return packet[packet_offset + 1] & 0x3F;
            }
        }
    }
}
