using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace CMemoryTest
{
    [TestClass]
    public class UnitTest_CMemory
    {
        [TestMethod]
        public void CMemory_WriteAndRead()
        {
            Memory mem = new Memory(MemSize.Size1Kb);

            mem.WriteByte(new CWord((UInt32)Byte.MaxValue), 0);
            mem.WriteByte(new CWord((UInt32)127), 1023);
            mem.WriteByte(new CWord((UInt32)170), 1024);
            mem.WriteHalfWord(new CWord((UInt32)0xAA0B), 2048);
            mem.WriteHalfWord(new CWord((UInt32)UInt16.MaxValue), 8190);

            mem.WriteWord(new CWord(0xA0B0C0D0), 3021);
            mem.WriteWord(new CWord(UInt32.MaxValue), 4102);

            Assert.AreEqual(Byte.MaxValue, mem.ReadByte(0).getSignedDecimal);
            Assert.AreEqual(127, mem.ReadByte(1023).getSignedDecimal);
            Assert.AreEqual(0xAA, mem.ReadByte(2048).getSignedDecimal);
            Assert.AreEqual(0x0B, mem.ReadByte(2049).getSignedDecimal);
            Assert.AreEqual(Byte.MaxValue, mem.ReadByte(8190).getSignedDecimal);
            Assert.AreEqual(Byte.MaxValue, mem.ReadByte(8191).getSignedDecimal);
            Assert.AreEqual(0xC0, mem.ReadByte(3023).getSignedDecimal);


            Assert.AreEqual(65280, mem.ReadHalfWord(0).getSignedDecimal);
            Assert.AreEqual(32682, mem.ReadHalfWord(1023).getSignedDecimal);
            Assert.AreEqual(0xAA0B, mem.ReadHalfWord(2048).getSignedDecimal);
            Assert.AreEqual(UInt16.MaxValue, mem.ReadHalfWord(8190).getSignedDecimal);
            Assert.AreEqual(0xC0D0, mem.ReadHalfWord(3023).getSignedDecimal);

            Assert.AreEqual(4278190080, mem.ReadWord(0).getUnsignedDecimal);
            Assert.AreEqual(0xA0B0C0D0, mem.ReadWord(3021).getUnsignedDecimal);
            Assert.AreEqual((UInt32) 0x0000A0B0, mem.ReadWord(3019).getUnsignedDecimal);
            Assert.AreEqual((UInt32)UInt32.MaxValue, mem.ReadWord(4102).getUnsignedDecimal);
        }

        [TestMethod]
        public void CMemory_WriteAndReadWithOffset()
        {
            Memory mem = new Memory(MemSize.Size1Kb);
            mem.Offset = new CWord(0x00400000);

            mem.WriteByte(new CWord((UInt32)Byte.MaxValue), 0x00400000);
            mem.WriteByte(new CWord((UInt32)127), 0x00400000 + 1023);
            mem.WriteByte(new CWord((UInt32)170), 0x00400000 + 1024);
            mem.WriteHalfWord(new CWord((UInt32)0xAA0B), 0x00400000 + 2048);
            mem.WriteHalfWord(new CWord((UInt32)UInt16.MaxValue), 0x00400000 + 8190);

            mem.WriteWord(new CWord(0xA0B0C0D0), 0x00400000 + 3021);
            mem.WriteWord(new CWord(UInt32.MaxValue), 0x00400000 + 4102);

            Assert.AreEqual(Byte.MaxValue, mem.ReadByte(0x00400000 + 0).getSignedDecimal);
            Assert.AreEqual(127, mem.ReadByte(0x00400000 + 1023).getSignedDecimal);
            Assert.AreEqual(0xAA, mem.ReadByte(0x00400000 + 2048).getSignedDecimal);
            Assert.AreEqual(0x0B, mem.ReadByte(0x00400000 + 2049).getSignedDecimal);
            Assert.AreEqual(Byte.MaxValue, mem.ReadByte(0x00400000 + 8190).getSignedDecimal);
            Assert.AreEqual(Byte.MaxValue, mem.ReadByte(0x00400000 + 8191).getSignedDecimal);
            Assert.AreEqual(0xC0, mem.ReadByte(0x00400000 + 3023).getSignedDecimal);


            Assert.AreEqual(65280, mem.ReadHalfWord(0x00400000 + 0).getSignedDecimal);
            Assert.AreEqual(32682, mem.ReadHalfWord(0x00400000 + 1023).getSignedDecimal);
            Assert.AreEqual(0xAA0B, mem.ReadHalfWord(0x00400000 + 2048).getSignedDecimal);
            Assert.AreEqual(UInt16.MaxValue, mem.ReadHalfWord(0x00400000 + 8190).getSignedDecimal);
            Assert.AreEqual(0xC0D0, mem.ReadHalfWord(0x00400000 + 3023).getSignedDecimal);

            Assert.AreEqual(4278190080, mem.ReadWord(0x00400000 + 0).getUnsignedDecimal);
            Assert.AreEqual(0xA0B0C0D0, mem.ReadWord(0x00400000 + 3021).getUnsignedDecimal);
            Assert.AreEqual((UInt32)0x0000A0B0, mem.ReadWord(0x00400000 + 3019).getUnsignedDecimal);
            Assert.AreEqual((UInt32)UInt32.MaxValue, mem.ReadWord(0x00400000 + 4102).getUnsignedDecimal);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CMemory_AddressOutOfBound_WriteByte()
        {
            Memory mem = new Memory(MemSize.Size4Kb);

            mem.WriteByte(new CWord(1), 4096*8);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CMemory_AddressOutOfBound_WriteHalfWord()
        {
            Memory mem = new Memory(MemSize.Size8Kb);

            mem.WriteHalfWord(new CWord(1), 8192 * 8 - 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CMemory_AddressOutOfBound_WriteWord()
        {
            Memory mem = new Memory(MemSize.Size8Kb);

            mem.WriteWord(new CWord(1), 8192 * 8 - 2);
        }
    }
}
