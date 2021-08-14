using System;
using System.IO;
using System.Linq;
using Matroska.Muxer;

namespace MatroskaDemuxerConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string folder = $"C:\\Users\\{Environment.UserName}\\Downloads\\Nuclear";

            foreach (var file in Directory.GetFiles(folder).Where(f => f.EndsWith(".webm")))
            {
                Console.WriteLine(file);
                var outputStream = File.OpenWrite(file.Replace(".webm", ".opus"));
                MatroskaDemuxer.ExtractOggOpusAudio(File.OpenRead(file), outputStream);

                outputStream.Close();
            }
        }
    }
}