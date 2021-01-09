using System;
using System.IO;
using System.Text;
using Matroska.Extensions;

namespace Matroska
{
    public static class VIntTests
    {
        public static void Test()
        {
			var s = "stefகுstef";

			var ms = new MemoryStream();
			var bw = new BinaryWriter(ms);
			bw.Write(s);
			bw.Flush();
			var sbytes = ms.ToArray();

			var br = new BinaryReader(new MemoryStream(sbytes));
			var stringbr = br.ReadString();

			var bytes = sbytes.AsSpan().Slice(1).ToArray();
			var s2 = Encoding.UTF8.GetString(bytes);


			//// [TestCase(0, 1, ExpectedResult = 0x80ul)]
			//var ss0 = new SpanStream(new byte[] {  });
			//var v0 = ss0.ReadVInt(4);
			//var i0 = v0.Info();

			// [TestCase(0, 1, ExpectedResult = 0x80ul)]
			var ss1 = new SpanReader(new byte[] { 0x80 });
			var v1 = ss1.ReadVInt(4);
			var i1 = v1.Info();

			// [TestCase(1, 1, ExpectedResult = 0x81ul)]
			var ss2 = new SpanReader(new byte[] { 0x81 });
			var v2 = ss2.ReadVInt(4);
			var i2 = v2.Info();

			// [TestCase(126, 1, ExpectedResult = 0xfeul)]
			var ss3 = new SpanReader(new byte[] { 0xfe });
			var v3 = ss3.ReadVInt(4);
			var i3 = v3.Info();

			// [TestCase(127, 2, ExpectedResult = 0x407ful)]
			var ss4 = new SpanReader(new byte[] { 0x40, 0x7F });
			var v4 = ss4.ReadVInt(4);
			var i4 = v4.Info();

			// [TestCase(128, 2, ExpectedResult = 0x4080ul)]
			var ss5 = new SpanReader(new byte[] { 0x40, 0x80 });
			var v5 = ss5.ReadVInt(4);
			var i5 = v5.Info();

			// [TestCase(0xdeffad, 4, ExpectedResult = 0x10deffadul)]
			var ss6 = new SpanReader(new byte[] { 0x10, 0xDE, 0xFF, 0xAD });
			var v6 = ss6.ReadVInt(4);
			var i6 = v6.Info();

			int x = 0;
        }
    }
}