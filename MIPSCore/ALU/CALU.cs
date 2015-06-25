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
        private bool overflow;

        private CCore core;
        private CWord result;

        public CALU(CCore core)
        {
            this.core = core;
            zero = false;
            overflow = false;
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

        

        private void performAdd()
        {
            // always add register RS with the ALUSource Register
            switch (core.getControlUnit.getAluSource)
            {
                case ALUSource.regFile:
                    try{ result.set(checked((Int32)core.getRegisterFile.readRs().getSignedDecimal + core.getRegisterFile.readRt().getSignedDecimal)); }
                    catch (System.OverflowException ) { overflow = true; }
                    break;
                case ALUSource.signExtend:
                    core.getInstructionFetch.getImmediate.signExtendSigned();
                    try { result.set((checked((Int32)core.getRegisterFile.readRs().getSignedDecimal + core.getInstructionFetch.getImmediate.getSignedDecimal))); }
                    catch (System.OverflowException) { overflow = true; }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluSrc out of range");
            }

            if (result.getSignedDecimal == 0)
                zero = true;
        }

        private void performAddU()
        {
            // always add register RS with the ALUSource Register
            switch (core.getControlUnit.getAluSource)
            {
                case ALUSource.regFile:
                    try{ result.set(checked((UInt32)core.getRegisterFile.readRs().getUnsignedDecimal + core.getRegisterFile.readRt().getUnsignedDecimal)); }
                    catch (System.OverflowException ) { overflow = true; }
                    break;
                case ALUSource.signExtend:
                    core.getInstructionFetch.getImmediate.signExtendUnsigned();
                    try { result.set((checked((UInt32)core.getRegisterFile.readRs().getUnsignedDecimal + core.getInstructionFetch.getImmediate.getUnsignedDecimal))); }
                    catch (System.OverflowException) { overflow = true; }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluSrc out of range");
            }

            if (result.getUnsignedDecimal == 0)
                zero = true;
        }

        public bool zeroFlag
        {
            get
            {
                return zero;
            }
        }

        public bool overflowFlag
        {
            get
            {
                return zero;
            }
        }
    }
}
