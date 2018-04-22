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

using STUN.Crypto;
using STUN.Message.Attributes;
using STUN.Message.Enums;
using STUN.NetBuffer;

namespace STUN.Message {
	/// <summary>
	/// A message builder for well-formed STUN messages.
	/// </summary>
	public struct STUNMessageBuilder {
		private static readonly int MINIMUM_BUFFER_SIZE = 512;
		internal const int HEADER_LENGTH = 20;

		private ByteBuffer buffer;

		public readonly STUNClass stunClass;
		public readonly STUNMethod stunMethod;
		public readonly Transaction transaction;

		public STUNMessageBuilder(byte[] buffer, STUNClass stunClass, STUNMethod stunMethod, Transaction transaction) : this(new ByteBuffer(buffer), stunClass, stunMethod, transaction) { }

		public STUNMessageBuilder(ByteBuffer buffer, STUNClass stunClass, STUNMethod stunMethod, Transaction transaction) {
			if (!buffer.HasData() || buffer.Length < MINIMUM_BUFFER_SIZE) {
				this.buffer = new ByteBuffer(new byte[MINIMUM_BUFFER_SIZE]);
				Logger.Warn("The buffer is null or not large enough (" + MINIMUM_BUFFER_SIZE + " bytes). A different internal buffer has been allocated");
			}
			else {
				this.buffer = buffer;
			}
			this.buffer.Position = HEADER_LENGTH;

			this.stunClass = stunClass;
			this.stunMethod = stunMethod;
			this.transaction = transaction;

			SetMessageType(stunClass, stunMethod);
			this.buffer.PutAt(4, STUNHeader.MAGIC_COOKIE);
			SetTransaction(transaction);
		}

		/// <summary>
		/// Set the message type.
		/// </summary>
		/// <param name="group">The STUN class</param>
		/// <param name="method">The STUN method</param>
		/// <returns>This builder, never null</returns>
		private void SetMessageType(STUNClass stunClass, STUNMethod stunMethod) {
			ushort stunMessageType = (ushort) (0x3FFF & ((int) stunClass | (int) stunMethod));
			buffer.PutAt(0, stunMessageType);
		}

		/// <summary>
		/// Set the STUN transaction value.
		/// </summary>
		/// <param name="transaction">The transaction value, will be clamped to last 96 bits</param>
		/// <returns>This builder, never null</returns>
		private void SetTransaction(Transaction transaction) {
			buffer.PutAt(4 + 4 + 0, transaction.b11);
			buffer.PutAt(4 + 4 + 1, transaction.b10);
			buffer.PutAt(4 + 4 + 2, transaction.b9);
			buffer.PutAt(4 + 4 + 3, transaction.b8);
			buffer.PutAt(4 + 4 + 4, transaction.b7);
			buffer.PutAt(4 + 4 + 5, transaction.b6);
			buffer.PutAt(4 + 4 + 6, transaction.b5);
			buffer.PutAt(4 + 4 + 7, transaction.b4);
			buffer.PutAt(4 + 4 + 8, transaction.b3);
			buffer.PutAt(4 + 4 + 9, transaction.b2);
			buffer.PutAt(4 + 4 + 10, transaction.b1);
			buffer.PutAt(4 + 4 + 11, transaction.b0);
		}

		/// <summary>
		/// Add a value to the STUN message.
		/// </summary>
		/// <param name="type">The message type</param>
		/// <param name="value">The message value</param>
		/// <returns>This builder, never null</returns>
		public void WriteAttribute(int type, byte[] value) {
			STUNTypeLengthValue.Value(type, value, ref buffer);
			UpdateHeaderAttributesLength(ref buffer, buffer.Position);
		}

		public void WriteAttribute<T>(T attribute) where T : struct, ISTUNAttr {
			attribute.WriteToBuffer(ref buffer);
			UpdateHeaderAttributesLength(ref buffer, buffer.Position);
		}

		public static void UpdateHeaderAttributesLength(ref ByteBuffer buffer, int attributesLength) {
			ushort length = (ushort) ((attributesLength - HEADER_LENGTH) & 0xFFFF);
			buffer.PutAt(2, length);
		}

		/// <summary>
		/// Return a copy of the current header value. The STUN magic cookie will not be set if <see cref="Transaction(ByteBuffer)"/> has not been called.
		/// </summary>
		/// <returns>The header value, never null, will always have length of 20</returns>
		public ByteBuffer GetHeader() {
			return new ByteBuffer(buffer.data, 0, HEADER_LENGTH);
		}

		/// <summary>
		/// Build a byte representation of the message.
		/// </summary>
		public ByteBuffer Build() {
			return buffer.GetCropToCurrentPosition();
		}
		/// <summary>
		/// Build a byte representation of the message.
		/// </summary>
		public ByteBuffer Build(string key, bool addFingerprint) {
			WriteAttribute(new STUNAttr_MessageIntegrity(key));
			if (addFingerprint)
				WriteAttribute(new STUNAttr_Fingerprint());
			return Build();
		}

		public override string ToString() {
			return new STUNMessageParser(Build(), new System.Collections.Generic.List<STUNAttr>()).ToString();
		}
	}
}
