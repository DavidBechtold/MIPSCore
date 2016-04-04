using System;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Instruction_Set;
using MIPSCore.Register_File;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace MIPSCore.Data_Memory
{
    public class DataMemory : Memory, IDataMemory
    {
        public IAlu Alu { get; set; }
        public IRegisterFile RegisterFile { get; set; }
        public IControlUnit ControlUnit { get; set; }
        public Word LoadedValue { get; private set; }
        public Word AddressOffset { set { Offset = value; } }

        public DataMemory(MemorySize size)
         : base(size)
        {
            LoadedValue = new Word(0);
        }

        public void Clock()
        {
            //check if we had to read/write from the memory
            if (ControlUnit.MemoryRead)
                LoadDataFromMemory();
            else if (ControlUnit.MemoryWrite)
                StoreDataToMemory();
        }

        private void LoadDataFromMemory()
        {
            var address = Alu.GetResultLo;
            switch (ControlUnit.DataMemoryWordSize)
            {
                case DataMemoryWordSize.SingleByte:
                    LoadedValue = ReadByte(address);
                    if (ControlUnit.MemorySignExtend)
                        LoadedValue.SignExtend(DataMemoryWordSize.SingleByte);
                    break;
                case DataMemoryWordSize.HalfWord:
                    LoadedValue =  ReadHalfWord(address);
                    if (ControlUnit.MemorySignExtend)
                        LoadedValue.SignExtend(DataMemoryWordSize.SingleByte);
                    break;
                case DataMemoryWordSize.Word:
                    LoadedValue = ReadWord(address);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StoreDataToMemory()
        {
            var address = Alu.GetResultLo;
            var valueToStore = RegisterFile.ReadRd();
            switch (ControlUnit.DataMemoryWordSize)
            {
                case DataMemoryWordSize.SingleByte:
                    WriteByte(valueToStore, address);
                    break;
                case DataMemoryWordSize.HalfWord:
                    WriteHalfWord(valueToStore, address);
                    break;
                case DataMemoryWordSize.Word:
                    WriteWord(valueToStore, address);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
