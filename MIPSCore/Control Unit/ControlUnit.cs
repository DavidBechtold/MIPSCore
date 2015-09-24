using System;
using MIPSCore.InstructionSet;
using MIPSCore.Instruction_Memory;

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

            // 2.) interpret the opCode and function
            try
            {
                PrepareControlSignals(opCode, function);
            }
            catch (ArgumentOutOfRangeException)
            {
                // TODO make own exception
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
