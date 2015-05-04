using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;
using MIPSCore.Util;

namespace MIPSCore.RegisterFile
{
    public class CRegisterFile
    {
        private CCore core;

        /* register */
        private CMIPSRegister registers;

        public CRegisterFile(CCore core)
        {
            this.core = core;
            registers = new CMIPSRegister();
        }

        public void clock()
        {
           
        }
    }
}
