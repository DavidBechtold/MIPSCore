using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Util;

namespace MIPSCore.InstructionMemory
{
    public class CInstructionMemory
    {
        /* Provide 1kB of word addressed instruction memory */
        public const UInt16 startAddress = 0x000;
        public const UInt16 endAddress = 0x3FF;
      
        private CWord[] instructionMemory;
        private bool invalidAddress;
        private UInt16 nextAddressRead;


        public CInstructionMemory()
        {
            invalidAddress = false;
            nextAddressRead = 0x000;
            instructionMemory = new CWord[endAddress + 1];

            /* initialise instruction memory with zero's */
            initialiseInstructionMemory();
        }

        public CWord readWord(UInt16 address)
        {
            if (address > endAddress)
            {
                invalidAddress = true;
                return new CWord(0);
            }
            invalidAddress = false;
            return instructionMemory[address];
        }

        public void setAddressOfNextInstruction(UInt16 address)
        {
            if (address > endAddress)
            {
                invalidAddress = true;
                return;
            }
            invalidAddress = false;
            nextAddressRead = address;
        }

        public CWord readNextInstuction()
        {
            invalidAddress = false;
            return instructionMemory[nextAddressRead];
        }

        public void writeWord(UInt16 address, CWord word)
        {
            if (address > endAddress)
            {
                invalidAddress = true;
                return;
            }
            invalidAddress = false;
            instructionMemory[address] = word;
        }

        public bool getInstructionInvalidAddress
        {
            get
            {
                return invalidAddress;
            }
        }

        private void initialiseInstructionMemory()
        {
            for (UInt32 i = 0; i < CInstructionMemory.endAddress; i++)
                this.writeWord((UInt16)i, new CWord(0));
        }
    }
}
