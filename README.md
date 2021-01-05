# Matroska
An Ebml based serializer to deserialize a Matroska file (.mkv or .webm)

# Usage
``` c#
var fileStream = new FileStream("test.webm", FileMode.Open, FileAccess.Read);

var doc = MatroskaSerializer.Deserialize(fileStream);
```

# Credits
- [Ellié Computing](http://www.elliecomputing.com) contributes to this project by giving free licences of ECMerge, comparison/merge tool.
