using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util
{
    public class CRegister : CWord
    {
        private UInt16 regNumber;
        private string name;

        public CRegister(UInt16 number, UInt32 unsignedDecimal, string name) 
            : base(unsignedDecimal)
        {
            regNumber = number;
            this.name = name;
        }

        public CRegister(UInt16 number, Int32 signedDecimal)
            : base(signedDecimal)
        {
            regNumber = number;
        }

        public CRegister(UInt16 number, string binary)
            : base(binary)
        {
            regNumber = number;
        }

        public string getName
        {
            get
            {
                return name;
            }
        }

    }
}
