using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util;

namespace CWordTest
{
    [TestClass]
    public class UnitTest_CWord
    {
        [TestMethod]
        public void CWord_Constructor_SignedInteger()
        {
            CWord arg = new CWord((Int32) Int32.MaxValue);
            Assert.AreEqual("0".PadRight(32, '1'), arg.getBinary);
            Assert.AreEqual("7FFFFFFF", arg.getHexadecimal);
            Assert.AreEqual((UInt32) Int32.MaxValue, arg.getUnsignedDecimal);

            arg = new CWord((Int32)Int32.MinValue);
            Assert.AreEqual("1".PadRight(32, '0'), arg.getBinary);
            Assert.AreEqual("80000000", arg.getHexadecimal);
            Assert.AreEqual((UInt32)Int32.MaxValue + 1, arg.getUnsignedDecimal);
        }

        [TestMethod]
        public void CWord_Constructor_UnsignedInteger()
        {
            CWord arg = new CWord((UInt32) UInt32.MaxValue);
            Assert.AreEqual("1".PadRight(32, '1'), arg.getBinary);
            Assert.AreEqual("FFFFFFFF", arg.getHexadecimal);
            Assert.AreEqual(-1, arg.getSignedDecimal);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Binary strings > 32 length are not allowed.")]
        public void CWord_Constructor_Binary()
        {
            CWord arg = new CWord("00000000000000000000000000000001");
            Assert.AreEqual("1".PadLeft(32, '0'), arg.getBinary);
            Assert.AreEqual("00000001", arg.getHexadecimal);
            Assert.AreEqual(1, arg.getSignedDecimal);
            Assert.AreEqual((UInt32) 1, arg.getUnsignedDecimal);

            arg = new CWord("11111111111111111111111111111111");
            Assert.AreEqual("1".PadLeft(32, '1'), arg.getBinary);
            Assert.AreEqual("FFFFFFFF", arg.getHexadecimal);
            Assert.AreEqual(-1, arg.getSignedDecimal);
            Assert.AreEqual((UInt32)UInt32.MaxValue, arg.getUnsignedDecimal);

            /* check if padding works correct => we want always a string which has the length of 32 (because of substrings) */
            arg = new CWord("11111111");
            Assert.AreEqual("11111111".PadLeft(32, '0'), arg.getBinary);
            Assert.AreEqual("000000FF", arg.getHexadecimal);
            Assert.AreEqual(255, arg.getSignedDecimal);
            Assert.AreEqual((UInt32) 255, arg.getUnsignedDecimal);


            /* argument out of range exeption */
            arg = new CWord("111111111111111111111111111111111");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Binary strings only contains 0 and 1's.")]
        public void CWord_Constructor_Binary_FormatException()
        {
            /* format exception */
            CWord word = new CWord("abcd");
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException), "Addition of signed and unsigned values are not allowed.")]
        public void CWord_Plus_Operator()
        {
            CWord arg1 = new CWord((Int32) (-4));
            CWord arg2 = new CWord((Int32) 5);

            CWord arg3 = arg1 + arg2;

            Assert.AreEqual("1".PadLeft(32, '0'), arg3.getBinary);
            Assert.AreEqual("00000001", arg3.getHexadecimal);

            arg1 = new CWord((UInt32) 4);
            arg3 = arg1 + arg2;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException), "Addition of signed and unsigned values are not allowed.")]
        public void CWord_Minus_Operator()
        {
            CWord arg1 = new CWord((Int32) 4);
            CWord arg2 = new CWord((Int32) 5);

            CWord arg3 = arg1 - arg2;

            Assert.AreEqual("1".PadLeft(32, '1'), arg3.getBinary);
            Assert.AreEqual("FFFFFFFF", arg3.getHexadecimal);

            arg1 = new CWord((UInt32)4);
            arg3 = arg1 - arg2;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Requesting a subword with an start > 32 or an length > 32 is not allowed.")]
        public void CWord_getSubwordFromBinary()
        {
            CWord word = new CWord("00001111000011111111000010011111");
            CWord subWord = word.getSubword(2, 6);
            Assert.AreEqual("001111".PadLeft(32, '0'), subWord.getBinary);
            Assert.AreEqual(15, subWord.getSignedDecimal);
            Assert.AreEqual((UInt16)15, subWord.getUnsignedDecimal);
            Assert.AreEqual("00001111000011111111000010011111", word.getBinary);

            word = new CWord("1111");
            subWord = word.getSubword(5, 10);
            Assert.AreEqual("0".PadLeft(32, '0'), subWord.getBinary);
            Assert.AreEqual(0, subWord.getSignedDecimal);
            Assert.AreEqual((UInt16) 0, subWord.getUnsignedDecimal);
            Assert.AreEqual("1111".PadLeft(32, '0'), word.getBinary);

            /* argument out of range */
            subWord = word.getSubword(2, 33);

            /* argument out of range */
            subWord = word.getSubword(32, 5);
        }


        [TestMethod]
        public void CWord_signExtendSigned()
        {
            CWord arg = new CWord(-5);
            arg = arg.getSubword(16, 16);
            arg.signExtendSigned();
            Assert.AreEqual("FFFFFFFB", arg.getHexadecimal);
            Assert.AreEqual(-5, arg.getSignedDecimal);

            arg = new CWord(5);
            arg.signExtendSigned();
            Assert.AreEqual("00000005", arg.getHexadecimal);
        }
    }
}
