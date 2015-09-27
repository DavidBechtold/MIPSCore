﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using System.Threading;

namespace SystemTest
{
    [TestClass]
    public class Insertionsort
    {
        [TestMethod]
        [Timeout(25000)]
        public void SystemTest_insertionsort()
        {
            var finished = new ManualResetEvent(false);

            IMipsCore core = new MipsCore();

            core.SetMode(ExecutionMode.RunToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//insertionsort.objdump"));
            core.ProgramObjdump("Testcode//insertionsort.objdump");

            core.Exception += delegate { Assert.Fail(core.GetExceptionString()); };
            core.Completed += delegate
            {
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(0));
                Assert.AreEqual((uint)1, core.ReadRegisterUnsigned(1));
                Assert.AreEqual((uint)10, core.ReadRegisterUnsigned(2));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(3));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(4));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(5));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(6));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(7));
                Assert.AreEqual((uint)15, core.ReadRegisterUnsigned(8));
                Assert.AreEqual(-1, core.ReadRegister(9));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(10));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(11));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(12));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(13));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(14));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(15));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(16));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(17));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(18));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(19));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(20));
                Assert.AreEqual((uint)56, core.ReadRegisterUnsigned(21));
                Assert.AreEqual((uint)56, core.ReadRegisterUnsigned(22));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(23));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(24));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(25));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(26));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(27));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(28));
                Assert.AreEqual(core.DataMemorySizeBytes(), core.ReadRegisterUnsigned(29));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(30));
                Assert.AreEqual((uint)0, core.ReadRegisterUnsigned(31));

                Assert.AreEqual(1, core.ReadWordDataMemory(0));
                Assert.AreEqual(2, core.ReadWordDataMemory(4));
                Assert.AreEqual(3, core.ReadWordDataMemory(8));
                Assert.AreEqual(4, core.ReadWordDataMemory(12));
                Assert.AreEqual(5, core.ReadWordDataMemory(16));
                Assert.AreEqual(6, core.ReadWordDataMemory(20));
                Assert.AreEqual(7, core.ReadWordDataMemory(24));
                Assert.AreEqual(8, core.ReadWordDataMemory(28));
                Assert.AreEqual(9, core.ReadWordDataMemory(32));
                Assert.AreEqual(10, core.ReadWordDataMemory(36));
                Assert.AreEqual(11, core.ReadWordDataMemory(40));
                Assert.AreEqual(12, core.ReadWordDataMemory(44));
                Assert.AreEqual(13, core.ReadWordDataMemory(48));
                Assert.AreEqual(14, core.ReadWordDataMemory(52));
                Assert.AreEqual(15, core.ReadWordDataMemory(56));
                Assert.AreEqual((-1), core.ReadWordDataMemory(60));
                finished.Set();
            };

            core.Clocked += delegate
            {
            };

            core.StartCore();
            Assert.IsTrue(finished.WaitOne(100000000));
        }
    }
}
