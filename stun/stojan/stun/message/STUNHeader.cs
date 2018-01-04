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
	/// Utilities for forming STUN message headers.
	/// </summary>
	public static class STUNHeader {
		/// <summary>
		/// The STUN "magic cookie". Network Byte Order
		/// </summary>
		internal const uint MAGIC_COOKIE = 0x2112A442;
		private const int ClassMask = 0x0110;
		private const int MethodMask = ~ClassMask;

		/// <summary>
		/// Returns the STUN class from the compound message type.
		/// </summary>
		/// <param name="messageType">The message type</param>
		/// <returns>The group</returns>
		public static STUNClass Class(int messageType) {
			return (STUNClass) (messageType & ClassMask);
		}

		/// <summary>
		/// Returns the STUN method from the compound message type.
		/// </summary>
		/// <param name="messageType">The message type</param>
		/// <returns>The method</returns>
		public static STUNMethod Method(int messageType) {
			return (STUNMethod) (messageType & MethodMask);
		}

		/// <summary>
		/// Returns the two starting bits from the byte array.
		/// </summary>
		/// <param name="header"> The header</param>
		/// <returns>The two starting bits</returns>
		public static int TwoStartingBits(ref ByteBuffer header) {
			return header[0] >> 6;
		}
	}
}
