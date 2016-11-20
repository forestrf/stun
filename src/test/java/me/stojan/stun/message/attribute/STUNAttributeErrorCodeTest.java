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

import org.junit.Test;

import java.nio.charset.Charset;

import static org.junit.Assert.*;

/**
 * Created by vuk on 20/11/16.
 */
public class STUNAttributeErrorCodeTest {

    @Test(expected = UnsupportedOperationException.class)
    public void noInstances() throws Exception {
        new STUNAttributeErrorCode();
    }

    @Test(expected = IllegalArgumentException.class)
    public void value_nullReason() throws Exception {
        STUNAttributeErrorCode.value(300, null);
    }

    @Test(expected = IllegalArgumentException.class)
    public void value_errorCodeLessThan300() throws Exception {
        STUNAttributeErrorCode.value(299, "");
    }

    @Test(expected = IllegalArgumentException.class)
    public void value_errorCodeGreaterThan699() throws Exception {
        STUNAttributeErrorCode.value(700, "");
    }

    @Test
    public void value_generateLongReason() throws Exception {
        final StringBuilder builder = new StringBuilder();

        for (int i = 0; i < 250; i++) {
            builder.append('!');
        }

        final String longReason = builder.toString();

        final byte[] attribute = STUNAttributeErrorCode.value(699, longReason);

        assertEquals(4 + 128, attribute.length);
        assertEquals(0, attribute[0]);
        assertEquals(0, attribute[1]);
        assertEquals(6, attribute[2]);
        assertEquals(99, attribute[3]);

        assertEquals(new String(attribute, 4, attribute.length - 4, Charset.forName("UTF-8")), longReason.substring(0, 128));
    }

    @Test(expected = IllegalArgumentException.class)
    public void checkAttribute_nullValue() throws Exception {
        STUNAttributeErrorCode.checkAttribute(null);
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_lengthLessThan4() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[3]);
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_nonZeroFirstByte() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[] { 1, 0, 3, 0 });
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_nonZeroSecondByte() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[] { 0, 1, 3, 0 });
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_thirdByteLessThan3() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[] { 0, 0, 2, 0 });
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_tirdByteGreaterThan6() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[] { 0, 0, 7, 0 });
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_fourthByteGreaterThan99() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[] { 0, 0, 3, 100 });
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_fourthByteGreaterThan128() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[] { 0, 0, 3, (byte) 129 });
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void checkAttribute_maxLength() throws Exception {
        STUNAttributeErrorCode.checkAttribute(new byte[4 + 764]);
    }

    @Test
    public void reason_extract() throws Exception {
        final String reason = "Hello, World!";

        final byte[] attribute = STUNAttributeErrorCode.value(300, reason);

        assertEquals(reason, STUNAttributeErrorCode.reason(attribute));
    }

    @Test
    public void reason_emptyExtract() throws Exception {
        final String reason = "";

        final byte[] attribute = STUNAttributeErrorCode.value(300, reason);

        assertEquals(reason, STUNAttributeErrorCode.reason(attribute));
    }

    @Test
    public void code_extract300() throws Exception {
        assertEquals(300, STUNAttributeErrorCode.code(STUNAttributeErrorCode.value(300, "")));
    }

    @Test
    public void code_extract699() throws Exception {
        assertEquals(699, STUNAttributeErrorCode.code(STUNAttributeErrorCode.value(699, "")));
    }
}
