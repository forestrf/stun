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

namespace me.stojan.stun.message.attribute {
	/**
	 * Created by vuk on 20/11/16.
	 */
	[TestFixture]
	public class STUNAttributeUnknownAttributesTest {
		[Test]
		public void TYPE() {
			Assert.AreEqual(0x000A, STUNAttributeUnknownAttributes.TYPE);
		}

		[Test]
		public void value_nullAttribute() {
			byte[] o;
			Assert.IsFalse(STUNAttributeUnknownAttributes.Value(null, out o));
		}

		[Test]
		public void value_zeroAttributes() {
			byte[] attribute;
			Assert.IsTrue(STUNAttributeUnknownAttributes.Value(new int[0], out attribute));

			Assert.AreEqual(0, attribute.Length);
		}

		[Test]
		public void value_multipleAttributes() {
			int[] attributes = new int[] { 0x1ABCD, 0x2EFAB, 0x3CDEF };

			byte[] attribute;
			Assert.IsTrue(STUNAttributeUnknownAttributes.Value(attributes, out attribute));

			Assert.AreEqual(attributes.Length * 2, attribute.Length);

			Assert.AreEqual((byte) 0xAB, attribute[0]);
			Assert.AreEqual((byte) 0xCD, attribute[1]);
			Assert.AreEqual((byte) 0xEF, attribute[2]);
			Assert.AreEqual((byte) 0xAB, attribute[3]);
			Assert.AreEqual((byte) 0xCD, attribute[4]);
			Assert.AreEqual((byte) 0xEF, attribute[5]);
		}

		[Test]
		public void attributes_nullAttribute() {
			int[] attribute;
			Assert.IsFalse(STUNAttributeUnknownAttributes.Attributes(null, out attribute));
		}

		[Test]
		public void attributes_emptyAttribute() {
			int[] attributes;
			Assert.IsTrue(STUNAttributeUnknownAttributes.Attributes(new byte[0], out attributes));
			Assert.AreEqual(0, attributes.Length);
		}

		[Test]
		public void attributes_oddAttributeLength() {
			int[] attributes;
			Assert.IsFalse(STUNAttributeUnknownAttributes.Attributes(new byte[1], out attributes));
		}

		[Test]
		public void attributes_extractAttributes() {
			int[] attributes = new int[] { 0xABCD, 0xEFAB, 0xCDEF, 0xABCD };
			byte[] attribute;
			Assert.IsTrue(STUNAttributeUnknownAttributes.Value(attributes, out attribute));
			int[] extracted;
			Assert.IsTrue(STUNAttributeUnknownAttributes.Attributes(attribute, out extracted));

			CollectionAssert.AreEqual(attributes, extracted);
		}
	}
}
