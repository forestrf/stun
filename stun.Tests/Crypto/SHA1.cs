using NUnit.Framework;
using System.Security.Cryptography;

namespace STUN.me.stojan.stun.message {
	[TestFixture]
	public class SHA1Test {
		[Test]
		public void groupExtracionFromMessageType() {
			var random = new System.Random(123);
			var bytes16 = new byte[16];
			var bytes64 = new byte[64];
			random.NextBytes(bytes64);
			random.NextBytes(bytes16);

			SHA1Managed sha1managed = new SHA1Managed();
			Assert.AreEqual(sha1managed.ComputeHash(bytes16), Crypto.SHA1.computeSHA1(bytes16));
			Assert.AreEqual(sha1managed.ComputeHash(bytes64), Crypto.SHA1.computeSHA1(bytes64));
		}
	}
}
