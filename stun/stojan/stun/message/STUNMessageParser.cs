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

namespace STUN.me.stojan.stun.message {
	/// <summary>
	/// Parses STUN messages.
	/// </summary>
	public class STUNMessageParser {

		private ByteBuffer inputStream;

		/// <summary>
		/// Create a parser from the input stream.
		/// </summary>
		/// <param name="inputStream">The input stream, must not be null</param>
		public STUNMessageParser(ByteBuffer inputStream) {
			this.inputStream = inputStream;
		}

		/// <summary>
		/// A STUN message header.
		/// </summary>
		public class Header {
			public ByteBuffer header;

			public static bool Create(ByteBuffer buffer, out Header header) {
				header = new Header();
				header.header = buffer;

				if (0 != STUNHeader.TwoStartingBits(buffer)) {
					Logger.Error("STUN header does not start with 0b00, first byte is [0x" + buffer[0].ToString("X") + "]");
					return false;
				}

				int length = header.Length();

				if (0 != length % 4) {
					Logger.Error("STUN header reports length [" + length + "] which is not a multiple of 4");
					return false;
				}
				return true;
			}

			/// <summary>
			/// Returns the message type.
			/// </summary>
			/// <returns>The message type</returns>
			public int MessageType() {
				return STUNHeader.Int16(header, 0);
			}

			/// <summary>
			/// Returns the length of the message.
			/// </summary>
			/// <returns>The message length</returns>
			public int Length() {
				return STUNHeader.Int16(header, 2);
			}

			/// <summary>
			/// Returns a copy of the magic cookie.
			/// </summary>
			/// <param name="cookie">The magic cookie</param>
			/// <returns>Successful</returns>
			public bool MagicCookie(out ByteBuffer cookie) {
				if (header.Length >= 8) {
					cookie = new ByteBuffer(header.Data, header.positionAbsolute + 4, 4);
					return true;
				} else {
					cookie = new ByteBuffer(null);
					return false;
				}
			}

			/// <summary>
			/// Returns a copy of the transcation-ID bytes.
			/// </summary>
			/// <returns>The transaction ID bytes</returns>
			public ByteBuffer Transaction() {
				return new ByteBuffer(header.Data, header.positionAbsolute + 8, 12);
			}

			/// <summary>
			/// Returns the STUN "class" of the message.
			/// </summary>
			/// <returns>The class</returns>
			public STUNClass Group() {
				return STUNHeader.Group(MessageType());
			}

			/// <summary>
			/// Returns the STUN "method" of the message.
			/// </summary>
			/// <returns>The method</returns>
			public STUNMethod Method() {
				return STUNHeader.Method(MessageType());
			}

			/// <summary>
			/// Checks if the {@link #magicCookie()} is valid.
			/// </summary>
			/// <returns>True if the magic cookie is valid, or false</returns>
			public bool IsMagicCookieValid() {
				ByteBuffer c;
				if (MagicCookie(out c)) {
					return c[0] == STUNHeader.MAGIC_COOKIE[0] &&
						c[1] == STUNHeader.MAGIC_COOKIE[1] &&
						c[2] == STUNHeader.MAGIC_COOKIE[2] &&
						c[3] == STUNHeader.MAGIC_COOKIE[3];
				} else {
					return false;
				}
			}

			/// <summary>
			/// Get the first STUN "type-length-value" from the message.
			/// </summary>
			/// <param name="parser"></param>
			/// <returns>The first TLV or null</returns>
			public TypeLengthValue Next(STUNMessageParser parser) {
				if (parser == null) return null;

				int length = Length();
				if (length <= 0) return null;

				TypeLengthValue tlv;
				return TypeLengthValue.Create(ref parser.inputStream, 0, length, out tlv) ? tlv : null;
			}
		}

		/// <summary>
		/// A STUN Type-Length-Value.
		/// </summary>
		public class TypeLengthValue {
			private int position;
			private int max;

			private ByteBuffer header;
			private ByteBuffer value;

			public static bool Create(ref ByteBuffer buffer, int position, int max, out TypeLengthValue tlv) {
				tlv = new TypeLengthValue();
				tlv.position = position;
				tlv.max = max;

				if (buffer.remaining() >= 4) {
					tlv.header = new ByteBuffer(buffer.Data, buffer.positionAbsolute, 4);
					buffer.positionAbsolute += 4;
				} else {
					tlv = null;
					Logger.Error("Could not read 4 bytes from stream for TLV header");
					return false;
				}
				
				int length = tlv.Length();

				if (buffer.remaining() >= length) {
					tlv.value = new ByteBuffer(buffer.Data, buffer.positionAbsolute, length);
					buffer.positionAbsolute += length;
				} else {
					tlv = null;
					Logger.Error("Could not read " + length + " bytes from stream for TLV value");
					return false;
				}

				int padding = tlv.Padding();

				if (0 != padding) {
					if (buffer.remaining() >= padding) {
						buffer.positionAbsolute += padding;
					} else {
						tlv = null;
						Logger.Error("Could not skip " + padding + " bytes from stream for TLV padding");
						return false;
					}
				}
				return true;
			}

			/// <summary>
			/// Returns the STUN TLV "type".
			/// </summary>
			/// <returns> The type</returns>
			public int Type() {
				return STUNHeader.Int16(header, 0);
			}

			/// <summary>
			/// Returns the STUN TLV "length".
			/// </summary>
			/// <returns>The length</returns>
			public int Length() {
				return STUNHeader.Int16(header, 2);
			}

			/// <summary>
			/// Returns the STUN TLV "padding" size.
			/// </summary>
			/// <returns>The padding size</returns>
			public int Padding() {
				int length = Length();

				if (0 == length % 4) {
					return 0;
				}

				return 4 - (length % 4);
			}

			/// <summary>
			/// Returns a copy of the TLV header bytes.
			/// </summary>
			/// <returns>The header bytes</returns>
			public ByteBuffer Header() {
				return new ByteBuffer(header.Data, header.positionAbsolute, 4);
			}

			/// <summary>
			/// Returns a copy of the TLV value bytes, without padding.
			/// </summary>
			/// <returns>The value bytes, without padding</returns>
			public ByteBuffer Value() {
				return new ByteBuffer(value.Data, value.positionAbsolute, value.Length);
			}

			/// <summary>
			/// Get the next STUN TLV in the sequence, or null.
			/// </summary>
			/// <param name="parser">The next TLV or null if at the end</param>
			/// <returns>Successful</returns>
			public TypeLengthValue Next(STUNMessageParser parser) {
				if (parser == null) return null;

				int nextPosition = position + header.Length + value.Length + Padding();
				if (nextPosition >= max) return null;
				
				TypeLengthValue tlv;
				return Create(ref parser.inputStream, nextPosition, max, out tlv) ? tlv : null;
			}
		}

		/// <summary>
		/// Start parsing the STUN message.
		/// </summary>
		/// <param name="header">The header of the STUN message</param>
		/// <returns>Successful</returns>
		public bool Start(out Header header) {
			if (inputStream.remaining() >= 20) {
				bool toReturn = Header.Create(ByteBuffer.wrap(inputStream.Data, inputStream.positionAbsolute, 20), out header);
				inputStream.positionAbsolute += 20;
				return toReturn;
			} else {
				header = null;
				Logger.Error("Could not read 20 byte STUN header from stream");
				return false;
			}
		}
	}
}
