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

using STUN.Utils;
using NUnit.Framework;
using System;
using STUN.Crypto;
using STUN.me.stojan.stun.message.attribute;

namespace STUN.me.stojan.stun.message {
	/**
	 * Created by vuk on 24/10/16.
	 */
	[TestFixture]
	public class STUNMessageBuilderTest {
		[Test]
		public void buildMessage() {
			STUNMessageBuilder builder = new STUNMessageBuilder(null);

			builder.SetMessageType(STUNClass.Error, STUNMethod.Binding);
			builder.SetTransaction(new Transaction(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1));
			builder.Value(0b111, new byte[] { 255 });

			byte[] message = builder.Build().ToArray();

			byte[] transaction = new byte[12];

			Array.Copy(message, 8, transaction, 0, 12);

			ByteBuffer transactionInt = new ByteBuffer(transaction);

			uint magicCookie = new ByteBuffer(message).GetUInt(4);

			// check length
			Assert.AreEqual(0, (message.Length - 20) % 4);
			// check length without header
			Assert.AreEqual(message.Length - 20, new ByteBuffer(message).GetUShort(2));
			// check group
			Assert.AreEqual(STUNClass.Error, STUNHeader.Class(new ByteBuffer(message).GetUShort(0)));
			// check method
			Assert.AreEqual(STUNMethod.Binding, STUNHeader.Method(new ByteBuffer(message).GetUShort(0)));
			// check magic cookie
			Assert.AreEqual(STUNHeader.MAGIC_COOKIE, magicCookie);
			// check transaction
			Assert.AreEqual(1, transactionInt[0]);
			for (int i = 1; i < transactionInt.Length; i++) {
				Assert.AreEqual(0, transactionInt[i]);
			}
			// check first byte of tlv
			Assert.AreEqual(0, message[20]);
			// check second byte of tlv
			Assert.AreEqual(0b111, message[20 + 1]);
			// check first byte of tlv length
			Assert.AreEqual(0, message[20 + 2]);
			// check second byte of tlv length
			Assert.AreEqual(1, message[20 + 2 + 1]);
			// check first byte of tlv value
			Assert.AreEqual(255, message[20 + 2 + 2]);
			// check second byte of tlv value
			Assert.AreEqual(0, message[20 + 2 + 2 + 1]);
			// check third byte of tlv value
			Assert.AreEqual(0, message[20 + 2 + 2 + 2]);
			// check fourth byte of tlv value
			Assert.AreEqual(0, message[20 + 2 + 2 + 3]);
		}

		[Test]
		public void header() {
			STUNMessageBuilder builder = new STUNMessageBuilder(null);

			builder.SetMessageType(STUNClass.Error, STUNMethod.Binding);
			builder.SetTransaction(new Transaction(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10));
			builder.Value(0xABABA, new byte[20]);

			byte[] header = builder.GetHeader().ToArray();

			byte[] transaction = new byte[12];

			Array.Copy(header, 8, transaction, 0, 12);

			ByteBuffer transactionInt = new ByteBuffer(transaction);

			uint magicCookie = new ByteBuffer(header).GetUInt(4);

			Assert.IsNotNull(header);
			Assert.AreEqual(20, header.Length);

			// check group
			Assert.AreEqual(STUNClass.Error, STUNHeader.Class(new ByteBuffer(header).GetUShort(0)));
			// check method
			Assert.AreEqual(STUNMethod.Binding, STUNHeader.Method(new ByteBuffer(header).GetUShort(0)));
			// check tlv length
			Assert.AreEqual(20 + 4, new ByteBuffer(header).GetUShort(2));
			// check magic cookie
			Assert.AreEqual(STUNHeader.MAGIC_COOKIE, magicCookie);
			// check transaction
			Assert.AreEqual(10, transactionInt[0]);
			for (int i = 1; i < transactionInt.Length; i++) {
				Assert.AreEqual(0, transactionInt[i]);
			}
		}

		[Test]
		public void Test() {
			byte[] reference = new byte[] {
				0x00, 0x01, 0x00, 0x2C, 0x21, 0x12, 0xA4, 0x42, 0x0A, 0x14, 0x1E, 0x28, 0x32, 0x3C, 0x46, 0x50,
				0x5A, 0x64, 0x6E, 0x78, 0x00, 0x06, 0x00, 0x03, 0x61, 0x3A, 0x62, 0x00, 0x00, 0x24, 0x00, 0x04,
				0x6E, 0x7F, 0x1E, 0xFF, 0x00, 0x25, 0x00, 0x00, 0x00, 0x08, 0x00, 0x14, 0xF5, 0xC6, 0x0F, 0x17,
				0xF5, 0xBB, 0xC0, 0x2D, 0xA6, 0xDE, 0x64, 0x4B, 0x36, 0xF8, 0xB6, 0xBE, 0x79, 0xA0, 0xA6, 0x16
			};
			
			HMAC_SHA1 hmacGenerator = null;

			// Test using an offseted ByteBuffer
			var msg = new STUNMessageBuilder(new ByteBuffer(new byte[1024], 30, 700));
			msg.SetMessageType(STUNClass.Request, STUNMethod.Binding);
			var tr = new Transaction(120, 110, 100, 90, 80, 70, 60, 50, 40, 30, 20, 10);
			msg.SetTransaction(tr);
			msg.WriteAttribute(new STUNAttribute_Username("a:b"));
			msg.WriteAttribute(new STUNAttribute_Priority(0x6e7f1eff));
			msg.WriteAttribute(new STUNAttribute_UseCandidate());
			var stunReq = msg.Build("pass", false, ref hmacGenerator);
			

			CollectionAssert.AreEqual(reference, stunReq.ToArray());
		}
	}
}
