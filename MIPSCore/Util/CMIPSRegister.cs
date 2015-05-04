using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util
{
    class CMIPSRegister
    {
        public const UInt16 numberOfRegisters = 32;
        private CRegister[] registers;

        public CMIPSRegister()
        {
            registers = new CRegister[numberOfRegisters];
            flush(); //init registers
        }

        public void  write(UInt16 number, UInt32 unsignedDecimal)
        {
            checkNumber(number);
            registers[number].set(unsignedDecimal);
        }

        public void write(UInt16 number, Int32 signedDecimal)
        {
            checkNumber(number);
            registers[number].set(signedDecimal);
        }

        public void write(UInt16 number, string binary)
        {
            checkNumber(number);
            registers[number].set(binary);
        }

        public CWord readCWord(UInt16 number)
        {
            checkNumber(number);
            return registers[number];
        }

        public string readBinary(UInt16 number)
        {
            checkNumber(number);
            return registers[number].getBinary;
        }

        public string readHexadecimal(UInt16 number)
        {
            checkNumber(number);
            return registers[number].getHexadecimal;
        }

        public UInt32 readUnsingedDecimal(UInt16 number)
        {
            checkNumber(number);
            return registers[number].getUnsignedDecimal;
        }

        public Int32 readSingedDecimal(UInt16 number)
        {
            checkNumber(number);
            return registers[number].getSignedDecimal;
        }

        private void flush()
        {
            for(UInt16 i = 0; i < numberOfRegisters; i++)
                registers[i] = new CRegister(i, (UInt32) 0);
        }

        private void checkNumber(UInt16 number)
        {
            if (number >= numberOfRegisters)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": number must be < 32");
        }
    }
}
