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

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.math.BigInteger;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;

/**
 * A message builder for well-formed STUN messages.
 */
public final class STUNMessageBuilder {

    private final byte[] header = new byte[20];

    private int totalValues = 0;
    private final List<byte[]> values = new LinkedList<>();

    /**
     * Set the message type.
     * @param group the STUN class
     * @param method the STUN method
     * @return this builder, never null
     */
    public STUNMessageBuilder messageType(int group, int method) {
        intAs16Bit(STUNMethod.join(method, group) & 0b0011_1111_1111_1111, header, 0);

        return this;
    }

    /**
     * Set the STUN transaction value.
     * @param transaction the transaction value, will be clamped to last 96 bits
     * @return this builder, never null
     */
    public STUNMessageBuilder transaction(BigInteger transaction) {
        final byte[] tx = STUNTransaction.transaction(transaction);

        System.arraycopy(STUNHeader.MAGIC_COOKIE, 0, header, 4, STUNHeader.MAGIC_COOKIE.length);
        System.arraycopy(tx, 0, header, 4 + STUNHeader.MAGIC_COOKIE.length, tx.length);

        return this;
    }

    /**
     * Add a value to the STUN message.
     * @param type the message type
     * @param value the message value
     * @return this builder, never null
     */
    public STUNMessageBuilder value(int type, byte[] value) {
        final byte[] raw = STUNTypeLengthValue.value(type, value);

        totalValues += raw.length;
        values.add(raw);

        intAs16Bit(totalValues & 0xFFFF, header, 2);

        return this;
    }

    /**
     * Return a copy of the current header value. The STUN magic cookie will not be set if
     * {@link #transaction(BigInteger)} has not been called.
     * @return the header value, never null, will always have length of 20
     */
    public byte[] header() {
        return Arrays.copyOf(header, header.length);
    }

    /**
     * Build a byte representation of the message.
     * @return the byte representation of the message, never null
     */
    public byte[] build() {
        final byte[] built = new byte[header.length + totalValues];

        System.arraycopy(header, 0, built, 0, header.length);

        int position = header.length;

        for (byte[] tlv : values) {
            System.arraycopy(tlv, 0, built, position, tlv.length);
            position += tlv.length;
        }

        return built;
    }

    private void intAs16Bit(int value, byte[] out, int position) {
        out[position] = (byte) ((value >> 8) & 255);
        out[position + 1] = (byte) (value & 255);
    }
}
