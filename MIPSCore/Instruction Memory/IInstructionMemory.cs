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
        Word GetOpCode { get; }
        Word GetFunction { get; }
        Word GetShiftAmount { get; }
        Word GetImmediate { get; }
        Word GetRd { get; }
        Word GetRs { get; }
        Word GetRt { get; }
        Word GetJumpTarget { get;  }
        Word GetProgramCounter { get; }
        Word GetActualInstruction { get; }
        Word SetAddressOffset { set; }

        IAlu Alu { get; set; }
        IControlUnit ControlUnit { get; set; }
        IRegisterFile RegisterFile { get; set; }
    }
}