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
 * Supports the creation of well-formed SOFTWARE STUN attributes.
 */
public final class STUNAttributeSoftware {

    /** STUN type of attribute. */
    public static final int TYPE = 0x8022;

    private static final Charset CHARSET = Charset.forName("UTF-8");

    STUNAttributeSoftware() {
        throw new UnsupportedOperationException();
    }

    /**
     * Create a well-formed SOFTWARE STUN attribute.
     * @param software the value, must not be null and will be clamped to first 128 bytes
     * @return the well-formed attribute
     *
     * @throws IllegalArgumentException if {@code software} is null
     */
    public static byte[] value(String software) {
        if (null == software) {
            throw new IllegalArgumentException("Argument software must not be null");
        }

        final String clamped = software.substring(0, Math.min(128, software.length()));

        return clamped.getBytes(CHARSET);
    }

    /**
     * Get the software string from a well-formed SOFTWARE STUN attribute bytes.
     * @param value the bytes, must not be null and must be at most 763 bytes long
     * @return the SOFTWARE value
     *
     * @throws IllegalArgumentException if {@code value} is null
     * @throws InvalidSTUNAttributeException if {@code value} is longer than 763 bytes
     */
    public static String software(byte[] value) throws InvalidSTUNAttributeException {
        if (null == value) {
            throw new IllegalArgumentException("Argument software must not be null");
        }

        if (value.length > 763) {
            throw new InvalidSTUNAttributeException("SOFTWARE attribute must not be longer than 763 bytes");
        }

        return new String(value, CHARSET);
    }
}
