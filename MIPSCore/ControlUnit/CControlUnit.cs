using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore;
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
            CWord opCode = core.getInstructionMemory.getOpCode;
            CWord function = core.getInstructionMemory.getFunction;

            // 2.) interpret the opCode and function
            try
            {
                base.prepareControlSignals(opCode, function);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": ProgramCounter: " + core.programCounter() + " no instruction for opcode " + opCode.getUnsignedDecimal + " and function " + function.getUnsignedDecimal + " found");
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
