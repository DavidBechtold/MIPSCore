using System;

namespace MIPSCore.Util
{
    public class Register : Word
    {
        // ReSharper disable once NotAccessedField.Local
        private ushort regNumber;
        private readonly string name;

        public Register(UInt16 number, UInt32 unsignedDecimal, string name) 
            : base(unsignedDecimal)
        {
            regNumber = number;
            this.name = name;
        }

        public Register(UInt16 number, Int32 signedDecimal)
            : base(signedDecimal)
        {
            regNumber = number;
        }

        public Register(UInt16 number, string binary)
            : base(binary)
        {
            regNumber = number;
        }

        public string GetName
        {
            get
            {
                return name;
            }
        }

    }
}
