## Matroska
An Ebml based serializer to deserialize a Matroska file (.mkv or .webm).

### Usage
Deserialize a stream to a MatroskaDocument
``` c#
var stream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);

var doc = MatroskaSerializer.Deserialize(stream);
```

### Sponsors

[Entity Framework Extensions](https://entityframework-extensions.net/?utm_source=StefH) and [Dapper Plus](https://dapper-plus.net/?utm_source=StefH) are major sponsors and proud to contribute to the development of **Matroska**.

[![Entity Framework Extensions](https://raw.githubusercontent.com/StefH/resources/main/sponsor/entity-framework-extensions-sponsor.png)](https://entityframework-extensions.net/bulk-insert?utm_source=StefH)

[![Dapper Plus](https://raw.githubusercontent.com/StefH/resources/main/sponsor/dapper-plus-sponsor.png)](https://dapper-plus.net/bulk-insert?utm_source=StefH)