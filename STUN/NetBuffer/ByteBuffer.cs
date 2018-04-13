using System;

namespace STUN.NetBuffer {
	/// <summary>
	/// Struct that wraps an <cref=data>array</cref> and writes/reads to it in Big Endian.
	/// It works on the hole array or on a subset of it.
	/// Because it is an struct, you don't need to pool it but you may need to pass it to other methods using the ref keyword
	/// </summary>
	public struct ByteBuffer {
		/// <summary>
		/// Endianness of the buffer. STUN uses big endian (Network bit order)
		/// </summary>
		public Endianness endianness;

		/// <summary>
		/// Wrapped array
		/// </summary>
		public byte[] data;

		/// <summary>
		/// Position of the read/write cursor in <see cref="data"/>
		/// </summary>
		public int absPosition;

		/// <summary>
		/// Position of the 0-index for this section inside <see cref="data"/>
		/// </summary>
		public int absOffset;

		/// <summary>
		/// usable bytes in <see cref="data"/> from 0
		/// </summary>
		public int absLength;



		public ByteBuffer(byte[] buffer, Endianness endianness = Endianness.Big) : this(buffer, 0, endianness) { }
		public ByteBuffer(byte[] buffer, int offset, Endianness endianness = Endianness.Big) : this(buffer, offset, null != buffer ? buffer.Length - offset : 0, endianness) { }
		public ByteBuffer(byte[] buffer, int offset, int length, Endianness endianness = Endianness.Big) {
			data = buffer;
			absPosition = offset;
			absLength = offset + length;
			absOffset = offset;
			this.endianness = endianness;
		}

		/// <summary>Relative to <see cref="absOffset"/></summary>
		public int Position {
			get { return absPosition - absOffset; }
			set { absPosition = value + absOffset; }
		}

		/// <summary>Relative to <see cref="absOffset"/></summary>
		public int Length {
			get { return absLength - absOffset; }
			set {
				absLength = value + absOffset;
				if (absPosition > absLength) absPosition = absLength;
			}
		}

		/// <summary>Relative to <see cref="absOffset"/></summary>
		public byte this[int index] {
			get { return data[absOffset + index]; }
			set { data[absOffset + index] = value; }
		}

		public int Remaining() {
			return absLength - absPosition;
		}

		public ByteBuffer GetCropToCurrentPosition() {
			ByteBuffer b = new ByteBuffer(data);
			b.absOffset = b.absPosition = absOffset;
			b.absLength = absPosition;
			return b;
		}

		public void SkipBytes(int numberOfBytes) {
			absPosition += numberOfBytes;
		}

		public void Rewind() {
			Position = 0;
		}

		public byte[] ToArray() {
			byte[] copy = new byte[Length];
			Buffer.BlockCopy(data, absOffset, copy, 0, copy.Length);
			return copy;
		}

		public bool BufferEquals(ByteBuffer other) {
			if (Length != other.Length) return false;
			for (int i = 0; i < Length; i++)
				if (this[i] != other[i])
					return false;
			return true;
		}

		public bool HasData() {
			return null != data;
		}

		#region PutMethods
		void UpdateDataSize(int position) {
			if (position > Length) Length = position;
		}

		public void Put(byte value) {
			data[absPosition] = value;
			absPosition += sizeof(byte);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, byte value) {
			data[offset + absOffset] = value;
			UpdateDataSize(offset + absOffset + sizeof(byte));
		}

		public void Put(short value) {
			new FastByte.Short(value).Write(data, absPosition, endianness);
			absPosition += sizeof(short);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, short value) {
			new FastByte.Short(value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(short));
		}

		public void Put(ushort value) {
			new FastByte.Short((short) value).Write(data, absPosition, endianness);
			absPosition += sizeof(ushort);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, ushort value) {
			new FastByte.Short((short) value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(ushort));
		}

		public void Put(int value) {
			new FastByte.Int(value).Write(data, absPosition, endianness);
			absPosition += sizeof(int);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, int value) {
			new FastByte.Int(value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(int));
		}
		public void PutDeltaCompress(int value, int previousValue) {
			PutVariableLength(value - previousValue);
		}
		public void PutDeltaCompressAt(int offset, int value, int previousValue) {
			PutVariableLengthAt(offset, value - previousValue);
		}

		public void Put(uint value) {
			new FastByte.Int((int) value).Write(data, absPosition, endianness);
			absPosition += sizeof(uint);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, uint value) {
			new FastByte.Int((int) value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(uint));
		}

