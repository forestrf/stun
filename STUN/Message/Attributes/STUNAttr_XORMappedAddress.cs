using BBuffer;
using NoGcSockets;
using STUN.Message.Enums;
using STUN.Utils;
using System.Net;

namespace STUN.Message.Attributes {
	public struct STUNAttr_XORMappedAddress : ISTUNAttr {
		public const STUNAttribute TYPE = STUNAttribute.XOR_MAPPED_ADDRESS;

		public ushort port;
		public IPHolder ip;
		private AddressFamily family;

		public STUNAttr_XORMappedAddress(IPEndPoint endPoint) : this(endPoint.Address, (ushort) endPoint.Port) { }
		public STUNAttr_XORMappedAddress(IPEndPointStruct endPoint) {
			this = new STUNAttr_XORMappedAddress(endPoint.ip, endPoint.port);
		}

		public STUNAttr_XORMappedAddress(IPAddress address, ushort port) {
			family = address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? AddressFamily.IPv4 : AddressFamily.IPv6;
			ip = new IPHolder(address);
			this.port = port;
		}
		public STUNAttr_XORMappedAddress(IPHolder ip, ushort port) {
			family = ip.isIPv4 ? AddressFamily.IPv4 : AddressFamily.IPv6;
			this.ip = ip;
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

			ip.Write(ref buffer);

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
			ip.Read(ref buffer, family == AddressFamily.IPv4);

			port ^= attr.stunMessage.GetUShortAt(4);
			if (AddressFamily.IPv4 == family) {
				ip.bits ^= attr.stunMessage.GetUIntAt(4);
			}
			else {
				ip.msb ^= attr.stunMessage.GetULongAt(4);
				ip.lsb ^= attr.stunMessage.GetULongAt(12);
			}
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
