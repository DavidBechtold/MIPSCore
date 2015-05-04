using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;
using MIPSCore.Util;
using MIPSCore.InstructionMemory;

namespace MIPSCore.ControlUnit
{
    class CControlUnit
    {
        public enum RegisterDestination {rt, rs};
        public enum ALUSource { regFile, signExtend }
        public enum ALUControl {  add, subtract, and, or, setOnLessThan}
        public enum ProgramCounterSource { signExtend, programCounter}

        /* control signals */
        private RegisterDestination     regDestination; //which register will be written
        private ALUSource               aluSource;      //alu take the value fom regFile or from the sign extender (immediate cmd)
        private ALUControl              aluControl;     //which operation the alu should perform
        private bool                    regWrite;       //true => write register | false => no register to write (jmp, beq commands)
        private bool                    memWrite;       //true => write memory
        private bool                    memRead;        //true => read memory
        private bool                    memToReg;       //true => write memory content to register
        private ProgramCounterSource    pcSource;       //take the source from the programcounter or from the sign extender (jmp,.. instruction)

        private CCore core;

        public CControlUnit(CCore core)
        {
            this.core = core;

            regDestination = RegisterDestination.rt;
            aluSource = ALUSource.regFile;
            aluControl = ALUControl.add;
            regWrite = false;
            memWrite = false;
            memRead = false;
            memToReg = false;
            pcSource = ProgramCounterSource.programCounter;
        }

        public void clock()
        {
            // 1.) take the input from the instruction fetch
            CWord opCode = core.getInstructionFetch.getOpCode;
            CWord function = core.getInstructionFetch.getFunction;

            // 2.) interpret the opCode
            // TODO make opcode classes => make a toString function
            
        }
    }
}
