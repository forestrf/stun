// https://code.google.com/archive/p/wit-incontrol/
// MIT License
// Bobby Cafazzo, Michael Muesch, Jason Torosian, Cory Zinkiewicz

using STUN.Utils;
using System;

namespace STUN.Crypto {
	public class SHA {
		// Number used in SHA256 hash function
		static readonly uint[] sha256_k = new uint[]
		{
			0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
			0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
			0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
			0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
			0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
			0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
			0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
			0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
		};

		// Rotate bits left
		static uint rotateleft(uint x, int n) {
			return ((x << n) | (x >> (32 - n)));
		}

		// Rotate bits right
		static uint rotateright(uint x, int n) {
			return ((x >> n) | (x << (32 - n)));
		}

		// Convert 4 bytes to big endian uint32
		static uint big_endian_from_bytes(byte[] input, uint offset) {
			uint r = 0;
			r |= (((uint) input[offset]) << 24);
			r |= (((uint) input[offset + 1]) << 16);
			r |= (((uint) input[offset + 2]) << 8);
			r |= (((uint) input[offset + 3]));
			return r;
		}

		// Convert big endian uint32 to bytes
		static void bytes_from_big_endian(uint input, ref byte[] output, int offset) {
			output[offset] = (byte) ((input & 0xFF000000) >> 24);
			output[offset + 1] = (byte) ((input & 0x00FF0000) >> 16);
			output[offset + 2] = (byte) ((input & 0x0000FF00) >> 8);
			output[offset + 3] = (byte) ((input & 0x000000FF));
		}

		// SHA-224/SHA-256 choice function
		static uint choice(uint x, uint y, uint z) {
			return ((x & y) ^ (~x & z));
		}

		// SHA-224/SHA-256 majority function
		static uint majority(uint x, uint y, uint z) {
			return ((x & y) ^ (x & z) ^ (y & z));
		}


		public static void computeHMAC_SHA1(byte[] secret, ByteBuffer data, ByteBuffer output) {
			byte[] bi = null;
			byte[] bo = null;
			byte[] processed = null;
			uint[] wordblock = null;
			computeHMAC_SHA1(secret, data, output, ref bi, ref bo, ref processed, ref wordblock);
		}

		/// <summary>
		/// Compute HMAC SHA-1
		/// </summary>
		/// <param name="secret">Secret</param>
		/// <param name="data">Password</param>
		/// <returns>20 byte HMAC_SHA1</returns>
		public static void computeHMAC_SHA1(byte[] secret, ByteBuffer data, ByteBuffer output, ref byte[] bi, ref byte[] bo, ref byte[] processed, ref uint[] wordblock) {
			// Create two arrays, bi and bo
			if (null == bi || bi.Length != 64 + data.Length) bi = new byte[64 + data.Length];
			if (null == bo || bo.Length != 64 + 20) bo = new byte[64 + 20];

			// Copy secret to both arrays
			Array.Copy(secret, bi, secret.Length);
			Array.Copy(secret, bo, secret.Length);

			for (int i = 0; i < 64; i++) {
				// Xor bi with 0x36
				bi[i] = (byte) (bi[i] ^ 0x36);

				// Xor bo with 0x5c
				bo[i] = (byte) (bo[i] ^ 0x5c);
			}

			// Append value to bi
			Array.Copy(data.data, data.absOffset, bi, 64, data.Length);

			// Append SHA1(bi) to bo
			computeSHA1(bi, output.data, output.absOffset, ref processed, ref wordblock);
			Array.Copy(output.data, output.absOffset, bo, 64, 20);

			// Return SHA1(bo)
			computeSHA1(bo, output.data, output.absOffset, ref processed, ref wordblock);
		}


		public static void computeHMAC_SHA256(byte[] secret, ByteBuffer data, ByteBuffer output) {
			byte[] bi = null;
			byte[] bo = null;
			byte[] processed = null;
			uint[] wordblock = null;
			computeHMAC_SHA256(secret, data, output, ref bi, ref bo, ref processed, ref wordblock);
		}

