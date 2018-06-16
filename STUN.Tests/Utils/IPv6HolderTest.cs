using NUnit.Framework;
using System.Net;

namespace STUN.Utils {
	[TestFixture]
	public class IPv6HolderTest {
		[Test]
		public void IPv6AndReflection() {
			Assert.AreEqual(IPAddress.IPv6Any, new IPv6Holder(IPAddress.IPv6Any, false).ToIPAddress());
			Assert.AreEqual(IPAddress.IPv6Any, new IPv6Holder(IPAddress.IPv6Any, true).ToIPAddress());
			Assert.AreEqual(IPAddress.IPv6Loopback, new IPv6Holder(IPAddress.IPv6Loopback, false).ToIPAddress());
			Assert.AreEqual(IPAddress.IPv6Loopback, new IPv6Holder(IPAddress.IPv6Loopback, true).ToIPAddress());
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), new IPv6Holder(IPAddress.Parse("2001:4860:4801:32::37"), false).ToIPAddress());
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), new IPv6Holder(IPAddress.Parse("2001:4860:4801:32::37"), true).ToIPAddress());
		}

		[Test]
		public void IPv6ByteAccessing() {
			IPv6Holder holder = new IPv6Holder();
			holder[0] = 0x20;
			holder[1] = 0x01;
			holder[2] = 0x48;
			holder[3] = 0x60;
			holder[4] = 0x48;
			holder[5] = 0x01;
			holder[6] = 0x00;
			holder[7] = 0x32;
			holder[8] = 0x00;
			holder[9] = 0x00;
			holder[10] = 0x00;
			holder[11] = 0x00;
			holder[12] = 0x00;
			holder[13] = 0x00;
			holder[14] = 0x00;
			holder[15] = 0x37;
			Assert.AreEqual(IPAddress.Parse("2001:4860:4801:32::37"), holder.ToIPAddress());
			Assert.AreEqual(0x20, holder[0]);
			Assert.AreEqual(0x01, holder[1]);
			Assert.AreEqual(0x48, holder[2]);
			Assert.AreEqual(0x60, holder[3]);
			Assert.AreEqual(0x48, holder[4]);
			Assert.AreEqual(0x01, holder[5]);
			Assert.AreEqual(0x00, holder[6]);
			Assert.AreEqual(0x32, holder[7]);
			Assert.AreEqual(0x00, holder[8]);
			Assert.AreEqual(0x00, holder[9]);
			Assert.AreEqual(0x00, holder[10]);
			Assert.AreEqual(0x00, holder[11]);
			Assert.AreEqual(0x00, holder[12]);
			Assert.AreEqual(0x00, holder[13]);
			Assert.AreEqual(0x00, holder[14]);
			Assert.AreEqual(0x37, holder[15]);
		}
	}
}
