using System;
using System.IO;
using System.Text.Json;
using Matroska.Muxer.OggOpus;

namespace Matroska
{
    class Program
    {
        static void Main(string[] args)
        {
            //string downloads = @"C:\Users\StefHeyenrath\Downloads\";
            string downloads = @"C:\Users\azurestef\Downloads\";

            //var orgStream = File.OpenRead(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_org.opus");
            //var oggHeader1 = new OggHeader();
            //var source = new BinaryReader(orgStream);
            //oggHeader1.ReadFromStream(source);
            //var orgData = File.ReadAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_org.opus");

            var f = "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm";
            var dataStream = new FileStream(downloads + f, FileMode.Open, FileAccess.Read);

            var doc = MatroskaSerializer.Deserialize(dataStream);

            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Info, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Cues, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Tracks, new JsonSerializerOptions { WriteIndented = true }));

            var ms = new MemoryStream();
            var o = new OggOpusMatroskaDocumentParser(doc, ms);
            o.Parse();

            File.WriteAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus", ms.ToArray());
        }
    }
}