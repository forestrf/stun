namespace STUN.Utils {
	internal struct IPv6Holder {
		public ulong msb, lsb;

		public IPv6Holder(byte[] bytes) {
			var buffer = new ByteBuffer(bytes);
			msb = buffer.GetULong();
			lsb = buffer.GetULong();
		}
		public void Write(ref ByteBuffer buffer) {
			buffer.Put(msb);
			buffer.Put(lsb);
		}
	}
}
