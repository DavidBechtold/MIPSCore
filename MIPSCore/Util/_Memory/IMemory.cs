namespace MIPSCore.Util._Memory
{
    public interface IMemory
    {
        void WriteByte(CWord byteVal, uint byteAddress);
        void WriteByte(CWord byteVal, CWord byteAddress);
        void WriteHalfWord(CWord halfword, uint byteAddress);
        void WriteHalfWord(CWord halfword, CWord byteAddress);
        void WriteWord(CWord word, uint byteAddress);
        void WriteWord(CWord word, CWord byteAddress);
        CWord ReadByte(uint byteAddress);
        CWord ReadByte(CWord byteAddress);
        CWord ReadHalfWord(uint byteAddress);
        CWord ReadHalfWord(CWord byteAddress);
        CWord ReadWord(uint byteAddress);
        CWord ReadWord(CWord byteAddress);

        void Flush();
        string Hexdump(uint startaddress, uint bytesToRead);

        MemSize Size { get; }
        CWord Offset { get; set; }
        uint GetLastByteAddress { get; }
        uint SizeBytes { get; }
    }
}
