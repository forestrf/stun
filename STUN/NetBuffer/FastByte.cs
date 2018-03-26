using System;
using System.Runtime.InteropServices;

namespace STUN.NetBuffer {
	public enum Endianness {
		/// <summary>
		/// Network Byte order
		/// </summary>
		Big = 0,
		/// <summary>
		/// x86 / x64
		/// </summary>
		Little = 1
	};

	/// <summary>
	/// structs for converting simple types to bytes and back
	/// Includes methods to write and read from a buffer
	/// </summary>
	public static class FastByte {
		private static int IsBigEndian = BitConverter.IsLittleEndian ? 0 : 1;

		private static bool WantReversedEndian(Endianness endianness) {
			return (IsBigEndian ^ (int) endianness) != 1;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Double {
			[FieldOffset(0)] public double value;
			[FieldOffset(0)] public byte b0;
			[FieldOffset(1)] public byte b1;
			[FieldOffset(2)] public byte b2;
			[FieldOffset(3)] public byte b3;
			[FieldOffset(4)] public byte b4;
			[FieldOffset(5)] public byte b5;
			[FieldOffset(6)] public byte b6;
			[FieldOffset(7)] public byte b7;

			public Double(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7) : this() {
				this.b0 = b0;
				this.b1 = b1;
				this.b2 = b2;
				this.b3 = b3;
				this.b4 = b4;
				this.b5 = b5;
				this.b6 = b6;
				this.b7 = b7;
			}
			public Double(double value) : this() {
				this.value = value;
			}

			public double GetReversed() {
				return new Double(b7, b6, b5, b4, b3, b2, b1, b0).value;
			}

			public void Write(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					buffer[offset + 7] = b0;
					buffer[offset + 6] = b1;
					buffer[offset + 5] = b2;
					buffer[offset + 4] = b3;
					buffer[offset + 3] = b4;
					buffer[offset + 2] = b5;
					buffer[offset + 1] = b6;
					buffer[offset] = b7;
				} else {
					buffer[offset] = b0;
					buffer[offset + 1] = b1;
					buffer[offset + 2] = b2;
					buffer[offset + 3] = b3;
					buffer[offset + 4] = b4;
					buffer[offset + 5] = b5;
					buffer[offset + 6] = b6;
					buffer[offset + 7] = b7;
				}
			}
			public double Read(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					b0 = buffer[offset + 7];
					b1 = buffer[offset + 6];
					b2 = buffer[offset + 5];
					b3 = buffer[offset + 4];
					b4 = buffer[offset + 3];
					b5 = buffer[offset + 2];
					b6 = buffer[offset + 1];
					b7 = buffer[offset];
				} else {
					b0 = buffer[offset];
					b1 = buffer[offset + 1];
					b2 = buffer[offset + 2];
					b3 = buffer[offset + 3];
					b4 = buffer[offset + 4];
					b5 = buffer[offset + 5];
					b6 = buffer[offset + 6];
					b7 = buffer[offset + 7];
				}
				return value;
			}
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct Long {
			[FieldOffset(0)] public long value;
			[FieldOffset(0)] public byte b0;
			[FieldOffset(1)] public byte b1;
			[FieldOffset(2)] public byte b2;
			[FieldOffset(3)] public byte b3;
			[FieldOffset(4)] public byte b4;
			[FieldOffset(5)] public byte b5;
			[FieldOffset(6)] public byte b6;
			[FieldOffset(7)] public byte b7;

			public Long(byte b0, byte b1, byte b2, byte b3, byte b4, byte b5, byte b6, byte b7) : this() {
				this.b0 = b0;
				this.b1 = b1;
				this.b2 = b2;
				this.b3 = b3;
				this.b4 = b4;
				this.b5 = b5;
				this.b6 = b6;
				this.b7 = b7;
			}
			public Long(long value) : this() {
				this.value = value;
			}

			public long GetReversed() {
				return new Long(b7, b6, b5, b4, b3, b2, b1, b0).value;
			}

			public void Write(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					buffer[offset + 7] = b0;
					buffer[offset + 6] = b1;
					buffer[offset + 5] = b2;
					buffer[offset + 4] = b3;
					buffer[offset + 3] = b4;
					buffer[offset + 2] = b5;
					buffer[offset + 1] = b6;
					buffer[offset] = b7;
				} else {
					buffer[offset] = b0;
					buffer[offset + 1] = b1;
					buffer[offset + 2] = b2;
					buffer[offset + 3] = b3;
					buffer[offset + 4] = b4;
					buffer[offset + 5] = b5;
					buffer[offset + 6] = b6;
					buffer[offset + 7] = b7;
				}
			}
			public long Read(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					b0 = buffer[offset + 7];
					b1 = buffer[offset + 6];
					b2 = buffer[offset + 5];
					b3 = buffer[offset + 4];
					b4 = buffer[offset + 3];
					b5 = buffer[offset + 2];
					b6 = buffer[offset + 1];
					b7 = buffer[offset];
				} else {
					b0 = buffer[offset];
					b1 = buffer[offset + 1];
					b2 = buffer[offset + 2];
					b3 = buffer[offset + 3];
					b4 = buffer[offset + 4];
					b5 = buffer[offset + 5];
					b6 = buffer[offset + 6];
					b7 = buffer[offset + 7];
				}
				return value;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Float {
			[FieldOffset(0)] public float value;
			[FieldOffset(0)] public byte b0;
			[FieldOffset(1)] public byte b1;
			[FieldOffset(2)] public byte b2;
			[FieldOffset(3)] public byte b3;

			public Float(byte b0, byte b1, byte b2, byte b3) : this() {
				this.b0 = b0;
				this.b1 = b1;
				this.b2 = b2;
				this.b3 = b3;
			}
			public Float(float value) : this() {
				this.value = value;
			}

			public float GetReversed() {
				return new Float(b3, b2, b1, b0).value;
			}

			public void Write(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					buffer[offset + 3] = b0;
					buffer[offset + 2] = b1;
					buffer[offset + 1] = b2;
					buffer[offset] = b3;
				} else {
					buffer[offset] = b0;
					buffer[offset + 1] = b1;
					buffer[offset + 2] = b2;
					buffer[offset + 3] = b3;
				}
			}
			public float Read(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					b0 = buffer[offset + 3];
					b1 = buffer[offset + 2];
					b2 = buffer[offset + 1];
					b3 = buffer[offset];
				} else {
					b0 = buffer[offset];
					b1 = buffer[offset + 1];
					b2 = buffer[offset + 2];
					b3 = buffer[offset + 3];
				}
				return value;
			}
		}
		[StructLayout(LayoutKind.Explicit)]
		public struct Int {
			[FieldOffset(0)] public int value;
			[FieldOffset(0)] public byte b0;
			[FieldOffset(1)] public byte b1;
			[FieldOffset(2)] public byte b2;
			[FieldOffset(3)] public byte b3;

			public Int(byte b0, byte b1, byte b2, byte b3) : this() {
				this.b0 = b0;
				this.b1 = b1;
				this.b2 = b2;
				this.b3 = b3;
			}
			public Int(int value) : this() {
				this.value = value;
			}

			public int GetReversed() {
				return new Int(b3, b2, b1, b0).value;
			}

			public void Write(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					buffer[offset + 3] = b0;
					buffer[offset + 2] = b1;
					buffer[offset + 1] = b2;
					buffer[offset] = b3;
				} else {
					buffer[offset] = b0;
					buffer[offset + 1] = b1;
					buffer[offset + 2] = b2;
					buffer[offset + 3] = b3;
				}
			}
			public int Read(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					b0 = buffer[offset + 3];
					b1 = buffer[offset + 2];
					b2 = buffer[offset + 1];
					b3 = buffer[offset];
				} else {
					b0 = buffer[offset];
					b1 = buffer[offset + 1];
					b2 = buffer[offset + 2];
					b3 = buffer[offset + 3];
				}
				return value;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Short {
			[FieldOffset(0)] public short value;
			[FieldOffset(0)] public byte b0;
			[FieldOffset(1)] public byte b1;

			public Short(byte b0, byte b1) : this() {
				this.b0 = b0;
				this.b1 = b1;
			}
			public Short(short value) : this() {
				this.value = value;
			}

			public short GetReversed() {
				return new Short(b1, b0).value;
			}

			public void Write(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					buffer[offset + 1] = b0;
					buffer[offset] = b1;
				} else {
					buffer[offset] = b0;
					buffer[offset + 1] = b1;
				}
			}
			public short Read(byte[] buffer, int offset, Endianness endianness) {
				if (WantReversedEndian(endianness)) {
					b0 = buffer[offset + 1];
					b1 = buffer[offset];
				} else {
					b0 = buffer[offset];
					b1 = buffer[offset + 1];
				}
				return value;
			}
		}
	}
}
