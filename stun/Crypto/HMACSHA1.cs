﻿using STUN.Utils;

namespace STUN.Crypto {
	public class HMAC_SHA1 {
		byte[] key;
		public HMAC_SHA1(byte[] key) {
			this.key = (byte[]) key.Clone();
		}

		byte[] tmp1, tmp2, tmp3;
		uint[] tmp4;
		public byte[] ComputeHash(byte[] arr, int offset, int count) {
			return SHA1.computeHMAC_SHA1(key, new ByteBuffer(arr, offset, count), ref tmp1, ref tmp2, ref tmp3, ref tmp4);
		}
	}
}