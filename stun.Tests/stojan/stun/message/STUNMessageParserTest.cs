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

using NUnit.Framework;
using Org.BouncyCastle.Math;

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
			builder.Transaction(BigInteger.One);
			builder.Value(0b111, new byte[] { 255 });
			builder.Value(0b010, new byte[] { 0, 255, 0, 255 });

			byte[] message = builder.Build();

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(message));

			STUNMessageParser.Header header = parser.start();

			CollectionAssert.AreEqual(Arrays.copyOf(message, 20), header.raw()));

			Assert.AreEqual(STUNMessageType.GROUP_RESPONSE_ERROR, header.group());
			Assert.AreEqual(STUNMessageType.METHOD_BINDING, header.method());
			Assert.AreEqual(BigInteger.One, new BigInteger(header.transaction()));
			Assert.IsTrue(0 == header.length() % 4);
			Assert.IsTrue(header.isMagicCookieValid());

			STUNMessageParser.TypeLengthValue tlv1 = header.next();

			Assert.IsNotNull(tlv1);
			Assert.AreEqual(0b111, tlv1.type());
			CollectionAssert.AreEqual(new byte [] { 0, 0b111, 0, 1 }, tlv1.header());
			CollectionAssert.AreEqual(new byte[] { 255 }, tlv1.value());
			Assert.AreEqual(1, tlv1.length());
			Assert.AreEqual(3, tlv1.padding());

			STUNMessageParser.TypeLengthValue tlv2 = tlv1.next();

			Assert.IsNotNull(tlv2);
			Assert.AreEqual(0b010, tlv2.type());
			CollectionAssert.AreEqual(new byte[] { 0, 0b010, 0, 4 }, tlv2.header());
			CollectionAssert.AreEqual(new byte[] { 0, 255, 0, 255 }, tlv2.value());
			Assert.AreEqual(4, tlv2.length());
			Assert.AreEqual(0, tlv2.padding());

			assertNull(tlv2.next());
		}

		[Test](expected = InvalidSTUNMessageException.class)
		public void shortHeader() {
			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(new byte[19]));

			parser.start();
		}

		[Test](expected = InvalidSTUNMessageException.class)
		public void headerDoesNotStartWith00() {
			byte[] header = new byte[20];
			header[0] = 255;

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(header));

			parser.start();
		}

		[Test](expected = InvalidSTUNMessageException.class)
		public void lengthNotAMultipleOf4() {
			byte[] header = new byte[20];

			header[2] = 0;
			header[3] = 3;

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(header));

			parser.start();
		}

		[Test]
		public void noTLV() {
			byte[] header = new byte[20];

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(header));

			assertNull(parser.start().next());
		}

		[Test](expected = InvalidSTUNMessageException.class)
		public void eosAtReadingFirstTLVHeader() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
			builder.Transaction(BigInteger.TEN);
			builder.Value(0b11, new byte[] { 255, 255 });

			byte[] message = builder.build();

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(Arrays.copyOf(message, 20 + 1)));

			parser.start().next();
		}

		[Test](expected = InvalidSTUNMessageException.class)
		public void eosAtReadingFirstTLVValue() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
			builder.Transaction(BigInteger.Ten);
			builder.Value(0b11, new byte[] { 255, 255 });

			byte[] message = builder.build();

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(Arrays.copyOf(message, 20 + 4 + 1)));

			parser.start().next();
		}

		[Test](expected = InvalidSTUNMessageException.class)
		public void eosAtReadingFirstTLVPadding() {
			STUNMessageBuilder builder = new STUNMessageBuilder();

			builder.MessageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
			builder.Transaction(BigInteger.Ten);
			builder.Value(0b11, new byte[] { 255, 255 });

			byte[] message = builder.build();

			STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(Arrays.copyOf(message, 20 + 4 + 2 + 1)));

			parser.start().next();
		}
	}
}
