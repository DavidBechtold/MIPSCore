using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util.MIPSEventArgs;
using System.Threading;

namespace SystemTest
{
    [TestClass]
    public class Fibonacci
    {
        [TestMethod]
        public void SytemTest_fibonacci()
        {
            var finished = new ManualResetEvent(false);

            IMipsCore core = new MipsCore();

            core.SetMode(ExecutionMode.RunToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//fibonacci.objdump"));
            core.ProgramObjdump("Testcode//fibonacci.objdump");



            core.Exception += (sender, e) =>
            {
                MIPSEventArgs args = (MIPSEventArgs)e;
                Assert.Fail(args.Message);
            };
            core.Completed += delegate {
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(0));
                Assert.AreEqual((uint) 1, core.ReadRegisterUnsigned(1));
                Assert.AreEqual((uint) 10, core.ReadRegisterUnsigned(2));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(3));
                Assert.AreEqual((uint) 4, core.ReadRegisterUnsigned(4));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(5));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(6));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(7));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(8));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(9));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(10));
                Assert.AreEqual((uint) 3, core.ReadRegisterUnsigned(11));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(12));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(13));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(14));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(15));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(16));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(17));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(18));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(19));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(20));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(21));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(22));
                Assert.AreEqual((uint) 2, core.ReadRegisterUnsigned(23));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(24));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(25));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(26));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(27));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(28));
                Assert.AreEqual(core.DataMemorySizeBytes(), core.ReadRegisterUnsigned(29));
                Assert.AreEqual((uint) 0, core.ReadRegisterUnsigned(30));
                Assert.AreEqual((uint) 8, core.ReadRegisterUnsigned(31));
                finished.Set();
            };

            core.Clocked += delegate
            {
            };

            core.StartCore();
            Assert.IsTrue(finished.WaitOne(10000));
        }
    }
}
