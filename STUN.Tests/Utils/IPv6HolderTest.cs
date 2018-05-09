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
	}
}
