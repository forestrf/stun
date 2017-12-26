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

namespace STUN.me.stojan.stun.message {
	/**
	 * Defines a STUN method.
	 */
	public static class STUNMethod {
		private static int GROUP_MAX = 0b11;
		private static int METHOD_MAX = 0xFFF;

		private static int BITS_5 = 0b111111;
		private static int BITS_4 = 0b1111;
		private static int BITS_3 = 0b111;

		/**
		 * Get the upper-5 bits from the method.
		 * @param method the method
		 * @return the upper-5 bits
		 */
		public static int Upper5(STUNMessageType method) {
			int m = METHOD_MAX & (int) method;
			return (m >> 7) & BITS_5;
		}

		/**
		 * Get the lower-4 bits from the method.
		 * @param method the method
		 * @return the lower-4 bits
		 */
		public static int Lower4(STUNMessageType method) {
			int m = METHOD_MAX & (int) method;
			return (m & BITS_4);
		}

		/**
		 * Get the inner-3 bits from the method.
		 * @param method the method
		 * @return the inner-3 bits
		 */
		public static int Inner3(STUNMessageType method) {
			int m = METHOD_MAX & (int) method;
			return (m >> 4) & BITS_3;
		}

		/**
		 * Format the STUN "class" as an integer.
		 * @param group the class
		 * @return the formatted class
		 */
		public static int Group(STUNMessageType group) {
			int g = GROUP_MAX & (int) group;
			return ((g & 0b10) << 7) | ((g & 1) << 4);
		}

		/**
		 * Join the STUN method and class as a single integer.
		 * @param method the method
		 * @param group the class
		 * @return the joined value
		 */
		public static int Join(STUNMessageType method, STUNMessageType group) {
			int joined = 0;

			joined = joined | Group(group);
			joined = joined | (Upper5(method) << 9);
			joined = joined | (Inner3(method) << 5);
			joined = joined | Lower4(method);

			return joined;
		}
	}
}
