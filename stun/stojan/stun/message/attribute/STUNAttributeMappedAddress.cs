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

using STUN;
using System;

namespace me.stojan.stun.message.attribute {
	/**
	 * Supports the creation of the STUN MAPPED-ADDRESS and XOR-MAPPED-ADDRESS attributes.
	 */
	public static class STUNAttributeMappedAddress {

		/** STUN reserved type for this attribute. */
		public const int TYPE = 0x0001;

		public const byte ADDRESS_IPV4 = 0x01;
		public const byte ADDRESS_IPV6 = 0x02;
		
		/**
		 * Create the MAPPED-ADDRESS attribute.
		 * @param addr the address, must not be null and must be 4 (IPv4) or 16 bytes (IPv6) long
		 * @param port the port, will be treated as 16-bit
		 * @return the value, never null
		 */
		public static bool Value(byte[] addr, int port, out byte[] bytes) {
			byte type;

			switch (addr.Length) {
				case 4:
					type = ADDRESS_IPV4;
					break;

				case 16:
					type = ADDRESS_IPV6;
					break;

				default:
					bytes = null;
					Logger.Error("Unsupported address of length " + addr.Length);
					return false;
			}

			bytes = new byte[1 + 1 + 2 + addr.Length];

			bytes[0] = 0;
			bytes[1] = type;
			bytes[2] = (byte) ((port >> 8) & 255);
			bytes[3] = (byte) (port & 255);

			Array.Copy(addr, 0, bytes, 4, addr.Length);

			return true;
		}

		/**
		 * Get the port from an attribute and header.
		 * @param attribute the attribute, msut be a valid attribute
		 * @return the port
		 * @throws IllegalArgumentException if attribute is null
		 * @throws InvalidSTUNAttributeException if attribute is not valid
		 */
		public static bool Port(byte[] attribute, out int value) {
			if (CheckAttribute(attribute)) {
				value = ((attribute[2] & 255) << 8) | (attribute[3] & 255);
				return true;
			} else {
				value = 0;
				return false;
			}
		}

		/**
		 * Get the address from an attribute and header.
		 * @param attribute the attribute, must be a valid attribute
		 * @return the address
		 * @throws IllegalArgumentException if attribute is null
		 * @throws InvalidSTUNAttributeException if attribute is not valid
		 */
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
			}

			address = new byte[addressLength];

			Array.Copy(attribute, 4, address, 0, address.Length);

			return true;
		}

		/**
		 * Check that the attribute is valid.
		 * @param attribute the attribute
		 * @throws IllegalArgumentException if attribute is null
		 * @throws InvalidSTUNAttributeException if attribute is not valid per STUN spec
		 */
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
