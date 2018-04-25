using NUnit.Framework;
using STUN.NetBuffer;
using System.Security.Cryptography;

namespace STUN.Crypto {
	[TestFixture]
	public class SHATest {
		[Test]
		public void Test_SHA1_SHA256() {
			var random = new System.Random(123);
			var bytes512 = new byte[512];
			random.NextBytes(bytes512);


			var output = new byte[20];


			var sha1managed = new SHA1Managed();
			byte[] tmp1 = null;
			uint[] tmp2 = null;


			for (int dataLength = 0; dataLength < bytes512.Length; dataLength++) {
				SHA.ComputeSHA1(bytes512, dataLength, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha1managed.ComputeHash(new ByteBuffer(bytes512, 0, dataLength).ToArray()), output);
			}

			output = new byte[32];

			var sha256managed = new SHA256Managed();

			for (int dataLength = 0; dataLength < bytes512.Length; dataLength++) {
				SHA.ComputeSHA256(bytes512, dataLength, output, 0, ref tmp1, ref tmp2);
				Assert.AreEqual(sha256managed.ComputeHash(new ByteBuffer(bytes512, 0, dataLength).ToArray()), output);
			}

			var key = new byte[64];
			random.NextBytes(key);

			output = new byte[20];

			for (int keyLength = 0; keyLength < key.Length; keyLength++) {
				for (int dataLength = 0; dataLength < bytes512.Length; dataLength++) {
					HMAC_SHA1.ComputeHash(key, keyLength, bytes512, 0, dataLength, output, 0);
					Assert.AreEqual(new HMACSHA1(new ByteBuffer(key, 0, keyLength).ToArray()).ComputeHash(bytes512, 0, dataLength), output);
				}
			}


			HMAC_SHA1.ComputeHash(key, key.Length, bytes512, 40, 200, output, 0);
			Assert.AreEqual(new HMACSHA1(key).ComputeHash(bytes512, 40, 200), output);
		}
	}
}
