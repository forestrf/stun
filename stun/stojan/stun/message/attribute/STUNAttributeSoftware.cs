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

using System;
using System.Text;

namespace STUN.me.stojan.stun.message.attribute {
	/// <summary>
	/// Supports the creation of well-formed SOFTWARE STUN attributes.
	/// </summary>
	public static class STUNAttributeSoftware {
		/// <summary>
		/// STUN type of attribute.
		/// </summary>
		public const int TYPE = 0x8022;
		
		/// <summary>
		/// Create a well-formed SOFTWARE STUN attribute.
		/// </summary>
		/// <param name="software">The value, must not be null and will be clamped to first 128 bytes</param>
		/// <param name="attribute">The well-formed attribute</param>
		/// <returns>Successful</returns>
		public static bool Value(string software, out byte[] attribute) {
			if (null == software) {
				attribute = null;
				Logger.Error("Argument software must not be null");
				return false;
			}
			
			string clamped = software.Substring(0, Math.Min(128, software.Length));

			attribute = Encoding.UTF8.GetBytes(clamped);

			return true;
		}

		/// <summary>
		/// Get the software string from a well-formed SOFTWARE STUN attribute bytes.
		/// </summary>
		/// <param name="value">The bytes, must not be null and must be at most 763 bytes long</param>
		/// <param name="output">The SOFTWARE value</param>
		/// <returns>Successful</returns>
		public static bool Software(byte[] value, out string output) {
			if (null == value) {
				output = "";
				Logger.Error("Argument software must not be null");
				return false;
			}

			if (value.Length > 763) {
				output = "";
				Logger.Error("SOFTWARE attribute must not be longer than 763 bytes");
				return false;
			}

			output = Encoding.UTF8.GetString(value);
			return true;
		}
	}
}
