using STUN.Message.Enums;
using STUN.NetBuffer;
using STUN.Utils;
using System;
using System.Net;

namespace STUN.Message.Attributes {
	public struct STUNAttr_XORMappedAddress : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.XOR_MAPPED_ADDRESS;

		public ushort port;
		public IPv4Holder ipv4;
		public IPv6Holder ipv6;
		private AddressFamily family;

		public STUNAttr_XORMappedAddress(IPAddress address, ushort port) {
			switch (address.AddressFamily) {
				case System.Net.Sockets.AddressFamily.InterNetwork:
					family = AddressFamily.IPv4;
					ipv4 = new IPv4Holder(address);
					ipv6 = new IPv6Holder();
					break;
				case System.Net.Sockets.AddressFamily.InterNetworkV6:
					family = AddressFamily.IPv6;
					ipv4 = new IPv4Holder();
					ipv6 = new IPv6Holder(address);
					break;
				default:
					throw new Exception();
			}
			this.port = port;
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ushort length = (ushort) (4 + (family == AddressFamily.IPv4 ? AddressLength.IPv4 : AddressLength.IPv6));
			STUNTypeLengthValue.WriteTypeLength(TYPE, length, ref buffer);

			ByteBuffer attr = new ByteBuffer(buffer.data, buffer.absPosition);

			// 4 bytes
			buffer.Put((byte) 0);
			buffer.Put((byte) family);
			buffer.Put((ushort) port);

			if (AddressFamily.IPv4 == family) {
				ipv4.Write(ref buffer);
			} else {
				ipv6.Write(ref buffer);
			}

			// XOR
			attr[2] = (byte) (attr[2] ^ buffer[4]);
			attr[3] = (byte) (attr[3] ^ buffer[5]);
			for (int i = 4; i < length; i++)
				attr[i] = (byte) (attr[i] ^ buffer[i]);
			
			STUNTypeLengthValue.AddPadding(ref buffer);
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			buffer.GetByte();
			family = (AddressFamily) buffer.GetByte();

			// XOR
			port = buffer.GetUShort();
			if (AddressFamily.IPv4 == family) {
				ipv4.Read(ref buffer);
			} else {
				ipv6.Read(ref buffer);
			}

			port ^= attr.stunMessage.GetUShort(4);
			if (AddressFamily.IPv4 == family) {
				ipv4.bits ^= attr.stunMessage.GetUInt(4);
			} else {
				ipv6.msb ^= attr.stunMessage.GetULong(4);
				ipv6.lsb ^= attr.stunMessage.GetULong(12);
			}
		}

		public bool isIPv4() {
			return AddressFamily.IPv4 == family;
		}
	}
}
