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

package me.stojan.stun.message;

/**
 * Utilities for forming STUN message headers.
 */
public final class STUNHeader {
    /** The STUN "magic cookie." */
    public static final byte[] MAGIC_COOKIE = {  0x21, 0x12, (byte) 0xA4, 0x42 };

    private static final int C1 =           0b0000_0001_0000_0000;
    private static final int C0 =           0b0000_0000_0001_0000;

    private static final int BITS_HIGH_5 =  0b0011_1110_0000_0000;
    private static final int BITS_INNER_3 = 0b0000_0000_1110_0000;
    private static final int BITS_LOWER_4 = 0b0000_0000_0000_1111;

    /**
     * Convert the two bytes (16 bits) from values starting at position into an integer.
     * @param values the values, must have length at least 2
     * @param position the position, must be at most {@code values.length - 2}
     * @return the integer
     */
    public static int int16(byte[] values, int position) {
        return (values[position + 0] << 8) | values[position + 1];
    }

    /**
     * Returns the STUN class from the compound message type.
     * @param messageType the message type
     * @return the group
     */
    public static int group(int messageType) {
        return ((messageType & C1) >> 7) | ((messageType & C0) >> 4);
    }

    /**
     * Returns the STUN method from the compound message type.
     * @param messageType the message type
     * @return the method
     */
    public static int method(int messageType) {
        return (((messageType & BITS_HIGH_5) >> 1) | (messageType & BITS_INNER_3)) >> 1 | (messageType & BITS_LOWER_4);
    }

    /**
     * Returns the two starting bits from the byte array.
     * @param header the header
     * @return the two starting bits
     */
    public static int twoStartingBits(byte[] header) {
        return header[0] >> 6;
    }

    STUNHeader() {
        throw new UnsupportedOperationException();
    }
}
