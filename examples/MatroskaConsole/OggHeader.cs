using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matroska
{
    struct OggHeader
    {
        public const string ID = "OggS";                                               // Always "OggS"
        public byte StreamVersion;                           // Stream structure version
        public OggHeaderType TypeFlag;                                        // Header type flag
        public ulong GranulePosition;                      // Absolute granule position
        public int Serial;                                       // Stream serial number
        public int PageNumber;                                   // Page sequence number
        public uint Checksum;                                              // Page CRC32
        public byte NumberOfSegments;                                 // Number of page segments
        public byte[] SegmentTable;                     // Lacing values - segment sizes

        //public void ReadFromStream(BinaryReader r)
        //{
        //    ID = Encoding.ASCII.GetString(r.ReadBytes(4));
        //    StreamVersion = r.ReadByte();
        //    TypeFlag = r.ReadByte();
        //    GranulePosition = r.ReadUInt64();
        //    Serial = r.ReadInt32();
        //    PageNumber = r.ReadInt32();
        //    Checksum = r.ReadUInt32();
        //    Segments = r.ReadByte();
        //    SegmentTable = r.ReadBytes(Segments);
        //}

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(StreamVersion);
            w.Write((byte) TypeFlag);
            w.Write(GranulePosition);
            w.Write(Serial);
            w.Write(PageNumber);
            w.Write(Checksum);
            w.Write(NumberOfSegments);
            w.Write(SegmentTable);
        }

        public int GetPageLength()
        {
            int length = 0;
            for (int i = 0; i < NumberOfSegments; i++)
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
