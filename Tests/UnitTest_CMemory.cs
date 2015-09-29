using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace Tests
{
    [TestClass]
    public class UnitTestCMemory
    {
        [TestMethod]
        public void Memory_WriteAndRead()
        {
            var mem = new Memory(MemorySize.Size1Kb);

            mem.WriteByte(new Word((uint)byte.MaxValue), 0);
            mem.WriteByte(new Word((uint)127), 1023);
            mem.WriteByte(new Word((uint)170), 1024);
            mem.WriteHalfWord(new Word((uint)0xAA0B), 2048);
            mem.WriteHalfWord(new Word((uint)ushort.MaxValue), 8190);

            mem.WriteWord(new Word(0xA0B0C0D0), 3021);
            mem.WriteWord(new Word(uint.MaxValue), 4102);

            Assert.AreEqual(byte.MaxValue, mem.ReadByte(0).SignedDecimal);
            Assert.AreEqual(127, mem.ReadByte(1023).SignedDecimal);
            Assert.AreEqual(0xAA, mem.ReadByte(2048).SignedDecimal);
            Assert.AreEqual(0x0B, mem.ReadByte(2049).SignedDecimal);
            Assert.AreEqual(byte.MaxValue, mem.ReadByte(8190).SignedDecimal);
            Assert.AreEqual(byte.MaxValue, mem.ReadByte(8191).SignedDecimal);
            Assert.AreEqual(0xC0, mem.ReadByte(3023).SignedDecimal);


            Assert.AreEqual(65280, mem.ReadHalfWord(0).SignedDecimal);
            Assert.AreEqual(32682, mem.ReadHalfWord(1023).SignedDecimal);
            Assert.AreEqual(0xAA0B, mem.ReadHalfWord(2048).SignedDecimal);
            Assert.AreEqual(ushort.MaxValue, mem.ReadHalfWord(8190).SignedDecimal);
            Assert.AreEqual(0xC0D0, mem.ReadHalfWord(3023).SignedDecimal);

            Assert.AreEqual(4278190080, mem.ReadWord(0).UnsignedDecimal);
            Assert.AreEqual(0xA0B0C0D0, mem.ReadWord(3021).UnsignedDecimal);
            Assert.AreEqual((uint) 0x0000A0B0, mem.ReadWord(3019).UnsignedDecimal);
            Assert.AreEqual(uint.MaxValue, mem.ReadWord(4102).UnsignedDecimal);
        }

        [TestMethod]
        public void Memory_WriteAndReadWithOffset()
        {
            var mem = new Memory(MemorySize.Size1Kb) {Offset = new Word(0x00400000)};

            mem.WriteByte(new Word((uint)byte.MaxValue), 0x00400000);
            mem.WriteByte(new Word((uint)127), 0x00400000 + 1023);
            mem.WriteByte(new Word((uint)170), 0x00400000 + 1024);
            mem.WriteHalfWord(new Word((uint)0xAA0B), 0x00400000 + 2048);
            mem.WriteHalfWord(new Word((uint)ushort.MaxValue), 0x00400000 + 8190);

            mem.WriteWord(new Word(0xA0B0C0D0), 0x00400000 + 3021);
            mem.WriteWord(new Word(uint.MaxValue), 0x00400000 + 4102);

            Assert.AreEqual(byte.MaxValue, mem.ReadByte(0x00400000 + 0).SignedDecimal);
            Assert.AreEqual(127, mem.ReadByte(0x00400000 + 1023).SignedDecimal);
            Assert.AreEqual(0xAA, mem.ReadByte(0x00400000 + 2048).SignedDecimal);
            Assert.AreEqual(0x0B, mem.ReadByte(0x00400000 + 2049).SignedDecimal);
            Assert.AreEqual(byte.MaxValue, mem.ReadByte(0x00400000 + 8190).SignedDecimal);
            Assert.AreEqual(byte.MaxValue, mem.ReadByte(0x00400000 + 8191).SignedDecimal);
            Assert.AreEqual(0xC0, mem.ReadByte(0x00400000 + 3023).SignedDecimal);


            Assert.AreEqual(65280, mem.ReadHalfWord(0x00400000 + 0).SignedDecimal);
            Assert.AreEqual(32682, mem.ReadHalfWord(0x00400000 + 1023).SignedDecimal);
            Assert.AreEqual(0xAA0B, mem.ReadHalfWord(0x00400000 + 2048).SignedDecimal);
            Assert.AreEqual(ushort.MaxValue, mem.ReadHalfWord(0x00400000 + 8190).SignedDecimal);
            Assert.AreEqual(0xC0D0, mem.ReadHalfWord(0x00400000 + 3023).SignedDecimal);

            Assert.AreEqual(4278190080, mem.ReadWord(0x00400000 + 0).UnsignedDecimal);
            Assert.AreEqual(0xA0B0C0D0, mem.ReadWord(0x00400000 + 3021).UnsignedDecimal);
            Assert.AreEqual((uint)0x0000A0B0, mem.ReadWord(0x00400000 + 3019).UnsignedDecimal);
            Assert.AreEqual(uint.MaxValue, mem.ReadWord(0x00400000 + 4102).UnsignedDecimal);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Memory_AddressOutOfBound_WriteByte()
        {
            var mem = new Memory(MemorySize.Size4Kb);
            mem.WriteByte(new Word(1), 4096*8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Memory_AddressOutOfBound_WriteHalfWord()
        {
            var mem = new Memory(MemorySize.Size8Kb);
            mem.WriteHalfWord(new Word(1), 8192 * 8 - 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Memory_AddressOutOfBound_WriteWord()
        {
            var mem = new Memory(MemorySize.Size8Kb);
            mem.WriteWord(new Word(1), 8192 * 8 - 2);
        }
    }
}
