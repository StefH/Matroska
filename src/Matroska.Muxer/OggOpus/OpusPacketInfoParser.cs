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
using Matroska.Muxer.OggOpus.Models;

namespace Matroska.Muxer.OggOpus
{
    internal static class OpusPacketInfoParser
    {
        /// <summary>
        /// Gets the number of frames in an Opus packet.
        /// </summary>
        /// <param name="packet">An Opus packet</param>
        /// <param name="packetOffset">Opus packet offset.</param>
        /// <param name="len">The packet's length (must be at least 1)</param>
        /// <returns>The number of frames in the packet</returns>
        public static int GetNumFrames(ReadOnlySpan<byte> packet, int packetOffset, int len)
        {
            if (len < 1)
            {
                return OpusStatus.OPUS_BAD_ARG;
            }

            var count = packet[packetOffset] & 0x3;
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
                return OpusStatus.OPUS_INVALID_PACKET;
            }
            else
            {
                return packet[packetOffset + 1] & 0x3F;
            }
        }

        /// <summary>
        /// Gets the number of samples of an Opus packet.
        /// </summary>
        /// <param name="packet">An Opus packet</param>
        /// <param name="packetOffset">Opus packet offset.</param>
        /// <param name="len">The packet's length</param>
        /// <param name="Fs">The decoder's sampling rate in Hz. This must be a multiple of 400</param>
        /// <returns>The size of the PCM samples that this packet will be decoded to at the specified sample rate</returns>
        public static int GetNumSamples(ReadOnlySpan<byte> packet, int packetOffset, int len, int Fs)
        {
            int count = GetNumFrames(packet, packetOffset, len);

            if (count < 0)
            {
                return count;
            }

            var samples = count * GetNumSamplesPerFrame(packet, packetOffset, Fs);
            if (samples * 25 > Fs * 3)
            {
                // Can't have more than 120 ms
                return OpusStatus.OPUS_INVALID_PACKET;
            }
            
            return samples;
        }

        /// <summary>
        /// Gets the number of samples per frame from an Opus packet.
        /// </summary>
        /// <param name="packet">Opus packet. This must contain at least one byte of data.</param>
        /// <param name="packetOffset">Opus packet offset.</param>
        /// <param name="Fs">Sampling rate in Hz. This must be a multiple of 400, or inaccurate results will be returned.</param>
        /// <returns>Number of samples per frame</returns>
        public static int GetNumSamplesPerFrame(ReadOnlySpan<byte> packet, int packetOffset, int Fs)
        {
            if ((packet[packetOffset] & 0x80) != 0)
            {
                var audiosize = (packet[packetOffset] >> 3) & 0x3;
                return (Fs << audiosize) / 400;
            }
            else if ((packet[packetOffset] & 0x60) == 0x60)
            {
                return ((packet[packetOffset] & 0x08) != 0) ? Fs / 50 : Fs / 100;
            }
            else
            {
                var audiosize = ((packet[packetOffset] >> 3) & 0x3);
                if (audiosize == 3)
                {
                    return Fs * 60 / 1000;
                }
                else
                {
                    return (Fs << audiosize) / 100;
                }
            }
        }
    }
}