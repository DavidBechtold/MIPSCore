﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util;

namespace CMemoryTest
{
    [TestClass]
    public class UnitTest_CMemory
    {
        [TestMethod]
        public void CMemory_WriteAndRead()
        {
            CMemory mem = new CMemory(MemSize.Size_1kB);

            mem.writeByte(new CWord((UInt32)Byte.MaxValue), 0);
            mem.writeByte(new CWord((UInt32)127), 1023);
            mem.writeByte(new CWord((UInt32)170), 1024);
            mem.writeHalfWord(new CWord((UInt32)0xAA0B), 2048);
            mem.writeHalfWord(new CWord((UInt32)UInt16.MaxValue), 8190);

            mem.writeWord(new CWord(0xA0B0C0D0), 3021);
            mem.writeWord(new CWord(UInt32.MaxValue), 4102);

            Assert.AreEqual(Byte.MaxValue, mem.readByte(0).getSignedDecimal);
            Assert.AreEqual(127, mem.readByte(1023).getSignedDecimal);
            Assert.AreEqual(0xAA, mem.readByte(2048).getSignedDecimal);
            Assert.AreEqual(0x0B, mem.readByte(2049).getSignedDecimal);
            Assert.AreEqual(Byte.MaxValue, mem.readByte(8190).getSignedDecimal);
            Assert.AreEqual(Byte.MaxValue, mem.readByte(8191).getSignedDecimal);
            Assert.AreEqual(0xC0, mem.readByte(3023).getSignedDecimal);


            Assert.AreEqual(65280, mem.readHalfWord(0).getSignedDecimal);
            Assert.AreEqual(32682, mem.readHalfWord(1023).getSignedDecimal);
            Assert.AreEqual(0xAA0B, mem.readHalfWord(2048).getSignedDecimal);
            Assert.AreEqual(UInt16.MaxValue, mem.readHalfWord(8190).getSignedDecimal);
            Assert.AreEqual(0xC0D0, mem.readHalfWord(3023).getSignedDecimal);

            Assert.AreEqual(4278190080, mem.readWord(0).getUnsignedDecimal);
            Assert.AreEqual(0xA0B0C0D0, mem.readWord(3021).getUnsignedDecimal);
            Assert.AreEqual((UInt32) 0x0000A0B0, mem.readWord(3019).getUnsignedDecimal);
            Assert.AreEqual((UInt32)UInt32.MaxValue, mem.readWord(4102).getUnsignedDecimal);
        }
    }
}
