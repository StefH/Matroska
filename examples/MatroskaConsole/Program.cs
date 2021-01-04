using System;
using System.Collections.Generic;
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
    struct SegmentEntry
    {
        public byte[] SegmentBytes { get; set; }

        public byte[] Data { get; set; }

        public short TimeCode { get; set; }
    }

    class Program
    {
        static List<SegmentEntry> ClusterToOggSegmentTable(Cluster cluster)
        {
            var list = new List<SegmentEntry>();
            foreach (var block in cluster.SimpleBlocks)
            {
                if (block.Data != null)
                {
                    byte[] segmentTable;
                    if (block.Data.Length < 255)
                    {
                        segmentTable = new byte[] { (byte)block.Data.Length };
                    }
                    else
                    {
                        segmentTable = new byte[] { 255, (byte)(block.Data.Length - 255) };
                    }

                    list.Add(new SegmentEntry
                    {
                        SegmentBytes = segmentTable,
                        Data = block.Data,
                        TimeCode = block.TimeCode
                    });
                }
            }

            return list;
        }

        static void Main(string[] args)
        {
            // string downloads = @"C:\Users\StefHeyenrath\Downloads\";
            string downloads = @"C:\Users\azurestef\Downloads\";

            var orgData = File.ReadAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_track1_[eng]_DELAY 0ms.opus");

            //var oggHeader1 = new OggHeader();
            //var source = new BinaryReader(org);
            //oggHeader1.ReadFromStream(source);

            var dataStream = new FileStream(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm", FileMode.Open, FileAccess.Read);

            var doc = MatroskaSerializer.Deserialize(dataStream);

            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Info, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Cues, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Tracks, new JsonSerializerOptions { WriteIndented = true }));

            var ms1 = new MemoryStream();

            int serial = -1159713906;
            var newOggHeader1 = new OggHeader
            {
                StreamVersion = 0,
                TypeFlag = OggHeaderType.BeginningOfStream,
                GranulePosition = 0,
                Serial = serial,
                PageNumber = 0,
                Checksum = 4209813745, //3713372948,
                Segments = 2,

                // Single byte giving the length of the following segment_table data. So there is 13(hex) bytes (16 decimal) bytes of segment_table data.
                SegmentTable = new byte[] { 0x13, 0x10 } // OpusHead , OpusTags
            };

            var bw = new BinaryWriter(ms1);
            newOggHeader1.WriteToStream(bw);
            bw.Flush();

            var opusHeader = new OpusHeader
            {
                Version = 1,
                OutputChannelCount = 2,
                PreSkip = 312,
                InputSampleRate = 48000,
                OutputGain = 0,
                ChannelMappingFamily = 0,
                StreamCount = 0
            };
            opusHeader.Write(bw);
            bw.Flush();

            int page = 1;
            ulong granulePosition = 0;

            void WriteOggPage(ulong timeCode, byte len, List<SegmentEntry> oggPages)
            {
                var data = oggPages.SelectMany(o => o.Data).ToArray();

                var sumTimeCodes = oggPages.Sum(o => o.TimeCode);

                //granulePosition += (ulong)(data[0] * 2 * len);

                // g1 = 18240
                // g2 = 29760 (11520)
                var oggHeader = new OggHeader
                {
                    StreamVersion = 0,
                    TypeFlag = page == 1 ? OggHeaderType.BeginningOfStream : OggHeaderType.Continuation,
                    GranulePosition = 18240, //timeCode + 8240, //cluster.Timecode, //18240,
                    Serial = serial,
                    PageNumber = page,
                    Checksum = 0, // 2519159819, //845169684,
                    Segments = len, // 0x20, // ??
                    SegmentTable = oggPages.SelectMany(o => o.SegmentBytes).ToArray()
                };

                using var oggPageStream = new MemoryStream();
                var oggPageWriter = new BinaryWriter(oggPageStream);
                oggHeader.WriteToStream(oggPageWriter);
                oggPageWriter.Flush();
                oggPageStream.Write(data);

                var oggPageBytes = oggPageStream.ToArray();

                oggHeader.Checksum = OggCRC32.CalculateCRC(0, oggPageBytes);

                oggPageWriter = new BinaryWriter(ms1);
                oggHeader.WriteToStream(oggPageWriter);
                oggPageWriter.Flush();

                ms1.Write(data);

                page++;
            }

            var oggPageWithSegments = new List<SegmentEntry>();
            granulePosition = 0;
            foreach (var cluster in doc.Segment.Clusters)
            {
                var oggSegmentTable = ClusterToOggSegmentTable(cluster);

                oggPageWithSegments.Clear();

                byte segmentParts = 0;
                foreach (var oggSegmentEntry in oggSegmentTable)
                {
                    if (segmentParts >= 0x20)
                    {
                        if (segmentParts == 0x21)
                        {
                            int y = 0;
                        }
                        WriteOggPage(cluster.Timecode, segmentParts, oggPageWithSegments);

                        segmentParts = 0;
                        oggPageWithSegments.Clear();
                    }

                    oggPageWithSegments.Add(oggSegmentEntry);

                    segmentParts = (byte)(segmentParts + oggSegmentEntry.SegmentBytes.Length);
                }

                if (segmentParts > 0)
                {
                    WriteOggPage(cluster.Timecode, segmentParts, oggPageWithSegments);
                }
            }

            File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms1.ToArray());
        }
    }
}