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

namespace STUN.me.stojan.stun.message.attribute {
	/**
	 * Created by vuk on 06/11/16.
	 */
	[TestFixture]
	public class STUNAttributeMappedAddressTest {
		[Test]
		public void correctRFCType() {
			Assert.AreEqual(0x0001, STUNAttributeMappedAddress.TYPE);
		}

		[Test]
		public void lessThanIPv4Address() {
			byte[] o;
			Assert.IsFalse(STUNAttributeMappedAddress.Value(new byte[3], -1, out o));
		}

		[Test]
		public void lessThanIPv6Address() {
			byte[] o;
			Assert.IsFalse(STUNAttributeMappedAddress.Value(new byte[8], -1, out o));
		}

		[Test]
		public void overThanIPv6Address() {
			byte[] o;
			Assert.IsFalse(STUNAttributeMappedAddress.Value(new byte[17], -1, out o));
		}

		[Test]
		public void ipv4Address() {
			byte[] addr = new byte[] { 192, 168, 1, 123 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(addr, -1, out attribute));

			Assert.AreEqual(8, attribute.Length);

			// zero-padding
			Assert.AreEqual(0, attribute[0]);

			// IPv4 address type
			Assert.AreEqual(STUNAttributeXORMappedAddress.ADDRESS_IPV4, attribute[1]);

			// port
			Assert.AreEqual(255, attribute[2]);
			Assert.AreEqual(255, attribute[3]);

			// ipv4 address
			Assert.AreEqual(192, attribute[4]);
			Assert.AreEqual(168, attribute[5]);
			Assert.AreEqual(1, attribute[6]);
			Assert.AreEqual(123, attribute[7]);
		}

		[Test]
		public void ipv6Address() {
			byte[] addr = new byte[] { 0x20, 0x01, 0x0d, (byte) 0xb8, (byte) 0x85, (byte) 0xa3, 0x08, (byte) 0xd3, 0x13, 0x19, (byte) 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x48 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(addr, 123, out attribute));

			Assert.AreEqual(20, attribute.Length);

			// 0 padding
			Assert.AreEqual((byte) 0, attribute[0]);

			// ipv6 address
			Assert.AreEqual(STUNAttributeXORMappedAddress.ADDRESS_IPV6, attribute[1]);

			// port
			Assert.AreEqual(0, attribute[2]);
			Assert.AreEqual(123, attribute[3]);

			for (int i = 0; i < addr.Length; i++) {
				Assert.AreEqual(addr[i], attribute[4 + i]);
			}
		}

		[Test]
		public void nullAttribute() {
			Assert.IsFalse(STUNAttributeMappedAddress.CheckAttribute(null));
		}

		[Test]
		public void shortAttribute() {
			Assert.IsFalse(STUNAttributeMappedAddress.CheckAttribute(new byte[0]));
		}

		[Test]
		public void nonZeroFirstByte() {
			Assert.IsFalse(STUNAttributeMappedAddress.CheckAttribute(new byte[] { 1, 0, 0, 0 }));
		}

		[Test]
		public void extractPort() {
			int port = 0b1010_1010_1010_1010;

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(new byte[] { (byte) 192, (byte) 168, 0, 1 }, port, out attribute));
			int portOut;
			Assert.IsTrue(STUNAttributeMappedAddress.Port(attribute, out portOut));
			Assert.AreEqual(port, portOut);
		}

		[Test]
		public void extractIPV4Address() {
			byte[] address = new byte[] { 192, 168, 7, 254};

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(address, 0, out attribute));
			byte[] outAddress;
			Assert.IsTrue(STUNAttributeMappedAddress.Address(attribute, out outAddress));
			CollectionAssert.AreEqual(address, outAddress);
		}

		[Test]
		public void extractIPV6Address() {
			byte[] address = new byte[] { 0x20, 0x01, 0x0d, 0xb8, 0x85, 0xa3, 0x08, 0xd3, 0x13, 0x19, 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x48 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(address, 0, out attribute));

			byte[] outAddress;
			Assert.IsTrue(STUNAttributeMappedAddress.Address(attribute, out outAddress));
			CollectionAssert.AreEqual(address, outAddress);
		}

		[Test]
		public void wrongAddressType() {
			byte[] address = new byte[] { 192, 168, 3, 254 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(address, 0, out attribute));

			attribute[1] = 192;

			byte[] outAddress;
			Assert.IsFalse(STUNAttributeMappedAddress.Address(attribute, out outAddress));
		}

		[Test]
		public void wrongAddressLength() {
			byte[] address = new byte[] { (byte) 192, (byte) 168, (byte) 3, (byte) 254 };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeMappedAddress.Value(address, 0, out attribute));

			byte[] wrongAttribute = new byte[attribute.Length + 4];

			Array.Copy(attribute, 0, wrongAttribute, 0, attribute.Length);

			byte[] outAddress;
			Assert.IsFalse(STUNAttributeMappedAddress.Address(wrongAttribute, out outAddress));
		}

		[Test]
		public void lengthNotMultipleOf4() {
			Assert.IsFalse(STUNAttributeMappedAddress.CheckAttribute(new byte[15]));
		}
	}
}
