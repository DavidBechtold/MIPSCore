using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util;
using MIPSCore.InstructionMemory;

namespace CInstructionMemoryTest
{
    [TestClass]
    public class UnitTest_InstructionMemory
    {
        [TestMethod]
        public void CInstuctionMemory_WriteAndReadWords()
        {
            CInstructionMemory memory = new CInstructionMemory();
            /* write image into the instruction memory and read it */
            for (UInt32 i = 0; i < CInstructionMemory.endAddress; i++)
            {
                memory.writeWord((UInt16) i, i);
                Assert.AreEqual(memory.getInstructionInvalidAddress, false);
            }

            for (UInt32 i = 0; i < CInstructionMemory.endAddress; i++)
            {
                CWord word = memory.readWord((UInt16)i);
                Assert.AreEqual(memory.getInstructionInvalidAddress, false);
                Assert.AreEqual(word.getUnsignedDecimal, i);
            }
        }

        [TestMethod]
        public void CInstuctionMemory_OutOfBoundWriteAndRead()
        {
            CInstructionMemory memory = new CInstructionMemory();
            memory.writeWord(CInstructionMemory.endAddress + 1, 100);
            Assert.AreEqual(memory.getInstructionInvalidAddress, true);

            memory = new CInstructionMemory();
            memory.readWord(CInstructionMemory.endAddress + 1);
            Assert.AreEqual(memory.getInstructionInvalidAddress, true);
        }
    }
}
