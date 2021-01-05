# Projects

## Matroska
An Ebml based serializer to deserialize a Matroska file (.mkv or .webm)

[![NuGet](https://buildstats.info/nuget/Matroska.Demux)](https://www.nuget.org/packages/Matroska.Demux)

### Usage
Deserialize a stream to a MatroskaDocument
``` c#
var stream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);

var doc = MatroskaSerializer.Deserialize(stream);
```


## Matroska.Demux
A Matroska demuxer to extract Ogg Opus audio from a .webm file

[![NuGet](https://buildstats.info/nuget/Matroska)](https://www.nuget.org/packages/Matroska)

### Usage
Extract
``` c#
var inputStream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);
var outputStream = File.OpenWrite("test.opus");

MatroskaDemuxer.ExtractOggOpusAudio(inputStream, outputStream);
```

# Credits
- [Ellié Computing](http://www.elliecomputing.com) contributes to this project by giving free licences of ECMerge, comparison/merge tool.
- [NEbml](https://github.com/OlegZee/NEbml)
- [concentus](https://github.com/lostromb/concentus)
- [atldotnet](https://github.com/Zeugma440/atldotnet)