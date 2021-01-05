namespace Matroska.Models
{
    /// <summary>
    /// http://matroska.sourceforge.net/technical/specs/index.html#simpleblock_structure
    /// </summary>
    public sealed class SimpleBlock : Block
    {
        /// <summary>
        /// Keyframe, set when the Block contains only keyframes
        /// </summary>
        public bool IsKeyFrame { get; private set; }

        /// <summary>
        /// Discardable, the frames of the Block can be discarded during playing if needed
        /// </summary>
        public bool IsDiscardable { get; private set; }

        public override void Parse(byte[] raw)
        {
            base.Parse(raw);

            IsKeyFrame = (Flags & 0x80) > 0;
            IsDiscardable = (Flags & 0x01) > 0;
        }
    }
}