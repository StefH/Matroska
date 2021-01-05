﻿using System.IO;
using System.Text;

namespace Matroska.Muxer.OggOpus.Models
{
    internal struct OggHeader
    {
        public const string OggS = "OggS";

        public string ID;
        public byte StreamVersion;
        public OggHeaderType TypeFlag;
        public ulong GranulePosition;
        public int Serial;
        public int PageNumber;
        public uint Checksum;
        public byte TotalSegments;
        public byte[] SegmentTable;

        public void ReadFromStream(BinaryReader r)
        {
            ID = Encoding.ASCII.GetString(r.ReadBytes(4));
            StreamVersion = r.ReadByte();
            TypeFlag = (OggHeaderType) r.ReadByte();
            GranulePosition = r.ReadUInt64();
            Serial = r.ReadInt32();
            PageNumber = r.ReadInt32();
            Checksum = r.ReadUInt32();
            TotalSegments = r.ReadByte();
            SegmentTable = r.ReadBytes(TotalSegments);
        }

        public void Write(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(OggS));
            w.Write(StreamVersion);
            w.Write((byte) TypeFlag);
            w.Write(GranulePosition);
            w.Write(Serial);
            w.Write(PageNumber);
            w.Write(Checksum);
            w.Write(TotalSegments);
            w.Write(SegmentTable);
        }

        public int GetPageLength()
        {
            int length = 0;
            for (int i = 0; i < TotalSegments; i++)
            {
                length += SegmentTable[i];
            }
            return length;
        }

        public int GetHeaderSize()
        {
            return 27 + SegmentTable.Length;
        }
    }
}