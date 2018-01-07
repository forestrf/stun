using STUN.Utils;
using System;
using System.Net;

namespace STUN.me.stojan.stun.message.attribute {
	public struct Attr_XORMappedAddress : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.XOR_MAPPED_ADDRESS;

		private ushort port;
		private uint ipv4;
		private IPv6Holder ipv6;
		private AddressFamily family;

		public Attr_XORMappedAddress(IPAddress address, ushort port) {
			switch (address.AddressFamily) {
				case System.Net.Sockets.AddressFamily.InterNetwork:
					family = AddressFamily.IPv4;
					ipv4 = (uint) address.Address; // Obsolete, but avoids generating Garbage
					if (BitConverter.IsLittleEndian) {
						// Reverse byte order
						ipv4 = ipv4 << 24 | (ipv4 & 0xff00) << 8 | (ipv4 & 0xff0000) >> 8 | ipv4 >> 24;
					}
					ipv6 = new IPv6Holder();
					break;
				case System.Net.Sockets.AddressFamily.InterNetworkV6:
					family = AddressFamily.IPv6;
					ipv4 = 0;
					ipv6 = new IPv6Holder(address.GetAddressBytes());
					break;
				default:
					throw new Exception();
			}
			this.port = port;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ushort length = (ushort) (4 + (family == AddressFamily.IPv4 ? AddressLength.IPv4 : AddressLength.IPv6));
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, length, ref buffer);

			ByteBuffer attr = new ByteBuffer(buffer.data, buffer.absPosition);

			// 4 bytes
			buffer.Put((byte) 0);
			buffer.Put((byte) family);
			buffer.Put((ushort) port);

			if (AddressFamily.IPv4 == family) {
				buffer.Put(ipv4);
			} else {
				ipv6.Write(ref buffer);
			}

			// XOR
			attr[2] = (byte) (attr[2] ^ buffer[4]);
			attr[3] = (byte) (attr[3] ^ buffer[5]);
			for (int i = 4; i < length; i++)
				attr[i] = (byte) (attr[i] ^ buffer[i]);
			
			STUNTypeLengthValue.WritePadding(ref buffer);
		}

		// I need something like "read from buffer" to fill the structs
		/*		
		/// <summary>
		/// Get the port from an attribute and header.
		/// </summary>
		/// <param name="header">The header, must be a valid STUN header</param>
		/// <param name="attribute">The attribute, msut be a valid attribute</param>
		/// <param name="portXored">The port</param>
		/// <returns>Successful</returns>
		public static bool Port(byte[] header, byte[] attribute, out int portXored) {
			int port;
			if (STUNAttributeMappedAddress.Port(new ByteBuffer(attribute), out port)) {
				int xor = ((header[4] & 255) << 8) | (header[5] & 255);

				portXored = xor ^ port;
				return true;
			}

			portXored = 0;
			return false;
		}

		/// <summary>
		/// Get the address from an attribute and header.
		/// </summary>
		/// <param name="header">The header, must be a valid STUN header</param>
		/// <param name="attribute">The attribute, must be a valid attribute</param>
		/// <param name="address">The address</param>
		/// <returns>Successful</returns>
		public static bool Address(byte[] header, byte[] attribute, out byte[] address) {
			if (STUNAttributeMappedAddress.Address(new ByteBuffer(attribute), out address)) {
				for (int i = 0; i < address.Length; i++) {
					address[i] = (byte) (attribute[4 + i] ^ header[4 + i]);
				}
				return true;
			}
			
			address = null;
			return false;
		}
		*/
	}
}
