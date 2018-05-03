using BBuffer;
using STUN.Message.Enums;
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

		public STUNAttr_XORMappedAddress(IPEndPoint endPoint) : this(endPoint.Address, (ushort) endPoint.Port) { }

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

		public STUNAttr_XORMappedAddress(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
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
			}
			else {
				ipv6.Write(ref buffer);
			}

			// XOR
			attr[2] = (byte) (attr[2] ^ buffer[4]);
			attr[3] = (byte) (attr[3] ^ buffer[5]);
			for (int i = 4; i < length; i++)
				attr[i] = (byte) (attr[i] ^ buffer[i]);

			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			buffer.GetByte();
			family = (AddressFamily) buffer.GetByte();

			// XOR
			port = buffer.GetUShort();
			if (AddressFamily.IPv4 == family) {
				ipv4.Read(ref buffer);
			}
			else {
				ipv6.Read(ref buffer);
			}

			port ^= attr.stunMessage.GetUShortAt(4);
			if (AddressFamily.IPv4 == family) {
				ipv4.bits ^= attr.stunMessage.GetUIntAt(4);
			}
			else {
				ipv6.msb ^= attr.stunMessage.GetULongAt(4);
				ipv6.lsb ^= attr.stunMessage.GetULongAt(12);
			}
		}

		public bool isIPv4() {
			return AddressFamily.IPv4 == family;
		}

		public IPEndPoint ToIPEndPoint() {
			return new IPEndPoint(isIPv4() ? ipv4.ToIPAddress() : ipv6.ToIPAddress(), port);
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			s.Append("Family: ").Append(family).Append("\n");
			s.Append("IP: ").Append(isIPv4() ? ipv4.ToIPAddress().ToString() : ipv6.ToIPAddress().ToString()).Append("\n");
			s.Append("Port: ").Append(port).Append("\n");
			return s.ToString();
		}
	}
}
