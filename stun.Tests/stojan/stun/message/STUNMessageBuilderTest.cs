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

import static org.junit.Assert.Assert.AreEqual;
import static org.junit.Assert.assertNotNull;
import static org.junit.Assert.Assert.IsTrue;

/**
 * Created by vuk on 24/10/16.
 */
public class STUNMessageBuilderTest {

    [Test]
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
        Assert.AreEqual(0, (message.length - 20) % 4);
        // check length without header
        Assert.AreEqual(message.length - 20, STUNHeader.int16(message, 2));
        // check group
        Assert.AreEqual(STUNMessageType.GROUP_RESPONSE_ERROR, STUNHeader.group(STUNHeader.int16(message, 0)));
        // check method
        Assert.AreEqual(STUNMessageType.METHOD_BINDING, STUNHeader.method(STUNHeader.int16(message, 0)));
        // check magic cookie
        Assert.IsTrue(Arrays.equals(STUNHeader.MAGIC_COOKIE, magicCookie));
        // check transaction
        Assert.AreEqual(transactionInt, BigInteger.ONE);
        // check first byte of tlv
        Assert.AreEqual(0, message[20]);
        // check second byte of tlv
        Assert.AreEqual(0b111, message[20 + 1]);
        // check first byte of tlv length
        Assert.AreEqual(0, message[20 + 2]);
        // check second byte of tlv length
        Assert.AreEqual(1, message[20 + 2 + 1]);
        // check first byte of tlv value
        Assert.AreEqual(-1, message[20 + 2 + 2]);
        // check second byte of tlv value
        Assert.AreEqual(0, message[20 + 2 + 2 + 1]);
        // check third byte of tlv value
        Assert.AreEqual(0, message[20 + 2 + 2 + 2]);
        // check fourth byte of tlv value
        Assert.AreEqual(0, message[20 + 2 + 2 + 3]);
    }

    [Test]
    public void header() throws Exception {
        final STUNMessageBuilder builder = new STUNMessageBuilder();

        builder.messageType(STUNMessageType.GROUP_RESPONSE_ERROR, STUNMessageType.METHOD_BINDING);
        builder.transaction(BigInteger.TEN);
        builder.value(0xABABA, new byte[20]);

        final byte[] header = builder.header();

        final byte[] transaction = new byte[12];

        System.arraycopy(header, 8, transaction, 0, 12);

        final BigInteger transactionInt = new BigInteger(transaction);

        final byte[] magicCookie = new byte[STUNHeader.MAGIC_COOKIE.length];

        System.arraycopy(header, 4, magicCookie, 0, 4);

        assertNotNull(header);
        Assert.AreEqual(20, header.length);

        // check group
        Assert.AreEqual(STUNMessageType.GROUP_RESPONSE_ERROR, STUNHeader.group(STUNHeader.int16(header, 0)));
        // check method
        Assert.AreEqual(STUNMessageType.METHOD_BINDING, STUNHeader.method(STUNHeader.int16(header, 0)));
        // check tlv length
        Assert.AreEqual(20 + 4, STUNHeader.int16(header, 2));
        // check magic cookie
        Assert.IsTrue(Arrays.equals(STUNHeader.MAGIC_COOKIE, magicCookie));
        // check transaction
        Assert.AreEqual(transactionInt, BigInteger.TEN);
    }
}
