using BBuffer;
using System.Net;
using System.Reflection;

namespace STUN.Utils {
	public struct IPv6Holder {
		private static FieldInfo field = typeof(IPAddress).GetField("m_Numbers", BindingFlags.NonPublic | BindingFlags.Instance);

		internal ulong msb, lsb;

		public IPv6Holder(IPAddress address, bool tryUsingReflection = true) {
			if (tryUsingReflection) {
				if (field != null) {
					ushort[] m_Numbers = (ushort[]) field.GetValue(address);
					msb = ((ulong) m_Numbers[0] << 48) | ((ulong) m_Numbers[1] << 32) | (uint) (m_Numbers[2] << 16) | m_Numbers[3];
					lsb = ((ulong) m_Numbers[4] << 48) | ((ulong) m_Numbers[5] << 32) | (uint) (m_Numbers[6] << 16) | m_Numbers[7];
					return;
				}
			}

			var buffer = new ByteBuffer(address.GetAddressBytes(), Endianness.Big);
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
