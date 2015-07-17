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
        private CWord resultLO;
        private CWord resultHI;

        public CALU(CCore core)
        {
            this.core = core;
            zero = false;
            overflow = false;
            resultLO = new CWord((UInt32) 0);
            resultHI = new CWord((UInt32) 0);
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
                case ALUControl.xor:            performXor();           break;
                case ALUControl.sub:            performSub();           break;
                case ALUControl.subu:           performSubU();          break;
                case ALUControl.setLessThan:    performSetOnLessThen(); break;
                case ALUControl.setLessThanU:   performSetOnLessThenU();break;
                case ALUControl.mult:           performMult();          break;
                case ALUControl.multu:          performMultU();         break;
                case ALUControl.div:            performDiv();           break;
                case ALUControl.shiftLeft:      performShift(true);     break;
                case ALUControl.shiftRight:     performShift(false);    break;
                case ALUControl.nor:
                    throw new NotImplementedException();
                
                case ALUControl.stall:  break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": AluControl out of range");
            }

            checkIfResultIsZero();
        }

        private void performAdd()
        {
            try { resultLO.set(checked((Int32)arg1.getSignedDecimal + arg2.getSignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }
        }

        private void performAddU()
        {
            try { resultLO.set(checked((UInt32)arg1.getUnsignedDecimal + arg2.getUnsignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }
        }

        private void performAnd()
        {
            resultLO.set((UInt32) arg1.getUnsignedDecimal & arg2.getUnsignedDecimal);
        }

        private void performOr()
        {
            resultLO.set((UInt32)arg1.getUnsignedDecimal | arg2.getUnsignedDecimal);
        }

        private void performXor()
        {
            resultLO.set((UInt32)arg1.getUnsignedDecimal ^ arg2.getUnsignedDecimal);
        }


        private void performSub()
        {
            try { resultLO.set(checked((Int32)arg1.getSignedDecimal - arg2.getSignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }
        }

        private void performSubU()
        {
            try { resultLO.set(checked((UInt32)arg1.getUnsignedDecimal - arg2.getUnsignedDecimal)); }
            catch (System.OverflowException) { overflow = true; }
        }

        private void performSetOnLessThen()
        {
            if (arg1.getSignedDecimal < arg2.getSignedDecimal)
                resultLO.set((Int32)1);
            else
                resultLO.set((Int32)0);
        }

        private void performSetOnLessThenU()
        {
            if (arg1.getUnsignedDecimal < arg2.getUnsignedDecimal)
                resultLO.set((UInt32)1);
            else
                resultLO.set((UInt32)0);
        }

        private void performShift(bool left)
        {
            UInt32 shiftAmount = core.getInstructionMemory.getShiftAmount.getUnsignedDecimal;
            UInt32 valueToShift = arg2.getUnsignedDecimal;
            for (int i = 0; i < shiftAmount; i++)
            {
                if (left)
                {
                    if ((valueToShift & 0x80000000) == 0x80000000)
                        overflow = true;
                    valueToShift = valueToShift << 1;
                }
                else
                {
                    if ((valueToShift & 0x00000001) == 0x00000001)
                        overflow = true;
                    valueToShift = valueToShift >> 1;
                }
            }
            resultLO.set((UInt32)valueToShift);
        }

        private void performMult()
        {
            Int64 res = arg1.getSignedDecimal * arg2.getSignedDecimal;
            resultLO.set((Int32) res);
            resultHI.set((Int32) (res >> 32));
        }

        private void performMultU()
        {
            UInt64 res = arg1.getUnsignedDecimal * arg2.getUnsignedDecimal;
            resultLO.set((UInt32)res);
            resultHI.set((UInt32)(res >> 32));
        }

        private void performDiv()
        {
            resultLO.set((UInt32) (arg1.getUnsignedDecimal / arg2.getUnsignedDecimal));
            resultHI.set((UInt32) (arg1.getUnsignedDecimal % arg2.getUnsignedDecimal));
        }

        private void setALUArguments()
        {
            overflow = false;
            zero = false;
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
            if (resultLO.getUnsignedDecimal == 0)
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

        public CWord getResultLO
        {
            get
            {
                return resultLO;
            }
        }

        public CWord getResultHI
        {
            get
            {
                return resultHI;
            }
        }
    }
}
