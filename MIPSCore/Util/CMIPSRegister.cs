using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util
{
    public class CMIPSRegister
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
                registers[i] = new CRegister(i, (UInt32) 0, registerName(i));
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

        public Int32[] readRegisters()
        {
            Int32[] allReg = new Int32[numberOfRegisters];
            for(int i = 0; i < numberOfRegisters; i++)
                allReg[i] = registers[i].getSignedDecimal;
            return allReg;
        }

        public Int32 readRegister(UInt16 num)
        {
            checkNumber(num);
            return registers[num].getSignedDecimal;
        }

        public UInt32 readRegisterUnsigned(UInt16 num)
        {
            checkNumber(num);
            return registers[num].getUnsignedDecimal;
        }

        public override string ToString()
        {
            string allreg = "";
            for (UInt16 i = 0; i < numberOfRegisters; i++)
                allreg += registerToStringDecSigned(i) + "\n";

            return allreg;
        }

        public string registerToStringDecSigned(UInt16 number)
        {
            checkNumber(number);
            return number + ". \t" + registers[number].getName + "\t:" + registers[number].getSignedDecimal;
        }

        public string registerToStringDecUnsigned(UInt16 number)
        {
            checkNumber(number);
            return number + ". \t" + registers[number].getName + "\t:" + registers[number].getUnsignedDecimal;
        }

        public string registerToStringHex(UInt16 number)
        {
            checkNumber(number);
            return number + ". \t" + registers[number].getName + "\t:" + registers[number].getHexadecimal;
        }

        public string registerName(UInt16 number)
        {
            checkNumber(number);
            switch (number)
            {
                case 0: return "$zero";
                case 1: return "$at";
                case 2: return "$v0";
                case 3: return "$v1";
                case 4: return "$a0";
                case 5: return "$a1";
                case 6: return "$a2";
                case 7: return "$a3";
                case 8: return "$t0";
                case 9: return "$t1";
                case 10: return "$t2";
                case 11: return "$t3";
                case 12: return "$t4";
                case 13: return "$t5";
                case 14: return "$t6";
                case 15: return "$t7";
                case 16: return "$s0";
                case 17: return "$s1";
                case 18: return "$s2";
                case 19: return "$s3";
                case 20: return "$s4";
                case 21: return "$s5";
                case 22: return "$s6";
                case 23: return "$s7";
                case 24: return "$t8";
                case 25: return "$t9";
                case 26: return "$k0";
                case 27: return "$k1";
                case 28: return "$gp";
                case 29: return "$sp";
                case 30: return "$fp";
                case 31: return "$ra";
            }
            throw new ArgumentOutOfRangeException(this.GetType().Name + ": registernumber out of range");
        }
    }
}
