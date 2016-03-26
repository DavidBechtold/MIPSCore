using System;
using System.Data;
using MIPSCore.Instruction_Set;

namespace MIPSCore.Util
{
    public class Word
    {
        public const ushort WordLength = 32;
        public int SignedDecimal { get; private set; }
        public uint UnsignedDecimal { get; private set; }
        public string Hexadecimal { get; private set; }
        public string Binary { get; private set; }

        private ushort maxBinaryLength = WordLength;
        private bool signed;

        public Word(int signedDecimal)
        {
            Set(signedDecimal);
        }

        public Word(uint unsignedDecimal)
        {
            Set(unsignedDecimal);
        }

        public Word(string binary)
        {
            SetBinary(binary);
        }

        public Word SignExtend(uint msb)
        {
            if ((SignedDecimal & msb) == msb)
            {
                Hexadecimal = Convert.ToString(SignedDecimal, 16).PadLeft(8, 'F').ToUpper();
                SignedDecimal = Convert.ToInt32(Hexadecimal, 16);
                UnsignedDecimal = Convert.ToUInt32(Hexadecimal, 16);
                Binary = Convert.ToString(SignedDecimal, 2).PadLeft(32, '1');
                signed = true;
            }
            else
            {
                Hexadecimal = Convert.ToString(SignedDecimal, 16).PadLeft(8, '0').ToUpper();
                UnsignedDecimal = Convert.ToUInt32((uint)SignedDecimal);
                Binary = Convert.ToString(SignedDecimal, 2).PadLeft(32, '0');
                signed = true;
            }
            return this;
        }

        public Word SignExtend(DataMemoryWordSize dataSize)
        {
            switch (dataSize)
            {
                case DataMemoryWordSize.SingleByte: return SignExtend(128); 
                case DataMemoryWordSize.HalfWord: return SignExtend(32768);
                default: throw new ArgumentOutOfRangeException("dataSize", dataSize, null);
            }
        }

        public Word SignExtend()
        {
            return SignExtend(32768);
        }

        public Word SignExtendZero()
        {
            Hexadecimal = Convert.ToString(SignedDecimal, 16).PadLeft(8, '0').ToUpper();
            UnsignedDecimal = Convert.ToUInt32((uint) SignedDecimal);
            Binary = Convert.ToString(SignedDecimal, 2).PadLeft(32, '0');
            signed = false;
            return this;
        }

        public static Word operator +(Word arg1, Word arg2)
        {
            if (arg1.signed && arg2.signed)
                return new Word(arg1.SignedDecimal + arg2.SignedDecimal);
            if (!arg1.signed && !arg2.signed)
                return new Word(arg1.UnsignedDecimal + arg2.UnsignedDecimal);
            throw new ArithmeticException("Word: Cannot add an signed Value to an unsigned Value");
        }

        public static Word operator +(Word arg1, uint arg2)
        {
            return new Word(arg1.UnsignedDecimal + arg2);
        }

        public static Word operator -(Word arg1, Word arg2)
        {
            if (arg1.signed && arg2.signed)
                return new Word(arg1.SignedDecimal - arg2.SignedDecimal);
            if (!arg1.signed && !arg2.signed)
                return new Word(arg1.UnsignedDecimal - arg2.UnsignedDecimal);
            throw new ArithmeticException("Word: Cannot subtract an signed Value off an unsigned Value");
        }

        public static Word operator *(Word arg1, uint arg2)
        {
            return new Word(arg1.UnsignedDecimal*arg2);
        }

        public static Word operator /(Word arg1, uint arg2)
        {
            return new Word(arg1.UnsignedDecimal/arg2);
        }

        public string GetSubBinary(ushort start, ushort length)
        {
            return Binary.Substring(start, length).PadLeft(32, '0');
        }

        public Word GetSubword(ushort start, ushort length)
        {
            return new Word(GetSubBinary(start, length));
        }

        public void Set(uint value)
        {
            UnsignedDecimal = value;
            Hexadecimal = Convert.ToString(UnsignedDecimal, 16).PadLeft(8, '0').ToUpper();
            SignedDecimal = Convert.ToInt32((int) UnsignedDecimal);
            Binary = Convert.ToString(UnsignedDecimal, 2).PadLeft(32, '0');
            signed = false;
        }

        public void Set(int value)
        {
            SignedDecimal = value;
            Hexadecimal = Convert.ToString(SignedDecimal, 16).PadLeft(8, '0').ToUpper();
            UnsignedDecimal = Convert.ToUInt32((uint) SignedDecimal);
            Binary = Convert.ToString(SignedDecimal, 2).PadLeft(32, '0');
            signed = true;
        }

        public void SetBinary(string binary)
        {
            if (binary.Length > maxBinaryLength)
                throw new ArgumentOutOfRangeException(GetType().Name + ": Binary length > 32");
            Binary = binary.PadLeft(maxBinaryLength, '0');
            UnsignedDecimal = Convert.ToUInt32(binary, 2);
            SignedDecimal = Convert.ToInt32(binary, 2);
            Hexadecimal = Convert.ToString(UnsignedDecimal, 16).PadLeft(8, '0').ToUpper();
            signed = false;
        }

        public void SetCWord(Word word)
        {
            Binary = word.Binary;
            UnsignedDecimal = word.UnsignedDecimal;
            SignedDecimal = word.SignedDecimal;
            Hexadecimal = word.Hexadecimal;
            signed = word.signed;
        }
    }
}
