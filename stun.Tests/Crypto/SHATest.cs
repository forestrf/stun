using NUnit.Framework;
using System.Security.Cryptography;

namespace STUN.me.stojan.stun.message {
	[TestFixture]
	public class SHATest {
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


			var output = new byte[20];


			var sha1managed = new SHA1Managed();
			byte[] tmp1 = null;
			uint[] tmp2 = null;

			for (int i = 0; i < 3; i++) {
				Crypto.SHA.computeSHA1(bytes16, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha1managed.ComputeHash(bytes16), output);

				Crypto.SHA.computeSHA1(bytes64, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha1managed.ComputeHash(bytes64), output);

				Crypto.SHA.computeSHA1(bytes256, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha1managed.ComputeHash(bytes256), output);

				Crypto.SHA.computeSHA1(bytes512, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha1managed.ComputeHash(bytes512), output);
			}
			
			output = new byte[32];

			var sha256managed = new SHA256Managed();
			for (int i = 0; i < 3; i++) {
				Crypto.SHA.computeSHA256(bytes16, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha256managed.ComputeHash(bytes16), output);

				Crypto.SHA.computeSHA256(bytes64, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha256managed.ComputeHash(bytes64), output);

				Crypto.SHA.computeSHA256(bytes256, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha256managed.ComputeHash(bytes256), output);

				Crypto.SHA.computeSHA256(bytes512, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha256managed.ComputeHash(bytes512), output);
			}

			var key = new byte[64];
			random.NextBytes(key);
			
			HMACSHA1 hMACSHA1_1 = new HMACSHA1(key);
			Crypto.HMAC_SHA1 hMACSHA1_2 = new Crypto.HMAC_SHA1(key);

			output = new byte[20];

			for (int i = 0; i < 3; i++) {
				hMACSHA1_2.ComputeHash(bytes16, 0, bytes16.Length, output, 0);
				Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes16), output);

				hMACSHA1_2.ComputeHash(bytes64, 0, bytes64.Length, output, 0);
				Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes64), output);

				hMACSHA1_2.ComputeHash(bytes256, 0, bytes256.Length, output, 0);
				Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes256), output);

				hMACSHA1_2.ComputeHash(bytes512, 0, bytes512.Length, output, 0);
				Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes512), output);
			}


			hMACSHA1_2.ComputeHash(bytes512, 40, 200, output, 0);
			Assert.AreEqual(hMACSHA1_1.ComputeHash(bytes512, 40, 200), output);
		}
	}
}
