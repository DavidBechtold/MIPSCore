using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;

namespace MIPSCore.ALU
{
    class CALU
    {
        private bool zero;
        private CCore core;

        public CALU(CCore core)
        {
            this.core = core;
            zero = false;
        }

        public void clock()
        {

        }

        public bool getZero
        {
            get
            {
                return zero;
            }
        }
    }
}
