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

import org.junit.Test;

import java.math.BigInteger;
import java.util.Arrays;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;

/**
 * Created by vuk on 24/10/16.
 */
public class STUNMessageBuilderTest {

    @Test
    public void buildMessage() {
        final STUNMessageBuilder builder = new STUNMessageBuilder();

        builder.messageType(STUNMessageType.GROUP_RESPONSE_ERROR, STUNMessageType.METHOD_BINDING);
        builder.transaction(BigInteger.ONE);
        builder.value(0b111, new byte[] { -1 });

        final byte[] message = builder.build();

        final byte[] transaction = new byte[12];

        System.arraycopy(message, 8, transaction, 0, 12);

        final BigInteger transactionInt = new BigInteger(transaction);

        final byte[] magicCookie = new byte[4];

        System.arraycopy(message, 4, magicCookie, 0, 4);

        // check length
        assertEquals(0, (message.length - 20) % 4);
        // check length without header
        assertEquals(message.length - 20, STUNHeader.int16(message, 2));
        // check group
        assertEquals(STUNMessageType.GROUP_RESPONSE_ERROR, STUNHeader.group(STUNHeader.int16(message, 0)));
        // check method
        assertEquals(STUNMessageType.METHOD_BINDING, STUNHeader.method(STUNHeader.int16(message, 0)));
        // check magic cookie
        assertTrue(Arrays.equals(STUNHeader.MAGIC_COOKIE, magicCookie));
        // check transaction
        assertEquals(transactionInt, BigInteger.ONE);
        // check first byte of tlv
        assertEquals(0, message[20]);
        // check second byte of tlv
        assertEquals(0b111, message[20 + 1]);
        // check first byte of tlv length
        assertEquals(0, message[20 + 2]);
        // check second byte of tlv length
        assertEquals(1, message[20 + 2 + 1]);
        // check first byte of tlv value
        assertEquals(-1, message[20 + 2 + 2]);
        // check second byte of tlv value
        assertEquals(0, message[20 + 2 + 2 + 1]);
        // check third byte of tlv value
        assertEquals(0, message[20 + 2 + 2 + 2]);
        // check fourth byte of tlv value
        assertEquals(0, message[20 + 2 + 2 + 3]);
    }
}
