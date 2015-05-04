using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Clock;
using MIPSCore.Util;
using MIPSCore.ALU;
using MIPSCore.InstructionMemory;
using MIPSCore.ControlUnit;
using MIPSCore.RegisterFile;

namespace MIPSCore.Core
{
    public class CCore
    {
        private CClock clock;
        private CInstructionMemory instructionMemory;
        private CInstructionFetch instructionFetch;
        private CRegisterFile regFile;
        private CALU alu;
        private CControlUnit controlUnit;

        private UInt16 address;

        public CCore()
        {
            instructionMemory = new CInstructionMemory();
            instructionFetch = new CInstructionFetch(this);
            regFile = new CRegisterFile(this);
            alu = new CALU(this);
            controlUnit = new CControlUnit(this);

            address = 0x000;
            clock = new CClock(1, clockTick);
        }

        public void startCore()
        {
            clock.start();
        }

        private void clockTick(object sender, EventArgs e)
        {
            clock.stop(); //for debugging and to avoid race conditions at the beginning stop the clock
            instructionMemory.setAddressOfNextInstruction(address);
            instructionFetch.clock();
            address += 4;
            clock.start();
        }

        public CInstructionMemory getInstructionMemory
        {
            get
            {
                return instructionMemory;
            }
        }

        public CInstructionFetch getInstructionFetch
        {
            get
            {
                return instructionFetch;
            }
        }
    }
}
