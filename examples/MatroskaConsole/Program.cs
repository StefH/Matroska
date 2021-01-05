using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ATL;
using Commons;
using Concentus.Structs;
using Matroska.Models;
using NEbml.Core;

namespace Matroska
{
    struct SegmentEntry
    {
        public byte[] SegmentBytes { get; set; }

        public byte[] Data { get; set; }

        public short TimeCode { get; set; }

        public int NumberOfFrames { get; set; }

        public int NumberOfSamples { get; set; }
    }

    class Program
    {
        static List<SegmentEntry> ClusterToOggOpusSegmentTable(Cluster cluster)
        {
            var list = new List<SegmentEntry>();
            foreach (var block in cluster.SimpleBlocks)
            {
                if (block.Data != null)
                {
                    int len = block.Data.Length;

                    byte[] segmentTable;
                    if (len == 0)
                    {
                        segmentTable = new byte[] { 255 };
                    }
                    else if (block.Data.Length < 255)
                    {
                        segmentTable = new byte[] { (byte)len };
                    }
                    else if (block.Data.Length < 2 * 255)
                    {
                        segmentTable = new byte[] { 255, (byte)(block.Data.Length - 255) };
                    }
                    else
                    {
                        int numberOfSegmentTableBytes = len / 255;
                        segmentTable = new byte[1 + numberOfSegmentTableBytes];
                        segmentTable.AsSpan().Fill(255);
                        segmentTable[numberOfSegmentTableBytes] = (byte) (len - (numberOfSegmentTableBytes * 255));
                    }
                    
                    //else if (block.Data.Length < 3 * 255)
                    //{
                    //    segmentTable = new byte[] { 255, 255, (byte)(block.Data.Length - 2 * 255) };
                    //}
                    //else
                    //{
                    //    segmentTable = new byte[] { 255, 255, 255, (byte)(block.Data.Length - 3 * 255) };
                    //}

                    list.Add(new SegmentEntry
                    {
                        SegmentBytes = segmentTable,
                        Data = block.Data,
                        TimeCode = block.TimeCode,
                        NumberOfSamples = OpusPacketInfo.GetNumSamples(block.Data, 0, len, 48000),
                        NumberOfFrames = OpusPacketInfoParser.GetNumFrames(block.Data, 0, len)
                    });
                }
            }

            return list;
        }

        static void Main(string[] args)
        {
            //string downloads = @"C:\Users\StefHeyenrath\Downloads\";
            string downloads = @"C:\Users\azurestef\Downloads\";

            var orgStream = File.OpenRead(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_org.opus");

            var oggHeader1 = new OggHeader();
            var source = new BinaryReader(orgStream);
            oggHeader1.ReadFromStream(source);

            var orgData = File.ReadAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_org.opus");

            var r = "Roxette - Listen To Your Heart (Official Music Video).webm";
            var f = "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm";
            var dataStream = new FileStream(downloads + f, FileMode.Open, FileAccess.Read);

            var doc = MatroskaSerializer.Deserialize(dataStream);

            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Info, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Cues, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Tracks, new JsonSerializerOptions { WriteIndented = true }));

            var ms1 = new MemoryStream();

            int serial = -1071269784; // 68 B8 25 C0
            var newOggHeader1 = new OggHeader
            {
                StreamVersion = 0,
                TypeFlag = OggHeaderType.BeginningOfStream,
                GranulePosition = 0,
                Serial = serial,
                PageNumber = 0,
                Checksum = 3476425714, //4209813745,

                TotalSegments = 1,
                SegmentTable = new byte[] { 0x13 } // OpusHead 

                //TotalSegments = 2,
                //SegmentTable = new byte[] { 0x13, 0x10 } // OpusHead , OpusTags
            };

            var bw = new BinaryWriter(ms1);
            //newOggHeader1.WriteToStream(bw);
            //bw.Flush();

            var opusHeader = new OpusHeader
            {
                Version = 1,
                OutputChannelCount = 2,
                PreSkip = 312,
                InputSampleRate = 48000,
                OutputGain = 0,
                ChannelMappingFamily = 0
            };
            //opusHeader.Write(bw);
            //bw.Flush();

            //var newOggHeaderForTags = new OggHeader
            //{
            //    StreamVersion = 0,
            //    TypeFlag = OggHeaderType.BeginningOfStream,
            //    GranulePosition = 0,
            //    Serial = serial,
            //    PageNumber = 0,
            //    Checksum = 3476425714, 

            //    TotalSegments = 1,
            //    SegmentTable = new byte[] { 0x13 } // OpusHead 
            //};

            ms1.Write(orgData, 0, 0x91);


            int page = 2;
            ulong granulePosition = 0;

            void WriteOggPage(ulong timeCode, byte numberOfSegments, List<SegmentEntry> oggPages)
            {
                var data = oggPages.SelectMany(o => o.Data).ToArray();

                granulePosition += (ulong)oggPages.Sum(op => op.NumberOfSamples * op.NumberOfFrames);

                var oggHeader = new OggHeader
                {
                    StreamVersion = 0,
                    TypeFlag = OggHeaderType.None,
                    GranulePosition = granulePosition,
                    Serial = serial,
                    PageNumber = page,
                    Checksum = 0,
                    TotalSegments = numberOfSegments,
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

            var d = new Dictionary<int, byte>
            {
                //{ 2, 0x5E },
                //{ 3, 0x64 },
                //{ 4, 0x64 }
            };

            //foreach (var oggSegmentEntry in oggSegmentTable)
            //{
            //    WriteOggPage(cluster.Timecode, (byte) oggSegmentEntry.SegmentBytes.Length, new List<SegmentEntry> { oggSegmentEntry });
            //}

            var oggPageWithSegments = new List<SegmentEntry>();
            granulePosition = 0;
            byte segmentParts = 0;
            foreach (var cluster in doc.Segment.Clusters)
            {
                var oggSegmentTable = ClusterToOggOpusSegmentTable(cluster);

                foreach (var oggSegmentEntry in oggSegmentTable)
                {
                    oggPageWithSegments.Add(oggSegmentEntry);
                    segmentParts = (byte)(segmentParts + oggSegmentEntry.SegmentBytes.Length);

                    if (page == 6)
                    {
                        int uuuuy = 0;
                    }

                    if (segmentParts >= (d.ContainsKey(page) ? d[page] : 0x64))
                    {
                        WriteOggPage(cluster.Timecode, segmentParts, oggPageWithSegments);

                        segmentParts = 0;
                        oggPageWithSegments.Clear();
                    }
                }

                if (segmentParts > 0)
                {
                    WriteOggPage(cluster.Timecode, segmentParts, oggPageWithSegments);

                    segmentParts = 0;
                    oggPageWithSegments.Clear();
                }
            }

            File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms1.ToArray());
        }
    }
}