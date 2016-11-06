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
import java.util.LinkedList;
import java.util.List;

/**
 * A message builder for well-formed STUN messages.
 */
public final class STUNMessageBuilder {

    private int group;
    private int method;
    private BigInteger transaction = BigInteger.ZERO;

    private int totalValues = 0;
    private final List<byte[]> values = new LinkedList<>();

    /**
     * Set the message type.
     * @param group the STUN class
     * @param method the STUN method
     * @return this builder, never null
     */
    public STUNMessageBuilder messageType(int group, int method) {
        this.group = group;
        this.method = method;

        return this;
    }

    /**
     * Set the STUN transaction value.
     * @param transaction the transaction value, will be clamped to last 96 bits
     * @return this builder, never null
     */
    public STUNMessageBuilder transaction(BigInteger transaction) {
        this.transaction = transaction.and(STUNTransaction.MAX);

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

        return this;
    }

    /**
     * Build the STUN message into the provided output stream.
     * @param stream the output stream, must not be null
     * @throws IOException thrown when writing to the stream
     */
    public void build(OutputStream stream) throws IOException {
        final byte[] intBytes = new byte[2];

        intAs16Bit(STUNMethod.join(method, group) & 0b0011_1111_1111_1111, intBytes, 0);
        stream.write(intBytes);

        stream.flush();

        intAs16Bit(totalValues & 0b1111_1111_1111_1111, intBytes, 0);
        stream.write(intBytes);

        stream.flush();

        stream.write(STUNHeader.MAGIC_COOKIE);
        stream.write(STUNTransaction.transaction(transaction));

        stream.flush();

        for (byte[] tlv : values) {
            stream.write(tlv);
        }

        stream.flush();
    }

    /**
     * Build a byte representation of the message.
     * @return the byte representation of the message, never null
     */
    public byte[] build() {
        final ByteArrayOutputStream stream = new ByteArrayOutputStream(20 + totalValues);

        try {
            build(stream);
            return stream.toByteArray();
        } catch (IOException e) {
            throw new RuntimeException("Unable to build STUN message", e);
        }
    }

    private void intAs16Bit(int value, byte[] out, int position) {
        out[position] = (byte) ((value >> 8) & 255);
        out[position + 1] = (byte) (value & 255);
    }
}
