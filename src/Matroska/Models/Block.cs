using System;
using System.IO;
using NEbml.Core;

namespace Matroska.Models
{
    /// <summary>
    /// http://matroska.sourceforge.net/technical/specs/index.html#block_structure
    /// </summary>
    public class Block : IParseRawBinary
    {
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
            byte[] small = new byte[size];
            Buffer.BlockCopy(raw, 0, small, 0, size);

            using var stream = new MemoryStream(small);
            using var bn = new BinaryReader(stream);

            var trackNumberAsVInt = VInt.Read(stream, 8, null);
            TrackNumber = trackNumberAsVInt.Value;

            TimeCode = bn.ReadInt16();
            Flags = bn.ReadByte();

            IsInvisible = (Flags & 0x08) > 0;
            Lacing = (Lacing)(Flags & (byte)Lacing.Any);

            int laceCodedSizeOfEachFrame = 0;
            if (Lacing != Lacing.No)
            {
                NumFrames = bn.ReadByte();

                if (Lacing != Lacing.FixedSize)
                {
                    laceCodedSizeOfEachFrame = bn.ReadByte();
                }
            }

            Data = raw.AsSpan().Slice((int)stream.Position).ToArray();
        }
    }
}