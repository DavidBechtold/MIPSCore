using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util
{
    public class CWord
    {
        public const UInt16 wordLength = 32;
        private UInt16 maxHexLength = wordLength / 4;
        private UInt16 maxBinaryLength = wordLength;
        private string binary;
        private UInt32 unsignedDecimal;
        private Int32 signedDecimal;
        private string hexadecimal;
        private bool signed;

        public CWord(Int32 signedDecimal)
        {
            set(signedDecimal);
        }

        public CWord(UInt32 unsignedDecimal)
        {
            set(unsignedDecimal);
        }

        public CWord(string binary)
        {
            setBinary(binary);
        }

        public void signExtendSigned()
        {
            if ((signedDecimal & 32768) == 32768)
            {
                this.hexadecimal = Convert.ToString(signedDecimal, 16).PadLeft(8, 'F').ToUpper();
                this.signedDecimal = Convert.ToInt32(hexadecimal, 16);
                this.unsignedDecimal = Convert.ToUInt32(hexadecimal, 16);
                this.binary = Convert.ToString(signedDecimal, 2).PadLeft(32, '1');
                signed = true;
            }
            else
            {
                this.hexadecimal = Convert.ToString(signedDecimal, 16).PadLeft(8, '0').ToUpper();
                this.unsignedDecimal = Convert.ToUInt32((UInt32)signedDecimal);
                this.binary = Convert.ToString(signedDecimal, 2).PadLeft(32, '0');
                signed = true;
            }
        }

        public void signExtendUnsigned()
        {
            this.hexadecimal = Convert.ToString(signedDecimal, 16).PadLeft(8, '0').ToUpper();
            this.unsignedDecimal = Convert.ToUInt32((UInt32)signedDecimal);
            this.binary = Convert.ToString(signedDecimal, 2).PadLeft(32, '0');
            signed = false;
        }

        public static CWord operator +(CWord arg1, CWord arg2)
        {
            if (arg1.signed && arg2.signed)
                return new CWord(arg1.getSignedDecimal + arg2.getSignedDecimal);
            else if (!arg1.signed && !arg2.signed)
                return new CWord(arg1.getUnsignedDecimal + arg2.getUnsignedDecimal);
            else
                throw new ArithmeticException("CWord: Cannot add an signed Value to an unsigned Value");
        }

        public static CWord operator +(CWord arg1, UInt32 arg2)
        {
            return new CWord(arg1.unsignedDecimal + arg2);
        }

        public static CWord operator -(CWord arg1, CWord arg2)
        {
            if (arg1.signed && arg2.signed)
                return new CWord(arg1.getSignedDecimal - arg2.getSignedDecimal);
            else if (!arg1.signed && !arg2.signed)
                return new CWord(arg1.getUnsignedDecimal - arg2.getUnsignedDecimal);
            else
                throw new ArithmeticException("CWord: Cannot subtract an signed Value off an unsigned Value");
        }

        public static CWord operator *(CWord arg1, UInt32 arg2)
        {
            return new CWord(arg1.unsignedDecimal * arg2);
        }

        public static CWord operator /(CWord arg1, UInt32 arg2)
        {
            return new CWord(arg1.unsignedDecimal / arg2);
        }

        public string getSubBinary(UInt16 start, UInt16 length)
        {
            return binary.Substring(start, length).PadLeft(32, '0');
        }

        public CWord getSubword(UInt16 start, UInt16 length)
        {
            return new CWord(getSubBinary(start, length));
        }

        public void set(UInt32 value)
        {
            this.unsignedDecimal = value;
            this.hexadecimal = Convert.ToString(unsignedDecimal, 16).PadLeft(8, '0').ToUpper();
            this.signedDecimal = Convert.ToInt32((Int32)unsignedDecimal);
            this.binary = Convert.ToString(unsignedDecimal, 2).PadLeft(32, '0');
            signed = false;
        }

        public void set(Int32 value)
        {
            this.signedDecimal = value;
            this.hexadecimal = Convert.ToString(signedDecimal, 16).PadLeft(8, '0').ToUpper();
            this.unsignedDecimal = Convert.ToUInt32((UInt32)signedDecimal);
            this.binary = Convert.ToString(signedDecimal, 2).PadLeft(32, '0');
            signed = true;
        }

        public void setBinary(string binary)
        {
            if (binary.Length > maxBinaryLength)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": Binary length > 32");
            this.binary = binary.PadLeft(maxBinaryLength, '0');
            this.unsignedDecimal = Convert.ToUInt32(binary, 2);
            this.signedDecimal = Convert.ToInt32(binary, 2);
            this.hexadecimal = Convert.ToString(this.unsignedDecimal, 16).PadLeft(8, '0').ToUpper();
            signed = false;
        }

        public void setCWord(CWord word)
        {
            this.binary = word.binary;
            this.unsignedDecimal = word.unsignedDecimal;
            this.signedDecimal = word.signedDecimal;
            this.hexadecimal = word.hexadecimal;
            this.signed = word.signed;
        }


        public Int32 getSignedDecimal
        {
            get
            {
                return signedDecimal;
            }
        }

        public UInt32 getUnsignedDecimal
        {
            get
            {
                return unsignedDecimal;
            }
        }

        public string getHexadecimal
        {
            get
            {
                return hexadecimal;
            }
        }

        public string getBinary
        {
            get
            {
                return binary;
            }
        }
    }
}
