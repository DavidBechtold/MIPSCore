using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;
using MIPSCore.Util;
using MIPSCore.InstructionSet;

namespace MIPSCore.ALU
{
    class CALU
    {
        private bool zero;
        private CCore core;
        private CWord result;

        public CALU(CCore core)
        {
            this.core = core;
            zero = false;
            result = new CWord((UInt32) 0);
        }

        public void clock()
        {
            switch (core.getControlUnit.getAluControl)
            {
                case ALUControl.add:
                    performAdd();
                    break;
                case ALUControl.addu:
                    performAddU();
                    break;
                case ALUControl.and:
                    throw new NotImplementedException();
                    break;
                case ALUControl.or:
                    throw new NotImplementedException();
                    break;
                case ALUControl.sub:
                    throw new NotImplementedException();
                    break;
                case ALUControl.setOnLessThan:
                    throw new NotImplementedException();
                    break;
                case ALUControl.nor:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluControl out of range");
            }
        }

        public bool getZero
        {
            get
            {
                return zero;
            }
        }

        private void performAdd()
        {
            // always add register RS with the ALUSource Register
            switch (core.getControlUnit.getAluSource)
            {
                case ALUSource.regFile:
                    result.set((Int32)core.getRegisterFile.readRs().getSignedDecimal + core.getRegisterFile.readRt().getSignedDecimal);
                    break;
                case ALUSource.signExtend:
                    core.getInstructionFetch.getImmediate.signExtendSigned();
                    result.set((Int32)core.getRegisterFile.readRs().getSignedDecimal + core.getInstructionFetch.getImmediate.getSignedDecimal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluSrc out of range");
            }
        }

        private void performAddU()
        {
            // always add register RS with the ALUSource Register
            switch (core.getControlUnit.getAluSource)
            {
                case ALUSource.regFile:
                    result.set((UInt32)core.getRegisterFile.readRs().getUnsignedDecimal + core.getRegisterFile.readRt().getUnsignedDecimal);
                    break;
                case ALUSource.signExtend:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluSrc out of range");
            }
        }
    }
}
