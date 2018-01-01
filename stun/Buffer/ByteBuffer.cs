using System;

namespace STUN.Utils {
	public struct ByteBuffer {
		private byte[] _data;

		/// <summary>
		/// Absolute
		/// </summary>
		public int positionAbsolute;
		/// <summary>
		/// Relative
		/// </summary>
		public int Position {
			get { return positionAbsolute - offset; }
			set { positionAbsolute = value + offset; }
		}

		/// <summary>
		/// Absolute
		/// </summary>
		public int offset;

		/// <summary>
		/// Absolute
		/// </summary>
		public int lengthAbsolute;
		/// <summary>
		/// Relative
		/// </summary>
		public int Length {
			get { return lengthAbsolute - offset; }
			set {
				lengthAbsolute = value + offset;
				if (positionAbsolute > lengthAbsolute) positionAbsolute = lengthAbsolute;
			}
		}

		public Endianness endianness;

		public ByteBuffer(byte[] buffer) {
			_data = buffer;
			positionAbsolute = 0;
			lengthAbsolute = buffer.Length;
			offset = 0;
			endianness = Endianness.Big;
		}

		public ByteBuffer(byte[] buffer, int offset, int length) {
			_data = buffer;
			positionAbsolute = offset;
			lengthAbsolute = offset + length;
			this.offset = offset;
			endianness = Endianness.Big;
		}

		public static ByteBuffer wrap(byte[] buffer, int offset, int length) {
			return new ByteBuffer(buffer, offset, length);
		}
		public static ByteBuffer wrap(byte[] buffer) {
			return new ByteBuffer(buffer);
		}
		public static ByteBuffer allocate(int length) {
			return new ByteBuffer(new byte[length], 0, length);
		}

		public byte[] Data {
			get { return _data; }
		}

		public byte this[int index] {
			get { return _data[offset + index]; }
			set { _data[offset + index] = value; }
		}

		public int AvailableBytes {
			get { return lengthAbsolute - positionAbsolute; }
		}

		public int remaining() {
			return lengthAbsolute - positionAbsolute;
		}
		public bool hasRemaining() {
			return lengthAbsolute > positionAbsolute;
		}

		public ByteBuffer slice() {
			ByteBuffer b = new ByteBuffer(_data);
			b.offset = b.positionAbsolute = positionAbsolute;
			b.lengthAbsolute = lengthAbsolute;
			return b;
		}
		public ByteBuffer flip() {
			lengthAbsolute = positionAbsolute;
			positionAbsolute = offset;
			return this;
		}

		public ByteBuffer GetCropToCurrentPosition() {
			ByteBuffer b = new ByteBuffer(_data);
			b.offset = b.positionAbsolute = offset;
			b.lengthAbsolute = positionAbsolute;
			return b;
		}

		public void rewind() {
			positionAbsolute = 0;
		}

		public byte[] ToArray() {
			byte[] copy = new byte[Length];
			Buffer.BlockCopy(_data, offset, copy, 0, Length);
			return copy;
		}

		public bool BufferEquals(ByteBuffer other) {
			if (Length != other.Length) return false;
			for (int i = 0; i < Length; i++)
				if (this[i] != other[i])
					return false;
			return true;
		}

		#region PutMethods
		void UpdateDataSize(int position) {
			if (position > Length) Length = position;
		}

		public void Put(float value) {
			new FastBit.Float(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 4;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, float value) {
			new FastBit.Float(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 4);
		}

		public void Put(double value) {
			new FastBit.Double(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 8;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, double value) {
			new FastBit.Double(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 8);
		}

		public void Put(long value) {
			new FastBit.Long(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 8;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, long value) {
			new FastBit.Long(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 8);
		}

		public void Put(ulong value) {
			new FastBit.Ulong(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 8;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, ulong value) {
			new FastBit.Ulong(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 8);
		}

		public void Put(int value) {
			new FastBit.Int(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 4;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, int value) {
			new FastBit.Int(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 4);
		}

		public void Put(uint value) {
			new FastBit.Uint(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 4;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, uint value) {
			new FastBit.Uint(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 4);
		}

		public void Put(ushort value) {
			new FastBit.Ushort(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 2;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, ushort value) {
			new FastBit.Ushort(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 2);
		}
		
		public void Put(short value) {
			new FastBit.Short(value).Write(_data, positionAbsolute, endianness);
			positionAbsolute += 2;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, short value) {
			new FastBit.Short(value).Write(_data, relativeOffset + offset, endianness);
			UpdateDataSize(relativeOffset + offset + 2);
		}
		
		public void Put(byte value) {
			_data[positionAbsolute] = value;
			positionAbsolute++;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(int relativeOffset, byte value) {
			_data[relativeOffset] = value;
			UpdateDataSize(relativeOffset + offset + 1);
		}

		public void Put(byte[] data, int offset, int length) {
			Buffer.BlockCopy(data, offset, _data, positionAbsolute, length);
			positionAbsolute += length;
			UpdateDataSize(positionAbsolute);
		}
		public void Put(byte[] data) {
			Put(data, 0, data.Length);
		}
		#endregion

		#region GetMethods
		public byte GetByte() {
			byte res = _data[positionAbsolute];
			positionAbsolute += 1;
			return res;
		}

		public ushort GetUShort(int offset) {
			return new FastBit.Ushort().Read(_data, this.offset + offset, endianness);
		}
		public ushort GetUShort() {
			ushort v = new FastBit.Ushort().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 2;
			return v;
		}

		public short GetShort(int offset) {
			return new FastBit.Short().Read(_data, this.offset + offset, endianness);
		}
		public short GetShort() {
			short result = new FastBit.Short().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 2;
			return result;
		}

		public long GetLong(int offset) {
			return new FastBit.Long().Read(_data, this.offset + offset, endianness);
		}
		public long GetLong() {
			long result = new FastBit.Long().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 8;
			return result;
		}

		public ulong GetULong(int offset) {
			return new FastBit.Ulong().Read(_data, this.offset + offset, endianness);
		}
		public ulong GetULong() {
			ulong result = new FastBit.Ulong().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 8;
			return result;
		}

		public int GetInt(int offset) {
			return new FastBit.Int().Read(_data, this.offset + offset, endianness);
		}
		public int GetInt() {
			int result = new FastBit.Int().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 4;
			return result;
		}

		public uint GetUInt(int offset) {
			return new FastBit.Uint().Read(_data, this.offset + offset, endianness);
		}
		public uint GetUInt() {
			uint result = new FastBit.Uint().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 4;
			return result;
		}

		public float GetFloat(int offset) {
			return new FastBit.Float().Read(_data, this.offset + offset, endianness);
		}
		public float GetFloat() {
			float result = new FastBit.Float().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 4;
			return result;
		}

		public double GetDouble(int offset) {
			return new FastBit.Double().Read(_data, this.offset + offset, endianness);
		}
		public double GetDouble() {
			double result = new FastBit.Double().Read(_data, positionAbsolute, endianness);
			positionAbsolute += 8;
			return result;
		}

		public void GetBytes(byte[] destination) {
			GetBytes(destination, destination.Length);
		}

		public void GetBytes(byte[] destination, int lenght) {
			GetBytes(destination, 0, lenght);
		}

		public void GetBytes(byte[] destination, int offset, int lenght) {
			Buffer.BlockCopy(_data, positionAbsolute, destination, offset, lenght);
			positionAbsolute += lenght;
		}
		#endregion
	}
}
