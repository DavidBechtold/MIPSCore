using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using System.Threading;

namespace SystemTest
{
    [TestClass]
    public class Bubblesort
    {
        [TestMethod]
        [Timeout(25000)] 
        public void SystemTest_bubblesort()
        {
            var finished = new ManualResetEvent(false);

            IMipsCore core = new MipsCore();

            core.SetMode(ExecutionMode.RunToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//bubblesort.objdump"));
            core.ProgramObjdump("Testcode//bubblesort.objdump");

            core.Exception += delegate
            {
                Assert.Fail(core.GetExceptionString());
            };
            core.Completed += delegate
            {
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(0));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(1));
                Assert.AreEqual((UInt32)10, core.ReadRegisterUnsigned(2));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(3));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(4));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(5));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(6));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(7));
                Assert.AreEqual((UInt32)123, core.ReadRegisterUnsigned(8));
                Assert.AreEqual((Int32) (-1), core.ReadRegister(9));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(10));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(11));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(12));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(13));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(14));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(15));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(16));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(17));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(18));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(19));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(20));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(21));
                Assert.AreEqual((UInt32)48, core.ReadRegisterUnsigned(22));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(23));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(24));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(25));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(26));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(27));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(28));
                Assert.AreEqual((UInt32)core.DataMemorySizeBytes(), core.ReadRegisterUnsigned(29));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(30));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(31));

                Assert.AreEqual(1, core.ReadWordDataMemory(0));
                Assert.AreEqual(2, core.ReadWordDataMemory(4));
                Assert.AreEqual(2, core.ReadWordDataMemory(8));
                Assert.AreEqual(2, core.ReadWordDataMemory(12));
                Assert.AreEqual(3, core.ReadWordDataMemory(16));
                Assert.AreEqual(4, core.ReadWordDataMemory(20));
                Assert.AreEqual(4, core.ReadWordDataMemory(24));
                Assert.AreEqual(4, core.ReadWordDataMemory(28));
                Assert.AreEqual(5, core.ReadWordDataMemory(32));
                Assert.AreEqual(8, core.ReadWordDataMemory(36));
                Assert.AreEqual(86, core.ReadWordDataMemory(40));
                Assert.AreEqual(95, core.ReadWordDataMemory(44));
                Assert.AreEqual(123, core.ReadWordDataMemory(48));
                Assert.AreEqual((-1), core.ReadWordDataMemory(52));
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
