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

namespace STUN.me.stojan.stun.message.attribute {
	/// <summary>
	/// Supports the creation of the STUN MAPPED-ADDRESS and XOR-MAPPED-ADDRESS attributes.
	/// </summary>
	public static class STUNAttributeMappedAddress {
		/// <summary>
		/// STUN reserved type for this attribute.
		/// </summary>
		public const int TYPE = 0x0001;

		public const byte ADDRESS_IPV4 = 0x01;
		public const byte ADDRESS_IPV6 = 0x02;

		/// <summary>
		/// Create the MAPPED-ADDRESS attribute.
		/// </summary>
		/// <param name="addr">The address, must not be null and must be 4 (IPv4) or 16 bytes (IPv6) long</param>
		/// <param name="port">The port, will be treated as 16-bit</param>
		/// <param name="value">The value</param>
		/// <returns>Successful</returns>
		public static bool Value(byte[] addr, int port, out byte[] value) {
			byte type;

			switch (addr.Length) {
				case 4:
					type = ADDRESS_IPV4;
					break;

				case 16:
					type = ADDRESS_IPV6;
					break;

				default:
					value = null;
					Logger.Error("Unsupported address of length " + addr.Length);
					return false;
			}

			value = new byte[1 + 1 + 2 + addr.Length];

			value[0] = 0;
			value[1] = type;
			value[2] = (byte) ((port >> 8) & 255);
			value[3] = (byte) (port & 255);

			Array.Copy(addr, 0, value, 4, addr.Length);

			return true;
		}

		/// <summary>
		/// Get the port from an attribute and header.
		/// </summary>
		/// <param name="attribute">The attribute, must be a valid attribute</param>
		/// <param name="value">The port</param>
		/// <returns>Successful</returns>
		public static bool Port(byte[] attribute, out int port) {
			if (CheckAttribute(attribute)) {
				port = (attribute[2] << 8) | attribute[3];
				return true;
			} else {
				port = 0;
				return false;
			}
		}

		/// <summary>
		/// Get the address from an attribute and header.
		/// </summary>
		/// <param name="attribute">The attribute, must be a valid attribute</param>
		/// <param name="address">The address</param>
		/// <returns>Successful</returns>
		public static bool Address(byte[] attribute, out byte[] address) {
			if (!CheckAttribute(attribute)) {
				address = null;
				return false;
			}

			int addressLength;

			switch (attribute[1]) {
				case ADDRESS_IPV4:
					addressLength = 4;
					break;

				case ADDRESS_IPV6:
					addressLength = 16;
					break;

				default:
					address = null;
					Logger.Error("Unknown address family");
					return false;
			}

			if (4 + addressLength != attribute.Length) {
				address = null;
				Logger.Error("Attribute has invalid length");
				return false;
			}

			address = new byte[addressLength];

			Array.Copy(attribute, 4, address, 0, address.Length);

			return true;
		}

		/// <summary>
		/// Check that the attribute is valid.
		/// </summary>
		/// <param name="attribute">The attribute</param>
		/// <returns>Successful</returns>
		public static bool CheckAttribute(byte[] attribute) {
			if (null == attribute) {
				Logger.Error("Argument attribute must not be null");
				return false;
			}

			if (0 != attribute.Length % 4) {
				Logger.Error("Attribute has length which is not a multiple of 4");
				return false;
			}

			if (4 > attribute.Length) {
				Logger.Error("Attribute length is less than 4");
				return false;
			}

			if (0 != attribute[0]) {
				Logger.Error("(XOR-)MAPPED-ADDRESS attribute does not start with 0");
				return false;
			}

			return true;
		}
	}
}
