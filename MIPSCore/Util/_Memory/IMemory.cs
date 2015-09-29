using System.Collections.Generic;

namespace MIPSCore.Util._Memory
{
    public interface IMemory
    {
        void WriteByte(Word byteVal, uint byteAddress);
        void WriteByte(Word byteVal, Word byteAddress);
        void WriteHalfWord(Word halfword, uint byteAddress);
        void WriteHalfWord(Word halfword, Word byteAddress);
        void WriteWord(Word word, uint byteAddress);
        void WriteWord(Word word, Word byteAddress);
        Word ReadByte(uint byteAddress);
        Word ReadByte(Word byteAddress);
        Word ReadHalfWord(uint byteAddress);
        Word ReadHalfWord(Word byteAddress);
        Word ReadWord(uint byteAddress);
        Word ReadWord(Word byteAddress);

        void Flush();
        string Hexdump(uint startaddress, uint bytesToRead);

        void SetSize(MemorySize size);
        List<uint> ChangedWordAddresses { get; }
        LastChangedAddressDto LastChangedAddress { get; }
        MemorySize Size { get; }
        Word Offset { get; set; }
        uint GetLastByteAddress { get; }
        uint SizeBytes { get; }
    }
}
