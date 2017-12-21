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
	public class STUNMethodTest {
		[Test]
		public void groupPosition() {
			Assert.AreEqual(0b0000_0001_0001_0000, STUNMethod.Group(0b11));
			Assert.AreEqual(0b0000_0000_0001_0000, STUNMethod.Group(0b11_01));
			Assert.AreEqual(0b0000_0001_0000_0000, STUNMethod.Group(0b11_10));
		}

		[Test]
		public void upper5Position() {
			Assert.AreEqual(0b11111, STUNMethod.Upper5(0b1111_1000_0000));
			Assert.AreEqual(0b01010, STUNMethod.Upper5(0b0101_0000_0000));
			Assert.AreEqual(0b10101, STUNMethod.Upper5(0b1010_1000_0000));
		}

		[Test]
		public void inner3Position() {
			Assert.AreEqual(0b111, STUNMethod.Inner3(0b0000_0111_0000));
			Assert.AreEqual(0b010, STUNMethod.Inner3(0b1111_0010_1111));
			Assert.AreEqual(0b101, STUNMethod.Inner3(0b1111_0101_1111));
		}

		[Test]
		public void lower4Position() {
			Assert.AreEqual(0b1111, STUNMethod.Lower4(0b0000_0000_1111));
			Assert.AreEqual(0b1010, STUNMethod.Lower4(0b1111_1111_1010));
			Assert.AreEqual(0b0101, STUNMethod.Lower4(0b1111_1111_0101));
		}

		[Test]
		public void join() {
			Assert.AreEqual(0b11_1110_0000_0000, STUNMethod.Join(0b1111_1000_0000, 0b00));
			Assert.AreEqual(0b11_1111_0000_0000, STUNMethod.Join(0b1111_1000_0000, 0b10));
			Assert.AreEqual(0b00_0000_1110_0000, STUNMethod.Join(0b0000_0111_0000, 0b00));
			Assert.AreEqual(0b00_0000_0000_1111, STUNMethod.Join(0b0000_0000_1111, 0b00));
			Assert.AreEqual(0b00_0000_0001_1111, STUNMethod.Join(0b0000_0000_1111, 0b01));
			Assert.AreEqual(0b00_0001_0000_1111, STUNMethod.Join(0b0000_0000_1111, 0b10));
			Assert.AreEqual(0b00_0000_0001_1111, STUNMethod.Join(0b0000_0000_1111, 0b01));
			Assert.AreEqual(0b00_0001_0000_1111, STUNMethod.Join(0b0000_0000_1111, 0b10));
		}
	}
}
