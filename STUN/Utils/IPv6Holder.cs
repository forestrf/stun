using STUN.NetBuffer;
using System.Net;

namespace STUN.Utils {
	public struct IPv6Holder {
		internal ulong msb, lsb;

		public IPv6Holder(IPAddress address) {
			var buffer = new ByteBuffer(address.GetAddressBytes());
			msb = buffer.GetULong();
			lsb = buffer.GetULong();
		}
		public void Write(ref ByteBuffer buffer) {
			buffer.Put(msb);
			buffer.Put(lsb);
		}
		public void Read(ref ByteBuffer buffer) {
			msb = buffer.GetULong();
			lsb = buffer.GetULong();
		}

		public IPAddress ToIPAddress() {
			var address = new byte[AddressLength.IPv6];
			var buffer = new ByteBuffer(address);
			buffer.Put(msb);
			buffer.Put(lsb);

			return new IPAddress(address);
		}
	}
}
