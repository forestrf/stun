using STUN.Utils;

namespace STUN.Crypto {
	public class HMACSHA1 {
		byte[] key; 
		public HMACSHA1(byte[] key) {
			this.key = (byte[]) key.Clone();
		}

		public byte[] ComputeHash(byte[] arr, int offset, int count) {
			return SHA1.computeHMAC_SHA1(key, new ByteBuffer(arr, offset, count).ToArray());
		}
	}
}
