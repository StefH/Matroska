using System;
using System.IO;
using NEbml.Core;

namespace Matroska.Models
{
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

        //public byte[]? Raw { get; set; }

        public void Parse(byte[] raw)
        {
            using var stream = new MemoryStream(raw);

            using var bn = new BinaryReader(stream);

            var trackNumberAsVInt = VInt.Read(stream, 4, new byte[8]);
            TrackNumber = trackNumberAsVInt.Value;

            TimeCode = bn.ReadInt16();
            var flags = bn.ReadByte();

            IsKeyFrame = (flags & 0x80) > 0;
            IsDiscardable = (flags & 0x01) > 0;
            IsInvisible = (flags & 0x08) > 0;
            Lacing = (Lacing)(flags & (byte)Lacing.Any);

            NumFrames = 0;

            int laceCodedSizeOfEachFrame = 0;
            if (Lacing != Lacing.No)
            {
                NumFrames = bn.ReadByte();

                var fixed_sizeLacing = (flags & 0x04) > 0;
                if (Lacing != Lacing.FixedSize)
                {
                    laceCodedSizeOfEachFrame = bn.ReadByte();
                }
            }

            Data = raw.AsSpan().Slice((int) stream.Position).ToArray();
        }
    }
}