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
    public class CControlUnit
    {
        private CCore core;
        private CControlSignals signals;

        public CControlUnit(CCore core)
        {
            this.core = core;
        }

        public void clock()
        {
            // 1.) take the input from the instruction fetch
            CWord opCode = core.getInstructionFetch.getOpCode;
            CWord function = core.getInstructionFetch.getFunction;

            // 2.) interpret the opCode and function
            signals = new CControlSignals(opCode, function);       
        }

        /*** SETTER | GETTER ***/
        public RegisterDestination getRegDestination { get { return getRegDestination; } }
        public ALUSource getAluSource { get { return signals.getAluSource; } }
        public ALUControl getAluControl { get { return signals.getAluControl; } }
        public bool getRegWrite { get { return signals.getRegWrite; } }
        public bool getMemWrite { get { return signals.getMemWrite; } }
        public bool getMemRead { get { return signals.getMemRead; } }
        public bool getMemToReg { get { return signals.getMemToReg; } }
        public ProgramCounterSource getPcSource { get { return signals.getPcSource; } }
    }
}
