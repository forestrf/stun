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

namespace STUN.me.stojan.stun.message {
	/// <summary>
	/// Defines a STUN transaction.
	/// </summary>
	public static class STUNTransaction {
		/// <summary>
		/// Maximum value for a STUN transaction.
		/// </summary>
		public static readonly byte[] MAX = new byte[12] {
			0xff, 0xff, 0xff, 0xff,
			0xff, 0xff, 0xff, 0xff,
			0xff, 0xff, 0xff, 0xff
		};

		/// <summary>
		/// Create the bytes for a transaction from a transaction ID.
		/// </summary>
		/// <param name="transaction">The transaction ID, must not be null</param>
		/// <returns>The transaction bytes, never null</returns>
		public static ByteBuffer Transaction(ByteBuffer transaction) {
			if (transaction.Length > 12) {
				return new ByteBuffer(transaction.data, transaction.absPosition + 1, 12);
			} else {
				return new ByteBuffer(transaction.data, transaction.absPosition, transaction.Length);
			}
		}
	}
}
