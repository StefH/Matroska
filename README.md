# Projects

## Span&lt;byte&gt; Reader/Writer
See this [link](https://github.com/StefH/Matroska/tree/main/src/Span.ReaderWriter).

## Matroska
An Ebml based serializer to deserialize a Matroska file (.mkv or .webm).

[![NuGet](https://buildstats.info/nuget/Matroska)](https://www.nuget.org/packages/Matroska)

### Usage
Deserialize a stream to a MatroskaDocument
``` c#
var stream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);

var doc = MatroskaSerializer.Deserialize(stream);
```


## Matroska.Muxer
A Matroska demuxer to extract Ogg Opus audio from a .webm file.

[![NuGet](https://buildstats.info/nuget/Matroska.Muxer)](https://www.nuget.org/packages/Matroska.Muxer)

### Usage
Extract
``` c#
var inputStream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);
var outputStream = File.OpenWrite("test.opus");

MatroskaDemuxer.ExtractOggOpusAudio(inputStream, outputStream);
```

# Credits
- [NEbml](https://github.com/OlegZee/NEbml)
- [concentus](https://github.com/lostromb/concentus)
- [atldotnet](https://github.com/Zeugma440/atldotnet)
- [Tedd.SpanUtils](https://github.com/tedd/Tedd.SpanUtils)
- [Microsoft Refence Source](https://referencesource.microsoft.com)
- [Ellié Computing](http://www.elliecomputing.com) contributes to this project by giving free licences of ECMerge, comparison/merge tool.