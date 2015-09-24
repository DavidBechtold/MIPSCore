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

            IMipsCore core = new MipsCore();

            core.SetMode(ExecutionMode.RunToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//gcd.objdump"));
            core.ProgramObjdump("Testcode//gcd.objdump");

            core.Exception += delegate
            {
                Assert.Fail(core.GetExceptionString());
            };
            core.Completed += delegate
            {
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(0));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(1));
                Assert.AreEqual((UInt32)10, core.ReadRegisterUnsigned(2));
                Assert.AreEqual((UInt32)8, core.ReadRegisterUnsigned(3));
                Assert.AreEqual((UInt32)8, core.ReadRegisterUnsigned(4));
                Assert.AreEqual((UInt32)8, core.ReadRegisterUnsigned(5));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(6));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(7));
                Assert.AreEqual((UInt32)1, core.ReadRegisterUnsigned(8));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(9));
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
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(22));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(23));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(24));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(25));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(26));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(27));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(28));
                Assert.AreEqual((UInt32)core.DataMemorySizeBytes(), core.ReadRegisterUnsigned(29));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(30));
                Assert.AreEqual((UInt32)0, core.ReadRegisterUnsigned(31));

                finished.Set();
            };

            core.Clocked += delegate
            {
            };

            core.StartCore();
            Assert.IsTrue(finished.WaitOne(10000000));
        }
    }
}
