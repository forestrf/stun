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

using Ashkatchap.Utils;
using NUnit.Framework;
using System;

namespace me.stojan.stun.message {
	/**
	 * Created by vuk on 24/10/16.
	 */
	[TestFixture]
	public class STUNMessageParserTest {
		[Test]
		public void parsing() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_RESPONSE_ERROR, STUNMessageType.METHOD_BINDING);
			builder.Transaction(new ByteBuffer(new byte[] { 1 }));
			builder.Value(0b111, new byte[] { 255 });
			builder.Value(0b010, new byte[] { 0, 255, 0, 255 });

			byte[] message = builder.Build();

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message));

			STUNMessageParser.Header header;
			Assert.IsTrue(parser.Start(out header));

			byte[] copy = new byte[20];
			Buffer.BlockCopy(message, 0, copy, 0, 20);
			CollectionAssert.AreEqual(copy, header.header.ToArray());

			Assert.AreEqual(STUNMessageType.GROUP_RESPONSE_ERROR, header.Group());
			Assert.AreEqual(STUNMessageType.METHOD_BINDING, header.Method());
			var transaction = header.Transaction();
			Assert.AreEqual(1, transaction[0]);
			for (int i = 1; i < transaction.Length; i++) Assert.AreEqual(0, transaction[i]);
			Assert.IsTrue(0 == header.Length() % 4);
			Assert.IsTrue(header.IsMagicCookieValid());

			STUNMessageParser.TypeLengthValue tlv1 = header.Next(parser);

			Assert.IsNotNull(tlv1);
			Assert.AreEqual(0b111, tlv1.Type());
			CollectionAssert.AreEqual(new byte[] { 0, 0b111, 0, 1 }, tlv1.Header().ToArray());
			CollectionAssert.AreEqual(new byte[] { 255 }, tlv1.Value().ToArray());
			Assert.AreEqual(1, tlv1.Length());
			Assert.AreEqual(3, tlv1.Padding());

			STUNMessageParser.TypeLengthValue tlv2 = tlv1.Next(parser);

			Assert.IsNotNull(tlv2);
			Assert.AreEqual(0b010, tlv2.Type());
			CollectionAssert.AreEqual(new byte[] { 0, 0b010, 0, 4 }, tlv2.Header().ToArray());
			CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, tlv2.Value().ToArray());
			Assert.AreEqual(4, tlv2.Length());
			Assert.AreEqual(0, tlv2.Padding());

			Assert.IsNull(tlv2.Next(parser));
		}

		[Test]
		public void shortHeader() {
			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(new byte[19]));

			STUNMessageParser.Header o;
			Assert.IsFalse(parser.Start(out o));
		}

		[Test]
		public void headerDoesNotStartWith00() {
			byte[] header = new byte[20];
			header[0] = 255;

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(header));

			STUNMessageParser.Header o;
			Assert.IsFalse(parser.Start(out o));
		}

		[Test]
		public void lengthNotAMultipleOf4() {
			byte[] header = new byte[20];

			header[2] = 0;
			header[3] = 3;

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(header));

			STUNMessageParser.Header o;
			Assert.IsFalse(parser.Start(out o));
		}

		[Test]
		public void noTLV() {
			byte[] header = new byte[20];

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(header));

			STUNMessageParser.Header h;
			Assert.IsTrue(parser.Start(out h));
			Assert.IsNull(h.Next(parser));
		}

		[Test]
		public void eosAtReadingFirstTLVHeader() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
			builder.Transaction(new ByteBuffer(new byte[] { 10 }));
			builder.Value(0b11, new byte[] { 255, 255 });

			byte[] message = builder.Build();

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message, 0, 20 + 1));

			STUNMessageParser.Header h;
			Assert.IsTrue(parser.Start(out h));
			Assert.IsNull(h.Next(parser));
		}

		[Test]
		public void eosAtReadingFirstTLVValue() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
			builder.Transaction(new ByteBuffer(new byte[] { 10 }));
			builder.Value(0b11, new byte[] { 255, 255 });

			byte[] message = builder.Build();

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message, 0, 20 + 4 + 1));

			STUNMessageParser.Header h;
			Assert.IsTrue(parser.Start(out h));
			Assert.IsNull(h.Next(parser));
		}

		[Test]
		public void eosAtReadingFirstTLVPadding() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
			builder.Transaction(new ByteBuffer(new byte[] { 10 }));
			builder.Value(0b11, new byte[] { 255, 255 });

			byte[] message = builder.Build();

			STUNMessageParser parser = new STUNMessageParser(new ByteBuffer(message, 0, 20 + 4 + 2 + 1));

			STUNMessageParser.Header h;
			Assert.IsTrue(parser.Start(out h));
			Assert.IsNull(h.Next(parser));
		}
	}
}
