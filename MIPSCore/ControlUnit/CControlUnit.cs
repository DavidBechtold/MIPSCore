using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;
using MIPSCore.Util;
using MIPSCore.InstructionMemory;
using MIPSCore.InstructionSet;

namespace MIPSCore.ControlUnit
{
    public class CControlUnit : CControlSignals
    {
        private CCore core;

        public CControlUnit(CCore core)
            : base()
        {
            this.core = core;
        }

        public void clock()
        {
            // 1.) take the input from the instruction fetch
            CWord opCode = core.getInstructionFetch.getOpCode;
            CWord function = core.getInstructionFetch.getFunction;

            // 2.) interpret the opCode and function
            base.prepareControlSignals(opCode, function);       
        }
    }
}
