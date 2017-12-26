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

namespace me.stojan.stun.message {
	/**
	 * Created by vuk on 24/10/16.
	 */
	[TestFixture]
	public class STUNHeaderTest {
		[Test]
		public void groupExtracionFromMessageType() {
			Assert.AreEqual(0b11, STUNHeader.Group(0b0000_0001_0001_0000));
			Assert.AreEqual(0b01, STUNHeader.Group(0b0000_0000_0001_0000));
			Assert.AreEqual(0b10, STUNHeader.Group(0b0000_0001_0000_0000));
		}

		[Test]
		public void methodExtractionFromMessageType() {
			Assert.AreEqual(0b111_111_111_111, STUNHeader.Method(0b1111_1111_1111_1111));
			Assert.AreEqual(0b111_111_111_111, STUNHeader.Method(0b1111_1110_1110_1111));
			Assert.AreEqual(0b101_010_101_010, STUNHeader.Method(0b1010_1011_0101_1010));
		}

		[Test]
		public void int16FromBytes() {
			Assert.AreEqual(257, STUNHeader.Int16(new byte[] { 1, 1 }, 0));
		}
	}
}
