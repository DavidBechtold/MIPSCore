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
    public class CALU
    {
        private bool zero;
        private bool overflow;

        private CCore core;
      
        private CWord arg1;
        private CWord arg2;
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
            /* set the arguments for the calculation */
            setALUArguments();

            /* perform the calculation */
            switch (core.getControlUnit.getAluControl)
            {
                case ALUControl.add:            performAdd();           break;
                case ALUControl.addu:           performAddU();          break;
                case ALUControl.and:            performAnd();           break;
                case ALUControl.or:             performOr();            break;
                case ALUControl.sub:            performSub();           break;
                case ALUControl.setOnLessThan:  performSetOnLessThen(); break;
                case ALUControl.nor:
                    throw new NotImplementedException();
                    break;
                case ALUControl.stall:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluControl out of range");
            }
        }

        private void performAdd()
        {
            try { result.set(checked((Int32)arg1.getSignedDecimal + arg2.getSignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }

            checkIfResultIsZero();
        }

        private void performAddU()
        {
            try { result.set(checked((UInt32)arg1.getUnsignedDecimal + arg2.getUnsignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }

            checkIfResultIsZero();
        }

        private void performAnd()
        {
            result.set((UInt32) arg1.getUnsignedDecimal & arg2.getUnsignedDecimal);
            checkIfResultIsZero();
        }

        private void performOr()
        {
            result.set((UInt32)arg1.getUnsignedDecimal | arg2.getUnsignedDecimal);
            checkIfResultIsZero();
        }

        private void performSub()
        {
            try { result.set(checked((Int32)arg1.getSignedDecimal - arg2.getSignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }

            checkIfResultIsZero();
        }

        private void performSetOnLessThen()
        {
            if (arg1.getSignedDecimal < arg2.getSignedDecimal)
                result.set((Int32)1);
            else
                result.set((Int32)0);

            checkIfResultIsZero();
        }

        private void setALUArguments()
        {
            arg1 = core.getRegisterFile.readRs();
            switch (core.getControlUnit.getAluSource)
            {
                /* if the regFile signal is set take the rt register from the register file */
                case ALUSource.regFile:
                    arg2 = core.getRegisterFile.readRt();
                    break;
                /* if the signExtend signal is set take the register from the sign extender */
                case ALUSource.signExtend:
                    core.getInstructionMemory.getImmediate.signExtendSigned();
                    arg2 = core.getInstructionMemory.getImmediate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluSrc out of range");
            }
        }

        private void checkIfResultIsZero()
        {
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

        public CWord getResult
        {
            get
            {
                return result;
            }
        }
    }
}
