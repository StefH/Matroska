using System.IO;
using FluentAssertions;
using Xunit;

namespace System
{
    public class SpanReaderTests
    {
        [Fact]
        public void Read()
        {
            // Arrange
            bool bo = true;
            char c = 'c';
            byte b = 45;
            sbyte sb = 67;
            short s = short.MinValue;
            ushort us = ushort.MaxValue;
            int i = int.MinValue;
            uint ui = uint.MaxValue;
            long l = long.MinValue;
            ulong ul = ulong.MaxValue;
            decimal d = decimal.MinValue;
            float f = float.MaxValue;
            double db = double.MaxValue;
            string st = "ᚠHello Worldಸ";

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(bo);
            binaryWriter.Write(c);
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

            var bytes = memoryStream.ToArray();

            // Act
            var spanReader = new SpanReader(bytes);

            // Assert
            spanReader.ReadBool().Should().Be(bo);
            spanReader.ReadChar().Should().Be(c);
            spanReader.ReadByte().Should().Be(b);
            spanReader.ReadSByte().Should().Be(sb);
            spanReader.ReadShort().Should().Be(s);
            spanReader.ReadUShort().Should().Be(us);
            spanReader.ReadInt().Should().Be(i);
            spanReader.ReadUInt().Should().Be(ui);
            spanReader.ReadLong().Should().Be(l);
            spanReader.ReadULong().Should().Be(ul);
            spanReader.ReadDecimal().Should().Be(d);
            spanReader.ReadFloat().Should().Be(f);
            spanReader.ReadDouble().Should().Be(db);
            spanReader.ReadString().Should().Be(st);
        }
    }
}