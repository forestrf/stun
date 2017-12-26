/*
 * Copyright (c) 2016 Stojan Dimitrovski
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using NUnit.Framework;
using System.Text;

namespace me.stojan.stun.message.attribute {
	/**
	 * Created by vuk on 03/12/16.
	 */
	[TestFixture]
	public class STUNAttributeSoftwareTest {
		[Test]
		public void TYPE() {
			Assert.AreEqual(0x8022, STUNAttributeSoftware.TYPE);
		}

		[Test]
		public void value_nullArgument() {
			byte[] o;
			Assert.IsFalse(STUNAttributeSoftware.Value(null, out o));
		}

		[Test]
		public void value_utf8ASCII() {
			string software = "12345678";

			byte[] value;
			Assert.IsTrue(STUNAttributeSoftware.Value(software, out value));

			Assert.IsNotNull(value);
			Assert.AreEqual(software.Length, value.Length);
			Assert.AreEqual(software, Encoding.UTF8.GetString(value));
		}

		[Test]
		public void value_software_max128Chars() {
			StringBuilder builder = new StringBuilder(129);

			for (int i = 0; i < 130; i++) {
				builder.Append('!');
			}

			string software = builder.ToString();

			Assert.IsTrue(software.Length > 128);

			byte[] value;
			Assert.IsTrue(STUNAttributeSoftware.Value(software, out value));

			Assert.IsNotNull(value);

			string softwareRet;
			Assert.IsTrue(STUNAttributeSoftware.Software(value, out softwareRet));

			Assert.IsNotNull(softwareRet);
			Assert.AreEqual(128, softwareRet.Length);
			Assert.AreEqual(software.Substring(0, 128), softwareRet);
		}

		[Test]
		public void software_nullValue() {
			string softwareRet;
			Assert.IsFalse(STUNAttributeSoftware.Software(null, out softwareRet));
		}

		[Test]
		public void software_moreThan763Bytes() {
			string softwareRet;
			Assert.IsFalse(STUNAttributeSoftware.Software(new byte[764], out softwareRet));
		}

		[Test]
		public void software_correctValue() {
			string software = "Hello, this is Cyrillic: Здраво!";

			byte[] value;
			Assert.IsTrue(STUNAttributeSoftware.Value(software, out value));
			Assert.IsNotNull(value);

			string softwareRet;
			Assert.IsTrue(STUNAttributeSoftware.Software(value, out softwareRet));
			Assert.IsNotNull(softwareRet);

			Assert.AreEqual(software, softwareRet);
		}
	}
}
