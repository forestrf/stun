using NUnit.Framework;
using STUN.Utils;
using System.Security.Cryptography;

namespace STUN.me.stojan.stun.message {
	[TestFixture]
	public class SHA1Test {
		[Test]
		public void Test_SHA1_SHA256() {
			var random = new System.Random(123);
			var bytes16 = new byte[16];
			var bytes64 = new byte[64];
			var bytes256 = new byte[256];
			var bytes512 = new byte[512];
			random.NextBytes(bytes64);
			random.NextBytes(bytes16);
			random.NextBytes(bytes256);
			random.NextBytes(bytes512);

			var sha1managed = new SHA1Managed();
			byte[] tmp1 = null;
			uint[] tmp2 = null;
			Assert.AreEqual(sha1managed.ComputeHash(bytes16), Crypto.SHA1.computeSHA1(bytes16, ref tmp1, ref tmp2));
			Assert.AreEqual(sha1managed.ComputeHash(bytes64), Crypto.SHA1.computeSHA1(bytes64, ref tmp1, ref tmp2));
			Assert.AreEqual(sha1managed.ComputeHash(bytes256), Crypto.SHA1.computeSHA1(bytes256, ref tmp1, ref tmp2));
			Assert.AreEqual(sha1managed.ComputeHash(bytes512), Crypto.SHA1.computeSHA1(bytes512, ref tmp1, ref tmp2));
			Assert.AreEqual(sha1managed.ComputeHash(bytes16), Crypto.SHA1.computeSHA1(bytes16, ref tmp1, ref tmp2));
			Assert.AreEqual(sha1managed.ComputeHash(bytes64), Crypto.SHA1.computeSHA1(bytes64, ref tmp1, ref tmp2));
			Assert.AreEqual(sha1managed.ComputeHash(bytes512), Crypto.SHA1.computeSHA1(bytes512, ref tmp1, ref tmp2));

			var sha256managed = new SHA256Managed();
			Assert.AreEqual(sha256managed.ComputeHash(bytes16), Crypto.SHA1.computeSHA256(bytes16, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes64), Crypto.SHA1.computeSHA256(bytes64, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes256), Crypto.SHA1.computeSHA256(bytes256, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes512), Crypto.SHA1.computeSHA256(bytes512, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes16), Crypto.SHA1.computeSHA256(bytes16, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes64), Crypto.SHA1.computeSHA256(bytes64, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes256), Crypto.SHA1.computeSHA256(bytes256, ref tmp1, ref tmp2));
			Assert.AreEqual(sha256managed.ComputeHash(bytes512), Crypto.SHA1.computeSHA256(bytes512, ref tmp1, ref tmp2));

			var key = new byte[64];
			random.NextBytes(key);

			HMACSHA1 hMACSHA1_1 = new HMACSHA1(key);
			Crypto.HMAC_SHA1 hMACSHA1_2 = new Crypto.HMAC_SHA1(key);
			Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes16), hMACSHA1_2.ComputeHash(bytes16, 0, bytes16.Length));
			Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes64), hMACSHA1_2.ComputeHash(bytes64, 0, bytes64.Length));
			Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes256), hMACSHA1_2.ComputeHash(bytes256, 0, bytes256.Length));
			Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes512), hMACSHA1_2.ComputeHash(bytes512, 0, bytes512.Length));
			
			Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes512, 40, 200), hMACSHA1_2.ComputeHash(bytes512, 40, 200));
		}
	}
}
