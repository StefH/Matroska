﻿using System;
using System.IO;
using Matroska.Enumerations;

namespace Matroska.Models;

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

    public virtual void Parse(Span<byte> span)
    {
        var spanReader = new SpanReader(span);

        TrackNumber = spanReader.ReadVInt().Value;
        TimeCode = spanReader.ReadShort(true); // EBML integer datatypes are big-endian
        Flags = spanReader.ReadByte();

        IsInvisible = (Flags & InvisibleBit) == InvisibleBit;
        Lacing = (Lacing)(Flags & LacingBits);

        if (Lacing != Lacing.No)
        {
            NumFrames = spanReader.ReadByte();

            if (Lacing != Lacing.FixedSize)
            {
                LaceCodedSizeOfEachFrame = spanReader.ReadByte();
            }
        }

        Data = span.Slice(spanReader.Position).ToArray();
    }
}