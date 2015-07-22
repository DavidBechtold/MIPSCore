using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using System.Threading;

namespace SystemTest
{
    [TestClass]
    public class GreatestCommonDivisor
    {
        [TestMethod]
        public void SystemTest_GreatestCommonDivisor()
        {
            var finished = new ManualResetEvent(false);

            ICore core = new CCore();

            core.setMode(ExecutionMode.runToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//gcd.objdump"));
            core.programObjdump("Testcode//gcd.objdump");

            core.exception += delegate
            {
                Assert.Fail(core.getExceptionString());
            };
            core.completed += delegate
            {
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(0));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(1));
                Assert.AreEqual((UInt32)10, core.readRegisterUnsigned(2));
                Assert.AreEqual((UInt32)8, core.readRegisterUnsigned(3));
                Assert.AreEqual((UInt32)8, core.readRegisterUnsigned(4));
                Assert.AreEqual((UInt32)8, core.readRegisterUnsigned(5));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(6));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(7));
                Assert.AreEqual((UInt32)1, core.readRegisterUnsigned(8));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(9));
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
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(22));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(23));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(24));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(25));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(26));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(27));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(28));
                Assert.AreEqual((UInt32)core.dataMemorySizeBytes(), core.readRegisterUnsigned(29));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(30));
                Assert.AreEqual((UInt32)0, core.readRegisterUnsigned(31));

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
