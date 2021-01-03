﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ATL;
using Commons;
using Matroska.Models;
using NEbml.Core;

namespace Matroska
{
    public static class OggCRC32
    {
        // CRC table for checksum calculating according to OGG specifics
        private static readonly uint[] CRC_TABLE = new uint[256] { 0x00000000, 0x04C11DB7, 0x09823B6E, 0x0D4326D9, 0x130476DC, 0x17C56B6B, 0x1A864DB2, 0x1E475005, 0x2608EDB8, 0x22C9F00F, 0x2F8AD6D6, 0x2B4BCB61, 0x350C9B64, 0x31CD86D3, 0x3C8EA00A, 0x384FBDBD, 0x4C11DB70, 0x48D0C6C7, 0x4593E01E, 0x4152FDA9, 0x5F15ADAC, 0x5BD4B01B, 0x569796C2, 0x52568B75, 0x6A1936C8, 0x6ED82B7F, 0x639B0DA6, 0x675A1011, 0x791D4014, 0x7DDC5DA3, 0x709F7B7A, 0x745E66CD, 0x9823B6E0, 0x9CE2AB57, 0x91A18D8E, 0x95609039, 0x8B27C03C, 0x8FE6DD8B, 0x82A5FB52, 0x8664E6E5, 0xBE2B5B58, 0xBAEA46EF, 0xB7A96036, 0xB3687D81, 0xAD2F2D84, 0xA9EE3033, 0xA4AD16EA, 0xA06C0B5D, 0xD4326D90, 0xD0F37027, 0xDDB056FE, 0xD9714B49, 0xC7361B4C, 0xC3F706FB, 0xCEB42022, 0xCA753D95, 0xF23A8028, 0xF6FB9D9F, 0xFBB8BB46, 0xFF79A6F1, 0xE13EF6F4, 0xE5FFEB43, 0xE8BCCD9A, 0xEC7DD02D, 0x34867077, 0x30476DC0, 0x3D044B19, 0x39C556AE, 0x278206AB, 0x23431B1C, 0x2E003DC5, 0x2AC12072, 0x128E9DCF, 0x164F8078, 0x1B0CA6A1, 0x1FCDBB16, 0x018AEB13, 0x054BF6A4, 0x0808D07D, 0x0CC9CDCA, 0x7897AB07, 0x7C56B6B0, 0x71159069, 0x75D48DDE, 0x6B93DDDB, 0x6F52C06C, 0x6211E6B5, 0x66D0FB02, 0x5E9F46BF, 0x5A5E5B08, 0x571D7DD1, 0x53DC6066, 0x4D9B3063, 0x495A2DD4, 0x44190B0D, 0x40D816BA, 0xACA5C697, 0xA864DB20, 0xA527FDF9, 0xA1E6E04E, 0xBFA1B04B, 0xBB60ADFC, 0xB6238B25, 0xB2E29692, 0x8AAD2B2F, 0x8E6C3698, 0x832F1041, 0x87EE0DF6, 0x99A95DF3, 0x9D684044, 0x902B669D, 0x94EA7B2A, 0xE0B41DE7, 0xE4750050, 0xE9362689, 0xEDF73B3E, 0xF3B06B3B, 0xF771768C, 0xFA325055, 0xFEF34DE2, 0xC6BCF05F, 0xC27DEDE8, 0xCF3ECB31, 0xCBFFD686, 0xD5B88683, 0xD1799B34, 0xDC3ABDED, 0xD8FBA05A, 0x690CE0EE, 0x6DCDFD59, 0x608EDB80, 0x644FC637, 0x7A089632, 0x7EC98B85, 0x738AAD5C, 0x774BB0EB, 0x4F040D56, 0x4BC510E1, 0x46863638, 0x42472B8F, 0x5C007B8A, 0x58C1663D, 0x558240E4, 0x51435D53, 0x251D3B9E, 0x21DC2629, 0x2C9F00F0, 0x285E1D47, 0x36194D42, 0x32D850F5, 0x3F9B762C, 0x3B5A6B9B, 0x0315D626, 0x07D4CB91, 0x0A97ED48, 0x0E56F0FF, 0x1011A0FA, 0x14D0BD4D, 0x19939B94, 0x1D528623, 0xF12F560E, 0xF5EE4BB9, 0xF8AD6D60, 0xFC6C70D7, 0xE22B20D2, 0xE6EA3D65, 0xEBA91BBC, 0xEF68060B, 0xD727BBB6, 0xD3E6A601, 0xDEA580D8, 0xDA649D6F, 0xC423CD6A, 0xC0E2D0DD, 0xCDA1F604, 0xC960EBB3, 0xBD3E8D7E, 0xB9FF90C9, 0xB4BCB610, 0xB07DABA7, 0xAE3AFBA2, 0xAAFBE615, 0xA7B8C0CC, 0xA379DD7B, 0x9B3660C6, 0x9FF77D71, 0x92B45BA8, 0x9675461F, 0x8832161A, 0x8CF30BAD, 0x81B02D74, 0x857130C3, 0x5D8A9099, 0x594B8D2E, 0x5408ABF7, 0x50C9B640, 0x4E8EE645, 0x4A4FFBF2, 0x470CDD2B, 0x43CDC09C, 0x7B827D21, 0x7F436096, 0x7200464F, 0x76C15BF8, 0x68860BFD, 0x6C47164A, 0x61043093, 0x65C52D24, 0x119B4BE9, 0x155A565E, 0x18197087, 0x1CD86D30, 0x029F3D35, 0x065E2082, 0x0B1D065B, 0x0FDC1BEC, 0x3793A651, 0x3352BBE6, 0x3E119D3F, 0x3AD08088, 0x2497D08D, 0x2056CD3A, 0x2D15EBE3, 0x29D4F654, 0xC5A92679, 0xC1683BCE, 0xCC2B1D17, 0xC8EA00A0, 0xD6AD50A5, 0xD26C4D12, 0xDF2F6BCB, 0xDBEE767C, 0xE3A1CBC1, 0xE760D676, 0xEA23F0AF, 0xEEE2ED18, 0xF0A5BD1D, 0xF464A0AA, 0xF9278673, 0xFDE69BC4, 0x89B8FD09, 0x8D79E0BE, 0x803AC667, 0x84FBDBD0, 0x9ABC8BD5, 0x9E7D9662, 0x933EB0BB, 0x97FFAD0C, 0xAFB010B1, 0xAB710D06, 0xA6322BDF, 0xA2F33668, 0xBCB4666D, 0xB8757BDA, 0xB5365D03, 0xB1F740B4 };