		/// <summary>
		/// Compute HMAC SHA-256
		/// </summary>
		/// <param name="secret">Secret</param>
		/// <param name="value">Password</param>
		/// <returns>32 byte HMAC_SHA256</returns>
		public static void computeHMAC_SHA256(byte[] secret, ByteBuffer data, ByteBuffer output, ref byte[] bi, ref byte[] bo, ref byte[] processed, ref uint[] wordblock) {
			// Create two arrays, bi and bo
			if (null == bi || bi.Length != 64 + data.Length) bi = new byte[64 + data.Length];
			if (null == bo || bo.Length != 64 + 20) bo = new byte[64 + 32];

			// Copy secret to both arrays
			Array.Copy(secret, bi, secret.Length);
			Array.Copy(secret, bo, secret.Length);

			for (int i = 0; i < 64; i++) {
				// Xor bi with 0x36
				bi[i] = (byte) (bi[i] ^ 0x36);

				// Xor bo with 0x5c
				bo[i] = (byte) (bo[i] ^ 0x5c);
			}

			// Append value to bi
			Array.Copy(data.data, data.absOffset, bi, 64, data.Length);

			// Append SHA256(bi) to bo
			computeSHA256(bi, output.data, output.absOffset, ref processed, ref wordblock);
			Array.Copy(output.data, output.absOffset, bo, 64, 20);

			// Return SHA256(bo)
			computeSHA256(bo, output.data, output.absOffset, ref processed, ref wordblock);
		}

		/// <summary>
		/// Compute SHA1 digest
		/// </summary>
		/// <param name="input">Input byte array</param>
		/// <returns>20 byte SHA1 of input</returns>
		public static void computeSHA1(byte[] input, byte[] output20, int outputOffset, ref byte[] processed, ref uint[] wordblock) {
			// Initialize working parameters
			uint a, b, c, d, e, i, temp;
			uint h0 = 0x67452301;
			uint h1 = 0xEFCDAB89;
			uint h2 = 0x98BADCFE;
			uint h3 = 0x10325476;
			uint h4 = 0xC3D2E1F0;
			uint blockstart = 0;

			// Calculate how long the padded message should be
			int newinputlength = input.Length + 1;
			while ((newinputlength % 64) != 56) // length mod 512bits = 448bits
			{
				newinputlength++;
			}

			// Create array for padded data
			int processedLength = newinputlength + 8;
			if (null == processed || processed.Length < processedLength)
				processed = new byte[processedLength];
			else
				Array.Clear(processed, input.Length, processedLength - input.Length);
			Array.Copy(input, processed, input.Length);

			// Pad data with an 1
			processed[input.Length] = 0x80;

			// Pad data with big endian 64bit length of message
			// We do only 32 bits becouse input.length is 32 bit
			bytes_from_big_endian((uint) input.Length * 8, ref processed, processedLength - 4);

			// Block of 32 bits values used in calculations
			if (null == wordblock || wordblock.Length < 80)
				wordblock = new uint[80];

			// Now process each 512 bit block
			while (blockstart < processedLength) {
				// break chunk into sixteen 32-bit big-endian words 
				for (i = 0; i < 16; i++)
					wordblock[i] = big_endian_from_bytes(processed, blockstart + (i * 4));

				// Extend the sixteen 32-bit words into eighty 32-bit words:
				for (i = 16; i < 80; i++)
					wordblock[i] = rotateleft(wordblock[i - 3] ^ wordblock[i - 8] ^ wordblock[i - 14] ^ wordblock[i - 16], 1);


				// Initialize hash value for this chunk
				a = h0;
				b = h1;
				c = h2;
				d = h3;
				e = h4;

				// Main loop
				for (i = 0; i < 80; i++) {
					// Perform function dependend of word number
					if (i <= 19)
						temp = (rotateleft(a, 5) + ((b & c) | (~b & d)) + e + wordblock[i] + 0x5A827999);
					else if (i <= 39)
						temp = (rotateleft(a, 5) + (b ^ c ^ d) + e + wordblock[i] + 0x6ED9EBA1);
					else if (i <= 59)
						temp = (rotateleft(a, 5) + ((b & c) | (b & d) | (c & d)) + e + wordblock[i] + 0x8F1BBCDC);
					else
						temp = (rotateleft(a, 5) + (b ^ c ^ d) + e + wordblock[i] + 0xCA62C1D6);

					// Perform standard function
					e = d;
					d = c;
					c = rotateleft(b, 30);
					b = a;
					a = temp;
				}

				// Add this chunk's hash to result so far
				h0 += a;
				h1 += b;
				h2 += c;
				h3 += d;
				h4 += e;

				// Next 512 bit block
				blockstart += 64;
			}

			// Prepare output
			bytes_from_big_endian(h0, ref output20, outputOffset + 0);
			bytes_from_big_endian(h1, ref output20, outputOffset + 4);
			bytes_from_big_endian(h2, ref output20, outputOffset + 8);
			bytes_from_big_endian(h3, ref output20, outputOffset + 12);
			bytes_from_big_endian(h4, ref output20, outputOffset + 16);
		}
		
