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

import java.net.InetAddress;
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
}
