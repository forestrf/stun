using NUnit.Framework;
using STUN.Message.Enums;
using System.Collections.Generic;

namespace STUN.Message.Attributes {
	[TestFixture]
	public class STUNAttr_FingerprintTest {
		[Test]
		public void Test() {
			byte[] reference = new byte[] {
				0x00, 0x01, 0x00, 0x34, 0x21, 0x12, 0xA4, 0x42, 0x0A, 0x14, 0x1E, 0x28, 0x32, 0x3C, 0x46, 0x50,
				0x5A, 0x64, 0x6E, 0x78, 0x00, 0x06, 0x00, 0x03, 0x61, 0x3A, 0x62, 0x00, 0x00, 0x24, 0x00, 0x04,
				0x6E, 0x7F, 0x1E, 0xFF, 0x00, 0x25, 0x00, 0x00, 0x00, 0x08, 0x00, 0x14, 0xF5, 0xC6, 0x0F, 0x17,
				0xF5, 0xBB, 0xC0, 0x2D, 0xA6, 0xDE, 0x64, 0x4B, 0x36, 0xF8, 0xB6, 0xBE, 0x79, 0xA0, 0xA6, 0x16,
				0x80, 0x28, 0x00, 0x04, 0x1E, 0xC5, 0xAA, 0x8B
			};

			var msg = new STUNMessageBuilder(new byte[1024],
				STUNClass.Request, STUNMethod.Binding,
				new Transaction(120, 110, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10));
			msg.WriteAttribute(new STUNAttr_Username("a:b"));
			msg.WriteAttribute(new STUNAttr_Priority(0x6e7f1eff));
			msg.WriteAttribute(new STUNAttr_UseCandidate());
			msg.WriteAttribute(new STUNAttr_MessageIntegrity("pass"));
			msg.WriteAttribute(new STUNAttr_Fingerprint());
			var stunReq = msg.Build();

			CollectionAssert.AreEqual(reference, stunReq.ToArray());

			var attrs = new List<STUNAttr>();
			var parsed = new STUNMessageParser(stunReq, false, attrs);
			int fingerprintIndex = STUNMessageParser.AttributeLastIndexOf(attrs, STUNAttribute.FINGERPRINT);
			Assert.AreEqual(4, fingerprintIndex);
			fingerprintIndex = STUNMessageParser.AttributeIndexOf(attrs, STUNAttribute.FINGERPRINT);
			Assert.AreEqual(4, fingerprintIndex);
			var fingerprint = new STUNAttr_Fingerprint(attrs[fingerprintIndex]);
			Assert.IsTrue(fingerprint.Check());
			stunReq[5]++;
			Assert.IsFalse(fingerprint.Check());
		}
	}
}
