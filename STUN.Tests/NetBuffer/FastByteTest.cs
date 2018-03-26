using NUnit.Framework;
using STUN.NetBuffer;
using System;
using System.Collections.Generic;

namespace STUN.Tests.NetBuffer {
	[TestFixture]
	public class FastByteTest {
		[Test]
		public void DoubleTest() {
			Random r = new Random(0);
			List<double> numbers = new List<double>(new double[] { 0, 1, -1, double.MinValue, double.MaxValue });
			for (int i = 0; i < 100; i++) numbers.Add(double.MaxValue * (r.NextDouble() * 2 - 1));

			byte[] tmp = new byte[sizeof(double) + 2];
			foreach (var n in numbers) {
				for (int off = 0; off < 2; off++) {
					new FastByte.Double(n).Write(tmp, off, Endianness.Big);
					Assert.AreEqual(n, new FastByte.Double().Read(tmp, off, Endianness.Big));

					new FastByte.Double(n).Write(tmp, off, Endianness.Little);
					Assert.AreEqual(n, new FastByte.Double().Read(tmp, off, Endianness.Little));
				}

				Assert.AreEqual(n, new FastByte.Double().Read(GetBigEndian(n), 0, Endianness.Big));
				Assert.AreEqual(n, new FastByte.Double().Read(GetLittleEndian(n), 0, Endianness.Little));
			}
		}

		[Test]
		public void FloatTest() {
			Random r = new Random(0);
			List<float> numbers = new List<float>(new float[] { 0, 1, -1, float.MinValue, float.MaxValue });
			for (int i = 0; i < 100; i++) numbers.Add(float.MaxValue * (float) (r.NextDouble() * 2 - 1));

			byte[] tmp = new byte[sizeof(float) + 2];
			foreach (var n in numbers) {
				for (int off = 0; off < 2; off++) {
					new FastByte.Float(n).Write(tmp, off, Endianness.Big);
					Assert.AreEqual(n, new FastByte.Float().Read(tmp, off, Endianness.Big));

					new FastByte.Float(n).Write(tmp, off, Endianness.Little);
					Assert.AreEqual(n, new FastByte.Float().Read(tmp, off, Endianness.Little));
				}

				Assert.AreEqual(n, new FastByte.Float().Read(GetBigEndian(n), 0, Endianness.Big));
				Assert.AreEqual(n, new FastByte.Float().Read(GetLittleEndian(n), 0, Endianness.Little));
			}
		}

		[Test]
		public void LongTest() {
			Random r = new Random(0);
			List<long> numbers = new List<long>(new long[] { 0, 1, -1, long.MinValue, long.MaxValue });
			for (int i = 0; i < 100; i++) numbers.Add(long.MaxValue * (long) (r.NextDouble() * 2 - 1));

			foreach (var n in numbers) {
				byte[] tmp = new byte[sizeof(long) + 2];
				for (int off = 0; off < 2; off++) {
					new FastByte.Long(n).Write(tmp, off, Endianness.Big);
					Assert.AreEqual(n, new FastByte.Long().Read(tmp, off, Endianness.Big));

					new FastByte.Long(n).Write(tmp, off, Endianness.Little);
					Assert.AreEqual(n, new FastByte.Long().Read(tmp, off, Endianness.Little));
				}

				Assert.AreEqual(n, new FastByte.Long().Read(GetBigEndian(n), 0, Endianness.Big));
				Assert.AreEqual(n, new FastByte.Long().Read(GetLittleEndian(n), 0, Endianness.Little));
			}
		}
		
		[Test]
		public void IntTest() {
			Random r = new Random(0);
			List<int> numbers = new List<int>(new int[] { 0, 1, -1, int.MinValue, int.MaxValue });
			for (int i = 0; i < 100; i++) numbers.Add(int.MaxValue * (int) (r.NextDouble() * 2 - 1));

			foreach (var n in numbers) {
				byte[] tmp = new byte[sizeof(int) + 2];
				for (int off = 0; off < 2; off++) {
					new FastByte.Int(n).Write(tmp, off, Endianness.Big);
					Assert.AreEqual(n, new FastByte.Int().Read(tmp, off, Endianness.Big));

					new FastByte.Int(n).Write(tmp, off, Endianness.Little);
					Assert.AreEqual(n, new FastByte.Int().Read(tmp, off, Endianness.Little));
				}

				Assert.AreEqual(n, new FastByte.Int().Read(GetBigEndian(n), 0, Endianness.Big));
				Assert.AreEqual(n, new FastByte.Int().Read(GetLittleEndian(n), 0, Endianness.Little));
			}
		}

		[Test]
		public void ShortTest() {
			Random r = new Random(0);
			List<short> numbers = new List<short>(new short[] { 0, 1, -1, short.MinValue, short.MaxValue });
			for (int i = 0; i < 100; i++) numbers.Add((short) (short.MaxValue * (short) (r.NextDouble() * 2 - 1)));

			foreach (var n in numbers) {
				byte[] tmp = new byte[sizeof(short) + 2];
				for (int off = 0; off < 2; off++) {
					new FastByte.Short(n).Write(tmp, off, Endianness.Big);
					Assert.AreEqual(n, new FastByte.Short().Read(tmp, off, Endianness.Big));

					new FastByte.Short(n).Write(tmp, off, Endianness.Little);
					Assert.AreEqual(n, new FastByte.Short().Read(tmp, off, Endianness.Little));
				}

				Assert.AreEqual(n, new FastByte.Short().Read(GetBigEndian(n), 0, Endianness.Big));
				Assert.AreEqual(n, new FastByte.Short().Read(GetLittleEndian(n), 0, Endianness.Little));
			}
		}



		static byte[] GetBigEndian(dynamic n) {
			var res = BitConverter.GetBytes(n);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(res);
			return res;
		}
		static byte[] GetLittleEndian(dynamic n) {
			var res = GetBigEndian(n);
			Array.Reverse(res);
			return res;
		}
	}
}
