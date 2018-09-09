/*
 * Copyright (c) 2016 Stojan Dimitrovski
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using BBuffer;
using NUnit.Framework;
using STUN.Message.Attributes;
using STUN.Message.Enums;
using System;
using System.Collections.Generic;

namespace STUN.Message {
	/**
	 * Created by vuk on 24/10/16.
	 */
	[TestFixture]
	public class STUNMessageParserTest {
		[Test]
		public void Parsing() {
			STUNMessageBuilder builder = new STUNMessageBuilder(null,
				STUNClass.Error, STUNMethod.Binding,
				new Transaction(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1));
			builder.WriteAttribute(0b111, new ByteBuffer(new byte[] { 255 }));
			builder.WriteAttribute(0b010, new ByteBuffer(new byte[] { 0, 255, 0, 255 }));

			byte[] message = builder.Build().ToArray();
			// Add some offset to test it too
			List<byte> b = new List<byte>();
			b.AddRange(new byte[] { 1, 2, 3 });
			b.AddRange(message);

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(b.ToArray(), 3), false, attrs);
			Assert.IsTrue(parser.isValid);

			byte[] copy = new byte[20];
			Buffer.BlockCopy(message, 0, copy, 0, 20);
			CollectionAssert.AreEqual(copy, parser.GetHeader().ToArray());

			Assert.AreEqual(STUNClass.Error, parser.stunClass);
			Assert.AreEqual(STUNMethod.Binding, parser.stunMethod);
			var transaction = parser.transaction;
			Assert.AreEqual(1, transaction[0]);
			for (int i = 1; i < Transaction.Length; i++) Assert.AreEqual(0, transaction[i]);
			Assert.IsTrue(0 == parser.length % 4);
			Assert.IsTrue(parser.isValid);

			Assert.AreEqual(2, attrs.Count, "Wrong number of attributes");

			var tlv1 = attrs[0];

			Assert.IsNotNull(tlv1);
			Assert.AreEqual((STUNAttribute) 0b111, tlv1.type);
			CollectionAssert.AreEqual(new byte[] { 255 }, tlv1.data.ToArray());
			Assert.AreEqual(1, tlv1.data.Length);

			var tlv2 = attrs[1];

			Assert.IsNotNull(tlv2);
			Assert.AreEqual((STUNAttribute) 0b010, tlv2.type);
			CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, tlv2.data.ToArray());
			Assert.AreEqual(4, tlv2.data.Length);
		}

		[Test]
		public void ShortHeader() {
			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(new byte[19]), false, attrs);
			Assert.IsFalse(parser.isValid);
		}

		[Test]
		public void HeaderDoesNotStartWith00() {
			byte[] header = new byte[20];
			header[0] = 255;

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(header), false, attrs);
			Assert.IsFalse(parser.isValid);
		}

		[Test]
		public void LengthNotAMultipleOf4() {
			byte[] header = new byte[20];

			header[2] = 0;
			header[3] = 3;

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(header), false, attrs);
			Assert.IsFalse(parser.isValid);
		}

		[Test]
		public void NoTLV() {
			byte[] header = new byte[20];

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(header), false, attrs);
			Assert.IsFalse(parser.isValid);
		}

		[Test]
		public void EosAtReadingFirstTLVHeader() {
			STUNMessageBuilder builder = new STUNMessageBuilder(null,
				STUNClass.Request, STUNMethod.Binding,
				new Transaction(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10));
			builder.WriteAttribute(0b11, new ByteBuffer(new byte[] { 255, 255 }));

			byte[] message = builder.Build().ToArray();

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message, 0, 20 + 1), false, attrs);

			Assert.IsFalse(parser.isValid);
			Assert.AreEqual(0, attrs.Count, "Wrong number of attributes");
		}

		[Test]
		public void EosAtReadingFirstTLVValue() {
			STUNMessageBuilder builder = new STUNMessageBuilder(null,
				STUNClass.Request, STUNMethod.Binding,
				new Transaction(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10));
			builder.WriteAttribute(0b11, new ByteBuffer(new byte[] { 255, 255 }));

			byte[] message = builder.Build().ToArray();

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message, 0, 20 + 4 + 1), false, attrs);

			Assert.IsFalse(parser.isValid);
			Assert.AreEqual(0, attrs.Count, "Wrong number of attributes");
		}

		[Test]
		public void EosAtReadingFirstTLVPadding() {
			STUNMessageBuilder builder = new STUNMessageBuilder(null,
				STUNClass.Request, STUNMethod.Binding,
				new Transaction(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10));
			builder.WriteAttribute(0b11, new ByteBuffer(new byte[] { 255, 255 }));

			byte[] message = builder.Build().ToArray();

			List<STUNAttr> attrs = new List<STUNAttr>();
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message, 0, 20 + 4 + 2 + 1), false, attrs);

			Assert.IsFalse(parser.isValid);
			Assert.AreEqual(0, attrs.Count, "Wrong number of attributes");
		}

		[Test]
		public void WrongLengthOfAttributes() {
			byte[] reference = new byte[] {
				0x00, 0x01, 0x00, 0x2C, 0x21, 0x12, 0xA4, 0x42, 0x0A, 0x14, 0x1E, 0x28, 0x32, 0x3C, 0x46, 0x50,
				0x5A, 0x64, 0x6E, 0x78, 0x00, 0x06, 0x00, 0x03, 0x61, 0x3A, 0x62, 0x00, 0x00, 0x24, 0x00, 0x04,
				0x6E, 0x7F, 0x1E, 0xFF, 0x00, 0x25, 0x00, 0x00, 0x00, 0x08, 0x00, 0x14, 0xF5, 0xC6, 0x0F, 0x17,
				0xF5, 0xBB, 0xC0, 0x2D, 0xA6, 0xDE, 0x64, 0x4B, 0x36, 0xF8, 0xB6, 0xBE, 0x79, 0xA0, 0xA6, 0x16
			};

			Assert.IsTrue(new STUNMessageParser(new ByteBuffer(reference), false).isValid);

			// Length too long
			reference[3] = 0x3c;
			Assert.IsFalse(new STUNMessageParser(new ByteBuffer(reference), false).isValid);

			// Length too short
			reference[3] = 0x04;
			Assert.IsFalse(new STUNMessageParser(new ByteBuffer(reference), false).isValid);

			// Length not % 4
			reference[3] = 0x2d;
			Assert.IsFalse(new STUNMessageParser(new ByteBuffer(reference), false).isValid);
		}
	}
}
