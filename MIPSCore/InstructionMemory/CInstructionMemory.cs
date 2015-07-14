using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;
using MIPSCore.InstructionSet;
using MIPSCore.Util;

namespace MIPSCore.InstructionMemory
{
    public class CInstructionMemory
    {
        private CCore core;
        private CMemory memory;
        private bool firstCommand;
        private CWord nextInstruction;
        private CWord programCounter;

        /* arguments for the control unit and register file */
        private CWord opCode;
        private CWord function;
        private CWord shiftAmount;
        private CWord immediate;
        private CWord rd; //destination register
        private CWord rs; //first source register
        private CWord rt; //second source register
        private CWord jumpTarget;

        public CInstructionMemory(CCore core, MemSize size)
        {
            this.core = core;
            programCounter = new CWord(0);
            nextInstruction = new CWord(0);
            memory = new CMemory(size);
            
            firstCommand = true;
        }

        public void clock()
        {
            // 1.) calculate new programcounter
            calcProgramCounter();

            // 2.) get instruction from memory to execute
            nextInstruction = readNextInstuction();

            // 3.) divide instruction for the register file and the control unit
            opCode = nextInstruction.getSubword(0, 6);
            rs = nextInstruction.getSubword(6, 5);
            rt = nextInstruction.getSubword(6 + 5, 5);

            /* R Format */
            function = nextInstruction.getSubword(32 - 6, 6);
            shiftAmount = nextInstruction.getSubword(6 + 5 + 5 + 5, 5);
            rd = nextInstruction.getSubword(6 + 5 + 5, 5);

            /* I Format */
            immediate = nextInstruction.getSubword(16, 16);

            /* J Format */
            jumpTarget = nextInstruction.getSubword(6, 26);
        }

        private void calcProgramCounter()
        {
            switch (core.getControlUnit.getPcSource)
            {
                case ProgramCounterSource.programCounter:
                    if (firstCommand)
                        firstCommand = false;
                    else
                        programCounter += 4;
                    break;
                case ProgramCounterSource.signExtendEqual:
                    if(core.getAlu.zeroFlag)
                        programCounter += immediate * 4 + 4;
                    else
                        programCounter += 4;
                    break;
                case ProgramCounterSource.signExtendUnequal:
                    if (!core.getAlu.zeroFlag)
                        programCounter += immediate * 4 + 4;
                    else
                        programCounter += 4;
                    break;
                case ProgramCounterSource.signExtendLessOrEqualZero:
                    if (!core.getAlu.zeroFlag || core.getAlu.getResultLO.getUnsignedDecimal == 1)
                        programCounter += immediate * 4 + 4;
                    else
                        programCounter += 4;
                    break;
                case ProgramCounterSource.jump:
                    programCounter = jumpTarget * 4;
                    break;
                case ProgramCounterSource.register:
                    programCounter = core.getRegisterFile.readRs();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": ProgramCounterSource out of range");
            }
        }

        public void flush()
        {
            opCode = new CWord((UInt32)0);
            function = new CWord((UInt32)0);
            shiftAmount = new CWord((UInt32)0);
            immediate = new CWord((UInt32)0);
            rd = new CWord((UInt32)0);
            rs = new CWord((UInt32)0);
            rt = new CWord((UInt32)0);
            jumpTarget = new CWord((UInt32)0);
        }

        public CWord readNextInstuction()
        {
            return memory.readWord(programCounter);
        }

        public void programWord(CWord word, UInt32 address)
        {
            memory.writeWord(word, address);
        }

        public UInt32 getSize
        {
            get
            {
                return memory.getEndByteAddress;
            }
        }

        public CWord getOpCode
        {
            get
            {
                return opCode;
            }
        }

        public CWord getFunction
        {
            get
            {
                return function;
            }
        }

        public CWord getShiftAmount
        {
            get
            {
                return shiftAmount;
            }
        }

        public CWord getImmediate
        {
            get
            {
                return immediate;
            }
        }

        public CWord getRd
        {
            get
            {
                return rd;
            }
        }

        public CWord getRs
        {
            get
            {
                return rs;
            }
        }

        public CWord getRt
        {
            get
            {
                return rt;
            }
        }

        public CWord getJumpTarget
        {
            get
            {
                return jumpTarget;
            }
        }

        public CWord getProgramCounter
        {
            get
            {
                return programCounter;
            }
        }

        public CWord getActualInstruction
        {
            get
            {
                return nextInstruction;
            }
        }

        public CWord setAddressOffset
        {
            set
            {
                memory.setOffset = value;
            }
        }

        public string hexdump(UInt32 startaddress, UInt32 bytesToRead)
        {
            return memory.hexdump(startaddress, bytesToRead);
        }
    }
}
