using System.IO;
using FluentAssertions;
using Xunit;

namespace System
{
    public class SpanWriterTests
    {
        [Fact]
        public void Write()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var dateTime = DateTime.UtcNow;
            bool bo = true;
            char c = 'c';
            char cUtf8 = 'ಸ';
            char[] chars = new[] { c, cUtf8 };
            byte b = 45;
            sbyte sb = 67;
            short s = short.MinValue;
            ushort us = ushort.MaxValue;
            int i = int.MinValue;
            uint ui = uint.MaxValue;
            long l = long.MinValue;
            ulong ul = ulong.MaxValue;
            decimal d = decimal.MinValue;
            float f = 533174.1f;
            double db = double.MaxValue;
            string st = "Hello World";
            string stLong = new string('x', 5000);
            string stUtf8 = "ᚠHello Worldಸ";

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(guid.ToByteArray());
            binaryWriter.Write(dateTime.ToBinary());
            binaryWriter.Write(bo);
            binaryWriter.Write(c);
            binaryWriter.Write(cUtf8);
            binaryWriter.Write(b);
            binaryWriter.Write(sb);
            binaryWriter.Write(s);
            binaryWriter.Write(us);
            binaryWriter.Write(i);
            binaryWriter.Write(ui);
            binaryWriter.Write(l);
            binaryWriter.Write(ul);
            binaryWriter.Write(d);
            binaryWriter.Write(f);
            binaryWriter.Write(db);
            binaryWriter.Write(st);
            binaryWriter.Write(stLong);
            binaryWriter.Write(stUtf8);
            binaryWriter.Write(!bo);
            binaryWriter.Write(chars);
            binaryWriter.Write(!bo);

            var bytes = memoryStream.ToArray();

            // Act
            var spanWriterBytes = new byte[bytes.Length];
            var spanWriter = new SpanWriter(spanWriterBytes);

            spanWriter.Write(guid);
            spanWriter.Write(dateTime);
            spanWriter.Write(bo);
            spanWriter.Write(c);
            spanWriter.Write(cUtf8);
            spanWriter.Write(b);
            spanWriter.Write(sb);
            spanWriter.Write(s);
            spanWriter.Write(us);
            spanWriter.Write(i);
            spanWriter.Write(ui);
            spanWriter.Write(l);
            spanWriter.Write(ul);
            spanWriter.Write(d);
            spanWriter.Write(f);
            spanWriter.Write(db);
            spanWriter.Write(st);
            spanWriter.Write(stLong);
            spanWriter.Write(stUtf8);
            spanWriter.Write(!bo);
            spanWriter.Write(chars);
            spanWriter.Write(!bo);

            // Assert
            spanWriterBytes.Should().BeEquivalentTo(bytes);

            var spanReader = new SpanReader(spanWriterBytes);
            spanReader.ReadGuid().Should().Be(guid);
            spanReader.ReadDateTime().Should().Be(dateTime);
            spanReader.ReadBoolean().Should().Be(bo);
            spanReader.ReadChar().Should().Be(c);
            spanReader.ReadChar().Should().Be(cUtf8);
            spanReader.ReadByte().Should().Be(b);
            spanReader.ReadSByte().Should().Be(sb);
            spanReader.ReadShort().Should().Be(s);
            spanReader.ReadUShort().Should().Be(us);
            spanReader.ReadInt().Should().Be(i);
            spanReader.ReadUInt().Should().Be(ui);
            spanReader.ReadLong().Should().Be(l);
            spanReader.ReadULong().Should().Be(ul);
            spanReader.ReadDecimal().Should().Be(d);
            spanReader.ReadSingle().Should().Be(f);
            spanReader.ReadDouble().Should().Be(db);
            spanReader.ReadString().Should().Be(st);
            spanReader.ReadString().Should().Be(stLong);
            spanReader.ReadString().Should().Be(stUtf8);
            spanReader.ReadBoolean().Should().Be(!bo);
        }
    }
}