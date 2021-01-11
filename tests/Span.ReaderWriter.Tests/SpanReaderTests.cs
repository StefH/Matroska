﻿using System.IO;
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
            char cUtf8 = 'ಸ';
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
            string stUtf8 = "ᚠHello Worldಸ";

            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
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
            binaryWriter.Write(stUtf8);

            var bytes = memoryStream.ToArray();

            // Act
            var spanReader = new SpanReader(bytes);

            // Assert
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
            spanReader.ReadString().Should().Be(stUtf8);
        }

        [Theory]
        [InlineData(new byte[] { 0x80 }, 1, 0x80ul, 0)]
        [InlineData(new byte[] { 0x81 }, 1, 0x81ul, 1)]
        [InlineData(new byte[] { 0xfe }, 1, 0xfeul, 126)]
        [InlineData(new byte[] { 0x40, 0x7f }, 2, 0x407ful, 127)]
        [InlineData(new byte[] { 0x40, 0x80 }, 2, 0x4080ul, 128)]
        [InlineData(new byte[] { 0x10, 0xDE, 0xFF, 0xAD }, 4, 0x10deffad, 0xdeffad)]

        public void TestVInt(byte[] bytes, int expectedLength, ulong expectedEncodedValue, ulong expectedValue)
        {
            var spanReader = new SpanReader(bytes);
            var vint = spanReader.ReadVInt(4);

            Assert.Equal(expectedLength, vint.Length);
            Assert.Equal(expectedEncodedValue, vint.EncodedValue);
            Assert.Equal(expectedValue, vint.Value);

            var writeSpan = new byte[VInt.GetSize(expectedValue)].AsSpan();
            var spanWriter = new SpanWriter(writeSpan);

            var writeLength = spanWriter.Write(vint);
            Assert.Equal(expectedLength, writeLength);
            Assert.Equal(bytes, writeSpan.ToArray());
        }
    }
}