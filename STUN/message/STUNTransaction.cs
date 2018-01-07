using STUN.Utils;
using System;

namespace STUN.me.stojan.stun.message {
	public struct Transaction {
		/// <summary>lsb</summary>
		public byte b0;
		public byte b1;
		public byte b2;
		public byte b3;
		public byte b4;
		public byte b5;
		public byte b6;
		public byte b7;
		public byte b8;
		public byte b9;
		public byte b10;
		/// <summary>msb</summary>
		public byte b11;

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

		public Transaction(byte[] b) : this(
			b[11], b[10], b[9], b[8], b[7], b[6], 
			b[5], b[4], b[3], b[2], b[1], b[0]) { }

		public Transaction(Random random) : this() {
			int r1 = random.Next();
			int r2 = random.Next();
			int r3 = random.Next();

			b0 = (byte) (r1 >> 24);
			b1 = (byte) (r1 >> 16);
			b2 = (byte) (r1 >> 8);
			b3 = (byte) (r1);

			b4 = (byte) (r2 >> 24);
			b5 = (byte) (r2 >> 16);
			b6 = (byte) (r2 >> 8);
			b7 = (byte) (r2);

			b8 = (byte) (r3 >> 24);
			b9 = (byte) (r3 >> 16);
			b10 = (byte) (r3 >> 8);
			b11 = (byte) (r3);
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
