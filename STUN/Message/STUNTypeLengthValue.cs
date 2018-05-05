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

using BBuffer;

namespace STUN.Message {
	/// <summary>
	/// Defines a STUN Type-Length-Value.
	/// </summary>
	public static class STUNTypeLengthValue {
		/// <summary>
		/// Create a value from the provided type and value.
		/// </summary>
		/// <param name="type">The type</param>
		/// <param name="value">The value</param>
		/// <returns>The TLV bytes, never null</returns>
		public static void Value(int type, byte[] value, ref ByteBuffer buffer) {
			buffer.Put((ushort) type);
			buffer.Put((ushort) value.Length);
			buffer.Put(value);
			buffer.Pad4();
		}

		public static void WriteTypeLength(Enums.STUNAttribute type, ushort length, ref ByteBuffer buffer) {
			buffer.Put((ushort) type);
			buffer.Put(length);
		}
		public static bool ReadTypeLength(ref ByteBuffer buffer, out Enums.STUNAttribute type, out ushort length) {
			type = (Enums.STUNAttribute) buffer.GetUShort();
			length = buffer.GetUShort();
			return length <= buffer.Remaining();
		}
	}
}
