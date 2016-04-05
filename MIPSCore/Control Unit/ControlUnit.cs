using System;
using MIPSCore.Instruction_Memory;
using MIPSCore.Instruction_Set;

namespace MIPSCore.Control_Unit
{
    public class ControlUnit : ControlSignals, IControlUnit
    {
        public IInstructionMemory InstructionMemory { get; set; }

        public void Clock()
        {
            // 1.) take the input from the instruction fetch
            var opCode = InstructionMemory.GetOpCode;
            var function = InstructionMemory.GetFunction;
            var rd = InstructionMemory.GetRd;

            // 2.) interpret the opCode and function
            try
            {
                PrepareControlSignals(opCode, function, rd);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
