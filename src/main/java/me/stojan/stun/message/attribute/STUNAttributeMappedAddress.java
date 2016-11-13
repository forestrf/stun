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

import java.util.Locale;

/**
 * Supports the creation of the STUN MAPPED-ADDRESS and XOR-MAPPED-ADDRESS attributes.
 */
public final class STUNAttributeMappedAddress {

    /** STUN reserved type for this attribute. */
    public static final int TYPE = 0x0001;

    public static final byte ADDRESS_IPV4 = 0x01;
    public static final byte ADDRESS_IPV6 = 0x02;

    STUNAttributeMappedAddress() {
        throw new UnsupportedOperationException();
    }

    /**
     * Create the MAPPED-ADDRESS attribute.
     * @param addr the address, must not be null and must be 4 (IPv4) or 16 bytes (IPv6) long
     * @param port the port, will be treated as 16-bit
     * @return the value, never null
     */
    public static byte[] value(byte[] addr, int port) {
        final byte type;

        switch (addr.length) {
            case 4:
                type = ADDRESS_IPV4;
                break;

            case 16:
                type = ADDRESS_IPV6;
                break;

            default:
                throw new UnsupportedOperationException(String.format((Locale) null, "Unsupported address of length %d", addr.length));
        }

        final byte[] bytes = new byte[1 + 1 + 2 + addr.length];

        bytes[0] = 0;
        bytes[1] = type;
        bytes[2] = (byte) ((port >> 8) & 255);
        bytes[3] = (byte) (port & 255);

        System.arraycopy(addr, 0, bytes, 4, addr.length);

        return bytes;
    }

    /**
     * Get the port from an attribute and header.
     * @param attribute the attribute, msut be a valid attribute
     * @return the port
     * @throws IllegalArgumentException if attribute is null
     * @throws InvalidSTUNAttributeException if attribute is not valid
     */
    public static int port(byte[] attribute) throws InvalidSTUNAttributeException {
        checkAttribute(attribute);

        return ((attribute[2] & 255) << 8) | (attribute[3] & 255);
    }

    /**
     * Get the address from an attribute and header.
     * @param attribute the attribute, must be a valid attribute
     * @return the address
     * @throws IllegalArgumentException if attribute is null
     * @throws InvalidSTUNAttributeException if attribute is not valid
     */
    public static byte[] address(byte[] attribute) throws InvalidSTUNAttributeException {
        checkAttribute(attribute);

        final int addressLength;

        switch (attribute[1]) {
            case ADDRESS_IPV4:
                addressLength = 4;
                break;

            case ADDRESS_IPV6:
                addressLength = 16;
                break;

            default:
                throw new InvalidSTUNAttributeException("Unknown address family");
        }

        if (4 + addressLength != attribute.length) {
            throw new InvalidSTUNAttributeException("Attribute has invalid length");
        }

        final byte[] address = new byte[addressLength];

        System.arraycopy(attribute, 4, address, 0, address.length);

        return address;
    }

    /**
     * Check that the attribute is valid.
     * @param attribute the attribute
     * @throws IllegalArgumentException if attribute is null
     * @throws InvalidSTUNAttributeException if attribute is not valid per STUN spec
     */
    static void checkAttribute(byte[] attribute) throws InvalidSTUNAttributeException {
        if (null == attribute) {
            throw new IllegalArgumentException("Argument attribute must not be null");
        }

        if (0 != attribute.length % 4) {
            throw new InvalidSTUNAttributeException("Attribute has length which is not a multiple of 4");
        }

        if (4 > attribute.length) {
            throw new InvalidSTUNAttributeException("Attribute length is less than 4");
        }

        if (0 != attribute[0]) {
            throw new InvalidSTUNAttributeException("(XOR-)MAPPED-ADDRESS attribute does not start with 0");
        }
    }
}
