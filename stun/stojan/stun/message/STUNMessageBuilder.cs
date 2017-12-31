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

		private ByteBuffer buffer;
		private ByteBuffer header;

		private int totalValues = 0;


		private STUNMessageBuilder() { }
		public STUNMessageBuilder(byte[] buffer) {
			if (buffer == null || buffer.Length < MINIMUM_BUFFER_SIZE) {
				buffer = new byte[MINIMUM_BUFFER_SIZE];
				Logger.Warn("The buffer is null or not large enough (" + MINIMUM_BUFFER_SIZE + " bytes). A different internal buffer has been allocated");
			}
			const int headerLength = 20;
			this.buffer = new ByteBuffer(buffer);
			header = new ByteBuffer(buffer, 0, headerLength);
			this.buffer.positionAbsolute = headerLength;
		}

		/// <summary>
		/// Set the message type.
		/// </summary>
		/// <param name="group">The STUN class</param>
		/// <param name="method">The STUN method</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder SetMessageType(STUNClass stunClass, STUNMethod stunMethod) {
			ushort stunMessageType = (ushort) (0x3FFF & ((int) stunClass | (int) stunMethod));
			header[0] = (byte) (stunMessageType >> 8);
			header[1] = (byte) (stunMessageType & 0xff);
			return this;
		}

		/// <summary>
		/// Set the STUN transaction value.
		/// </summary>
		/// <param name="transaction">The transaction value, will be clamped to last 96 bits</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder Transaction(ByteBuffer transaction) {
			ByteBuffer tx = STUNTransaction.Transaction(transaction);

			Array.Copy(STUNHeader.MAGIC_COOKIE, 0, header.Data, header.offset + 4, STUNHeader.MAGIC_COOKIE.Length);
			Array.Copy(tx.Data, tx.positionAbsolute, header.Data, header.offset + 4 + STUNHeader.MAGIC_COOKIE.Length, tx.Length);

			return this;
		}

		/// <summary>
		/// Add a value to the STUN message.
		/// </summary>
		/// <param name="type">The message type</param>
		/// <param name="value">The message value</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder Value(int type, byte[] value) {
			byte[] raw = STUNTypeLengthValue.Value(type, value);

			totalValues += raw.Length;
			buffer.Put(raw);

			IntAs16Bit(totalValues & 0xFFFF, header, 2);

			return this;
		}

		/// <summary>
		/// Return a copy of the current header value. The STUN magic cookie will not be set if <see cref="Transaction(ByteBuffer)"/> has not been called.
		/// </summary>
		/// <returns>The header value, never null, will always have length of 20</returns>
		public ByteBuffer GetHeader() {
			return header;
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

		private void IntAs16Bit(int value, ByteBuffer o, int position) {
			o.Put(position, (ushort) value);
		}
	}
}
