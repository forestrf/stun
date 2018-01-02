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
using STUN.me.stojan.stun.message.attribute;
using STUN.Utils;

namespace STUN.me.stojan.stun.message {
	/// <summary>
	/// A message builder for well-formed STUN messages.
	/// </summary>
	public class STUNMessageBuilder {
		private static readonly int MINIMUM_BUFFER_SIZE = 1024;
		internal const int HEADER_LENGTH = 20;

		private ByteBuffer buffer;


		private STUNMessageBuilder() { }
		public STUNMessageBuilder(byte[] buffer) {
			if (buffer == null || buffer.Length < MINIMUM_BUFFER_SIZE) {
				buffer = new byte[MINIMUM_BUFFER_SIZE];
				Logger.Warn("The buffer is null or not large enough (" + MINIMUM_BUFFER_SIZE + " bytes). A different internal buffer has been allocated");
			}
			this.buffer = new ByteBuffer(buffer);
			this.buffer.absPosition = HEADER_LENGTH;
		}

		/// <summary>
		/// Set the message type.
		/// </summary>
		/// <param name="group">The STUN class</param>
		/// <param name="method">The STUN method</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder SetMessageType(STUNClass stunClass, STUNMethod stunMethod) {
			ushort stunMessageType = (ushort) (0x3FFF & ((int) stunClass | (int) stunMethod));
			buffer.Put(0, stunMessageType);
			return this;
		}

		/// <summary>
		/// Set the STUN transaction value.
		/// </summary>
		/// <param name="transaction">The transaction value, will be clamped to last 96 bits</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder SetTransaction(Transaction transaction) {
			buffer.Put(buffer.absOffset + 4, STUNHeader.MAGIC_COOKIE);

			buffer.Put(buffer.absOffset + 4 + 4, transaction.b11);
			buffer.Put(buffer.absOffset + 4 + 4 + 1, transaction.b10);
			buffer.Put(buffer.absOffset + 4 + 4 + 2, transaction.b9);
			buffer.Put(buffer.absOffset + 4 + 4 + 3, transaction.b8);
			buffer.Put(buffer.absOffset + 4 + 4 + 4, transaction.b7);
			buffer.Put(buffer.absOffset + 4 + 4 + 5, transaction.b6);
			buffer.Put(buffer.absOffset + 4 + 4 + 6, transaction.b5);
			buffer.Put(buffer.absOffset + 4 + 4 + 7, transaction.b4);
			buffer.Put(buffer.absOffset + 4 + 4 + 8, transaction.b3);
			buffer.Put(buffer.absOffset + 4 + 4 + 9, transaction.b2);
			buffer.Put(buffer.absOffset + 4 + 4 + 10, transaction.b1);
			buffer.Put(buffer.absOffset + 4 + 4 + 11, transaction.b0);

			return this;
		}
		
		/// <summary>
		/// Add a value to the STUN message.
		/// </summary>
		/// <param name="type">The message type</param>
		/// <param name="value">The message value</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder Value(int type, byte[] value) {
			STUNTypeLengthValue.Value(type, value, ref buffer);
			UpdateAttributesLength(ref buffer, buffer.Position);
			return this;
		}
		
		public STUNMessageBuilder WriteAttribute<T>(T attribute) where T : struct, ISTUNAttribute {
			attribute.WriteToBuffer(ref buffer);
			UpdateAttributesLength(ref buffer, buffer.Position);
			return this;
		} 

		public static void UpdateAttributesLength(ref ByteBuffer buffer, int attributesLength) {
			ushort length = (ushort) ((attributesLength - HEADER_LENGTH) & 0xFFFF);
			buffer.Put(2, length);
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
		public ByteBuffer Build(string key, bool addFingerprint, ref HMAC_SHA1 hmacGenerator) {
			WriteAttribute(new STUNAttribute_MessageIntegrity(key, ref hmacGenerator));
			if (addFingerprint)
				WriteAttribute(new STUNAttribute_Fingerprint());
			return buffer.GetCropToCurrentPosition();
		}
	}
}
