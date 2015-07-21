using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util
{
    public interface IMemory
    {
        void writeByte(CWord byteVal, UInt32 byteAddress);
        void writeByte(CWord byteVal, CWord byteAddress);
        void writeHalfWord(CWord halfword, UInt32 byteAddress);
        void writeHalfWord(CWord halfword, CWord byteAddress);
        void writeWord(CWord word, UInt32 byteAddress);
        void writeWord(CWord word, CWord byteAddress);
        CWord readByte(UInt32 byteAddress);
        CWord readByte(CWord byteAddress);
        CWord readHalfWord(UInt32 byteAddress);
        CWord readHalfWord(CWord byteAddress);
        CWord readWord(UInt32 byteAddress);
        CWord readWord(CWord byteAddress);

    }
}