		public void Put(long value) {
			new FastByte.Long(value).Write(data, absPosition, endianness);
			absPosition += sizeof(long);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, long value) {
			new FastByte.Long(value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(long));
		}
		public void PutDeltaCompress(long value, long previousValue) {
			PutVariableLength(value - previousValue);
		}
		public void PutDeltaCompressAt(int offset, long value, long previousValue) {
			PutVariableLengthAt(offset, value - previousValue);
		}

		public void Put(ulong value) {
			new FastByte.Long((long) value).Write(data, absPosition, endianness);
			absPosition += sizeof(ulong);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, ulong value) {
			new FastByte.Long((long) value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(ulong));
		}

		public void Put(float value) {
			new FastByte.Float(value).Write(data, absPosition, endianness);
			absPosition += sizeof(float);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, float value) {
			new FastByte.Float(value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(float));
		}

		public void Put(double value) {
			new FastByte.Double(value).Write(data, absPosition, endianness);
			absPosition += sizeof(double);
			UpdateDataSize(absPosition);
		}
		public void PutAt(int offset, double value) {
			new FastByte.Double(value).Write(data, absOffset + offset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(double));
		}


		public void PutVariableLength(int value) {
			// Right shift 1, moving the MSB to the LSB, so negative numbers can be compressed too
			uint zigzag = (uint) (value << 1) ^ (uint) (value >> (sizeof(int) * 8 - 1));
			PutVariableLength(zigzag);
		}
		public int PutVariableLengthAt(int offset, int value) {
			// Right shift 1, moving the MSB to the LSB, so negative numbers can be compressed too
			uint zigzag = (uint) (value << 1) ^ (uint) (value >> (sizeof(int) * 8 - 1));
			return PutVariableLengthAt(offset, zigzag);
		}

		public void PutVariableLength(uint value) {
			absPosition += PutVariableLengthAt(Position, value);
			UpdateDataSize(absPosition);
		}
		public int PutVariableLengthAt(int offset, uint value) {
			return PutVariableLengthAt(offset, (ulong) value);
		}

		public void PutVariableLength(long value) {
			// Right shift 1, moving the MSB to the LSB, so negative numbers can be compressed too
			ulong zigzag = (ulong) (value << 1) ^ (ulong) (value >> (sizeof(long) * 8 - 1));
			PutVariableLength(zigzag);
		}
		public int PutVariableLengthAt(int offset, long value) {
			// Right shift 1, moving the MSB to the LSB, so negative numbers can be compressed too
			ulong zigzag = (ulong) (value << 1) ^ (ulong) (value >> (sizeof(long) * 8 - 1));
			return PutVariableLengthAt(offset, zigzag);
		}

		public void PutVariableLength(ulong value) {
			absPosition += PutVariableLengthAt(Position, value);
			UpdateDataSize(absPosition);
		}
		public int PutVariableLengthAt(int offset, ulong value) {
			int bytes = 0;
			while (value >= 0x80) {
				PutAt(offset + bytes++, (byte) (0x80 | value));
				value >>= 7;
			}
			PutAt(offset + bytes++, (byte) value);
			return bytes;
		}


		public void Put(byte[] src, int srcOffset, int length) {
			Buffer.BlockCopy(src, absOffset + srcOffset, data, absPosition, length);
			absPosition += length;
			UpdateDataSize(absPosition);
		}
		public void Put(ByteBuffer src, int srcOffset, int length) {
			Buffer.BlockCopy(src.data, src.absOffset + absOffset + srcOffset, data, absPosition, length);
			absPosition += length;
			UpdateDataSize(absPosition);
		}
		public void Put(byte[] data) {
			Put(data, 0, data.Length);
		}
		public void Put(ByteBuffer data) {
			Put(data, 0, data.Length);
		}
		#endregion

		#region GetMethods
		public byte GetByte() {
			byte res = data[absPosition];
			absPosition++;
			return res;
		}

		public short GetShort() {
			short result = new FastByte.Short().Read(data, absPosition, endianness);
			absPosition += sizeof(short);
			return result;
		}
		public short GetShortAt(int offset) {
			return new FastByte.Short().Read(data, absOffset + offset, endianness);
		}

		public ushort GetUShort() {
			ushort v = (ushort) new FastByte.Short().Read(data, absPosition, endianness);
			absPosition += sizeof(ushort);
			return v;
		}
		public ushort GetUShortAt(int offset) {
			return (ushort) new FastByte.Short().Read(data, absOffset + offset, endianness);
		}

		public int GetInt() {
			int result = new FastByte.Int().Read(data, absPosition, endianness);
			absPosition += sizeof(int);
			return result;
		}
		public int GetIntAt(int offset) {
			return new FastByte.Int().Read(data, absOffset + offset, endianness);
		}
		public int GetIntDeltaCompress(int previousValue) {
			return GetIntVariableLength() + previousValue;
		}
		public int GetIntDeltaCompressAt(int offset, int previousValue) {
			int bytes;
			return GetIntVariableLengthAt(offset, out bytes) + previousValue;
		}

