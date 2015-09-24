using MIPSCore.Control_Unit;
using MIPSCore.Instruction_Memory;
using MIPSCore.Register_File;
using MIPSCore.Util;

namespace MIPSCore.ALU
{
    public interface IAlu
    {
        void Clock();
        bool ZeroFlag { get; }
        bool OverflowFlag { get; }
        CWord GetResultLo { get; }
        CWord GetResultHi { get; }

        IControlUnit ControlUnit { get; set; }
        IInstructionMemory InstructionMemory { get; set; }
        IRegisterFile RegisterFile { get; set; }
    }
}