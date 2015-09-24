using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Register_File;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace MIPSCore.Data_Memory
{
    public interface IDataMemory : IMemory
    {
        void Clock();
        IAlu Alu { get; set; }
        IRegisterFile RegisterFile { get; set; }
        IControlUnit ControlUnit { get; set; }
        Word LoadedValue { get; }
        Word AddressOffset { set; }
    }
}