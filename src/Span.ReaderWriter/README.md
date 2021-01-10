# Info
SpanReader and SpanWriter which wrap a Span&lt;byte&gt; and provide a convenient functionality for reading and writing.

[![NuGet](https://buildstats.info/nuget/Span.ReaderWriter)](https://www.nuget.org/packages/Span.ReaderWriter)

### Usage
Read some values from a Span&lt;byte&gt;
``` c#
var bytes = new [] { ... }; 
var reader = new SpanReader(bytes);

var int = reader.ReadInt();
```