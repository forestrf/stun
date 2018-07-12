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
		public IPHolder ip;
		private AddressFamily family;

		public STUNAttr_MappedAddress(IPEndPoint endPoint) : this(endPoint.Address, (ushort) endPoint.Port) { }
		public STUNAttr_MappedAddress(IPEndPointStruct endPoint) {
			this = new STUNAttr_MappedAddress(endPoint.ip, endPoint.port);
		}

		public STUNAttr_MappedAddress(IPAddress address, ushort port) {
			family = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? AddressFamily.IPv4 : AddressFamily.IPv6;
			ip = new IPHolder(address);
			this.port = port;
		}
		public STUNAttr_MappedAddress(IPHolder ip, ushort port) {
			family = ip.isIPv4 ? AddressFamily.IPv4 : AddressFamily.IPv6;
			this.ip = ip;
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

			ip.Write(ref buffer);

			buffer.Pad4();
		}

		public void ReadFromBuffer(STUNAttr attr) {
			var buffer = attr.data;
			buffer.GetByte();
			family = (AddressFamily) buffer.GetByte();
			port = buffer.GetUShort();

			ip.Read(ref buffer, family == AddressFamily.IPv4);
		}

		public bool isIPv4() {
			return AddressFamily.IPv4 == family;
		}

		public IPEndPoint ToIPEndPoint() {
			return new IPEndPoint(ip.ToIPAddress(), port);
		}

		public IPEndPointStruct ToIPEndPointStruct() {
			return new IPEndPointStruct(ip, port);
		}

		public override string ToString() {
			var s = new System.Text.StringBuilder();
			s.Append("TYPE=").Append(TYPE).Append("\n");
			s.Append("Family: ").Append(family).Append("\n");
			s.Append("IP: ").Append(ip.ToIPAddress().ToString()).Append("\n");
			s.Append("Port: ").Append(port).Append("\n");
			return s.ToString();
		}
	}
}
