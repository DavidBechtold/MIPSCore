using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore;
using MIPSCore.Util;
using MIPSCore.InstructionSet;

namespace MIPSCore.DataMemory
{
    public class CDataMemory 
    {
        CCore core;
        CMemory memory;
        CWord loadedValue;

        public CDataMemory(CCore core, MemSize size)
        {
            this.core = core;
            memory = new CMemory(size);
            loadedValue = new CWord(0);
        }

        public void clock()
        {
            //check if we had to read/write from the memory
            if (core.getControlUnit.getMemRead)
                loadDataFromMemory();
            else if (core.getControlUnit.getMemWrite)
                storeDataToMemory();
        }

        private void loadDataFromMemory()
        {
            CWord address = core.getAlu.getResultLO;

            switch (core.getControlUnit.getDataMemoryWordSize)
            {
                case DataMemoryWordSize.singleByte:
                    loadedValue = memory.readByte(address);
                    break;
                case DataMemoryWordSize.halfWord:
                    loadedValue =  memory.readHalfWord(address);
                    break;
                case DataMemoryWordSize.word:
                    loadedValue = memory.readWord(address);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": DataMemoryWordSize out of range");
            }
        }

        private void storeDataToMemory()
        {
            CWord address = core.getAlu.getResultLO;
            CWord valueToStore = core.getRegisterFile.readRt();
            switch (core.getControlUnit.getDataMemoryWordSize)
            {
                case DataMemoryWordSize.singleByte:
                    memory.writeByte(valueToStore, address);
                    break;
                case DataMemoryWordSize.halfWord:
                    memory.writeHalfWord(valueToStore, address);
                    break;
                case DataMemoryWordSize.word:
                    memory.writeWord(valueToStore, address);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": DataMemoryWordSize out of range");
            }
        }

        public CWord getLoadedValue
        {
            get
            {
                return loadedValue;
            }
        }

        public CWord setAddressOffset
        {
            set
            {
                memory.setOffset = value;
            }
        }

        public UInt32 getEndAddress
        {
            get
            {
                return memory.getEndByteAddress;
            }
        }
    }
}
