using System;

namespace MIPSCore.Util._Memory
{
    public enum MemSize { Size1Kb = 1, Size2Kb, Size4Kb, Size8Kb, Size16Kb}
    public class Memory : IMemory
    {
        private byte[] memory;
        public MemSize Size { get;  private set; }
        public CWord Offset { get; set; }
        public uint GetLastByteAddress { get; private set; }
        public uint SizeBytes { get { return GetLastByteAddress + 1; } }

        public Memory(MemSize size)
        {
            Size = size;
            CreateMemory(size);
            Offset = new CWord((uint) 0);
        }

        private void CreateMemory(MemSize size)
        {
            switch (size)
            {
                case MemSize.Size1Kb:
                    memory = new byte[1024 * 8];
                    GetLastByteAddress = 1024 * 8 - 1;
                    break;
                case MemSize.Size2Kb:
                    memory = new byte[2048 * 8];
                    GetLastByteAddress = 2048 * 8 - 1;
                    break;
                case MemSize.Size4Kb:
                    memory = new byte[4096 * 8];
                    GetLastByteAddress = 4096 * 8 - 1;
                    break;
                case MemSize.Size8Kb:
                    memory = new byte[8192 * 8];
                    GetLastByteAddress = 8192 * 8 - 1;
                    break;
                case MemSize.Size16Kb:
                    memory = new byte[16384 * 8];
                    GetLastByteAddress = 16384 * 8 - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("size");
            }
        }

        public void WriteByte(CWord byteVal, uint byteAddress)
        {
            if (byteVal == null) throw new ArgumentNullException("byteVal");
            if (byteVal.getUnsignedDecimal > byte.MaxValue) throw new ArgumentOutOfRangeException("byteVal");

            byteAddress = CheckAndCalculateAddress(byteAddress, 0);
            memory[byteAddress] = (byte)byteVal.getUnsignedDecimal;
        }

        public void WriteByte(CWord byteVal, CWord byteAddress)
        {
            WriteByte(byteVal, byteAddress.getUnsignedDecimal);
        }

        public void WriteHalfWord(CWord halfword, uint byteAddress)
        {
            if (halfword == null) throw new ArgumentNullException("halfword");
            if (halfword.getUnsignedDecimal > ushort.MaxValue) throw new ArgumentOutOfRangeException("halfword");

            byteAddress = CheckAndCalculateAddress(byteAddress, 1);
            memory[byteAddress] = (byte)(halfword.getUnsignedDecimal >> 8);
            memory[byteAddress + 1] = (byte)(halfword.getUnsignedDecimal);
        }

        public void WriteHalfWord(CWord halfword, CWord byteAddress)
        {
            WriteHalfWord(halfword, byteAddress.getUnsignedDecimal);
        }

        public void WriteWord(CWord word, uint byteAddress)
        {
            if (word == null) throw new ArgumentNullException("word");
            byteAddress = CheckAndCalculateAddress(byteAddress, 3);

            memory[byteAddress] = (byte)(word.getUnsignedDecimal >> 24);
            memory[byteAddress + 1] = (byte)(word.getUnsignedDecimal >> 16);
            memory[byteAddress + 2] = (byte)(word.getUnsignedDecimal >> 8);
            memory[byteAddress + 3] = (byte)(word.getUnsignedDecimal);
        }

        public void WriteWord(CWord word, CWord byteAddress)
        {
            WriteWord(word, byteAddress.getUnsignedDecimal);
        }

        public CWord ReadByte(uint byteAddress)
        {
            byteAddress = CheckAndCalculateAddress(byteAddress, 0);

            return new CWord((uint) memory[byteAddress]);
        }

        public CWord ReadByte(CWord byteAddress)
        {
            if (byteAddress == null) throw new ArgumentNullException("byteAddress");
            return ReadByte(byteAddress.getUnsignedDecimal);
        }

        public CWord ReadHalfWord(uint byteAddress)
        {
            byteAddress = CheckAndCalculateAddress(byteAddress, 1);

            return new CWord((uint) (memory[byteAddress] << 8 | memory[byteAddress + 1]));
        }

        public CWord ReadHalfWord(CWord byteAddress)
        {
            if (byteAddress == null) throw new ArgumentNullException("byteAddress");
            return ReadHalfWord(byteAddress.getUnsignedDecimal);
        }

        public CWord ReadWord(uint byteAddress)
        {
            byteAddress = CheckAndCalculateAddress(byteAddress, 3);

            return new CWord((uint)((memory[byteAddress] << 24 | memory[byteAddress + 1] << 16) | (memory[byteAddress + 2] << 8 | memory[byteAddress + 3])));
        }

        public CWord ReadWord(CWord byteAddress)
        {
            if (byteAddress == null) throw new ArgumentNullException("byteAddress");
            return ReadWord(byteAddress.getUnsignedDecimal);
        }

        // ReSharper disable once UnusedParameter.Local
        private uint CheckAndCalculateAddress(uint byteAddress, uint bytesRead)
        {
            var realAddress = byteAddress - Offset.getUnsignedDecimal;
            if (realAddress + bytesRead > GetLastByteAddress) throw new ArgumentOutOfRangeException("byteAddress");
            return realAddress;
        }

        public virtual void Flush()
        {
            for (uint i = 0; i < GetLastByteAddress; i += 4)
                WriteWord(new CWord(0), i);
        }

        public string Hexdump(uint startaddress, uint bytesToRead)
        {
            uint realStartaddress = CheckAndCalculateAddress(startaddress, bytesToRead);
            string result = "";

            for (uint i = realStartaddress; i < realStartaddress + bytesToRead; i+=4)
                result += string.Format("{0:X8} {1:X2} {2:X2} {3:X2} {4:X2}\n", i + Offset.getUnsignedDecimal, memory[i], memory[i + 1], memory[i + 2], memory[i + 3]);
            return result;
        }
    }
}
