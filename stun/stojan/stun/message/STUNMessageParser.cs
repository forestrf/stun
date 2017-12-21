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

using System;

namespace me.stojan.stun.message {
	/**
	 * Parses STUN messages.
	 */
	public class STUNMessageParser {

		private InputStream inputStream;

		/**
		 * Create a parser from the input stream.
		 * @param inputStream the input stream, must not be null
		 */
		public STUNMessageParser(InputStream inputStream) {
			this.inputStream = inputStream;
		}

		/**
		 * A STUN message header.
		 */
		public class Header {
			private byte[] header;

			private Header(byte[] header) {
				this.header = header;

				if (0 != STUNHeader.TwoStartingBits(header)) {
					throw new InvalidSTUNMessageException(String.format((Locale) null, "STUN header does not start with 0b00, first byte is [0x%x]", header[0]));
				}

				int length = length();

				if (0 != length % 4) {
					throw new InvalidSTUNMessageException(String.format((Locale) null, "STUN header reports length [%d] which is not a multiple of 4", length));
				}
			}

			/**
			 * Returns a copy of the raw header.
			 * @return the raw bytes of the header
			 */
			public byte[] raw() {
				return Arrays.copyOf(header, header.length);
			}

			/**
			 * Returns the message type.
			 * @return the message type
			 */
			public int messageType() {
				return STUNHeader.Int16(header, 0);
			}

			/**
			 * Returns the length of the message.
			 * @return the message length
			 */
			public int length() {
				return STUNHeader.Int16(header, 2);
			}

			/**
			 * Returns a copy of the magic cookie.
			 * @return the magic cookie
			 */
			public byte[] magicCookie() {
				byte[] cookie = new byte[4];

				Array.Copy(header, 4, cookie, 0, 4);

				return cookie;
			}

			/**
			 * Returns a copy of the transcation-ID bytes.
			 * @return the transaction ID bytes
			 */
			public byte[] transaction() {
				byte[] transaction = new byte[12];

				Array.Copy(header, 8, transaction, 0, 12);

				return transaction;
			}

			/**
			 * Returns the STUN "class" of the message.
			 * @return the class
			 */
			public int group() {
				return STUNHeader.Group(messageType());
			}

			/**
			 * Returns the STUN "method" of the message.
			 * @return the method
			 */
			public int method() {
				return STUNHeader.Method(messageType());
			}

			/**
			 * Checks if the {@link #magicCookie()} is valid.
			 * @return true if the magic cookie is valid, or false
			 */
			public bool isMagicCookieValid() {
				return Arrays.equals(STUNHeader.MAGIC_COOKIE, magicCookie());
			}

			/**
			 * Get the first STUN "type-length-value" from the message.
			 * @return the first TLV or null
			 * @throws IOException an IO exception from the stream
			 * @throws InvalidSTUNMessageException if the message is invalid
			 */
			public TypeLengthValue next() throws IOException, InvalidSTUNMessageException {
				final int length = length();

				if (length <= 0) {
					return null;
				}

				return new TypeLengthValue(0, length);
			}
		}

		/**
		 * A STUN Type-Length-Value.
		 */
		public class TypeLengthValue {
			private final int position;
			private final int max;

			private final byte[] header = new byte[4];
			private final byte[] value;

			private TypeLengthValue(int position, int max) throws InvalidSTUNMessageException, IOException {
				this.position = position;
				this.max = max;

				if (4 != inputStream.read(header, 0, 4)) {
					throw new InvalidSTUNMessageException("Could not read 4 bytes from stream for TLV header");
				}

				final int length = length();
				value = new byte[length];

				if (length != inputStream.read(value)) {
					throw new InvalidSTUNMessageException(String.format((Locale) null, "Could not read %d bytes from stream for TLV value", length));
				}

				final int padding = padding();

				if (0 != padding) {
					if (padding != inputStream.skip(padding)) {
						throw new InvalidSTUNMessageException(String.format((Locale) null, "Could not skip %d bytes from stream for TLV padding", padding));
					}
				}
			}

			/**
			 * Returns the STUN TLV "type."
			 * @return the type
			 */
			public int type() {
				return STUNHeader.int16(header, 0);
			}

			/**
			 * Returns the STUN TLV "length."
			 * @return the length
			 */
			public int length() {
				return STUNHeader.int16(header, 2);
			}

			/**
			 * Returns the STUN TLV "padding" size.
			 * @return the padding size
			 */
			public int padding() {
				final int length = length();

				if (0 == length % 4) {
					return 0;
				}

				return 4 - (length % 4);
			}

			/**
			 * Returns a copy of the TLV header bytes.
			 * @return the header bytes
			 */
			public byte[] header() {
				return Arrays.copyOf(header, header.length);
			}

			/**
			 * Returns a copy of the TLV value bytes, without padding.
			 * @return the value bytes, without padding
			 */
			public byte[] value() {
				return Arrays.copyOf(value, value.length);
			}

			/**
			 * Get the next STUN TLV in the sequence, or null.
			 * @return the next TLV or null if at the end
			 * @throws IOException an IO exception from the stream
			 * @throws InvalidSTUNMessageException if the next TLV is invalid
			 */
			public TypeLengthValue next() throws IOException, InvalidSTUNMessageException {
				final int nextPosition = position + header.length + value.length + padding();

				if (nextPosition >= max) {
					return null;
				}

				return new TypeLengthValue(nextPosition, max);
			}
		}

		/**
		 * Start parsing the STUN message.
		 * @return the header of the STUN message
		 * @throws IOException an IO exception from the stream
		 * @throws InvalidSTUNMessageException if could not read the 20-byte STUN header
		 */
		public Header start() throws IOException, InvalidSTUNMessageException {
			final byte[] header = new byte[20];

			if (header.length != inputStream.read(header)) {
				throw new InvalidSTUNMessageException("Could not read 20 byte STUN header from stream");
			} else {
				return new Header(header);
			}
		}
	}
}
