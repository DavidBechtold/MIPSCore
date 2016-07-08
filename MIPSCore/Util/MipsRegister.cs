using System;

namespace MIPSCore.Util
{
    public class MipsRegister
    {
        public const ushort NumberOfRegisters = 32;
        private readonly Register[] registers;

        public MipsRegister()
        {
            registers = new Register[NumberOfRegisters];
            Flush(); //init registers
        }

        public void Write(ushort number, uint unsignedDecimal)
        {
            if (CheckNumber(number))
                return;
            registers[number].Set(unsignedDecimal);
        }

        public void Write(ushort number, int signedDecimal)
        {
            if (CheckNumber(number))
                return;
            registers[number].Set(signedDecimal);
        }

        public void Write(ushort number, string binary)
        {
            if (CheckNumber(number))
                return;
            registers[number].SetBinary(binary);
        }

        public void Write(ushort number, Word word)
        {
            if (CheckNumber(number))
                return;

            registers[number].SetCWord(word);
        }

        public Word ReadCWord(ushort number)
        {
            CheckNumber(number);
            return registers[number];
        }

        public string ReadBinary(ushort number)
        {
            CheckNumber(number);
            return registers[number].Binary;
        }

        public string ReadHexadecimal(ushort number)
        {
            CheckNumber(number);
            return registers[number].Hexadecimal;
        }

        public uint ReadUnsingedDecimal(ushort number)
        {
            CheckNumber(number);
            return registers[number].UnsignedDecimal;
        }

        public int ReadSingedDecimal(ushort number)
        {
            CheckNumber(number);
            return registers[number].SignedDecimal;
        }

        public void Flush()
        {
            for(ushort i = 0; i < NumberOfRegisters; i++)
                registers[i] = new Register(i, 0, RegisterName(i));
        }

        private bool CheckNumber(ushort number)
        {
            if (number >= NumberOfRegisters) throw new ArgumentOutOfRangeException();
            
            //register $zero cannot be overwritten => this register is constant zero
            return number == 0;
        }

        public int[] ReadRegisters()
        {
            var allReg = new int[NumberOfRegisters];
            for(var i = 0; i < NumberOfRegisters; i++)
                allReg[i] = registers[i].SignedDecimal;
            return allReg;
        }

        public int ReadRegister(ushort num)
        {
            CheckNumber(num);
            return registers[num].SignedDecimal;
        }

        public uint ReadRegisterUnsigned(ushort num)
        {
            CheckNumber(num);
            return registers[num].UnsignedDecimal;
        }

        public override string ToString()
        {
            var allreg = "";
            for (ushort i = 0; i < NumberOfRegisters; i++)
                allreg += RegisterToStringDecSigned(i) + "\n";

            return allreg;
        }

        public string RegisterToStringDecSigned(ushort number)
        {
            CheckNumber(number);
            return number + ". \t" + registers[number].GetName + "\t:" + registers[number].SignedDecimal;
        }

        public string RegisterToStringDecUnsigned(ushort number)
        {
            CheckNumber(number);
            return number + ". \t" + registers[number].GetName + "\t:" + registers[number].UnsignedDecimal;
        }

        public string RegisterToStringHex(ushort number)
        {
            CheckNumber(number);
            return number + ". \t" + registers[number].GetName + "\t:" + registers[number].Hexadecimal;
        }

        public string RegisterName(ushort number)
        {
            CheckNumber(number);
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
                case 30: return "$fp/$s8";
                case 31: return "$ra";
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
