using MIPSCore.InstructionSet;
using MIPSCore.Instruction_Memory;

namespace MIPSCore.Control_Unit
{
    public interface IControlUnit : IControlSignals
    {
        void Clock();
        IInstructionMemory InstructionMemory { get; set; }
    }
}