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

using Org.BouncyCastle.Math;
using System;

namespace me.stojan.stun.message {
	/**
	 * Defines a STUN transaction.
	 */
	public static class STUNTransaction {
		/** Maximum value for a STUN transaction. */
		public static BigInteger MAX = BigInteger.One.ShiftLeft(96).Subtract(BigInteger.One);

		/**
		 * Create the bytes for a transaction from a transaction ID.
		 * @param transaction the transaction ID, must not be null
		 * @return the transaction bytes, never null
		 */
		public static byte[] Transaction(BigInteger transaction) {
			byte[] raw = new byte[12];

			byte[] bytes = transaction.ToByteArray();

			if (bytes.Length > 12) {
				Array.Copy(bytes, 1, raw, 0, 12);
			} else {
				Array.Copy(bytes, 0, raw, 12 - bytes.Length, bytes.Length);
			}

			return raw;
		}
	}
}
