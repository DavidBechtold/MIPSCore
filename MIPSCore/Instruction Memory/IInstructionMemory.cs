using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Register_File;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace MIPSCore.Instruction_Memory
{
    public interface IInstructionMemory: IMemory
    {
        void Clock();
        new void Flush();
        CWord GetOpCode { get; }
        CWord GetFunction { get; }
        CWord GetShiftAmount { get; }
        CWord GetImmediate { get; }
        CWord GetRd { get; }
        CWord GetRs { get; }
        CWord GetRt { get; }
        CWord GetJumpTarget { get;  }
        CWord GetProgramCounter { get; }
        CWord GetActualInstruction { get; }
        CWord SetAddressOffset { set; }

        IAlu Alu { get; set; }
        IControlUnit ControlUnit { get; set; }
        IRegisterFile RegisterFile { get; set; }
    }
}