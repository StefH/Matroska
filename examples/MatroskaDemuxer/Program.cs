using System.IO;
using System.Threading.Tasks;
using MediaContainers.Matroska;

namespace MatroskaDemuxer;

class Program
{
    private static async Task Main(string[] args)
    {
        var doc = await MatroskaReader.Read(new BufferedStream(File.OpenRead("issue16.webm")));

        await doc.ReadTrackInfo();

        while (true)
        {
            var frame = await doc.ReadFrame();
            if (frame.Buffer == null)
            {
                break;
            }
        }

        var outputStream1 = File.OpenWrite("issue16.opus");
        Matroska.Muxer.MatroskaDemuxer.ExtractOggOpusAudio(File.OpenRead("issue16.webm"), outputStream1);
        outputStream1.Close();

        var outputStream2 = File.OpenWrite("Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).opus");
        Matroska.Muxer.MatroskaDemuxer.ExtractOggOpusAudio(File.OpenRead("Estas Tonne - Internal Flight Experience (Live in Cluj Napoca).webm"), outputStream2);
        outputStream2.Close();
    }
}