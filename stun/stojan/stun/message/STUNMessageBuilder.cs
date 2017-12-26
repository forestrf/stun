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
using System.Collections.Generic;

namespace STUN.me.stojan.stun.message {
	/// <summary>
	/// A message builder for well-formed STUN messages.
	/// </summary>
	public class STUNMessageBuilder {
		private byte[] header = new byte[20];

		private int totalValues = 0;
		private List<byte[]> values = new List<byte[]>();

		/// <summary>
		/// Set the message type.
		/// </summary>
		/// <param name="group">The STUN class</param>
		/// <param name="method">The STUN method</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder MessageType(STUNMessageType group, STUNMessageType method) {
			IntAs16Bit(STUNMethod.Join(method, group) & 0b0011_1111_1111_1111, header, 0);
			return this;
		}

		/// <summary>
		/// Set the STUN transaction value.
		/// </summary>
		/// <param name="transaction">The transaction value, will be clamped to last 96 bits</param>
		/// <returns>This builder, never null</returns>
		public STUNMessageBuilder Transaction(ByteBuffer transaction) {
			ByteBuffer tx = STUNTransaction.Transaction(transaction);

			Array.Copy(STUNHeader.MAGIC_COOKIE, 0, header, 4, STUNHeader.MAGIC_COOKIE.Length);
			Array.Copy(tx.Data, tx.positionAbsolute, header, 4 + STUNHeader.MAGIC_COOKIE.Length, tx.Length);

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
			values.Add(raw);

			IntAs16Bit(totalValues & 0xFFFF, header, 2);

			return this;
		}

		/// <summary>
		/// Return a copy of the current header value. The STUN magic cookie will not be set if <see cref="Transaction(ByteBuffer)"/> has not been called.
		/// </summary>
		/// <returns>The header value, never null, will always have length of 20</returns>
		public byte[] GetHeaderCopy() {
			byte[] c = new byte[header.Length];
			header.CopyTo(c, 0);
			return c;
		}

		/// <summary>
		/// Build a byte representation of the message.
		/// </summary>
		/// <returns>The byte representation of the message, never null</returns>
		public byte[] Build() {
			byte[] built = new byte[header.Length + totalValues];

			Array.Copy(header, 0, built, 0, header.Length);

			int position = header.Length;

			foreach (byte[] tlv in values) {
				Array.Copy(tlv, 0, built, position, tlv.Length);
				position += tlv.Length;
			}

			return built;
		}

		private void IntAs16Bit(int value, byte[] o, int position) {
			o[position] = (byte) ((value >> 8) & 255);
			o[position + 1] = (byte) (value & 255);
		}
	}
}
