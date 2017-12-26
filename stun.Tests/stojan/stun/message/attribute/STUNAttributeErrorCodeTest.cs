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
	 * Created by vuk on 20/11/16.
	 */
	[TestFixture]
	public class STUNAttributeErrorCodeTest {
		[Test]
		public void TYPE() {
			Assert.AreEqual(0x0009, STUNAttributeErrorCode.TYPE);
		}

		[Test]
		public void value_nullReason() {
			byte[] b;
			Assert.AreEqual(false, STUNAttributeErrorCode.Value(300, null, out b));
		}

		[Test]
		public void value_errorCodeLessThan300() {
			byte[] b;
			Assert.AreEqual(false, STUNAttributeErrorCode.Value(299, "", out b));
		}

		[Test]
		public void value_errorCodeGreaterThan699() {
			byte[] b;
			Assert.AreEqual(false, STUNAttributeErrorCode.Value(700, "", out b));
		}

		[Test]
		public void value_generateLongReason() {
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < 250; i++) {
				builder.Append('!');
			}

			string longReason = builder.ToString();

			byte[] attribute;
			Assert.AreEqual(true, STUNAttributeErrorCode.Value(699, longReason, out attribute));

			Assert.AreEqual(4 + 128, attribute.Length);
			Assert.AreEqual(0, attribute[0]);
			Assert.AreEqual(0, attribute[1]);
			Assert.AreEqual(6, attribute[2]);
			Assert.AreEqual(99, attribute[3]);

			Assert.AreEqual(Encoding.UTF8.GetString(attribute, 4, attribute.Length - 4), longReason.Substring(0, 128));
		}

		[Test]
		public void checkAttribute_nullValue() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(null));
		}

		[Test]
		public void checkAttribute_lengthLessThan4() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[3]));
		}

		[Test]
		public void checkAttribute_nonZeroFirstByte() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[] { 1, 0, 3, 0 }));
		}

		[Test]
		public void checkAttribute_nonZeroSecondByte() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[] { 0, 1, 3, 0 }));
		}

		[Test]
		public void checkAttribute_thirdByteLessThan3() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[] { 0, 0, 2, 0 }));
		}

		[Test]
		public void checkAttribute_tirdByteGreaterThan6() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[] { 0, 0, 7, 0 }));
		}

		[Test]
		public void checkAttribute_fourthByteGreaterThan99() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[] { 0, 0, 3, 100 }));
		}

		[Test]
		public void checkAttribute_fourthByteGreaterThan128() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[] { 0, 0, 3, (byte) 129 }));
		}

		[Test]
		public void checkAttribute_maxLength() {
			Assert.AreEqual(false, STUNAttributeErrorCode.CheckAttribute(new byte[4 + 764]));
		}

		[Test]
		public void reason_extract() {
			string reason = "Hello, World!";

			byte[] attribute;
			Assert.AreEqual(true, STUNAttributeErrorCode.Value(300, reason, out attribute));

			string outReason;
			Assert.AreEqual(true, STUNAttributeErrorCode.Reason(attribute, out outReason));
			Assert.AreEqual(reason, outReason);
		}

		[Test]
		public void reason_emptyExtract() {
			string reason = "";

			byte[] attribute;
			Assert.AreEqual(true, STUNAttributeErrorCode.Value(300, reason, out attribute));

			string outReason;
			Assert.IsTrue(STUNAttributeErrorCode.Reason(attribute, out outReason));
			Assert.AreEqual(reason, outReason);
		}

		[Test]
		public void code_extract300() {
			byte[] attribute;
			Assert.AreEqual(true, STUNAttributeErrorCode.Value(300, "", out attribute));
			int code;
			Assert.AreEqual(true, STUNAttributeErrorCode.Code(attribute, out code));
			Assert.AreEqual(300, code);
		}

		[Test]
		public void code_extract699() {
			byte[] attribute;
			Assert.AreEqual(true, STUNAttributeErrorCode.Value(699, "", out attribute));
			int code;
			Assert.AreEqual(true, STUNAttributeErrorCode.Code(attribute, out code));
			Assert.AreEqual(699, code);
		}
	}
}
