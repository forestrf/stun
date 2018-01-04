using STUN.Utils;
using System;
using System.Net;

namespace STUN.me.stojan.stun.message.attribute {
	public struct IPv6 {
		public ulong msb, lsb;

		public IPv6(byte[] bytes) {
			var buffer = new ByteBuffer(bytes);
			msb = buffer.GetULong();
			lsb = buffer.GetULong();
		}
		public void Write(ref ByteBuffer buffer) {
			buffer.Put(msb);
			buffer.Put(lsb);
		}
	}

	public struct STUNAttribute_XORMappedAddress : ISTUNAttribute {
		public const STUNAttribute TYPE = STUNAttribute.XOR_MAPPED_ADDRESS;
		private const int IPV4_LENGTH = 4;
		private const int IPV6_LENGTH = 16;

		private enum Family {
			ADDRESS_IPV4 = 0x01,
			ADDRESS_IPV6 = 0x02
		}

		private Family family;
		private uint ipv4;
		private IPv6 ipv6;

		private ushort port;

		public STUNAttribute_XORMappedAddress(IPAddress address, ushort port) {
			switch (address.AddressFamily) {
				case System.Net.Sockets.AddressFamily.InterNetwork:
					family = Family.ADDRESS_IPV4;
					ipv4 = (uint) address.Address; // Obsolete, but avoids generating Garbage
					ipv6 = new IPv6();
					break;
				case System.Net.Sockets.AddressFamily.InterNetworkV6:
					family = Family.ADDRESS_IPV6;
					ipv4 = 0;
					ipv6 = new IPv6(address.GetAddressBytes());
					break;
				default:
					throw new Exception();
			}
			this.port = port;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ushort length = (ushort) (family == Family.ADDRESS_IPV4 ? 4 + IPV4_LENGTH : 4 + IPV6_LENGTH);
			STUNTypeLengthValue.WriteTypeLength((ushort) TYPE, length, ref buffer);

			ByteBuffer attr = new ByteBuffer(buffer.data, buffer.absPosition);

			// 4 bytes
			buffer.Put((byte) 0);
			buffer.Put((byte) family);
			buffer.Put((ushort) port);

			if (Family.ADDRESS_IPV4 == family) {
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
