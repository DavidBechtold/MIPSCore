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
        [Timeout(20000)] 
        public void SystemTest_bubblesort()
        {
            var finished = new ManualResetEvent(false);

            ICore core = new CCore();

            core.setMode(ExecutionMode.runToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//bubblesort.objdump"));
            core.programObjdump("Testcode//bubblesort.objdump");

            core.exception += delegate
            {
                Assert.Fail(core.getExceptionString());
            };
            core.completed += delegate
            {
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(0));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(1));
                Assert.AreEqual((UInt32)10, core.readRegisterUnsigned(2));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(3));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(4));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(5));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(6));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(7));
                Assert.AreEqual((UInt32)123, core.readRegisterUnsigned(8));
                Assert.AreEqual((Int32) (-1), core.readRegister(9));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(10));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(11));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(12));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(13));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(14));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(15));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(16));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(17));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(18));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(19));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(20));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(21));
                Assert.AreEqual((UInt32)48, core.readRegisterUnsigned(22));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(23));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(24));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(25));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(26));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(27));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(28));
                Assert.AreEqual((UInt32)core.dataMemorySizeBytes(), core.readRegisterUnsigned(29));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(30));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(31));

                Assert.AreEqual(1, core.readWordDataMemory(0));
                Assert.AreEqual(2, core.readWordDataMemory(4));
                Assert.AreEqual(2, core.readWordDataMemory(8));
                Assert.AreEqual(2, core.readWordDataMemory(12));
                Assert.AreEqual(3, core.readWordDataMemory(16));
                Assert.AreEqual(4, core.readWordDataMemory(20));
                Assert.AreEqual(4, core.readWordDataMemory(24));
                Assert.AreEqual(4, core.readWordDataMemory(28));
                Assert.AreEqual(5, core.readWordDataMemory(32));
                Assert.AreEqual(8, core.readWordDataMemory(36));
                Assert.AreEqual(86, core.readWordDataMemory(40));
                Assert.AreEqual(95, core.readWordDataMemory(44));
                Assert.AreEqual(123, core.readWordDataMemory(48));
                Assert.AreEqual((-1), core.readWordDataMemory(52));
                finished.Set();
            };

            core.clocked += delegate
            {
            };

            core.startCore();
            Assert.IsTrue(finished.WaitOne(10000000));
        }
    }
}
