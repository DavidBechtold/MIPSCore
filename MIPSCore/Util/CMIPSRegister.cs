using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util
{
    public class CMIPSRegister
    {
        public const UInt32 numberOfRegisters = 32;
        private CRegister[] registers;

        public CMIPSRegister()
        {
            registers = new CRegister[numberOfRegisters];
            flush(); //init registers
        }

        public void  write(UInt16 number, UInt32 unsignedDecimal)
        {
            if (checkNumber(number))
                return;
            registers[number].set(unsignedDecimal);
        }

        public void write(UInt16 number, Int32 signedDecimal)
        {
            if (checkNumber(number))
                return;
            registers[number].set(signedDecimal);
        }

        public void write(UInt16 number, string binary)
        {
            if (checkNumber(number))
                return;
            registers[number].setBinary(binary);
        }

        public void write(UInt16 number, CWord word)
        {
            if (checkNumber(number))
                return;

            registers[number].setCWord(word);
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

        public void flush()
        {
            for(UInt16 i = 0; i < numberOfRegisters; i++)
                registers[i] = new CRegister(i, (UInt32) 0);
        }

        private bool checkNumber(UInt16 number)
        {
            if (number >= numberOfRegisters)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": number must be < 32");
            
            //register $zero cannot be overwritten => this register is constant zero
            if (number == 0)
                return true;
            else
                return false;
        }
    }
}
