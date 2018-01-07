using STUN.Message.Enums;
using STUN.NetBuffer;
using STUN.Utils;
using System;
using System.Net;

namespace STUN.Message.Attributes {
	public struct STUNAttr_MappedAddress : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.MAPPED_ADDRESS;

		private ushort port;
		private uint ipv4;
		private IPv6Holder ipv6;
		private AddressFamily family;

		public STUNAttr_MappedAddress(IPAddress address, ushort port) {
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

			STUNTypeLengthValue.WritePadding(ref buffer);
		}
	}
}
