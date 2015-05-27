using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore;
using MIPSCore.InstructionMemory;
using MIPSCore.Util;
using MIPSCore.Core;

namespace MIPSCore.InstructionMemory
{
    public class CInstructionFetch
    {
        private CCore core;
        private CWord nextInstruction;
        private bool stalled;
        
        /* arguments for the control unit and register file */
        private CWord opCode;
        private CWord function;
        private CWord shiftAmount;
        private CWord immediate;
        private CWord rd; //destination register
        private CWord rs; //first source register
        private CWord rt; //second source register
        private CWord jumpTarget;

        public CInstructionFetch(CCore core)
        {
            stalled = false;
            this.core = core;
            flush();
        }

        public void clock()
        {
            // 1.) check if stalled 
            if (stalled)
            {
                stalled = false;
                return;
            }
            
            // 2.) fetch next instruction from the instruction memory when not stalled
            nextInstruction = core.getInstructionMemory.readNextInstuction();

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

        public void flush()
        {
            opCode = new CWord((UInt32) 0);
            function = new CWord((UInt32)0);
            shiftAmount = new CWord((UInt32)0);
            immediate = new CWord((UInt32)0);
            rd = new CWord((UInt32)0);
            rs = new CWord((UInt32)0);
            rt = new CWord((UInt32)0);
            jumpTarget = new CWord((UInt32)0);
        }

        public void stallOneClock()
        {
            stalled = true;
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
                return immediate;
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
    }
}