		public uint GetUInt() {
			uint result = (uint) new FastByte.Int().Read(data, absPosition, endianness);
			absPosition += sizeof(uint);
			return result;
		}
		public uint GetUIntAt(int offset) {
			return (uint) new FastByte.Int().Read(data, absOffset + offset, endianness);
		}

		public long GetLong() {
			long result = new FastByte.Long().Read(data, absPosition, endianness);
			absPosition += sizeof(long);
			return result;
		}
		public long GetLongAt(int offset) {
			return new FastByte.Long().Read(data, absOffset + offset, endianness);
		}
		public long GetLongDeltaCompress(long previousValue) {
			return GetLongVariableLength() + previousValue;
		}
		public long GetLongDeltaCompressAt(int offset, long previousValue) {
			int bytes;
			return GetLongVariableLengthAt(offset, out bytes) + previousValue;
		}

		public ulong GetULong() {
			ulong result = (ulong) new FastByte.Long().Read(data, absPosition, endianness);
			absPosition += sizeof(ulong);
			return result;
		}
		public ulong GetULongAt(int offset) {
			return (ulong) new FastByte.Long().Read(data, absOffset + offset, endianness);
		}

		public float GetFloat() {
			float result = new FastByte.Float().Read(data, absPosition, endianness);
			absPosition += sizeof(float);
			return result;
		}
		public float GetFloatAt(int offset) {
			return new FastByte.Float().Read(data, absOffset + offset, endianness);
		}

		public double GetDouble() {
			double result = new FastByte.Double().Read(data, absPosition, endianness);
			absPosition += sizeof(double);
			return result;
		}
		public double GetDoubleAt(int offset) {
			return new FastByte.Double().Read(data, absOffset + offset, endianness);
		}


		public int GetIntVariableLength() {
			uint zigzag = GetUIntVariableLength();
			return (int) ((zigzag >> 1) ^ -(zigzag & 1));
		}
		public int GetIntVariableLengthAt(int offset, out int bytes) {
			uint zigzag = GetUIntVariableLengthAt(offset, out bytes);
			return (int) ((zigzag >> 1) ^ -(zigzag & 1));
		}

		public uint GetUIntVariableLength() {
			int bytes;
			uint value = GetUIntVariableLengthAt(Position, out bytes);
			absPosition += bytes;
			UpdateDataSize(absPosition);
			return value;
		}
		public uint GetUIntVariableLengthAt(int offset, out int bytes) {
			return (uint) GetULongVariableLengthAt(offset, out bytes);
		}

		public long GetLongVariableLength() {
			ulong zigzag = GetULongVariableLength();
			return (long) ((zigzag >> 1) ^ (zigzag << (sizeof(long) * 8 - 1)));
		}
		public long GetLongVariableLengthAt(int offset, out int bytes) {
			ulong zigzag = GetULongVariableLengthAt(offset, out bytes);
			return (long) ((zigzag >> 1) ^ (zigzag << (sizeof(long) * 8 - 1)));
		}

		public ulong GetULongVariableLength() {
			int bytes;
			ulong value = GetULongVariableLengthAt(Position, out bytes);
			absPosition += bytes;
			UpdateDataSize(absPosition);
			return value;
		}
		public ulong GetULongVariableLengthAt(int offset, out int bytes) {
			ulong value = 0;
			int bitOffset = 0;
			bytes = 0;
			while (absLength - absPosition >= 1) {
				byte b = data[absOffset + offset + bytes];
				value |= (0x7ful & b) << bitOffset;
				bitOffset += 7;
				bytes++;
				if (0 == (0x80 & b))
					return value;
			}

			// Malformed
			return value;
		}


		public void GetBytes(int srcOffset, byte[] dst) {
			GetBytes(srcOffset, dst, dst.Length);
		}
		public void GetBytes(int srcOffset, byte[] dst, int lenght) {
			GetBytes(srcOffset, dst, 0, lenght);
		}
		public void GetBytes(int srcOffset, byte[] dst, int dstOffset, int lenght) {
			Buffer.BlockCopy(data, absOffset + srcOffset, dst, dstOffset, lenght);
		}
		public void GetBytes(byte[] dst) {
			GetBytes(dst, dst.Length);
		}
		public void GetBytes(byte[] dst, int lenght) {
			GetBytes(dst, 0, lenght);
		}
		public void GetBytes(byte[] dst, int dstOffset, int lenght) {
			Buffer.BlockCopy(data, absPosition, dst, dstOffset, lenght);
			absPosition += lenght;
		}
		#endregion
	}
}
