using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Util;


namespace MIPSCore.Util
{
    public enum MemSize { Size_1kB, Size_2kB, Size_4kB, Size_8kB, Size_16kB}
    public class CMemory
    {
        private Byte[] memory;
        private MemSize size;
        private UInt32 endByteAddress;
       

        public CMemory(MemSize size)
        {
            this.size = size;
            createMemory(size);

        }

        private void createMemory(MemSize size)
        {
            switch (size)
            {
                case MemSize.Size_1kB:
                    memory = new Byte[1024 * 8];
                    endByteAddress = 1024 * 8 - 1;
                    break;
                case MemSize.Size_2kB:
                    memory = new Byte[2048 * 8];
                    endByteAddress = 2048 * 8 - 1;
                    break;
                case MemSize.Size_4kB:
                    memory = new Byte[4096 * 8];
                    endByteAddress = 4096 * 8 - 1;
                    break;
                case MemSize.Size_8kB:
                    memory = new Byte[8192 * 8];
                    endByteAddress = 8192 * 8 - 1;
                    break;
                case MemSize.Size_16kB:
                    memory = new Byte[16384 * 8];
                    endByteAddress = 16384 * 8 - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": MemSize out of range");
            }
        }

        public void writeByte(CWord byteVal, UInt32 byteAddress)
        {
            checkAddress(byteAddress);

            if (byteVal.getUnsignedDecimal > Byte.MaxValue)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": byte out of range");

            memory[byteAddress] = (Byte)byteVal.getUnsignedDecimal;
        }

        public void writeByte(CWord byteVal, CWord byteAddress)
        {
            writeByte(byteVal, byteAddress.getUnsignedDecimal);
        }

        public void writeHalfWord(CWord halfword, UInt32 byteAddress)
        {
            checkAddress(byteAddress + 1);

            if (halfword.getUnsignedDecimal > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": halfWord out of range");

            memory[byteAddress] = (Byte)(halfword.getUnsignedDecimal >> 8);
            memory[byteAddress + 1] = (Byte)(halfword.getUnsignedDecimal);
        }

        public void writeHalfWord(CWord halfword, CWord byteAddress)
        {
            writeHalfWord(halfword, byteAddress.getUnsignedDecimal);
        }

        public void writeWord(CWord word, UInt32 byteAddress)
        {
            checkAddress(byteAddress + 3);

            if (word.getUnsignedDecimal > UInt32.MaxValue)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": halfWord out of range");

            memory[byteAddress] = (Byte)(word.getUnsignedDecimal >> 24);
            memory[byteAddress + 1] = (Byte)(word.getUnsignedDecimal >> 16);
            memory[byteAddress + 2] = (Byte)(word.getUnsignedDecimal >> 8);
            memory[byteAddress + 3] = (Byte)(word.getUnsignedDecimal);
        }

        public void writeWord(CWord word, CWord byteAddress)
        {
            writeWord(word, byteAddress.getUnsignedDecimal);
        }

        public CWord readByte(UInt32 byteAddress)
        {
            checkAddress(byteAddress);

            return new CWord((UInt32) memory[byteAddress]);
        }

        public CWord readByte(CWord byteAddress)
        {
            return readByte(byteAddress.getUnsignedDecimal);
        }

        public CWord readHalfWord(UInt32 byteAddress)
        {
            checkAddress(byteAddress + 1);

            return new CWord((UInt32) (memory[byteAddress] << 8 | memory[byteAddress + 1]));
        }

        public CWord readHalfWord(CWord byteAddress)
        {
            return readHalfWord(byteAddress.getUnsignedDecimal);
        }

        public CWord readWord(UInt32 byteAddress)
        {
            checkAddress(byteAddress + 3);

            return new CWord((UInt32)((memory[byteAddress] << 24 | memory[byteAddress + 1] << 16) | (memory[byteAddress + 2] << 8 | memory[byteAddress + 3])));
        }

        public CWord readWord(CWord byteAddress)
        {
            return readWord(byteAddress.getUnsignedDecimal);
        }

        private void checkAddress(UInt32 byteAddress)
        {
            if (byteAddress > endByteAddress)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": byteAddress out of range");
        }


        public UInt32 getEndByteAddress
        {
            get
            {
                return endByteAddress;
            }
        }

        public void flush()
        {
            for (UInt32 i = 0; i < endByteAddress; i += 4)
                writeWord(new CWord(0), i);
        }
    }
}
