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

package me.stojan.stun.message.attribute;

import java.nio.charset.Charset;

/**
 * Supports the creation of STUN's ERROR-CODE attribute.
 */
public final class STUNAttributeErrorCode {
    /** The STUN type of this attribute. */
    public static final int TYPE = 0x0009;

    private final static Charset REASON_CHARSET = Charset.forName("UTF-8");

    STUNAttributeErrorCode() {
        throw new UnsupportedOperationException();
    }

    /**
     * Create a valid ERROR-CODE STUN attribute.
     * @param code the code, must be within [300, 700)
     * @param reason the reason, must not be null and will be taken to be at most 128 chars long
     * @return the value, length may not be a multiple of 4
     */
    public static byte[] value(int code, String reason) {
        if (code < 300 || code >= 700) {
            throw new IllegalArgumentException("Argument code must be within [300, 700)");
        }

        if (null == reason) {
            throw new IllegalArgumentException("Argument reason must not be null");
        }

        final int maxReasonLength = Math.min(reason.length(), 128);

        final byte[] reasonBytes = reason.substring(0, maxReasonLength).getBytes(REASON_CHARSET);

        final byte[] bytes = new byte[4 + reasonBytes.length];

        final int errorClass = code / 100;
        final int errorNumber = code % 100;

        bytes[0] = 0;
        bytes[1] = 0;
        bytes[2] = (byte) (errorClass & 0b111);
        bytes[3] = (byte) (errorNumber & 0b0111_1111);

        System.arraycopy(reasonBytes, 0, bytes, 4, reasonBytes.length);

        return bytes;
    }

    /**
     * Extract the reason from a well-formed ERROR-CODE STUN attribute.
     * @param attribute the attribute, must be well formed and not null
     * @return the reason, at most 128 chars long, never null, may be empty
     * @throws InvalidSTUNAttributeException if the attribute is not well formed
     */
    public static String reason(byte[] attribute) throws InvalidSTUNAttributeException {
        checkAttribute(attribute);

        return new String(attribute, 4, attribute.length - 4, REASON_CHARSET);
    }

    /**
     * Extract the error code from a well-formed ERROR-CODE STUN attribute.
     * @param attribute the attribute, must be well-formed and not null
     * @return the code, a value within [300, 700)
     * @throws InvalidSTUNAttributeException if the attribute is not well formed
     */
    public static int code(byte[] attribute) throws InvalidSTUNAttributeException {
        checkAttribute(attribute);

        return (attribute[2] & 0b111) * 100 + (attribute[3] & 0b0111_1111);
    }

    /**
     * Check that the attribute is well formed.
     * @param attribute the attribute
     * @throws IllegalArgumentException if attribute is null
     * @throws InvalidSTUNAttributeException if the attribute is not well formed
     */
    static void checkAttribute(byte[] attribute) throws InvalidSTUNAttributeException {
        if (null == attribute) {
            throw new IllegalArgumentException("Argument attribute must not be null");
        }

        if (attribute.length < 4) {
            throw new InvalidSTUNAttributeException("Argument attribute must be at least 4 bytes long");
        }

        if (attribute.length > (4 + 763)) {
            throw new InvalidSTUNAttributeException("Argument attribute can be at most 4 + 763 bytes long");
        }

        if (0 != attribute[0] || 0 != attribute[1]) {
            throw new InvalidSTUNAttributeException("Argument attribute must start with two 0 bytes");
        }

        if (attribute[2] < 3 || attribute[2] > 6) {
            throw new InvalidSTUNAttributeException("Argument attribute at 3-rd position must contain a value within [3, 7)");
        }

        if (attribute[3] < 0 || attribute[3] > 99) {
            throw new InvalidSTUNAttributeException("Argument attribute at 4-th position must contain a value within [0, 100)");
        }
    }
}