        public static uint CalculateCRC(uint CRC, byte[] data, uint size)
        {
            for (uint i = 0; i < size; i++)
            {
                CRC = (CRC << 8) ^ CRC_TABLE[((CRC >> 24) & 0xFF) ^ data[i]];
            }

            return CRC;
        }
    }

    class OpusHeader
    {
        /*
         * typedef struct {
   int version;
   int channels; // Number of channels: 1..255
        int preskip;
        ogg_uint32_t input_sample_rate;
        int gain; // in dB S7.8 should be zero whenever possible 
        int channel_mapping;
        // The rest is only used if channel_mapping != 0 
        int nb_streams;
        int nb_coupled;
        unsigned char stream_map[255];
        unsigned char dmatrix[OPUS_DEMIXING_MATRIX_SIZE_MAX];
    }
    OpusHeader;*/
        public string ID = "OpusHead";
        public byte Version;
        public byte OutputChannelCount;
        public ushort PreSkip;
        public uint InputSampleRate;
        public short OutputGain;
        public byte ChannelMappingFamily;

        public byte StreamCount;
        public byte CoupledStreamCount;
        public byte[] ChannelMapping;

        public void Write(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(Version);
            w.Write(OutputChannelCount);
            w.Write(PreSkip);
            w.Write(InputSampleRate);
            w.Write(OutputGain);
            w.Write(ChannelMappingFamily);
            // w.Write(StreamCount);
            //w.Write(CoupledStreamCount);
            //w.Write(ChannelMapping);
        }
    }

    class OggHeader
    {
        // Vorbis identification packet (frame) ID
        static readonly string VORBIS_HEADER_ID = (char)1 + "vorbis";

        // Vorbis tag packet (frame) ID
        static readonly string VORBIS_TAG_ID = (char)3 + "vorbis";

        // Vorbis setup packet (frame) ID
        static readonly string VORBIS_SETUP_ID = (char)5 + "vorbis";

        // Vorbis parameter frame ID
        const string OPUS_HEADER_ID = "OpusHead";

        // Opus tag frame ID
        const string OPUS_TAG_ID = "OpusTags";


        public string ID = "OggS";                                               // Always "OggS"
        public byte StreamVersion;                           // Stream structure version
        public byte TypeFlag;                                        // Header type flag
        public ulong GranulePosition;                      // Absolute granule position
        public int Serial;                                       // Stream serial number
        public int PageNumber;                                   // Page sequence number
        public uint Checksum;                                              // Page CRC32
        public byte Segments;                                 // Number of page segments
        public byte[] SegmentTable;                     // Lacing values - segment sizes

