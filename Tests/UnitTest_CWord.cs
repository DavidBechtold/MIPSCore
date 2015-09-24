using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore.Util;

namespace Tests
{
    [TestClass]
    public class UnitTestCWord
    {
        [TestMethod]
        public void CWord_Constructor_SignedInteger()
        {
            Word arg = new Word(int.MaxValue);
            Assert.AreEqual("0".PadRight(32, '1'), arg.Binary);
            Assert.AreEqual("7FFFFFFF", arg.Hexadecimal);
            Assert.AreEqual((uint) int.MaxValue, arg.UnsignedDecimal);

            arg = new Word(int.MinValue);
            Assert.AreEqual("1".PadRight(32, '0'), arg.Binary);
            Assert.AreEqual("80000000", arg.Hexadecimal);
            Assert.AreEqual((uint)int.MaxValue + 1, arg.UnsignedDecimal);
        }

        [TestMethod]
        public void CWord_Constructor_UnsignedInteger()
        {
            Word arg = new Word(uint.MaxValue);
            Assert.AreEqual("1".PadRight(32, '1'), arg.Binary);
            Assert.AreEqual("FFFFFFFF", arg.Hexadecimal);
            Assert.AreEqual(-1, arg.SignedDecimal);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Binary strings > 32 length are not allowed.")]
        public void CWord_Constructor_Binary()
        {
            Word arg = new Word("00000000000000000000000000000001");
            Assert.AreEqual("1".PadLeft(32, '0'), arg.Binary);
            Assert.AreEqual("00000001", arg.Hexadecimal);
            Assert.AreEqual(1, arg.SignedDecimal);
            Assert.AreEqual((uint) 1, arg.UnsignedDecimal);

            arg = new Word("11111111111111111111111111111111");
            Assert.AreEqual("1".PadLeft(32, '1'), arg.Binary);
            Assert.AreEqual("FFFFFFFF", arg.Hexadecimal);
            Assert.AreEqual(-1, arg.SignedDecimal);
            Assert.AreEqual(uint.MaxValue, arg.UnsignedDecimal);

            /* check if padding works correct => we want always a string which has the length of 32 (because of substrings) */
            arg = new Word("11111111");
            Assert.AreEqual("11111111".PadLeft(32, '0'), arg.Binary);
            Assert.AreEqual("000000FF", arg.Hexadecimal);
            Assert.AreEqual(255, arg.SignedDecimal);
            Assert.AreEqual((uint) 255, arg.UnsignedDecimal);

            /* argument out of range exeption */
            // ReSharper disable once RedundantAssignment
            arg = new Word("111111111111111111111111111111111");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException), "Binary strings only contains 0 and 1's.")]
        public void CWord_Constructor_Binary_FormatException()
        {
            /* format exception */
            // ReSharper disable once UnusedVariable
            Word word = new Word("abcd");
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException), "Addition of signed and unsigned values are not allowed.")]
        public void CWord_Plus_Operator()
        {
            Word arg1 = new Word(-4);
            Word arg2 = new Word(5);

            Word arg3 = arg1 + arg2;

            Assert.AreEqual("1".PadLeft(32, '0'), arg3.Binary);
            Assert.AreEqual("00000001", arg3.Hexadecimal);

            arg1 = new Word((uint) 4);
            // ReSharper disable once RedundantAssignment
            arg3 = arg1 + arg2;
        }

        [TestMethod]
        [ExpectedException(typeof(ArithmeticException), "Addition of signed and unsigned values are not allowed.")]
        public void CWord_Minus_Operator()
        {
            Word arg1 = new Word(4);
            Word arg2 = new Word(5);

            Word arg3 = arg1 - arg2;

            Assert.AreEqual("1".PadLeft(32, '1'), arg3.Binary);
            Assert.AreEqual("FFFFFFFF", arg3.Hexadecimal);

            arg1 = new Word((uint)4);
            // ReSharper disable once RedundantAssignment
            arg3 = arg1 - arg2;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Requesting a subword with an start > 32 or an length > 32 is not allowed.")]
        public void CWord_getSubwordFromBinary()
        {
            Word word = new Word("00001111000011111111000010011111");
            Word subWord = word.GetSubword(2, 6);
            Assert.AreEqual("001111".PadLeft(32, '0'), subWord.Binary);
            Assert.AreEqual(15, subWord.SignedDecimal);
            Assert.AreEqual((ushort)15, subWord.UnsignedDecimal);
            Assert.AreEqual("00001111000011111111000010011111", word.Binary);

            word = new Word("1111");
            subWord = word.GetSubword(5, 10);
            Assert.AreEqual("0".PadLeft(32, '0'), subWord.Binary);
            Assert.AreEqual(0, subWord.SignedDecimal);
            Assert.AreEqual((ushort) 0, subWord.UnsignedDecimal);
            Assert.AreEqual("1111".PadLeft(32, '0'), word.Binary);

            /* argument out of range */
            // ReSharper disable once RedundantAssignment
            subWord = word.GetSubword(2, 33);

            /* argument out of range */
            // ReSharper disable once RedundantAssignment
            subWord = word.GetSubword(32, 5);
        }

        [TestMethod]
        public void CWord_signExtendSigned()
        {
            Word arg = new Word(-5);
            arg = arg.GetSubword(16, 16);
            arg.SignExtendSigned();
            Assert.AreEqual("FFFFFFFB", arg.Hexadecimal);
            Assert.AreEqual(-5, arg.SignedDecimal);

            arg = new Word(5);
            arg.SignExtendSigned();
            Assert.AreEqual("00000005", arg.Hexadecimal);
        }
    }
}
