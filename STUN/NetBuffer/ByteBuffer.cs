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



		public ByteBuffer(byte[] buffer) : this(buffer, 0) { }
		public ByteBuffer(byte[] buffer, int offset) : this(buffer, offset, null != buffer ? buffer.Length - offset : 0) { }
		public ByteBuffer(byte[] buffer, int offset, int length) : this() {
			data = buffer;
			absPosition = offset;
			absLength = offset + length;
			absOffset = offset;
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

		public ByteBuffer slice() {
			ByteBuffer b = new ByteBuffer(data);
			b.absOffset = b.absPosition = absPosition;
			b.absLength = absLength;
			return b;
		}
		public ByteBuffer flip() {
			absLength = absPosition;
			absPosition = absOffset;
			return this;
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
			absPosition = 0;
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
		public void Put(int offset, byte value) {
			data[offset + absOffset] = value;
			UpdateDataSize(offset + absOffset + sizeof(byte));
		}

		public void Put(short value) {
			new FastByte.Short(value).Write(data, absPosition, endianness);
			absPosition += sizeof(short);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, short value) {
			new FastByte.Short(value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(short));
		}

		public void Put(ushort value) {
			new FastByte.Short((short) value).Write(data, absPosition, endianness);
			absPosition += sizeof(ushort);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, ushort value) {
			new FastByte.Short((short) value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(ushort));
		}

		public void Put(int value) {
			new FastByte.Int(value).Write(data, absPosition, endianness);
			absPosition += sizeof(int);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, int value) {
			new FastByte.Int(value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(int));
		}

		public void Put(uint value) {
			new FastByte.Int((int) value).Write(data, absPosition, endianness);
			absPosition += sizeof(uint);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, uint value) {
			new FastByte.Int((int) value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(uint));
		}

		public void Put(long value) {
			new FastByte.Long(value).Write(data, absPosition, endianness);
			absPosition += sizeof(long);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, long value) {
			new FastByte.Long(value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(long));
		}

		public void Put(ulong value) {
			new FastByte.Long((long) value).Write(data, absPosition, endianness);
			absPosition += sizeof(ulong);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, ulong value) {
			new FastByte.Long((long) value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(ulong));
		}

		public void Put(float value) {
			new FastByte.Float(value).Write(data, absPosition, endianness);
			absPosition += sizeof(float);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, float value) {
			new FastByte.Float(value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(float));
		}

		public void Put(double value) {
			new FastByte.Double(value).Write(data, absPosition, endianness);
			absPosition += sizeof(double);
			UpdateDataSize(absPosition);
		}
		public void Put(int offset, double value) {
			new FastByte.Double(value).Write(data, offset + absOffset, endianness);
			UpdateDataSize(offset + absOffset + sizeof(double));
		}

		public void Put(byte[] data, int offset, int length) {
			Buffer.BlockCopy(data, offset + absOffset, this.data, absPosition, length);
			absPosition += length;
			UpdateDataSize(absPosition);
		}
		public void Put(ByteBuffer data, int offset, int length) {
			Buffer.BlockCopy(data.data, data.absOffset + offset + absOffset, this.data, absPosition, length);
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

		public short GetShort(int offset) {
			return new FastByte.Short().Read(data, absOffset + offset, endianness);
		}
		public short GetShort() {
			short result = new FastByte.Short().Read(data, absPosition, endianness);
			absPosition += sizeof(short);
			return result;
		}

		public ushort GetUShort(int offset) {
			return (ushort) new FastByte.Short().Read(data, absOffset + offset, endianness);
		}
		public ushort GetUShort() {
			ushort v = (ushort) new FastByte.Short().Read(data, absPosition, endianness);
			absPosition += sizeof(ushort);
			return v;
		}

		public int GetInt(int offset) {
			return new FastByte.Int().Read(data, absOffset + offset, endianness);
		}
		public int GetInt() {
			int result = new FastByte.Int().Read(data, absPosition, endianness);
			absPosition += sizeof(int);
			return result;
		}

		public uint GetUInt(int offset) {
			return (uint) new FastByte.Int().Read(data, absOffset + offset, endianness);
		}
		public uint GetUInt() {
			uint result = (uint) new FastByte.Int().Read(data, absPosition, endianness);
			absPosition += sizeof(uint);
			return result;
		}

		public long GetLong(int offset) {
			return new FastByte.Long().Read(data, absOffset + offset, endianness);
		}
		public long GetLong() {
			long result = new FastByte.Long().Read(data, absPosition, endianness);
			absPosition += sizeof(long);
			return result;
		}

		public ulong GetULong(int offset) {
			return (ulong) new FastByte.Long().Read(data, absOffset + offset, endianness);
		}
		public ulong GetULong() {
			ulong result = (ulong) new FastByte.Long().Read(data, absPosition, endianness);
			absPosition += sizeof(ulong);
			return result;
		}

		public float GetFloat(int offset) {
			return new FastByte.Float().Read(data, absOffset + offset, endianness);
		}
		public float GetFloat() {
			float result = new FastByte.Float().Read(data, absPosition, endianness);
			absPosition += sizeof(float);
			return result;
		}

		public double GetDouble(int offset) {
			return new FastByte.Double().Read(data, absOffset + offset, endianness);
		}
		public double GetDouble() {
			double result = new FastByte.Double().Read(data, absPosition, endianness);
			absPosition += sizeof(double);
			return result;
		}

		public void GetBytes(int srcOffset, byte[] destination) {
			GetBytes(srcOffset, destination, destination.Length);
		}
		public void GetBytes(int srcOffset, byte[] destination, int lenght) {
			GetBytes(srcOffset, destination, 0, lenght);
		}
		public void GetBytes(int srcOffset, byte[] destination, int dstOffset, int lenght) {
			Buffer.BlockCopy(data, absOffset + srcOffset, destination, dstOffset, lenght);
		}
		public void GetBytes(byte[] destination) {
			GetBytes(destination, destination.Length);
		}
		public void GetBytes(byte[] destination, int lenght) {
			GetBytes(destination, 0, lenght);
		}
		public void GetBytes(byte[] destination, int offset, int lenght) {
			Buffer.BlockCopy(data, absPosition, destination, offset, lenght);
			absPosition += lenght;
		}
		#endregion
	}
}
