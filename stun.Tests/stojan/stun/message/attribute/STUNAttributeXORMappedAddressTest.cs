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
using System;

namespace me.stojan.stun.message.attribute {
	/**
	 * Created by vuk on 08/11/16.
	 */
	[TestFixture]
	public class STUNAttributeXORMappedAddressTest {
		[Test]
		public void correctRFCType() {
			Assert.AreEqual(0x0020, STUNAttributeXORMappedAddress.TYPE);
		}

		[Test]
		public void lessThanIPv4Address() {
			byte[] o;
			Assert.IsFalse(STUNAttributeXORMappedAddress.Value(new byte[20], new byte[3], -1, out o));
		}

		[Test]
		public void lessThanIPv6Address() {
			byte[] o;
			Assert.IsFalse(STUNAttributeXORMappedAddress.Value(new byte[20], new byte[8], -1, out o));
		}

		[Test]
		public void overThanIPv6Address() {
			byte[] o;
			Assert.IsFalse(STUNAttributeXORMappedAddress.Value(new byte[20], new byte[17], -1, out o));
		}

		[Test]
		public void ipv4XORWithMagicCookieAndTransactionId() {
			byte[] header = new byte[20];

			for (int i = 4; i < header.Length; i++) {
				header[i] = (byte) i;
			}

			byte[] addr = new byte[] { 192, 168, 3, 234 };

			int port = 0b1010_1010_1010_1010;

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, addr, port, out attribute));

			// 0 padding
			Assert.AreEqual(0, attribute[0]);

			// IPv4
			Assert.AreEqual(STUNAttributeXORMappedAddress.ADDRESS_IPV4, attribute[1]);

			// port
			Assert.AreEqual((byte) ((port >> 8) & 255), (byte) (attribute[2] ^ header[4]));
			Assert.AreEqual((byte) (port & 255), (byte) (attribute[3] ^ header[5]));

			// address
			for (int i = 0; i < addr.Length; i++) {
				Assert.AreEqual(addr[i], attribute[4 + i] ^ header[4 + i]);
			}
		}

		[Test]
		public void ipv6XORWithMagicCookieAndTransactionId() {
			byte[] header = new byte[20];

			for (int i = 4; i < header.Length; i++) {
				header[i] = (byte) i;
			}

			byte[] addr = new byte[] { 0x20, 0x01, 0x0d, 0xb8, 0x85, 0xa3, 0x08, 0xd3, 0x13, 0x19, 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x48 };

			int port = 0b1010_1010_1010_1010;

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, addr, port, out attribute));

			// 0 padding
			Assert.AreEqual(0, attribute[0]);

			// IPv6
			Assert.AreEqual(STUNAttributeMappedAddress.ADDRESS_IPV6, attribute[1]);

			// port
			Assert.AreEqual((byte) (port >> 8), attribute[2] ^ header[4]);
			Assert.AreEqual((byte) (port & 255), attribute[3] ^ header[5]);

			// address
			for (int i = 0; i < addr.Length; i++) {
				Assert.AreEqual(addr[i], attribute[4 + i] ^ header[4 + i]);
			}
		}

		[Test]
		public void checkHeaderNull() {
			Assert.IsFalse(STUNAttributeXORMappedAddress.CheckHeader(null));
		}

		[Test]
		public void checkHeaderLessThan20BytesLong() {
			Assert.IsFalse(STUNAttributeXORMappedAddress.CheckHeader(new byte[19]));
		}

		[Test]
		public void checkHeaderMoreThan20BytesLong() {
			Assert.IsFalse(STUNAttributeXORMappedAddress.CheckHeader(new byte[21]));
		}

		[Test]
		public void checkHeader20BytesLong() {
			Assert.IsTrue(STUNAttributeXORMappedAddress.CheckHeader(new byte[20]));
		}

		[Test]
		public void checkHeaderDoesNotStartWiht00Bits() {
			byte[] bytes = new byte[20];

			bytes[0] = 0b1100_0000;

			Assert.IsFalse(STUNAttributeXORMappedAddress.CheckHeader(bytes));
		}

		[Test]
		public void extractPort() {
			int port = 0b1010_1010_1010_1010;
			byte[] header = new byte[20];

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, new byte[4], port, out attribute));
			int outPort;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Port(header, attribute, out outPort));
			Assert.AreEqual(port, outPort);
		}

		[Test]
		public void extractIPV4Address() {
			byte[] header = new byte[20];

			for (int i = 4; i < 8; i++) {
				header[i] = (byte) i;
			}

			byte[] address = new byte[] { 192, 168, 3, 254 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, address, 0, out attribute));
			byte[] o;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Address(header, attribute, out o));
			CollectionAssert.AreEqual(address, o);
		}

		[Test]
		public void extractIPV6Address() {
			byte[] header = new byte[20];

			for (int i = 4; i < (4 + 16); i++) {
				header[i] = (byte) i;
			}

			byte[] address = new byte[] { 0x20, 0x01, 0x0d, 0xb8, 0x85, 0xa3, 0x08, 0xd3, 0x13, 0x19, 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x48 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, address, 0, out attribute));
			byte[] o;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Address(header, attribute, out o));
			CollectionAssert.AreEqual(address, o);
		}

		[Test]
		public void wrongAddressType() {
			byte[] header = new byte[20];
			byte[] address = new byte[] { 192, 168, 3, 254 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, address, 0, out attribute));

			attribute[1] = (byte) 192;

			byte[] o;
			Assert.IsFalse(STUNAttributeXORMappedAddress.Address(header, attribute, out o));
		}

		[Test]
		public void wrongAddressLength() {
			byte[] header = new byte[20];
			byte[] address = new byte[] { 192, 168, 3, 254 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeXORMappedAddress.Value(header, address, 0, out attribute));

			byte[] wrongAttribute = new byte[attribute.Length + 4];

			Array.Copy(attribute, 0, wrongAttribute, 0, attribute.Length);
			byte[] a;
			Assert.IsFalse(STUNAttributeXORMappedAddress.Address(header, wrongAttribute, out a));
		}
	}
}
