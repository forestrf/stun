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

import java.util.Arrays;

import static org.junit.Assert.*;

/**
 * Created by vuk on 20/11/16.
 */
public class STUNAttributeUnknownAttributesTest {

    @Test(expected = UnsupportedOperationException.class)
    public void noInstance() {
        new STUNAttributeUnknownAttributes();
    }

    @Test(expected = IllegalArgumentException.class)
    public void value_nullAttribute() throws Exception {
        STUNAttributeUnknownAttributes.value(null);
    }

    @Test
    public void value_zeroAttributes() throws Exception {
        final byte[] attribute = STUNAttributeUnknownAttributes.value(new int[0]);

        assertEquals(0, attribute.length);
    }

    @Test
    public void value_multipleAttributes() throws Exception {
        final int[] attributes = new int[] { 0x1ABCD, 0x2EFAB, 0x3CDEF };

        final byte[] attribute = STUNAttributeUnknownAttributes.value(attributes);

        assertEquals(attributes.length * 2, attribute.length);

        assertEquals((byte) 0xAB, attribute[0]);
        assertEquals((byte) 0xCD, attribute[1]);
        assertEquals((byte) 0xEF, attribute[2]);
        assertEquals((byte) 0xAB, attribute[3]);
        assertEquals((byte) 0xCD, attribute[4]);
        assertEquals((byte) 0xEF, attribute[5]);
    }

    @Test(expected = IllegalArgumentException.class)
    public void attributes_nullAttribute() throws Exception {
        STUNAttributeUnknownAttributes.attributes(null);
    }

    @Test
    public void attributes_emptyAttribute() throws Exception {
        assertEquals(0, STUNAttributeUnknownAttributes.attributes(new byte[0]).length);
    }

    @Test(expected = InvalidSTUNAttributeException.class)
    public void attributes_oddAttributeLength() throws Exception {
        STUNAttributeUnknownAttributes.attributes(new byte[1]);
    }

    @Test
    public void attributes_extractAttributes() throws Exception {
        final int[] attributes = new int[] { 0xABCD, 0xEFAB, 0xCDEF, 0xABCD };
        final byte[] attribute = STUNAttributeUnknownAttributes.value(attributes);
        final int[] extracted = STUNAttributeUnknownAttributes.attributes(attribute);

        assertTrue(Arrays.equals(attributes, extracted));
    }
}
