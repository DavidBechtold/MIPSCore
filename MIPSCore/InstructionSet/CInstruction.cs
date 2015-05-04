using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.InstructionSet
{
    class CInstruction
    {
        private UInt16 opCode;
        private UInt16 function;
        private string instruction;

        public CInstruction(UInt16 opCode, UInt16 function, string instruction)
        {
            this.opCode = opCode;
            this.function = function;
            this.instruction = instruction;
        }

        public override string ToString()
        {
            return instruction + " OpCode: " + opCode + " | Function: " + function; 
        }
    }
}
