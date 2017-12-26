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

using Ashkatchap.Utils;
using STUN;
using System;

namespace me.stojan.stun.message {
	/**
	 * Parses STUN messages.
	 */
	public class STUNMessageParser {

		private ByteBuffer inputStream;

		/**
		 * Create a parser from the input stream.
		 * @param inputStream the input stream, must not be null
		 */
		public STUNMessageParser(ByteBuffer inputStream) {
			this.inputStream = inputStream;
		}

		/**
		 * A STUN message header.
		 */
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

			/**
			 * Returns the message type.
			 * @return the message type
			 */
			public int MessageType() {
				return STUNHeader.Int16(header, 0);
			}

			/**
			 * Returns the length of the message.
			 * @return the message length
			 */
			public int Length() {
				return STUNHeader.Int16(header, 2);
			}

			/**
			 * Returns a copy of the magic cookie.
			 * @return the magic cookie
			 */
			public bool MagicCookie(out FastBit.Uint cookie) {
				if (header.Length >= 8) {
					cookie = new FastBit.Uint(header[4], header[5], header[6], header[7]);
					return true;
				} else {
					cookie = new FastBit.Uint();
					return false;
				}
			}

			/**
			 * Returns a copy of the transcation-ID bytes.
			 * @return the transaction ID bytes
			 */
			public ByteBuffer Transaction() {
				return new ByteBuffer(header.Data, header.positionAbsolute + 8, 12);
			}

			/**
			 * Returns the STUN "class" of the message.
			 * @return the class
			 */
			public STUNMessageType Group() {
				return STUNHeader.Group(MessageType());
			}

			/**
			 * Returns the STUN "method" of the message.
			 * @return the method
			 */
			public STUNMessageType Method() {
				return STUNHeader.Method(MessageType());
			}

			/**
			 * Checks if the {@link #magicCookie()} is valid.
			 * @return true if the magic cookie is valid, or false
			 */
			public bool IsMagicCookieValid() {
				FastBit.Uint c;
				if (MagicCookie(out c)) {
					return c.b0 == STUNHeader.MAGIC_COOKIE[0] &&
						c.b1 == STUNHeader.MAGIC_COOKIE[1] &&
						c.b2 == STUNHeader.MAGIC_COOKIE[2] &&
						c.b3 == STUNHeader.MAGIC_COOKIE[3];
				} else {
					return false;
				}
			}

			/**
			 * Get the first STUN "type-length-value" from the message.
			 * @return the first TLV or null
			 * @throws IOException an IO exception from the stream
			 * @throws InvalidSTUNMessageException if the message is invalid
			 */
			public TypeLengthValue Next(STUNMessageParser parser) {
				if (parser == null) return null;

				int length = Length();
				if (length <= 0) return null;

				TypeLengthValue tlv;
				return TypeLengthValue.Create(ref parser.inputStream, 0, length, out tlv) ? tlv : null;
			}
		}

		/**
		 * A STUN Type-Length-Value.
		 */
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

			/**
			 * Returns the STUN TLV "type."
			 * @return the type
			 */
			public int Type() {
				return STUNHeader.Int16(header, 0);
			}

			/**
			 * Returns the STUN TLV "length."
			 * @return the length
			 */
			public int Length() {
				return STUNHeader.Int16(header, 2);
			}

			/**
			 * Returns the STUN TLV "padding" size.
			 * @return the padding size
			 */
			public int Padding() {
				int length = Length();

				if (0 == length % 4) {
					return 0;
				}

				return 4 - (length % 4);
			}

			/**
			 * Returns a copy of the TLV header bytes.
			 * @return the header bytes
			 */
			public ByteBuffer Header() {
				return new ByteBuffer(header.Data, header.positionAbsolute, 4);
			}

			/**
			 * Returns a copy of the TLV value bytes, without padding.
			 * @return the value bytes, without padding
			 */
			public ByteBuffer Value() {
				return new ByteBuffer(value.Data, value.positionAbsolute, value.Length);
			}

			/**
			 * Get the next STUN TLV in the sequence, or null.
			 * @return the next TLV or null if at the end
			 * @throws IOException an IO exception from the stream
			 * @throws InvalidSTUNMessageException if the next TLV is invalid
			 */
			public TypeLengthValue Next(STUNMessageParser parser) {
				if (parser == null) return null;

				int nextPosition = position + header.Length + value.Length + Padding();
				if (nextPosition >= max) return null;
				
				TypeLengthValue tlv;
				return Create(ref parser.inputStream, nextPosition, max, out tlv) ? tlv : null;
			}
		}

		/**
		 * Start parsing the STUN message.
		 * @return the header of the STUN message
		 * @throws IOException an IO exception from the stream
		 * @throws InvalidSTUNMessageException if could not read the 20-byte STUN header
		 */
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
