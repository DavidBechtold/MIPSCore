using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIPSCore;
using MIPSCore.Util;


namespace CMIPSRegisterTest
{
    [TestClass]
    public class UnitTest_CMIPSRegister
    {
        [TestMethod]
        public void CMIPSRegister_write()
        {
            CMIPSRegister register = new CMIPSRegister();

            register.write(0, 1);
            register.write(31, UInt32.MaxValue);

            Assert.AreEqual(UInt32.MaxValue, register.readUnsingedDecimal(31));
            register.flush();

            Assert.AreEqual(0, register.readSingedDecimal(1));
            Assert.AreEqual(0, register.readSingedDecimal(31));
        }
    }
}
