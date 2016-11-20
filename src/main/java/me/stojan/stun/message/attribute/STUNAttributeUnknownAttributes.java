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

/**
 * Supports the creation of STUN's UNKNOWN-ATTRIBUTES attribute.
 */
public final class STUNAttributeUnknownAttributes {

    /** The STUN type. */
    public static final int TYPE = 0x000A;

    STUNAttributeUnknownAttributes() {
        throw new UnsupportedOperationException();
    }

    /**
     * Create a well-formed UNKNOWN-ATTRIBUTES attribute from the array of attribute types.
     * @param attributes the attribute types, must not be null
     * @return the bytes, will always have an even length
     */
    public static byte[] value(int[] attributes) {
        if (null == attributes) {
            throw new IllegalArgumentException("Argument attributes must not be null");
        }

        final byte[] bytes = new byte[attributes.length * 2];

        for (int i = 0; i < attributes.length; i++) {
            bytes[0 + 2 * i] = (byte) ((attributes[i] >> 8) & 255);
            bytes[1 + 2 * i] = (byte) (attributes[i] & 255);
        }

        return bytes;
    }

    /**
     * Extract the attributes from a well-formed byte representation of STUN's UNKNOWN-ATTRIBUTES attribute.
     * @param attribute the well-formed attribute, must not be null
     * @return the array of attributes, will not be null, may be empty
     * @throws InvalidSTUNAttributeException if the attribute is not well-formed
     */
    public static int[] attributes(byte[] attribute) throws InvalidSTUNAttributeException {
        if (null == attribute) {
            throw new IllegalArgumentException("Argument attribute must not be null");
        }

        if (0 != (attribute.length % 2)) {
            throw new InvalidSTUNAttributeException("Argument attribute must have an even length");
        }

        final int[] attributes = new int[attribute.length / 2];

        for (int i = 0; i < attributes.length; i++) {
            attributes[i] = ((attribute[0 + 2 * i] & 255) << 8) | (attribute[1 + 2 * i] & 255);
        }

        return attributes;
    }
}
