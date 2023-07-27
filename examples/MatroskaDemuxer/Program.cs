using System;
using System.IO;
using System.Linq;

namespace MatroskaDemuxer;

class Program
{
    static void Main(string[] args)
    {
        var outputStream = File.OpenWrite("issue16.opus");
        Matroska.Muxer.MatroskaDemuxer.ExtractOggOpusAudio(File.OpenRead("issue16.webm"), outputStream);

        outputStream.Close();

        //string folder = $"C:\\Users\\{Environment.UserName}\\Downloads\\Nuclear";

        //foreach (var file in Directory.GetFiles(folder).Where(f => f.EndsWith(".webm")))
        //{
        //    Console.WriteLine(file);
        //    var outputStream = File.OpenWrite(file.Replace(".webm", ".opus"));
        //    Matroska.Muxer.MatroskaDemuxer.ExtractOggOpusAudio(File.OpenRead(file), outputStream);

        //    outputStream.Close();
        //}
    }
}