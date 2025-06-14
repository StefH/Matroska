## Matroska.Muxer
A Matroska demuxer to extract Ogg Opus audio from a .webm file.

### Usage
Extract
``` c#
var inputStream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);
var outputStream = File.OpenWrite("test.opus");

MatroskaDemuxer.ExtractOggOpusAudio(inputStream, outputStream);
```

### Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=StefH) and [Dapper Plus](https://dapper-plus.net/?utm_source=StefH) are major sponsors and proud to contribute to the development of **Matroska.Muxer**.

[![Entity Framework Extensions](https://raw.githubusercontent.com/StefH/resources/main/sponsor/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=StefH)

[![Dapper Plus](https://raw.githubusercontent.com/StefH/resources/main/sponsor/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=StefH)