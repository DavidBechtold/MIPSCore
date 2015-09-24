using System;
using MIPSCore.Control_Unit;
using MIPSCore.Util;
using MIPSCore.InstructionSet;
using MIPSCore.Instruction_Memory;
using MIPSCore.Register_File;

namespace MIPSCore.ALU
{
    public class Alu : IAlu
    {
        public IControlUnit ControlUnit { get; set; }
        public IInstructionMemory InstructionMemory { get; set; }
        public IRegisterFile RegisterFile { get; set; }

        private CWord arg1;
        private CWord arg2;

        // TODO make a state register
        public bool ZeroFlag { get; private set; }
        public bool OverflowFlag { get; private set; }
        public CWord GetResultLo { get; private set; }
        public CWord GetResultHi { get; private set; }

        public Alu()
        {
            ZeroFlag = false;
            OverflowFlag = false;
            GetResultLo = new CWord((uint) 0);
            GetResultHi = new CWord((uint) 0);
        }

        public void Clock()
        {
            /* set the arguments for the calculation */
            SetAluArguments();

            /* perform the calculation */
            switch (ControlUnit.AluControl)
            {
                case AluControl.Add:            PerformAdd();           break;
                case AluControl.Addu:           PerformAddU();          break;
                case AluControl.And:            PerformAnd();           break;
                case AluControl.Or:             PerformOr();            break;
                case AluControl.Xor:            PerformXor();           break;
                case AluControl.Sub:            PerformSub();           break;
                case AluControl.Subu:           PerformSubU();          break;
                case AluControl.SetLessThan:    PerformSetOnLessThen(); break;
                case AluControl.SetLessThanU:   PerformSetOnLessThenU();break;
                case AluControl.SetLessThanZero:PerformSetOnLessThanZ();break;
                case AluControl.Mult:           PerformMult();          break;
                case AluControl.Multu:          PerformMultU();         break;
                case AluControl.Div:            PerformDiv();           break;
                case AluControl.ShiftLeft:      PerformShift(true);     break;
                case AluControl.ShiftRight:     PerformShift(false);    break;
                case AluControl.ShiftLeft16:    PerformShiftLeft16();   break;
                case AluControl.Nor:
                    throw new NotImplementedException();
                
                case AluControl.Stall:  break;
                default:
                    throw new ArgumentOutOfRangeException(GetType().Name + ": AluControl out of range");
            }
            CheckIfResultIsZero();
        }

        private void PerformAdd()
        {
            try { GetResultLo.set(checked(arg1.getSignedDecimal + arg2.getSignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformAddU()
        {
            try { GetResultLo.set(checked(arg1.getUnsignedDecimal + arg2.getUnsignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformAnd()
        {
            GetResultLo.set(arg1.getUnsignedDecimal & arg2.getUnsignedDecimal);
        }

        private void PerformOr()
        {
            GetResultLo.set(arg1.getUnsignedDecimal | arg2.getUnsignedDecimal);
        }

        private void PerformXor()
        {
            GetResultLo.set(arg1.getUnsignedDecimal ^ arg2.getUnsignedDecimal);
        }

        private void PerformSub()
        {
            try { GetResultLo.set(checked(arg1.getSignedDecimal - arg2.getSignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformSubU()
        {
            try { GetResultLo.set(checked(arg1.getUnsignedDecimal - arg2.getUnsignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformSetOnLessThen()
        {
            if (arg1.getSignedDecimal < arg2.getSignedDecimal)
                GetResultLo.set(1);
            else
                GetResultLo.set(0);
        }

        private void PerformSetOnLessThenU()
        {
            if (arg1.getUnsignedDecimal < arg2.getUnsignedDecimal)
                GetResultLo.set((UInt32)1);
            else
                GetResultLo.set((UInt32)0);
        }

        private void PerformSetOnLessThanZ()
        {
            if (arg1.getSignedDecimal < 0)
                GetResultLo.set((UInt32)1);
            else
                GetResultLo.set((UInt32)0);
        }

        private void PerformShift(bool left)
        {
            UInt32 shiftAmount = InstructionMemory.GetShiftAmount.getUnsignedDecimal;
            UInt32 valueToShift = arg2.getUnsignedDecimal;
            Shift(left, valueToShift, shiftAmount);
        }

        private void PerformShiftLeft16()
        {
            UInt32 shiftAmount = 16;
            UInt32 valueToShift = arg2.getUnsignedDecimal;
            Shift(true, valueToShift, shiftAmount);
        }

        private void Shift(bool left, UInt32 valueToShift, UInt32 shiftAmount)
        {
            for (int i = 0; i < shiftAmount; i++)
            {
                if (left)
                {
                    if ((valueToShift & 0x80000000) == 0x80000000)
                        OverflowFlag = true;
                    valueToShift = valueToShift << 1;
                }
                else
                {
                    if ((valueToShift & 0x00000001) == 0x00000001)
                        OverflowFlag = true;
                    valueToShift = valueToShift >> 1;
                }
            }
            GetResultLo.set(valueToShift);
        }

        private void PerformMult()
        {
            Int64 res = arg1.getSignedDecimal * arg2.getSignedDecimal;
            GetResultLo.set((Int32) res);
            GetResultHi.set((Int32) (res >> 32));
        }

        private void PerformMultU()
        {
            UInt64 res = arg1.getUnsignedDecimal * arg2.getUnsignedDecimal;
            GetResultLo.set((uint)res);
            GetResultHi.set((uint)(res >> 32));
        }

        private void PerformDiv()
        {
            GetResultLo.set(arg1.getUnsignedDecimal / arg2.getUnsignedDecimal);
            GetResultHi.set(arg1.getUnsignedDecimal % arg2.getUnsignedDecimal);
        }

        private void SetAluArguments()
        {
            OverflowFlag = false;
            ZeroFlag = false;
            arg1 = RegisterFile.ReadRs();
            switch (ControlUnit.AluSource)
            {
                /* if the regFile signal is set take the rt register from the register file */
                case AluSource.RegFile:
                    arg2 = RegisterFile.ReadRt();
                    break;
                /* if the signExtend signal is set take the register from the sign extender */
                case AluSource.SignExtend:
                    InstructionMemory.GetImmediate.signExtendSigned();
                    arg2 = InstructionMemory.GetImmediate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(GetType().Name + ": AluSrc out of range");
            }
        }

        private void CheckIfResultIsZero()
        {
            if (GetResultLo.getUnsignedDecimal == 0)
                ZeroFlag = true;
        }
    }
}
