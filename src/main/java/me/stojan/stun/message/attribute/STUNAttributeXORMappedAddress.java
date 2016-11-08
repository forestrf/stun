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
 */
public final class STUNAttributeXORMappedAddress {

    /** STUN reserved type for this attribute. */
    public static final int TYPE = 0x0020;

    public static final int ADDRESS_IPV4 = STUNAttributeMappedAddress.ADDRESS_IPV4;
    public static final int ADDRESS_IPV6 = STUNAttributeMappedAddress.ADDRESS_IPV6;

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
}