        public void ReadFromStream(BinaryReader r)
        {
            ID = Encoding.ASCII.GetString(r.ReadBytes(4));
            StreamVersion = r.ReadByte();
            TypeFlag = r.ReadByte();
            GranulePosition = r.ReadUInt64();
            Serial = r.ReadInt32();
            PageNumber = r.ReadInt32();
            Checksum = r.ReadUInt32();
            Segments = r.ReadByte();
            SegmentTable = r.ReadBytes(Segments);
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Encoding.ASCII.GetBytes(ID));
            w.Write(StreamVersion);
            w.Write(TypeFlag);
            w.Write(GranulePosition);
            w.Write(Serial);
            w.Write(PageNumber);
            w.Write(Checksum);
            w.Write(Segments);
            w.Write(SegmentTable);
        }

        public int GetPageLength()
        {
            int length = 0;
            for (int i = 0; i < Segments; i++)
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



    class Program
    {
        static byte[] ClusterToOggSegmentTable(Cluster cluster)
        {
            using var ms = new MemoryStream();
            using var br = new BinaryWriter(ms);

            foreach (var block in cluster.SimpleBlocks)
            {
                if (block.Data != null)
                {
                    if (block.Data.Length < 255)
                    {
                        br.Write((byte)block.Data.Length);
                    }
                    else
                    {
                        br.Write((byte)255);
                        br.Write((byte)block.Data.Length - 255);
                    }
                }
            }

            br.Flush();

            return ms.ToArray();
        }

        static void Main(string[] args)
        {
            string downloads = @"C:\Users\StefHeyenrath\Downloads\";

            var orgData = File.ReadAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_track1_[eng]_DELAY 0ms.opus");

            //var oggHeader1 = new OggHeader();
            //var source = new BinaryReader(org);
            //oggHeader1.ReadFromStream(source);

            var dataStream = new FileStream(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm", FileMode.Open, FileAccess.Read);

            var doc = MatroskaSerializer.Deserialize(dataStream);

            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Info, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Cues, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Tracks, new JsonSerializerOptions { WriteIndented = true }));

            for (int bid = 0; bid < doc.Segment.Clusters[0].SimpleBlocks.Count; bid++)
            {
                Console.WriteLine(bid + " " + doc.Segment.Clusters[0].SimpleBlocks[bid].Data.Length);
            }

            var ms1 = new MemoryStream();

            int serial = -1159713906;
            var newOggHeader1 = new OggHeader
            {
                StreamVersion = 0,
                TypeFlag = 2, // Beginning Of Stream
                GranulePosition = 0,
                Serial = serial,
                PageNumber = 0,
                Checksum = 3713372948,
                Segments = 1,
                SegmentTable = new byte[] { 0x13 } // Single byte giving the length of the following segment_table data. So there is 13(hex) bytes (16 decimal) bytes of segment_table data.
            };

            var bw = new BinaryWriter(ms1);
            newOggHeader1.WriteToStream(bw);
            bw.Flush();

            var opusHeader = new OpusHeader
            {
                Version = 1,
                OutputChannelCount = 2,
                PreSkip = 312, //? ??????
                InputSampleRate = 48000,
                OutputGain = 0,
                ChannelMappingFamily = 0,
                StreamCount = 0
                //ChannelMapping = new byte[0]  //{ 0, 0 }
            };
            opusHeader.Write(bw);
            bw.Flush();


            int page = 1;
            foreach (var cluster in doc.Segment.Clusters.Take(1))
            {
                var segments = ClusterToOggSegmentTable(cluster);

                byte[] x = new byte[0];
                do
                {
                   // x = segments.Slice(0, 255)


                } while (x.Length > 0);



                var newOggHeaderForEachCluster = new OggHeader
                {
                    StreamVersion = 0,
                    TypeFlag = 0,
                    GranulePosition = cluster.Timecode, //18240,
                    Serial = serial,
                    PageNumber = page, // eigenlijk moet dit 1 zijn..
                    Checksum = 845169684,
                    Segments = (byte) x.Length, // 0x20, // ??
                    SegmentTable = x
                };

                newOggHeaderForEachCluster.WriteToStream(bw);
                bw.Flush();

                foreach (var b in cluster.SimpleBlocks)
                {
                    ms1.Write(b.Data);
                }

                page++;
            }

            File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms1.ToArray());
        }
    }
}