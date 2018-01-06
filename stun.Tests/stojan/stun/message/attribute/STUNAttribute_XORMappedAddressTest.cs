using NUnit.Framework;

namespace STUN.me.stojan.stun.message.attribute {
	[TestFixture]
	public class STUNAttribute_XORMappedAddressTest {
		/*
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
		*/

		[Test]
		public void FullTestIPv4() {
			byte[] expected = new byte[] {
				0x01, 0x01, 0x00, 0x0C, 0x21, 0x12, 0xA4, 0x42,
				0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
				0x08, 0x09, 0x0A, 0x0B, 0x00, 0x20,	0x00, 0x08,
				0x00, 0x01, 0xFC, 0xC7, 0x20, 0x10, 0xA7, 0x46
			};

			var msg = new STUNMessageBuilder(new byte[128]);
			msg.SetMessageType(STUNClass.Success, STUNMethod.Binding);
			msg.SetTransaction(new Transaction(new byte[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }));
			msg.WriteAttribute(new STUNAttribute_XORMappedAddress(System.Net.IPAddress.Parse("1.2.3.4"), 56789));
			var stunReq = msg.Build();

			CollectionAssert.AreEqual(expected, stunReq.ToArray());
		}

		[Test]
		public void FullTestIPv6() {
			// WRONG: regenerate
			byte[] expected = new byte[] {
				0x01, 0x01, 0x00, 0x18, 0x21, 0x12, 0xA4, 0x42,
				0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
				0x08, 0x09, 0x0A, 0x0B, 0x00, 0x20, 0x00, 0x14,
				0x00, 0x02, 0xFC, 0xC7, 0x01, 0x13, 0xA9, 0xFA,
				0x85, 0xA2, 0x02, 0x03, 0x04, 0x05, 0x8C, 0x29,
				0x0B, 0x79, 0x79, 0x3F
			};

			var msg = new STUNMessageBuilder(new byte[128]);
			msg.SetMessageType(STUNClass.Success, STUNMethod.Binding);
			msg.SetTransaction(new Transaction(new byte[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }));
			msg.WriteAttribute(new STUNAttribute_XORMappedAddress(System.Net.IPAddress.Parse("[2001:0db8:85a3:0000:0000:8a2e:0370:7334]"), 56789));
			var stunReq = msg.Build();

			CollectionAssert.AreEqual(expected, stunReq.ToArray());
		}
	}
}
