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

using BBuffer;
using NUnit.Framework;
using STUN.Message.Enums;

namespace STUN.Message {
	/**
	 * Created by vuk on 24/10/16.
	 */
	[TestFixture]
	public class STUNTypeLengthValueTest {
		[Test]
		public void typePosition() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(257, new ByteBuffer(new byte[] { }), ref result);

			Assert.AreEqual(1, result[0]);
			Assert.AreEqual(1, result[1]);
			Assert.AreEqual(0, result[2]);
			Assert.AreEqual(0, result[3]);
		}

		[Test]
		public void lengthPosition() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[257]), ref result);

			Assert.AreEqual(0, result[0]);
			Assert.AreEqual(0, result[1]);
			Assert.AreEqual(1, result[2]);
			Assert.AreEqual(1, result[3]);
		}

		[Test]
		public void resultingSizeZeroLValue() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[0]), ref result);

			Assert.AreEqual(4, result.Position);
		}

		[Test]
		public void resultingSizeOneLValue() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[] { 0xff }), ref result);

			Assert.AreEqual(8, result.Position);
			Assert.AreEqual(0xff, result[4]);
			Assert.AreEqual(0, result[5]);
			Assert.AreEqual(0, result[6]);
			Assert.AreEqual(0, result[7]);
		}

		[Test]
		public void resultingSizeTwoLValue() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[] { 0xff, 0xff }), ref result);

			Assert.AreEqual(8, result.Position);
			Assert.AreEqual(0xff, result[4]);
			Assert.AreEqual(0xff, result[5]);
			Assert.AreEqual(0, result[6]);
			Assert.AreEqual(0, result[7]);
		}

		[Test]
		public void resultingSizeThreeLValue() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[] { 0xff, 0xff, 0xff }), ref result);

			Assert.AreEqual(8, result.Position);
			Assert.AreEqual(0xff, result[4]);
			Assert.AreEqual(0xff, result[5]);
			Assert.AreEqual(0xff, result[6]);
			Assert.AreEqual(0, result[7]);
		}

		[Test]
		public void resultingSizeFourLValue() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[] { 0xff, 0xff, 0xff, 0xff }), ref result);

			Assert.AreEqual(8, result.Position);
			Assert.AreEqual(0xff, result[4]);
			Assert.AreEqual(0xff, result[5]);
			Assert.AreEqual(0xff, result[6]);
			Assert.AreEqual(0xff, result[7]);
		}

		[Test]
		public void resultingSizeFiveLValue() {
			ByteBuffer result = new ByteBuffer(new byte[1024]);
			STUNTypeLengthValue.Value(0, new ByteBuffer(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff }), ref result);

			Assert.AreEqual(12, result.Position);
			Assert.AreEqual(0xff, result[4]);
			Assert.AreEqual(0xff, result[5]);
			Assert.AreEqual(0xff, result[6]);
			Assert.AreEqual(0xff, result[7]);
			Assert.AreEqual(0xff, result[8]);
			Assert.AreEqual(0, result[9]);
			Assert.AreEqual(0, result[10]);
			Assert.AreEqual(0, result[11]);
		}
	}
}
