using BBuffer;
using System;
using System.Net;

namespace STUN.Utils {
	public struct IPv4Holder {
		internal uint bits;

		public IPv4Holder(IPAddress address) {
			bits = (uint) address.Address; // Obsolete, but avoids generating Garbage
			if (BitConverter.IsLittleEndian) {
				// Reverse byte order
				bits = bits << 24 | (bits & 0xff00) << 8 | (bits & 0xff0000) >> 8 | bits >> 24;
			}
		}
		public void Write(ref ByteBuffer buffer) {
			buffer.Put(bits);
		}
		public void Read(ref ByteBuffer buffer) {
			bits = buffer.GetUInt();
		}

		public IPAddress ToIPAddress() {
			var address = new byte[AddressLength.IPv4];
			var buffer = new ByteBuffer(address);
			buffer.Put(bits);

			return new IPAddress(address);
		}

		public byte this[int i] {
			get {
				return (byte) (0xffu & (bits >> ((3 - i) * 8)));
			}
			set {
				bits = (bits & (~(0xffu << ((3 - i) * 8)))) | ((uint) value << ((3 - i) * 8));
			}
		}
	}
}
