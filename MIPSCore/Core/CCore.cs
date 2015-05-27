using System;
using System.IO;
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
        public const UInt16 CCoreWordLength = CWord.wordLength;

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

        public void programCore(string path)
        {
            string[] code = System.IO.File.ReadAllLines(path);

            if (code.Length >= CInstructionMemory.endAddress)
                throw new IndexOutOfRangeException("Codelength is greater than " + CInstructionMemory.endAddress + ".");

            for (UInt16 i = 0; i < code.Length; i++)
                instructionMemory.writeWord(i, new CWord(Convert.ToUInt32(code[i], 16)));
        }

        private void clockTick(object sender, EventArgs e)
        {
            //for debugging and to avoid race conditions at the beginning stop the clock,
            //if we don't stop the clock here, it can happen that the clock interrupt interrupts the next instructions (specially at debugging)
            clock.stop(); 
            instructionMemory.setAddressOfNextInstruction(address);
            instructionFetch.clock();
            controlUnit.clock();
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
