using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ATL;
using Commons;

namespace Matroska
{
    class OpusHeader
    {
        public String ID;
        public byte Version;
        public byte OutputChannelCount;
        public UInt16 PreSkip;
        public UInt32 InputSampleRate;
        public Int16 OutputGain;
        public byte ChannelMappingFamily;

        public byte StreamCount;
        public byte CoupledStreamCount;
        public byte[] ChannelMapping;

        public void Reset()
        {
            ID = "";
            Version = 0;
            OutputChannelCount = 0;
            PreSkip = 0;
            InputSampleRate = 0;
            OutputGain = 0;
            ChannelMappingFamily = 0;
            StreamCount = 0;
            CoupledStreamCount = 0;
        }
    }

    class OggHeader
    {
        // Ogg page header ID
        const String OGG_PAGE_ID = "OggS";

        // Vorbis identification packet (frame) ID
        static readonly String VORBIS_HEADER_ID = (char)1 + "vorbis";

        // Vorbis tag packet (frame) ID
        static readonly String VORBIS_TAG_ID = (char)3 + "vorbis";

        // Vorbis setup packet (frame) ID
        static readonly String VORBIS_SETUP_ID = (char)5 + "vorbis";

        // Vorbis parameter frame ID
        const String OPUS_HEADER_ID = "OpusHead";

        // Opus tag frame ID
        const String OPUS_TAG_ID = "OpusTags";


        public string ID;                                               // Always "OggS"
        public byte StreamVersion;                           // Stream structure version
        public byte TypeFlag;                                        // Header type flag
        public ulong AbsolutePosition;                      // Absolute granule position
        public int Serial;                                       // Stream serial number
        public int PageNumber;                                   // Page sequence number
        public uint Checksum;                                              // Page CRC32
        public byte Segments;                                 // Number of page segments
        public byte[] LacingValues;                     // Lacing values - segment sizes

        public void Reset()
        {
            ID = "";
            StreamVersion = 0;
            TypeFlag = 0;
            AbsolutePosition = 0;
            Serial = 0;
            PageNumber = 0;
            Checksum = 0;
            Segments = 0;
        }

        public void ReadFromStream(BufferedBinaryReader r)
        {
            ID = Utils.Latin1Encoding.GetString(r.ReadBytes(4));
            StreamVersion = r.ReadByte();
            TypeFlag = r.ReadByte();
            AbsolutePosition = r.ReadUInt64();
            Serial = r.ReadInt32();
            PageNumber = r.ReadInt32();
            Checksum = r.ReadUInt32();
            Segments = r.ReadByte();
            LacingValues = r.ReadBytes(Segments);
        }

        public void ReadFromStream(BinaryReader r)
        {
            ID = Utils.Latin1Encoding.GetString(r.ReadBytes(4));
            StreamVersion = r.ReadByte();
            TypeFlag = r.ReadByte();
            AbsolutePosition = r.ReadUInt64();
            Serial = r.ReadInt32();
            PageNumber = r.ReadInt32();
            Checksum = r.ReadUInt32();
            Segments = r.ReadByte();
            LacingValues = r.ReadBytes(Segments);
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Utils.Latin1Encoding.GetBytes(ID));
            w.Write(StreamVersion);
            w.Write(TypeFlag);
            w.Write(AbsolutePosition);
            w.Write(Serial);
            w.Write(PageNumber);
            w.Write(Checksum);
            w.Write(Segments);
            w.Write(LacingValues);
        }

        public int GetPageLength()
        {
            int length = 0;
            for (int i = 0; i < Segments; i++)
            {
                length += LacingValues[i];
            }
            return length;
        }

        public int GetHeaderSize()
        {
            return 27 + LacingValues.Length;
        }

        public bool IsValid()
        {
            return ((ID != null) && ID.Equals(OGG_PAGE_ID));
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string downloads = @"C:\Users\StefHeyenrath\Downloads\";

            var org = File.OpenRead(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_track1_[eng]_DELAY 0ms.opus");

            var oggHeader1 = new OggHeader();
            var source = new BinaryReader(org);
            oggHeader1.ReadFromStream(source);

            var dataStream = new FileStream(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm", FileMode.Open, FileAccess.Read);

            var doc = MatroskaSerializer.Deserialize(dataStream);

            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Info, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Cues, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Tracks, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Clusters.First().SimpleBlocks.Take(10), new JsonSerializerOptions { WriteIndented = true }));

            var ms1 = new MemoryStream();

            int len = 0x115B - 0xE8; 

            foreach (var cluster in doc.Segment.Clusters)
            {
                if (ms1.Position > 4210)
                {
                    //  continue;
                }

                // ms1.Write(System.Text.Encoding.ASCII.GetBytes("X"));
                foreach (var b in cluster.SimpleBlocks)
                {
                    if (ms1.Position > 4210)
                    {
                        // continue;
                    }

                    ms1.Write(b.Data);
                }
            }

            File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms1.ToArray());

        }
    }
}