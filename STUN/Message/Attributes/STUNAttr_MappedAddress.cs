using BBuffer;
using NoGcSockets;
using STUN.Message.Enums;
using STUN.Utils;
using System;
using System.Net;

namespace STUN.Message.Attributes {
	public struct STUNAttr_MappedAddress : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.MAPPED_ADDRESS;

		public ushort port;
		public IPv4Holder ipv4;
		public IPv6Holder ipv6;
		private AddressFamily family;

		public STUNAttr_MappedAddress(IPEndPoint endPoint) : this(endPoint.Address, (ushort) endPoint.Port) { }
		public STUNAttr_MappedAddress(IPEndPointStruct endPoint) {
			if (System.Net.Sockets.AddressFamily.InterNetwork == endPoint.addressFamily) {
				this = new STUNAttr_MappedAddress(endPoint.ipv4, endPoint.port);
			}
			else {
				this = new STUNAttr_MappedAddress(endPoint.ipv6, endPoint.port);
			}
		}

		public STUNAttr_MappedAddress(IPAddress address, ushort port) {
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
		public STUNAttr_MappedAddress(IPv4Holder ipv4, ushort port) {
			family = AddressFamily.IPv4;
			this.ipv4 = ipv4;
			ipv6 = new IPv6Holder();
			this.port = port;
		}
		public STUNAttr_MappedAddress(IPv6Holder ipv6, ushort port) {
			family = AddressFamily.IPv6;
			ipv4 = new IPv4Holder();
			this.ipv6 = ipv6;
			this.port = port;
		}

		public STUNAttr_MappedAddress(STUNAttr attr) : this() {
			ReadFromBuffer(attr);
		}

		public void WriteToBuffer(ref ByteBuffer buffer) {
			ushort length = (ushort) (4 + (family == AddressFamily.IPv4 ? AddressLength.IPv4 : AddressLength.IPv6));
			STUNTypeLengthValue.WriteTypeLength(TYPE, length, ref buffer);

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

			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			buffer.GetByte();
			family = (AddressFamily) buffer.GetByte();
			port = buffer.GetUShort();

			if (AddressFamily.IPv4 == family) {
				ipv4.Read(ref buffer);
			}
			else {
				ipv6.Read(ref buffer);
			}
		}

		public bool isIPv4() {
			return AddressFamily.IPv4 == family;
		}

		public IPEndPoint ToIPEndPoint() {
			return new IPEndPoint(isIPv4() ? ipv4.ToIPAddress() : ipv6.ToIPAddress(), port);
		}

		public IPEndPointStruct ToIPEndPointStruct() {
			if (AddressFamily.IPv4 == family) {
				return new IPEndPointStruct(ipv4, port);
			}
			else {
				return new IPEndPointStruct(ipv6, port);
			}
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
