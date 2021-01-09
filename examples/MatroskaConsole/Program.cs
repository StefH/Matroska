using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using ATL;
using CSCore;
using CSCore.Codecs.OPUS;
using CSCore.SoundOut;
using Matroska.Muxer;
using Tedd;

namespace Matroska
{
    class Program
    {
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        static void Main(string[] args)
        {
            VIntTests.Test();
            string downloads = $"C:\\Users\\{Environment.UserName}\\Downloads\\";

            //var orgStream = File.OpenRead(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_org.opus");
            //var oggHeader1 = new OggHeader();
            //var source = new BinaryReader(orgStream);
            //oggHeader1.ReadFromStream(source);
            //var orgData = File.ReadAllBytes(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca)_org.opus");

            var f = "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm";
            var dataStream = new FileStream(downloads + f, FileMode.Open, FileAccess.Read);

            //var doc1 = MatroskaSerializer.Deserialize(new FileStream(downloads + @"matroska_test_w1_1\test1.mkv", FileMode.Open, FileAccess.Read));
            //var doc2 = MatroskaSerializer.Deserialize(new FileStream(downloads + @"matroska_test_w1_1\test2.mkv", FileMode.Open, FileAccess.Read));
            //var doc3 = MatroskaSerializer.Deserialize(new FileStream(downloads + @"matroska_test_w1_1\test3.mkv", FileMode.Open, FileAccess.Read));
            //var doc5 = MatroskaSerializer.Deserialize(new FileStream(downloads + @"matroska_test_w1_1\test5.mkv", FileMode.Open, FileAccess.Read));
            //var doc6 = MatroskaSerializer.Deserialize(new FileStream(downloads + @"matroska_test_w1_1\test6.mkv", FileMode.Open, FileAccess.Read));
            //var doc8 = MatroskaSerializer.Deserialize(new FileStream(downloads + @"matroska_test_w1_1\test8.mkv", FileMode.Open, FileAccess.Read));

            var doc = MatroskaSerializer.Deserialize(dataStream);

            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Info, jsonOptions));
            Console.WriteLine(JsonSerializer.Serialize(doc.Segment.Tracks, jsonOptions));

            var fileStream = File.OpenWrite(downloads + "Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus");
            MatroskaDemuxer.ExtractOggOpusAudio(doc, fileStream);

            var stream = new MemoryStream();
            MatroskaDemuxer.ExtractOggOpusAudio(doc, stream);

            ISoundOut soundOut;
            if (WasapiOut.IsSupportedOnCurrentPlatform)
            {
                soundOut = new WasapiOut();
            }
            else
            {
                soundOut = new DirectSoundOut();
            }

            stream.Position = 0;

            var track = new Track(stream, ".opus");
            Console.WriteLine(JsonSerializer.Serialize(track, new JsonSerializerOptions { WriteIndented = true }));

            var waveSource = new OpusSource(stream, (int)track.SampleRate, 2);

            Console.WriteLine("len = {0} {1}", waveSource.Length, waveSource.GetLength());

            soundOut.Initialize(waveSource);

            waveSource.SetPosition(TimeSpan.FromMinutes(6));

            soundOut.Play();
        }
    }
}