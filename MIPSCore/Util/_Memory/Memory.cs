using System;

namespace MIPSCore.Util._Memory
{
    public enum MemSize { Size1Kb = 1, Size2Kb, Size4Kb, Size8Kb, Size16Kb}
    public class Memory : IMemory
    {
        private byte[] memory;
        public MemSize Size { get;  private set; }
        public Word Offset { get; set; }
        public uint GetLastByteAddress { get; private set; }
        public uint SizeBytes { get { return GetLastByteAddress + 1; } }

        public Memory(MemSize size)
        {
            Size = size;
            CreateMemory(size);
            Offset = new Word((uint) 0);
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

        public void WriteByte(Word byteVal, uint byteAddress)
        {
            if (byteVal == null) throw new ArgumentNullException("byteVal");
            if (byteVal.UnsignedDecimal > byte.MaxValue) throw new ArgumentOutOfRangeException("byteVal");

            byteAddress = CheckAndCalculateAddress(byteAddress, 0);
            memory[byteAddress] = (byte)byteVal.UnsignedDecimal;
        }

        public void WriteByte(Word byteVal, Word byteAddress)
        {
            WriteByte(byteVal, byteAddress.UnsignedDecimal);
        }

        public void WriteHalfWord(Word halfword, uint byteAddress)
        {
            if (halfword == null) throw new ArgumentNullException("halfword");
            if (halfword.UnsignedDecimal > ushort.MaxValue) throw new ArgumentOutOfRangeException("halfword");

            byteAddress = CheckAndCalculateAddress(byteAddress, 1);
            memory[byteAddress] = (byte)(halfword.UnsignedDecimal >> 8);
            memory[byteAddress + 1] = (byte)(halfword.UnsignedDecimal);
        }

        public void WriteHalfWord(Word halfword, Word byteAddress)
        {
            WriteHalfWord(halfword, byteAddress.UnsignedDecimal);
        }

        public void WriteWord(Word word, uint byteAddress)
        {
            if (word == null) throw new ArgumentNullException("word");
            byteAddress = CheckAndCalculateAddress(byteAddress, 3);

            memory[byteAddress] = (byte)(word.UnsignedDecimal >> 24);
            memory[byteAddress + 1] = (byte)(word.UnsignedDecimal >> 16);
            memory[byteAddress + 2] = (byte)(word.UnsignedDecimal >> 8);
            memory[byteAddress + 3] = (byte)(word.UnsignedDecimal);
        }

        public void WriteWord(Word word, Word byteAddress)
        {
            WriteWord(word, byteAddress.UnsignedDecimal);
        }

        public Word ReadByte(uint byteAddress)
        {
            byteAddress = CheckAndCalculateAddress(byteAddress, 0);

            return new Word((uint) memory[byteAddress]);
        }

        public Word ReadByte(Word byteAddress)
        {
            if (byteAddress == null) throw new ArgumentNullException("byteAddress");
            return ReadByte(byteAddress.UnsignedDecimal);
        }

        public Word ReadHalfWord(uint byteAddress)
        {
            byteAddress = CheckAndCalculateAddress(byteAddress, 1);

            return new Word((uint) (memory[byteAddress] << 8 | memory[byteAddress + 1]));
        }

        public Word ReadHalfWord(Word byteAddress)
        {
            if (byteAddress == null) throw new ArgumentNullException("byteAddress");
            return ReadHalfWord(byteAddress.UnsignedDecimal);
        }

        public Word ReadWord(uint byteAddress)
        {
            byteAddress = CheckAndCalculateAddress(byteAddress, 3);

            return new Word((uint)((memory[byteAddress] << 24 | memory[byteAddress + 1] << 16) | (memory[byteAddress + 2] << 8 | memory[byteAddress + 3])));
        }

        public Word ReadWord(Word byteAddress)
        {
            if (byteAddress == null) throw new ArgumentNullException("byteAddress");
            return ReadWord(byteAddress.UnsignedDecimal);
        }

        // ReSharper disable once UnusedParameter.Local
        private uint CheckAndCalculateAddress(uint byteAddress, uint bytesRead)
        {
            var realAddress = byteAddress - Offset.UnsignedDecimal;
            if (realAddress + bytesRead > GetLastByteAddress) throw new ArgumentOutOfRangeException("byteAddress");
            return realAddress;
        }

        public virtual void Flush()
        {
            for (uint i = 0; i < GetLastByteAddress; i += 4)
                WriteWord(new Word(0), i);
        }

        public string Hexdump(uint startaddress, uint bytesToRead)
        {
            uint realStartaddress = CheckAndCalculateAddress(startaddress, bytesToRead);
            string result = "";

            for (uint i = realStartaddress; i < realStartaddress + bytesToRead; i+=4)
                result += string.Format("{0:X8} {1:X2} {2:X2} {3:X2} {4:X2}\n", i + Offset.UnsignedDecimal, memory[i], memory[i + 1], memory[i + 2], memory[i + 3]);
            return result;
        }
    }
}
