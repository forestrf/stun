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

import static org.junit.Assert.*;

/**
 * Created by vuk on 24/10/16.
 */
public class STUNTransactionTest {
    private static final byte OX7F = (byte) 0x7F;
    private static final byte OXFF = (byte) 0xFF;

    @Test
    public void upTo95Bits() {
        final byte[] val = new byte[] { OX7F,  OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF };

        assertEquals(12, val.length);
        assertTrue(Arrays.equals(val, STUNTransaction.transaction(BigInteger.ONE.shiftLeft(95).subtract(BigInteger.ONE))));
    }

    @Test
    public void at96Bits() {
        final byte[] val = new byte[] { OXFF,  OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF };

        assertEquals(12, val.length);
        assertTrue(Arrays.equals(val, STUNTransaction.transaction(BigInteger.ONE.shiftLeft(96).subtract(BigInteger.ONE))));
    }

    @Test
    public void over96Bits() {
        final byte[] val = new byte[] { OXFF,  OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF, OXFF };

        assertEquals(12, val.length);
        assertTrue(Arrays.equals(val, STUNTransaction.transaction(BigInteger.ONE.shiftLeft(97).subtract(BigInteger.ONE))));
    }
}
