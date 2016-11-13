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
 * Supports the creation of the STUN XOR-MAPPED-ADDRESS attribute.
 *
 * @see STUNAttributeMappedAddress
 */
public final class STUNAttributeXORMappedAddress {

    /** STUN reserved type for this attribute. */
    public static final int TYPE = 0x0020;

    /** STUN XOR-MAPPED-ADDRESS IPv4 address family. */
    public static final byte ADDRESS_IPV4 = STUNAttributeMappedAddress.ADDRESS_IPV4;

    /** STUN XOR-MAPPED-ADDRESS IPv6 address family. */
    public static final byte ADDRESS_IPV6 = STUNAttributeMappedAddress.ADDRESS_IPV6;

    STUNAttributeXORMappedAddress() {
        throw new UnsupportedOperationException();
    }

    /**
     * Create the XOR-MAPPED-ADDRESS attribute.
     * @param header the STUN message header, must not be null and must be 20 bytes long
     * @param addr the address, must not be null and must be either 4 or 16 bytes long
     * @param port the port, will be treated as 16-bits
     * @return the XOR-MAPPED-ADDRESS value, never null
     */
    public static byte[] value(byte[] header, byte[] addr, int port) {
        final byte[] attribute = STUNAttributeMappedAddress.value(addr, port);

        attribute[2] = (byte) (attribute[2] ^ header[4]);
        attribute[3] = (byte) (attribute[3] ^ header[5]);

        for (int i = 4; i < attribute.length; i++) {
            attribute[i] = (byte) (attribute[i] ^ header[i]);
        }

        return attribute;
    }

    /**
     * Get the port from an attribute and header.
     * @param header the header, must be a valid STUN header
     * @param attribute the attribute, msut be a valid attribute
     * @return the port
     * @throws IllegalArgumentException if arguments, and header are not valid
     * @throws InvalidSTUNAttributeException if attribute is not valid
     */
    public static int port(byte[] header, byte[] attribute) throws InvalidSTUNAttributeException {
        checkHeader(header);

        final int port = STUNAttributeMappedAddress.port(attribute);
        final int xor = ((header[4] & 255) << 8) | (header[5] & 255);

        return xor ^ port;
    }

    /**
     * Get the address from an attribute and header.
     * @param header the header, must be a valid STUN header
     * @param attribute the attribute, must be a valid attribute
     * @return the address
     * @throws IllegalArgumentException if arguments, and header are not valid
     * @throws InvalidSTUNAttributeException if attribute is not valid
     */
    public static byte[] address(byte[] header, byte[] attribute) throws InvalidSTUNAttributeException {
        checkHeader(header);

        final byte[] address = STUNAttributeMappedAddress.address(attribute);

        for (int i = 0; i < address.length; i++) {
            address[i] = (byte) (attribute[4 + i] ^ header[4 + i]);
        }

        return address;
    }

    /**
     * Checks that the provided header is valid as an argument and as STUN data.
     * @param header the header
     * @throws IllegalArgumentException if header is null, not 20 bytes long or does not start with 00 bits
     */
    static void checkHeader(byte[] header) {
        if (null == header) {
            throw new IllegalArgumentException("Argument header must not be null");
        }

        if (20 != header.length) {
            throw new IllegalArgumentException("Argument header is not 20 bytes long");
        }

        if (0 != (0b1100_0000 & header[0])) {
            throw new IllegalArgumentException("Argument header does not start with 00 as MSB");
        }
    }

}
