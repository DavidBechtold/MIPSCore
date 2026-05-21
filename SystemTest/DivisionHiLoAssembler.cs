using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util.MIPSEventArgs;
using System.Threading;

namespace SystemTest
{
    [TestClass]
    public class DivisionHiLoAssembler
    {
        [TestMethod]
        [Timeout(60000)]
        public void SystemTest_division_hi_lo_assembler()
        {
            var finished = new ManualResetEvent(false);

            IMipsCore core = new MipsCore();

            core.SetMode(ExecutionMode.RunToCompletion);
            Assert.IsTrue(System.IO.File.Exists("Testcode//division_hilo.objdump"));
            core.ProgramObjdump("Testcode//division_hilo.objdump");

            core.Exception += (sender, e) =>
            {
                MIPSEventArgs args = (MIPSEventArgs)e;
                Assert.Fail(args.Message);
            };

            core.Completed += delegate
            {
                Assert.AreEqual((uint)10, core.ReadRegisterUnsigned(8));
                Assert.AreEqual((uint)3, core.ReadRegisterUnsigned(9));
                Assert.AreEqual((uint)3, core.ReadRegisterUnsigned(12));
                Assert.AreEqual((uint)1, core.ReadRegisterUnsigned(10));
                Assert.AreEqual((uint)3, core.ReadRegisterUnsigned(11));
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
