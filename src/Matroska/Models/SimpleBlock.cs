using System;
using System.IO;
using NEbml.Core;

namespace Matroska.Models
{
    /// <summary>
    /// http://matroska.sourceforge.net/technical/specs/index.html#simpleblock_structure
    /// </summary>
    public sealed class SimpleBlock : IParseRawBinary
    {
        /// <summary>
        /// Track Number (Track Entry)
        /// </summary>
        public ulong TrackNumber { get; private set; }

        /// <summary>
        /// Number of frames in the lace-1 (uint8)
        /// </summary>
        public int NumFrames { get; private set; }

        /// <summary>
        /// Timecode (relative to Cluster timecode, signed int16)
        /// </summary>
        public short TimeCode { get; private set; }

        /// <summary>
        /// Keyframe, set when the Block contains only keyframes
        /// </summary>
        public bool IsKeyFrame { get; private set; }

        /// <summary>
        /// Discardable, the frames of the Block can be discarded during playing if needed
        /// </summary>
        public bool IsDiscardable { get; private set; }

        /// <summary>
        /// Invisible, the codec should decode this frame but not display it
        /// </summary>
        public bool IsInvisible { get; private set; }

        /// <summary>
        /// Lacing
        /// </summary>
        public Lacing Lacing { get; private set; }

        public byte[]? Data { get; private set; }

        public void Parse(byte[] raw)
        {
            if (raw.Length > 7)
            {
                int y = 0;
            }

            int size = Math.Min(raw.Length, 16);
            byte[] small = new byte[size];
            Buffer.BlockCopy(raw, 0, small, 0, size);

            using var stream = new MemoryStream(small);
            using var bn = new BinaryReader(stream);

            var trackNumberAsVInt = VInt.Read(stream, 8, null);
            TrackNumber = trackNumberAsVInt.Value;

            TimeCode = bn.ReadInt16();
            var flags = bn.ReadByte();

            IsKeyFrame = (flags & 0x80) > 0;
            IsDiscardable = (flags & 0x01) > 0;
            IsInvisible = (flags & 0x08) > 0;
            Lacing = (Lacing)(flags & (byte)Lacing.Any);

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