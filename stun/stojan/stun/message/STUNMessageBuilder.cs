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
using System;

namespace STUN.me.stojan.stun.message {
	/// <summary>
	/// A message builder for well-formed STUN messages.
	/// </summary>
	public class STUNMessageBuilder {
		private static readonly int MINIMUM_BUFFER_SIZE = 1024;
		private const int headerLength = 20;

		private ByteBuffer buffer;


		private STUNMessageBuilder() { }
		public STUNMessageBuilder(byte[] buffer) {
			if (buffer == null || buffer.Length < MINIMUM_BUFFER_SIZE) {
				buffer = new byte[MINIMUM_BUFFER_SIZE];
				Logger.Warn("The buffer is null or not large enough (" + MINIMUM_BUFFER_SIZE + " bytes). A different internal buffer has been allocated");
			}
			this.buffer = new ByteBuffer(buffer);
			this.buffer.absPosition = headerLength;
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
		public STUNMessageBuilder Transaction(ByteBuffer transaction) {
			ByteBuffer tx = STUNTransaction.Transaction(transaction);

			buffer.Put(buffer.absOffset + 4, STUNHeader.MAGIC_COOKIE);
			Array.Copy(tx.data, tx.absPosition, buffer.data, buffer.absOffset + 4 + 4, tx.Length);

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
			ushort length = (ushort) ((buffer.Position - headerLength) & 0xFFFF);
			buffer.Put(2, length);
			return this;
		}

		/// <summary>
		/// Return a copy of the current header value. The STUN magic cookie will not be set if <see cref="Transaction(ByteBuffer)"/> has not been called.
		/// </summary>
		/// <returns>The header value, never null, will always have length of 20</returns>
		public ByteBuffer GetHeader() {
			return new ByteBuffer(buffer.data, 0, headerLength);
		}

		/// <summary>
		/// Build a byte representation of the message.
		/// </summary>
		/// <returns>The byte representation of the message, never null</returns>
		public byte[] Build() {
			return BuildByteBuffer().ToArray();
		}

		public ByteBuffer BuildByteBuffer() {
			return buffer.GetCropToCurrentPosition();
		}
	}
}
