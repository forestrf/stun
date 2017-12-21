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

import java.io.ByteArrayInputStream;
import java.math.BigInteger;
import java.util.Arrays;

import static org.junit.Assert.*;

/**
 * Created by vuk on 24/10/16.
 */
public class STUNMessageParserTest {

    @Test
    public void parsing() throws Exception {
        final STUNMessageBuilder builder = new STUNMessageBuilder();

        builder.messageType(STUNMessageType.GROUP_RESPONSE_ERROR, STUNMessageType.METHOD_BINDING);
        builder.transaction(BigInteger.ONE);
        builder.value(0b111, new byte[] { -1 });
        builder.value(0b010, new byte[] { 0, -1, 0, -1 });

        final byte[] message = builder.build();

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(message));

        final STUNMessageParser.Header header = parser.start();

        assertTrue(Arrays.equals(Arrays.copyOf(message, 20), header.raw()));

        assertEquals(STUNMessageType.GROUP_RESPONSE_ERROR, header.group());
        assertEquals(STUNMessageType.METHOD_BINDING, header.method());
        assertEquals(BigInteger.ONE, new BigInteger(header.transaction()));
        assertTrue(0 == header.length() % 4);
        assertTrue(header.isMagicCookieValid());

        final STUNMessageParser.TypeLengthValue tlv1 = header.next();

        assertNotNull(tlv1);
        assertEquals(0b111, tlv1.type());
        assertTrue(Arrays.equals(new byte [] { 0, 0b111, 0, 1 }, tlv1.header()));
        assertTrue(Arrays.equals(new byte[] { -1 }, tlv1.value()));
        assertEquals(1, tlv1.length());
        assertEquals(3, tlv1.padding());

        final STUNMessageParser.TypeLengthValue tlv2 = tlv1.next();

        assertNotNull(tlv2);
        assertEquals(0b010, tlv2.type());
        assertTrue(Arrays.equals(new byte[] { 0, 0b010, 0, 4 }, tlv2.header()));
        assertTrue(Arrays.equals(new byte[] { 0, -1, 0, -1 }, tlv2.value()));
        assertEquals(4, tlv2.length());
        assertEquals(0, tlv2.padding());

        assertNull(tlv2.next());
    }

    @Test(expected = InvalidSTUNMessageException.class)
    public void shortHeader() throws Exception {
        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(new byte[19]));

        parser.start();
    }

    @Test(expected = InvalidSTUNMessageException.class)
    public void headerDoesNotStartWith00() throws Exception {
        final byte[] header = new byte[20];
        header[0] = -1;

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(header));

        parser.start();
    }

    @Test(expected = InvalidSTUNMessageException.class)
    public void lengthNotAMultipleOf4() throws Exception {
        final byte[] header = new byte[20];

        header[2] = 0;
        header[3] = 3;

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(header));

        parser.start();
    }

    @Test
    public void noTLV() throws Exception {
        final byte[] header = new byte[20];

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(header));

        assertNull(parser.start().next());
    }

    @Test(expected = InvalidSTUNMessageException.class)
    public void eosAtReadingFirstTLVHeader() throws Exception {
        final STUNMessageBuilder builder = new STUNMessageBuilder();

        builder.messageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
        builder.transaction(BigInteger.TEN);
        builder.value(0b11, new byte[] { -1, -1 });

        final byte[] message = builder.build();

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(Arrays.copyOf(message, 20 + 1)));

        parser.start().next();
    }

    @Test(expected = InvalidSTUNMessageException.class)
    public void eosAtReadingFirstTLVValue() throws Exception {
        final STUNMessageBuilder builder = new STUNMessageBuilder();

        builder.messageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
        builder.transaction(BigInteger.TEN);
        builder.value(0b11, new byte[] { -1, -1 });

        final byte[] message = builder.build();

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(Arrays.copyOf(message, 20 + 4 + 1)));

        parser.start().next();
    }

    @Test(expected = InvalidSTUNMessageException.class)
    public void eosAtReadingFirstTLVPadding() throws Exception {
        final STUNMessageBuilder builder = new STUNMessageBuilder();

        builder.messageType(STUNMessageType.GROUP_REQUEST, STUNMessageType.METHOD_BINDING);
        builder.transaction(BigInteger.TEN);
        builder.value(0b11, new byte[] { -1, -1 });

        final byte[] message = builder.build();

        final STUNMessageParser parser = new STUNMessageParser(new ByteArrayInputStream(Arrays.copyOf(message, 20 + 4 + 2 + 1)));

        parser.start().next();
    }
}
