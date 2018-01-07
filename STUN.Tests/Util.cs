using System.Text;

namespace STUN.Tests {
	public static class Util {
		public static string ByteArrayToCSharpHexDefinition(byte[] arr) {
			StringBuilder t = new StringBuilder(arr.Length);
			foreach (var i in arr) {
				t.Append("0x");
				string p = i.ToString("X");
				if (p.Length == 1)
					t.Append("0");
				t.Append(p);
				t.Append(", ");
			}
			return t.ToString();
		}
	}
}
