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

namespace STUN.me.stojan.stun.message.attribute {
	/// <summary>
	/// Supports the creation of STUN's UNKNOWN-ATTRIBUTES attribute.
	/// </summary>
	public static class STUNAttributeUnknownAttributes {
		/// <summary>
		/// The STUN type.
		/// </summary>
		public const int TYPE = 0x000A;
		
		/// <summary>
		/// Create a well-formed UNKNOWN-ATTRIBUTES attribute from the array of attribute types.
		/// </summary>
		/// <param name="attributes">The attribute types, must not be null</param>
		/// <param name="bytes">The bytes, will always have an even length</param>
		/// <returns>Successful</returns>
		public static bool Value(int[] attributes, out byte[] bytes) {
			if (null == attributes) {
				bytes = null;
				Logger.Error("Argument attributes must not be null");
				return false;
			}

			bytes = new byte[attributes.Length * 2];

			for (int i = 0; i < attributes.Length; i++) {
				bytes[0 + 2 * i] = (byte) ((attributes[i] >> 8) & 255);
				bytes[1 + 2 * i] = (byte) (attributes[i] & 255);
			}

			return true;
		}

		/// <summary>
		/// Extract the attributes from a well-formed byte representation of STUN's UNKNOWN-ATTRIBUTES attribute.
		/// </summary>
		/// <param name="attribute">The well-formed attribute, must not be null</param>
		/// <param name="attributes">The array of attributes, will not be null, may be empty</param>
		/// <returns>Successful</returns>
		public static bool Attributes(byte[] attribute, out int[] attributes) {
			if (null == attribute) {
				attributes = null;
				Logger.Error("Argument attribute must not be null");
				return false;
			}

			if (0 != (attribute.Length % 2)) {
				attributes = null;
				Logger.Error("Argument attribute must have an even length");
				return false;
			}

			attributes = new int[attribute.Length / 2];

			for (int i = 0; i < attributes.Length; i++) {
				attributes[i] = ((attribute[0 + 2 * i] & 255) << 8) | (attribute[1 + 2 * i] & 255);
			}

			return true;
		}
	}
}
