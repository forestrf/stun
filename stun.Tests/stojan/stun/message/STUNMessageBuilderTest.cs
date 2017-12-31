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
			builder.Transaction(new ByteBuffer(new byte[] { 1 }));
			builder.Value(0b111, new byte[] { 255 });

			byte[] message = builder.Build();

			byte[] transaction = new byte[12];

			Array.Copy(message, 8, transaction, 0, 12);

			ByteBuffer transactionInt = new ByteBuffer(transaction);

			byte[] magicCookie = new byte[4];

			Array.Copy(message, 4, magicCookie, 0, 4);

			// check length
			Assert.AreEqual(0, (message.Length - 20) % 4);
			// check length without header
			Assert.AreEqual(message.Length - 20, STUNHeader.Int16(new ByteBuffer(message), 2));
			// check group
			Assert.AreEqual(STUNClass.Error, STUNHeader.Group(STUNHeader.Int16(new ByteBuffer(message), 0)));
			// check method
			Assert.AreEqual(STUNMethod.Binding, STUNHeader.Method(STUNHeader.Int16(new ByteBuffer(message), 0)));
			// check magic cookie
			CollectionAssert.AreEqual(STUNHeader.MAGIC_COOKIE, magicCookie);
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
			builder.Transaction(new ByteBuffer(new byte[] { 10 }));
			builder.Value(0xABABA, new byte[20]);

			byte[] header = builder.GetHeader().ToArray();

			byte[] transaction = new byte[12];

			Array.Copy(header, 8, transaction, 0, 12);

			ByteBuffer transactionInt = new ByteBuffer(transaction);

			byte[] magicCookie = new byte[STUNHeader.MAGIC_COOKIE.Length];

			Array.Copy(header, 4, magicCookie, 0, 4);

			Assert.IsNotNull(header);
			Assert.AreEqual(20, header.Length);

			// check group
			Assert.AreEqual(STUNClass.Error, STUNHeader.Group(STUNHeader.Int16(new ByteBuffer(header), 0)));
			// check method
			Assert.AreEqual(STUNMethod.Binding, STUNHeader.Method(STUNHeader.Int16(new ByteBuffer(header), 0)));
			// check tlv length
			Assert.AreEqual(20 + 4, STUNHeader.Int16(new ByteBuffer(header), 2));
			// check magic cookie
			CollectionAssert.AreEqual(STUNHeader.MAGIC_COOKIE, magicCookie);
			// check transaction
			Assert.AreEqual(10, transactionInt[0]);
			for (int i = 1; i < transactionInt.Length; i++) {
				Assert.AreEqual(0, transactionInt[i]);
			}
		}
	}
}
