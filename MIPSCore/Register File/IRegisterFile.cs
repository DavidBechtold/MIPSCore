﻿using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Data_Memory;
using MIPSCore.Instruction_Memory;
using MIPSCore.Util;

namespace MIPSCore.Register_File
{
    public interface IRegisterFile
    {
        void Clock();
        void Flush();

        CWord ReadRs();
        CWord ReadRt();
        CWord ReadRd();
        string ToString();
        void InitStackPointer();
        int[] ReadAllRegister();
        int ReadRegister(ushort num);
        uint ReadRegisterUnsigned(ushort num);
        string ToStringRegister(ushort num);
        string ToStringRegisterUnsigned(ushort num);
        string ToStringRegisterHex(ushort num);

        IAlu Alu { set; get; }
        IDataMemory DataMemory { get; set; }
        IControlUnit ControlUnit { set; get; }
        IInstructionMemory InstructionMemory { set; get; }
    }
}