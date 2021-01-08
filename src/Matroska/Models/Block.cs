using System;
using System.IO;
using Matroska.Enumerations;
using NEbml.Core;

namespace Matroska.Models
{
    /// <summary>
    /// http://matroska.sourceforge.net/technical/specs/index.html#block_structure
    /// </summary>
    public class Block : IParseRawBinary
    {
        private const byte LacingBits = 0b0000110;
        private const byte InvisibleBit = 0b00010000;

        /// <summary>
        /// Track Number (Track Entry)
        /// </summary>
        public ulong TrackNumber { get; private set; }

        public byte Flags { get; private set; }

        /// <summary>
        /// Number of frames in the lace-1 (uint8)
        /// </summary>
        public int NumFrames { get; private set; }

        /// <summary>
        /// Lace-coded size of each frame of the lace, except for the last one (multiple uint8). 
        /// *This is not used with Fixed-size lacing as it is calculated automatically from (total size of lace) / (number of frames in lace).
        /// </summary>
        public byte LaceCodedSizeOfEachFrame { get; private set; }

        /// <summary>
        /// Timecode (relative to Cluster timecode, signed int16)
        /// </summary>
        public short TimeCode { get; private set; }

        /// <summary>
        /// Invisible, the codec should decode this frame but not display it
        /// </summary>
        public bool IsInvisible { get; private set; }

        /// <summary>
        /// Lacing
        /// </summary>
        public Lacing Lacing { get; private set; }

        public byte[]? Data { get; private set; }



        public virtual void Parse(byte[] raw)
        {
            int size = Math.Min(raw.Length, 16);
            using var stream = new MemoryStream(raw.AsSpan().Slice(0, size).ToArray());
            using var binaryReader = new BinaryReader(stream);

            var trackNumberAsVInt = VInt.Read(stream, 8, null);
            TrackNumber = trackNumberAsVInt.Value;

            TimeCode = binaryReader.ReadInt16();
            Flags = binaryReader.ReadByte();

            IsInvisible = (Flags & InvisibleBit) == InvisibleBit;
            Lacing = (Lacing)(Flags & LacingBits);

            if (Lacing != Lacing.No)
            {
                NumFrames = binaryReader.ReadByte();

                if (Lacing != Lacing.FixedSize)
                {
                    LaceCodedSizeOfEachFrame = binaryReader.ReadByte();
                }
            }

            Data = raw.AsSpan().Slice((int)stream.Position).ToArray();
        }
    }
}