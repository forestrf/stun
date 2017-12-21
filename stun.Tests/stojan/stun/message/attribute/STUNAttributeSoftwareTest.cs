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

import static org.junit.Assert.*;

/**
 * Created by vuk on 03/12/16.
 */
public class STUNAttributeSoftwareTest {

    [Test](expected = UnsupportedOperationException.class)
    public void noInstances() {
        new STUNAttributeSoftware();
    }

    [Test]
    public void TYPE() {
        Assert.AreEqual(0x8022, STUNAttributeSoftware.TYPE);
    }

    [Test](expected = IllegalArgumentException.class)
    public void value_nullArgument() throws Exception {
        STUNAttributeSoftware.value(null);
    }

    [Test]
    public void value_utf8ASCII() throws Exception {
        final String software = "12345678";

        final byte[] value = STUNAttributeSoftware.value(software);

        assertNotNull(value);
        Assert.AreEqual(software.length(), value.length);
        Assert.AreEqual(software, new String(value, "UTF-8"));
    }

    [Test]
    public void value_software_max128Chars() throws Exception {
        final StringBuilder builder = new StringBuilder(129);

        for (int i = 0; i < 130; i++) {
            builder.append('!');
        }

        final String software = builder.toString();

        Assert.IsTrue(software.length() > 128);

        final byte[] value = STUNAttributeSoftware.value(software);

        assertNotNull(value);

        final String softwareRet = STUNAttributeSoftware.software(value);

        assertNotNull(softwareRet);
        Assert.AreEqual(128, softwareRet.length());
        Assert.AreEqual(software.substring(0, 128), softwareRet);
    }

    [Test](expected = IllegalArgumentException.class)
    public void software_nullValue() throws Exception {
        STUNAttributeSoftware.software(null);
    }

    [Test](expected = InvalidSTUNAttributeException.class)
    public void software_moreThan763Bytes() throws Exception {
        STUNAttributeSoftware.software(new byte[764]);
    }

    [Test]
    public void software_correctValue() throws Exception {
        final String software = "Hello, this is Cyrillic: Здраво!";

        final byte[] value = STUNAttributeSoftware.value(software);
        assertNotNull(value);

        final String softwareRet = STUNAttributeSoftware.software(value);
        assertNotNull(softwareRet);

        Assert.AreEqual(software, softwareRet);
    }
}
