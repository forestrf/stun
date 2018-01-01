using System;

namespace STUN.Utils {
	public struct ByteBuffer {

		public byte[] data;

		/// <summary>
		/// Position of the read/write cursor in <see cref="data"/>
		/// </summary>
		public int absPosition;

		/// <summary>
		/// Position of the 0-index
		/// </summary>
		public int absOffset;

		/// <summary>
		/// usable bits in <see cref="data"/> from 0
		/// </summary>
		public int absLength;



		public ByteBuffer(byte[] buffer) : this(buffer, 0, null != buffer ? buffer.Length : 0) { }
		public ByteBuffer(byte[] buffer, int offset, int length) {
			data = buffer;
			absPosition = offset;
			absLength = offset + length;
			this.absOffset = offset;
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

		public void Rewind() {
			absPosition = 0;
		}

		public byte[] ToArray() {
			byte[] copy = new byte[Length];
			Buffer.BlockCopy(data, absOffset, copy, 0, Length);
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

		public void Put(float value) {
			new FastBit.Float(value).Write(data, absPosition, Endianness.Big);
			absPosition += 4;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, float value) {
			new FastBit.Float(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 4);
		}

		public void Put(double value) {
			new FastBit.Double(value).Write(data, absPosition, Endianness.Big);
			absPosition += 8;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, double value) {
			new FastBit.Double(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 8);
		}

		public void Put(long value) {
			new FastBit.Long(value).Write(data, absPosition, Endianness.Big);
			absPosition += 8;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, long value) {
			new FastBit.Long(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 8);
		}

		public void Put(ulong value) {
			new FastBit.Ulong(value).Write(data, absPosition, Endianness.Big);
			absPosition += 8;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, ulong value) {
			new FastBit.Ulong(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 8);
		}

		public void Put(int value) {
			new FastBit.Int(value).Write(data, absPosition, Endianness.Big);
			absPosition += 4;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, int value) {
			new FastBit.Int(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 4);
		}

		public void Put(uint value) {
			new FastBit.Uint(value).Write(data, absPosition, Endianness.Big);
			absPosition += 4;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, uint value) {
			new FastBit.Uint(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 4);
		}

		public void Put(ushort value) {
			new FastBit.Ushort(value).Write(data, absPosition, Endianness.Big);
			absPosition += 2;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, ushort value) {
			new FastBit.Ushort(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 2);
		}
		
		public void Put(short value) {
			new FastBit.Short(value).Write(data, absPosition, Endianness.Big);
			absPosition += 2;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, short value) {
			new FastBit.Short(value).Write(data, relativeOffset + absOffset, Endianness.Big);
			UpdateDataSize(relativeOffset + absOffset + 2);
		}
		
		public void Put(byte value) {
			data[absPosition] = value;
			absPosition++;
			UpdateDataSize(absPosition);
		}
		public void Put(int relativeOffset, byte value) {
			data[relativeOffset] = value;
			UpdateDataSize(relativeOffset + absOffset + 1);
		}

		public void Put(byte[] data, int offset, int length) {
			Buffer.BlockCopy(data, offset, this.data, absPosition, length);
			absPosition += length;
			UpdateDataSize(absPosition);
		}
		public void Put(ByteBuffer data, int offset, int length) {
			Buffer.BlockCopy(data.data, data.absOffset + offset, this.data, absPosition, length);
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
			absPosition += 1;
			return res;
		}

		public ushort GetUShort(int offset) {
			return new FastBit.Ushort().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public ushort GetUShort() {
			ushort v = new FastBit.Ushort().Read(data, absPosition, Endianness.Big);
			absPosition += 2;
			return v;
		}

		public short GetShort(int offset) {
			return new FastBit.Short().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public short GetShort() {
			short result = new FastBit.Short().Read(data, absPosition, Endianness.Big);
			absPosition += 2;
			return result;
		}

		public long GetLong(int offset) {
			return new FastBit.Long().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public long GetLong() {
			long result = new FastBit.Long().Read(data, absPosition, Endianness.Big);
			absPosition += 8;
			return result;
		}

		public ulong GetULong(int offset) {
			return new FastBit.Ulong().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public ulong GetULong() {
			ulong result = new FastBit.Ulong().Read(data, absPosition, Endianness.Big);
			absPosition += 8;
			return result;
		}

		public int GetInt(int offset) {
			return new FastBit.Int().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public int GetInt() {
			int result = new FastBit.Int().Read(data, absPosition, Endianness.Big);
			absPosition += 4;
			return result;
		}

		public uint GetUInt(int offset) {
			return new FastBit.Uint().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public uint GetUInt() {
			uint result = new FastBit.Uint().Read(data, absPosition, Endianness.Big);
			absPosition += 4;
			return result;
		}

		public float GetFloat(int offset) {
			return new FastBit.Float().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public float GetFloat() {
			float result = new FastBit.Float().Read(data, absPosition, Endianness.Big);
			absPosition += 4;
			return result;
		}

		public double GetDouble(int offset) {
			return new FastBit.Double().Read(data, this.absOffset + offset, Endianness.Big);
		}
		public double GetDouble() {
			double result = new FastBit.Double().Read(data, absPosition, Endianness.Big);
			absPosition += 8;
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
