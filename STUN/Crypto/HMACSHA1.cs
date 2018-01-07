using STUN.NetBuffer;

namespace STUN.Crypto {
	public class HMAC_SHA1 {
		byte[] key;
		public HMAC_SHA1(byte[] key) {
			this.key = (byte[]) key.Clone();
		}

		byte[] tmp1, tmp2, tmp3;
		uint[] tmp4;
		public void ComputeHash(byte[] data, int offset, int count, byte[] dst, int dstOffset) {
			SHA.computeHMAC_SHA1(key, new ByteBuffer(data, offset, count), new ByteBuffer(dst, dstOffset, 20), ref tmp1, ref tmp2, ref tmp3, ref tmp4);
		}
	}
}
