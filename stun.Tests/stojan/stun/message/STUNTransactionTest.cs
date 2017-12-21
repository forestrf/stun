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
using Org.BouncyCastle.Math;

namespace me.stojan.stun.message {
	/**
	 * Created by vuk on 24/10/16.
	 */
	public class STUNTransactionTest {
		[Test]
		public void upTo95Bits() {
			byte[] val = new byte[] { 0x7F, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

			Assert.AreEqual(12, val.Length);
			CollectionAssert.AreEqual(val, STUNTransaction.Transaction(BigInteger.One.ShiftLeft(95).Subtract(BigInteger.One)));
		}

		[Test]
		public void at96Bits() {
			byte[] val = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

			Assert.AreEqual(12, val.Length);
			CollectionAssert.AreEqual(val, STUNTransaction.Transaction(BigInteger.One.ShiftLeft(96).Subtract(BigInteger.One)));
		}

		[Test]
		public void over96Bits() {
			byte[] val = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

			Assert.AreEqual(12, val.Length);
			CollectionAssert.AreEqual(val, STUNTransaction.Transaction(BigInteger.One.ShiftLeft(97).Subtract(BigInteger.One)));
		}
	}
}
