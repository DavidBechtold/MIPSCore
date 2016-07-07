using System;
using System.Collections.Generic;

namespace MIPSCore.Util._Memory
{
    public enum MemorySize {
    [Text("4 KByte")]
    Size4Kb,
    [Text("8 KByte")]
    Size8Kb,
    [Text("16 KByte")]
    Size16Kb,
    [Text("32 KByte")]
    Size32Kb,
    [Text("64 KByte")]
    Size64Kb,
    [Text("128 KByte")]
    Size128Kb,
    /*[Text("256 KByte")]
    Size256Kb,
    [Text("512 KByte")]
    Size512Kb,
    [Text("1024 KByte")]
    Size1024Kb,*/
    }

    public class Memory : IMemory
    {
        private byte[] memory;
        private readonly List<uint> changedWordAddresses;
        public MemorySize Size { get;  private set; }
        public Word Offset { get; set; }
        public uint GetLastByteAddress { get; private set; }
        public uint SizeBytes { get { return GetLastByteAddress + 1; } }
        private readonly LastChangedAddressDto lastChangedAddress;

        public Memory(MemorySize size)
        {
            changedWordAddresses = new List<uint>();
            lastChangedAddress = new LastChangedAddressDto(false, 0);
            SetSize(size);
        }

        public void SetSize(MemorySize size)
        {
            Size = size;
            CreateMemory(size);
            Offset = new Word((uint) 0);
            changedWordAddresses.Clear();
        }

        private void CreateMemory(MemorySize size)
        {
            switch (size)
            {
                case MemorySize.Size4Kb:
                    memory = new byte[1024 * 4];
                    GetLastByteAddress = 1024 * 4 - 1;
                    break;
                case MemorySize.Size8Kb:
                    memory = new byte[8192];
                    GetLastByteAddress = 8192 - 1;
                    break;
                case MemorySize.Size16Kb:
                    memory = new byte[16384];
                    GetLastByteAddress = 16384 - 1;
                    break;
                case MemorySize.Size32Kb:
                    memory = new byte[32768];
                    GetLastByteAddress = 32768 - 1;
                    break;
                case MemorySize.Size64Kb:
                    memory = new byte[65536];
                    GetLastByteAddress = 65536 - 1;
                    break;
                case MemorySize.Size128Kb:
                    memory = new byte[131072];
                    GetLastByteAddress = 131072 - 1;
                    break;
                /*case MemorySize.Size256Kb:
                    memory = new byte[262144];
                    GetLastByteAddress = 262144 - 1;
                    break;
                case MemorySize.Size512Kb:
                    memory = new byte[524288];
                    GetLastByteAddress = 524288 - 1;
                    break;
                case MemorySize.Size1024Kb:
                    memory = new byte[1048576];
                    GetLastByteAddress = 1048576 - 1;
                    break;*/
                default:
                    throw new ArgumentOutOfRangeException("size");
            }
        }

        public void WriteByte(Word byteVal, uint byteAddress)
        {
            if (byteVal == null) throw new ArgumentNullException("byteVal");

            byteAddress = CheckAndCalculateAddress(byteAddress, 0);
            changedWordAddresses.Add(byteAddress / 4);
            lastChangedAddress.Address = byteAddress;
            lastChangedAddress.Changed = true;
            memory[byteAddress] = (byte)byteVal.UnsignedDecimal;
        }

        public void WriteByte(Word byteVal, Word byteAddress)
        {
            WriteByte(byteVal, byteAddress.UnsignedDecimal);
        }

        public void WriteHalfWord(Word halfword, uint byteAddress)
        {
            if (halfword == null) throw new ArgumentNullException("halfword");

            byteAddress = CheckAndCalculateAddress(byteAddress, 1);
            changedWordAddresses.Add(byteAddress / 4);
            lastChangedAddress.Address = byteAddress;
            lastChangedAddress.Changed = true;
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
            changedWordAddresses.Add(byteAddress);
            lastChangedAddress.Address = byteAddress;
            lastChangedAddress.Changed = true;
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
            if (changedWordAddresses.Contains(byteAddress))
                changedWordAddresses.Remove(byteAddress);
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
            if (changedWordAddresses.Contains(byteAddress))
                changedWordAddresses.Remove(byteAddress);
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
            if (changedWordAddresses.Contains(byteAddress))
                changedWordAddresses.Remove(byteAddress);

            return new Word((uint)((memory[byteAddress] << 24 | memory[byteAddress + 1] << 16) | (memory[byteAddress + 2] << 8 | memory[byteAddress + 3])));
            //return new Word((uint)((memory[byteAddress + 1] << 24 | memory[byteAddress] << 16) | (memory[byteAddress + 3] << 8 | memory[byteAddress + 2])));
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
            var realStartaddress = CheckAndCalculateAddress(startaddress, bytesToRead);
            var result = "";

            for (var i = realStartaddress; i < realStartaddress + bytesToRead; i+=4)
                result += string.Format("{0:X8} {1:X2} {2:X2} {3:X2} {4:X2}\n", i + Offset.UnsignedDecimal, memory[i], memory[i + 1], memory[i + 2], memory[i + 3]);
            return result;
        }

        public List<uint> ChangedWordAddresses
        {
            get
            {
                var helper = new List<uint>(changedWordAddresses);
                changedWordAddresses.Clear();
                return helper;
            }
        }

        public LastChangedAddressDto LastChangedAddress
        {
            get
            {
                var helper = new LastChangedAddressDto(lastChangedAddress.Changed, lastChangedAddress.Address);
                lastChangedAddress.Changed = false;
                return helper;
            }
        }
    }
}