		/// <summary>
		/// Compute SHA-256 digest
		/// </summary>
		/// <param name="input">Input array</param>
		public static void computeSHA256(byte[] input, byte[] output32, int outputOffset, ref byte[] processed, ref uint[] wordblock) {
			// Initialize working parameters
			uint a, b, c, d, e, f, g, h, i, s0, s1, t1, t2;
			uint h0 = 0x6a09e667;
			uint h1 = 0xbb67ae85;
			uint h2 = 0x3c6ef372;
			uint h3 = 0xa54ff53a;
			uint h4 = 0x510e527f;
			uint h5 = 0x9b05688c;
			uint h6 = 0x1f83d9ab;
			uint h7 = 0x5be0cd19;
			uint blockstart = 0;

			// Calculate how long the padded message should be
			int newinputlength = input.Length + 1;
			while ((newinputlength % 64) != 56) // length mod 512bits = 448bits
			{
				newinputlength++;
			}

			// Create array for padded data
			int processedLength = newinputlength + 8;
			if (null == processed || processed.Length < processedLength)
				processed = new byte[processedLength];
			else
				Array.Clear(processed, input.Length, processedLength - input.Length);
			Array.Copy(input, processed, input.Length);

			// Pad data with an 1
			processed[input.Length] = 0x80;

			// Pad data with big endian 64bit length of message
			// We do only 32 bits becouse input.length is 32 bit
			processed[processedLength - 4] = (byte) (((input.Length * 8) & 0xFF000000) >> 24);
			processed[processedLength - 3] = (byte) (((input.Length * 8) & 0x00FF0000) >> 16);
			processed[processedLength - 2] = (byte) (((input.Length * 8) & 0x0000FF00) >> 8);
			processed[processedLength - 1] = (byte) (((input.Length * 8) & 0x000000FF));

			// Block of 32 bits values used in calculations
			if (null == wordblock || wordblock.Length < 64)
				wordblock = new uint[64];

			// Now process each 512 bit block
			while (blockstart < processedLength) {
				// break chunk into sixteen 32-bit big-endian words 
				for (i = 0; i < 16; i++)
					wordblock[i] = big_endian_from_bytes(processed, blockstart + (i * 4));

				// Extend the sixteen 32-bit words into sixty-four 32-bit words:
				for (i = 16; i < 64; i++) {
					s0 = rotateright(wordblock[i - 15], 7) ^ rotateright(wordblock[i - 15], 18) ^ (wordblock[i - 15] >> 3);
					s1 = rotateright(wordblock[i - 2], 17) ^ rotateright(wordblock[i - 2], 19) ^ (wordblock[i - 2] >> 10);
					wordblock[i] = wordblock[i - 16] + s0 + wordblock[i - 7] + s1;
				}

				// Initialize hash value for this chunk:
				a = h0;
				b = h1;
				c = h2;
				d = h3;
				e = h4;
				f = h5;
				g = h6;
				h = h7;

				// Main loop
				for (i = 0; i < 64; i++) {
					t1 = h + (rotateright(e, 6) ^ rotateright(e, 11) ^ rotateright(e, 25)) + choice(e, f, g) + sha256_k[i] + wordblock[i];
					t2 = (rotateright(a, 2) ^ rotateright(a, 13) ^ rotateright(a, 22)) + majority(a, b, c);
					h = g;
					g = f;
					f = e;
					e = d + t1;
					d = c;
					c = b;
					b = a;
					a = t1 + t2;
				}

				// Add this chunk's hash to result so far
				h0 += a;
				h1 += b;
				h2 += c;
				h3 += d;
				h4 += e;
				h5 += f;
				h6 += g;
				h7 += h;

				// Process next 512bit block
				blockstart += 64;
			}

			// Prepare output
			bytes_from_big_endian(h0, ref output32, outputOffset + 0);
			bytes_from_big_endian(h1, ref output32, outputOffset + 4);
			bytes_from_big_endian(h2, ref output32, outputOffset + 8);
			bytes_from_big_endian(h3, ref output32, outputOffset + 12);
			bytes_from_big_endian(h4, ref output32, outputOffset + 16);
			bytes_from_big_endian(h5, ref output32, outputOffset + 20);
			bytes_from_big_endian(h6, ref output32, outputOffset + 24);
			bytes_from_big_endian(h7, ref output32, outputOffset + 28);
		}
	}
}
