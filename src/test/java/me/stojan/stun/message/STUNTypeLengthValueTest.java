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
import static org.junit.Assert.*;

/**
 * Created by vuk on 24/10/16.
 */
public class STUNTypeLengthValueTest {

    @Test(expected = UnsupportedOperationException.class)
    public void noInstances() {
        new STUNTypeLengthValue();
    }

    @Test
    public void typePosition() {
        final byte[] result = STUNTypeLengthValue.value(257, new byte[] {});

        assertEquals(1, result[0]);
        assertEquals(1, result[1]);
        assertEquals(0, result[2]);
        assertEquals(0, result[3]);
    }

    @Test
    public void lengthPosition() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[257]);

        assertEquals(0, result[0]);
        assertEquals(0, result[1]);
        assertEquals(1, result[2]);
        assertEquals(1, result[3]);
    }

    @Test
    public void resultingSizeZeroLValue() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[0]);

        assertEquals(4, result.length);
    }

    @Test
    public void resultingSizeOneLValue() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[] { -1 });

        assertEquals(8, result.length);
        assertEquals(-1, result[4]);
        assertEquals(0, result[5]);
        assertEquals(0, result[6]);
        assertEquals(0, result[7]);
    }

    @Test
    public void resultingSizeTwoLValue() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[] { -1, -1 });

        assertEquals(8, result.length);
        assertEquals(-1, result[4]);
        assertEquals(-1, result[5]);
        assertEquals(0, result[6]);
        assertEquals(0, result[7]);
    }

    @Test
    public void resultingSizeThreeLValue() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[] { -1, -1, -1 });

        assertEquals(8, result.length);
        assertEquals(-1, result[4]);
        assertEquals(-1, result[5]);
        assertEquals(-1, result[6]);
        assertEquals(0, result[7]);
    }

    @Test
    public void resultingSizeFourLValue() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[] { -1, -1, -1, -1 });

        assertEquals(8, result.length);
        assertEquals(-1, result[4]);
        assertEquals(-1, result[5]);
        assertEquals(-1, result[6]);
        assertEquals(-1, result[7]);
    }

    @Test
    public void resultingSizeFiveLValue() {
        final byte[] result = STUNTypeLengthValue.value(0, new byte[] { -1, -1, -1, -1, -1 });

        assertEquals(12, result.length);
        assertEquals(-1, result[4]);
        assertEquals(-1, result[5]);
        assertEquals(-1, result[6]);
        assertEquals(-1, result[7]);
        assertEquals(-1, result[8]);
        assertEquals(0, result[9]);
        assertEquals(0, result[10]);
        assertEquals(0, result[11]);
    }
}
