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
	/**
	 * Supports the creation of STUN's ERROR-CODE attribute.
	 */
	public static class STUNAttributeErrorCode {
		/** The STUN type of this attribute. */
		public const int TYPE = 0x0009;
		
		/**
		 * Create a valid ERROR-CODE STUN attribute.
		 * @param code the code, must be within [300, 700)
		 * @param reason the reason, must not be null and will be taken to be at most 128 chars long
		 * @return the value, length may not be a multiple of 4
		 */
		public static bool Value(int code, string reason, out byte[] attribute) {
			if (code < 300 || code >= 700) {
				attribute = null;
				Logger.Error("Argument code must be within [300, 700)");
				return false;
			}

			if (null == reason) {
				attribute = null;
				Logger.Error("Argument reason must not be null");
				return false;
			}

			int maxReasonLength = Math.Min(reason.Length, 128);

			byte[] reasonBytes = Encoding.UTF8.GetBytes(reason.Substring(0, maxReasonLength));

			attribute = new byte[4 + reasonBytes.Length];

			int errorClass = code / 100;
			int errorNumber = code % 100;

			attribute[0] = 0;
			attribute[1] = 0;
			attribute[2] = (byte) (errorClass & 0b111);
			attribute[3] = (byte) (errorNumber & 0b0111_1111);

			Array.Copy(reasonBytes, 0, attribute, 4, reasonBytes.Length);

			return true;
		}

		/**
		 * Extract the reason from a well-formed ERROR-CODE STUN attribute.
		 * @param attribute the attribute, must be well formed and not null
		 * @return the reason, at most 128 chars long, never null, may be empty
		 * @throws InvalidSTUNAttributeException if the attribute is not well formed
		 */
		public static bool Reason(byte[] attribute, out string output) {
			if (CheckAttribute(attribute)) {
				output = Encoding.UTF8.GetString(attribute, 4, attribute.Length - 4);
				return true;
			} else {
				output = "";
				return false;
			}
		}

		/**
		 * Extract the error code from a well-formed ERROR-CODE STUN attribute.
		 * @param attribute the attribute, must be well-formed and not null
		 * @return the code, a value within [300, 700)
		 * @throws InvalidSTUNAttributeException if the attribute is not well formed
		 */
		public static bool Code(byte[] attribute, out int output) {
			if (CheckAttribute(attribute)) {
				output = (attribute[2] & 0b111) * 100 + (attribute[3] & 0b0111_1111);
				return true;
			} else {
				output = 0;
				return false;
			}
		}

		/**
		 * Check that the attribute is well formed.
		 * @param attribute the attribute
		 * @throws IllegalArgumentException if attribute is null
		 * @throws InvalidSTUNAttributeException if the attribute is not well formed
		 */
		public static bool CheckAttribute(byte[] attribute) {
			if (null == attribute) {
				Logger.Error("Argument attribute must not be null");
				return false;
			}

			if (attribute.Length < 4) {
				Logger.Error("Argument attribute must be at least 4 bytes long");
				return false;
			}

			if (attribute.Length > (4 + 763)) {
				Logger.Error("Argument attribute can be at most 4 + 763 bytes long");
				return false;
			}

			if (0 != attribute[0] || 0 != attribute[1]) {
				Logger.Error("Argument attribute must start with two 0 bytes");
				return false;
			}

			if (attribute[2] < 3 || attribute[2] > 6) {
				Logger.Error("Argument attribute at 3-rd position must contain a value within [3, 7)");
				return false;
			}

			if (attribute[3] < 0 || attribute[3] > 99) {
				Logger.Error("Argument attribute at 4-th position must contain a value within [0, 100)");
				return false;
			}

			return true;
		}
	}
}
