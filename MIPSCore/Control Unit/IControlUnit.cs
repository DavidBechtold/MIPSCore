using MIPSCore.Instruction_Memory;
using MIPSCore.Instruction_Set;

namespace MIPSCore.Control_Unit
{
    public interface IControlUnit : IControlSignals
    {
        void Clock();
        IInstructionMemory InstructionMemory { get; set; }
    }
}