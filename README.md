# Projects

## Matroska
An Ebml based serializer to deserialize a Matroska file (.mkv or .webm).

[![NuGet](https://img.shields.io/nuget/v/Matroska)](https://www.nuget.org/packages/Matroska)

### Usage
Deserialize a stream to a MatroskaDocument
``` c#
var stream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);

var doc = MatroskaSerializer.Deserialize(stream);
```


## Matroska.Muxer
A Matroska demuxer to extract Ogg Opus audio from a .webm file.

[![NuGet](https://img.shields.io/nuget/v/Matroska.Muxer)](https://www.nuget.org/packages/Matroska.Muxer)

### Usage
Extract
``` c#
var inputStream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);
var outputStream = File.OpenWrite("test.opus");

MatroskaDemuxer.ExtractOggOpusAudio(inputStream, outputStream);
```

## Credits / References
- [NEbml](https://github.com/OlegZee/NEbml)
- [concentus](https://github.com/lostromb/concentus)
- [atldotnet](https://github.com/Zeugma440/atldotnet)
- [ebml-specification](https://github.com/ietf-wg-cellar/ebml-specification) / [matroska-specification](https://github.com/ietf-wg-cellar/matroska-specification)
- [Ellié Computing](http://www.elliecomputing.com) contributes to this project by giving free licences of ECMerge, comparison/merge tool.

## Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=StefH) and [Dapper Plus](https://dapper-plus.net/?utm_source=StefH) are major sponsors and proud to contribute to the development of **Matroska.Muxer** and **Matroska**.

[![Entity Framework Extensions](https://raw.githubusercontent.com/StefH/resources/main/sponsor/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=StefH)

[![Dapper Plus](https://raw.githubusercontent.com/StefH/resources/main/sponsor/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=StefH)