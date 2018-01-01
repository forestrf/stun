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

using STUN.Utils;
using System.Runtime.InteropServices;

namespace STUN.me.stojan.stun.message {
	[StructLayout(LayoutKind.Explicit)]
	public struct Transaction {
		/// <summary>lsb</summary>
		[FieldOffset(0)] public byte b0;
		[FieldOffset(1)] public byte b1;
		[FieldOffset(2)] public byte b2;
		[FieldOffset(3)] public byte b3;
		[FieldOffset(4)] public byte b4;
		[FieldOffset(5)] public byte b5;
		[FieldOffset(6)] public byte b6;
		[FieldOffset(7)] public byte b7;
		[FieldOffset(8)] public byte b8;
		[FieldOffset(9)] public byte b9;
		[FieldOffset(10)] public byte b10;
		/// <summary>msb</summary>
		[FieldOffset(11)] public byte b11;

		public Transaction(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7, byte b8, byte b9, byte b10, byte b11) {
			this.b0 = b0;
			this.b1 = b1;
			this.b2 = b2;
			this.b3 = b3;
			this.b4 = b4;
			this.b5 = b5;
			this.b6 = b6;
			this.b7 = b7;
			this.b8 = b8;
			this.b9 = b9;
			this.b10 = b10;
			this.b11 = b11;
		}

		public void Write(ByteBuffer buffer, int offset) {
			buffer[offset + 11] = b0;
			buffer[offset + 10] = b1;
			buffer[offset + 9] = b2;
			buffer[offset + 8] = b3;
			buffer[offset + 7] = b4;
			buffer[offset + 6] = b5;
			buffer[offset + 5] = b6;
			buffer[offset + 4] = b7;
			buffer[offset + 3] = b8;
			buffer[offset + 2] = b9;
			buffer[offset + 1] = b10;
			buffer[offset] = b11;
		}
		public void Read(ByteBuffer buffer, int offset) {
			b0 = buffer[offset + 11];
			b1 = buffer[offset + 10];
			b2 = buffer[offset + 9];
			b3 = buffer[offset + 8];
			b4 = buffer[offset + 7];
			b5 = buffer[offset + 6];
			b6 = buffer[offset + 5];
			b7 = buffer[offset + 4];
			b8 = buffer[offset + 3];
			b9 = buffer[offset + 2];
			b10 = buffer[offset + 1];
			b11 = buffer[offset];
		}
	}
}
