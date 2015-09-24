using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore.Util;

namespace Tests
{
    [TestClass]
    public class UnitTestCmipsRegister
    {
        [TestMethod]
        public void MipsRegister_write()
        {
            var register = new MipsRegister();

            register.Write(0, 1);
            register.Write(31, uint.MaxValue);

            Assert.AreEqual(uint.MaxValue, register.ReadUnsingedDecimal(31));
            register.Flush();

            Assert.AreEqual(0, register.ReadSingedDecimal(1));
            Assert.AreEqual(0, register.ReadSingedDecimal(31));
        }
    }
}
