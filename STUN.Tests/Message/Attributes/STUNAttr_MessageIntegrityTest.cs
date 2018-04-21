using NUnit.Framework;
using STUN.Crypto;
using STUN.Message.Enums;
using System.Collections.Generic;

namespace STUN.Message.Attributes {
	[TestFixture]
	public class STUNAttr_MessageIntegrityTest {
		[Test]
		public void Test() {
			byte[] reference = new byte[] {
				0x00, 0x01, 0x00, 0x2C, 0x21, 0x12, 0xA4, 0x42, 0x0A, 0x14, 0x1E, 0x28, 0x32, 0x3C, 0x46, 0x50,
				0x5A, 0x64, 0x6E, 0x78, 0x00, 0x06, 0x00, 0x03, 0x61, 0x3A, 0x62, 0x00, 0x00, 0x24, 0x00, 0x04,
				0x6E, 0x7F, 0x1E, 0xFF, 0x00, 0x25, 0x00, 0x00, 0x00, 0x08, 0x00, 0x14, 0xF5, 0xC6, 0x0F, 0x17,
				0xF5, 0xBB, 0xC0, 0x2D, 0xA6, 0xDE, 0x64, 0x4B, 0x36, 0xF8, 0xB6, 0xBE, 0x79, 0xA0, 0xA6, 0x16
			};

			string username = "a:b";
			uint priority = 0x6e7f1eff;

			var msg = new STUNMessageBuilder(new byte[1024],
				STUNClass.Request, STUNMethod.Binding,
				new Transaction(120, 110, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10));
			msg.WriteAttribute(new STUNAttr_Username(username));
			msg.WriteAttribute(new STUNAttr_Priority(priority));
			msg.WriteAttribute(new STUNAttr_UseCandidate());
			var stunReq = msg.Build("pass", false);
			
			CollectionAssert.AreEqual(reference, stunReq.ToArray());

			var attrs = new List<STUNAttr>();
			var parser = new STUNMessageParser(stunReq, attrs);
			Assert.IsTrue(parser.isValid);

			STUNAttr_Username parsedAttr0 = new STUNAttr_Username();
			parsedAttr0.ReadFromBuffer(attrs[0]);
			Assert.AreEqual(username, parsedAttr0.usernameInString, "Parser can't read the username correctly");
			STUNAttr_Priority parsedAttr1 = new STUNAttr_Priority();
			parsedAttr1.ReadFromBuffer(attrs[1]);
			Assert.AreEqual(priority, parsedAttr1.priority, "Parser can't read the priority correctly");
		}
	}
}
