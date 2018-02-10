using NUnit.Framework;
using STUN.Message.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace STUN.Message.Attributes {
	[TestFixture]
	public class STUNAttr_ErrorCodeTest {
		[Test]
		public void NoErrors() {
			byte[] expected = new byte[] {
				0x01, 0x01, 0x00, 0x10, 0x21, 0x12, 0xA4, 0x42,
				0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
				0x08, 0x09, 0x0A, 0x0B, 0x00, 0x09, 0x00, 0x0A,
				0x00, 0x00, 0x04, 0x38, 0x31, 0x32, 0x33, 0x34,
				0x35, 0x36, 0x00, 0x00
			};

			ushort errorCode = 456;
			string errorMessage = "123456";

			var msg = new STUNMessageBuilder(new byte[128],
				STUNClass.Success, STUNMethod.Binding,
				new Transaction(new byte[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }));
			msg.WriteAttribute(new STUNAttr_ErrorCode(errorCode, errorMessage));
			var stunReq = msg.Build();

			CollectionAssert.AreEqual(expected, stunReq.ToArray());

			var attrs = new List<STUNAttr>();
			var parser = new STUNMessageParser(stunReq, ref attrs);
			Assert.IsTrue(parser.valid);

			STUNAttr_ErrorCode parsedAttr = new STUNAttr_ErrorCode();
			parsedAttr.ReadFromBuffer(attrs[0]);
			Assert.AreEqual(errorCode, parsedAttr.code, "Parser can't read the errorCode correctly");
			Assert.AreEqual(errorMessage, parsedAttr.reason, "Parser can't read the errorMessage correctly");
		}

		[Test]
		public void Max128() {
			byte[] expected = new byte[] {
				0x01, 0x01, 0x00, 0xF0, 0x21, 0x12, 0xA4, 0x42,
				0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
				0x08, 0x09, 0x0A, 0x0B, 0x00, 0x09, 0x00, 0xEA,
				0x00, 0x00, 0x05, 0x2B, 0x00, 0x05, 0x0A, 0x0F,
				0x14, 0x19, 0x1E, 0x23, 0x28, 0x2D, 0x32, 0x37,
				0x3C, 0x41, 0x46, 0x4B, 0x50, 0x55, 0x5A, 0x5F,
				0x64, 0x69, 0x6E, 0x73, 0x78, 0x7D, 0xC2, 0x82,
				0xC2, 0x87, 0xC2, 0x8C, 0xC2, 0x91, 0xC2, 0x96,
				0xC2, 0x9B, 0xC2, 0xA0, 0xC2, 0xA5, 0xC2, 0xAA,
				0xC2, 0xAF, 0xC2, 0xB4, 0xC2, 0xB9, 0xC2, 0xBE,
				0xC3, 0x83, 0xC3, 0x88, 0xC3, 0x8D, 0xC3, 0x92,
				0xC3, 0x97, 0xC3, 0x9C, 0xC3, 0xA1, 0xC3, 0xA6,
				0xC3, 0xAB, 0xC3, 0xB0, 0xC3, 0xB5, 0xC3, 0xBA,
				0xC3, 0xBF, 0xC4, 0x84, 0xC4, 0x89, 0xC4, 0x8E,
				0xC4, 0x93, 0xC4, 0x98, 0xC4, 0x9D, 0xC4, 0xA2,
				0xC4, 0xA7, 0xC4, 0xAC, 0xC4, 0xB1, 0xC4, 0xB6,
				0xC4, 0xBB, 0xC5, 0x80, 0xC5, 0x85, 0xC5, 0x8A,
				0xC5, 0x8F, 0xC5, 0x94, 0xC5, 0x99, 0xC5, 0x9E,
				0xC5, 0xA3, 0xC5, 0xA8, 0xC5, 0xAD, 0xC5, 0xB2,
				0xC5, 0xB7, 0xC5, 0xBC, 0xC6, 0x81, 0xC6, 0x86,
				0xC6, 0x8B, 0xC6, 0x90, 0xC6, 0x95, 0xC6, 0x9A,
				0xC6, 0x9F, 0xC6, 0xA4, 0xC6, 0xA9, 0xC6, 0xAE,
				0xC6, 0xB3, 0xC6, 0xB8, 0xC6, 0xBD, 0xC7, 0x82,
				0xC7, 0x87, 0xC7, 0x8C, 0xC7, 0x91, 0xC7, 0x96,
				0xC7, 0x9B, 0xC7, 0xA0, 0xC7, 0xA5, 0xC7, 0xAA,
				0xC7, 0xAF, 0xC7, 0xB4, 0xC7, 0xB9, 0xC7, 0xBE,
				0xC8, 0x83, 0xC8, 0x88, 0xC8, 0x8D, 0xC8, 0x92,
				0xC8, 0x97, 0xC8, 0x9C, 0xC8, 0xA1, 0xC8, 0xA6,
				0xC8, 0xAB, 0xC8, 0xB0, 0xC8, 0xB5, 0xC8, 0xBA,
				0xC8, 0xBF, 0xC9, 0x84, 0xC9, 0x89, 0xC9, 0x8E,
				0xC9, 0x93, 0xC9, 0x98, 0xC9, 0x9D, 0xC9, 0xA2,
				0xC9, 0xA7, 0xC9, 0xAC, 0xC9, 0xB1, 0xC9, 0xB6,
				0xC9, 0xBB, 0x00, 0x00
			};

			Random r = new Random(123);
			StringBuilder builder = new StringBuilder(129);
			for (int i = 0; i < 130; i++) {
				builder.Append((char) (i * 5));
			}

			var msg = new STUNMessageBuilder(new byte[256],
				STUNClass.Success, STUNMethod.Binding,
				new Transaction(new byte[12] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }));
			msg.WriteAttribute(new STUNAttr_ErrorCode(543, builder.ToString()));
			var stunReq = msg.Build();

			CollectionAssert.AreEqual(expected, stunReq.ToArray());
		}
	}
}
