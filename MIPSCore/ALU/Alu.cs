﻿using System;
using MIPSCore.Control_Unit;
using MIPSCore.Util;
using MIPSCore.Instruction_Memory;
using MIPSCore.Instruction_Set;
using MIPSCore.Register_File;

namespace MIPSCore.ALU
{
    public class Alu : IAlu
    {
        public IControlUnit ControlUnit { get; set; }
        public IInstructionMemory InstructionMemory { get; set; }
        public IRegisterFile RegisterFile { get; set; }

        private Word arg1;
        private Word arg2;

        // TODO make a state register
        public bool ZeroFlag { get; private set; }
        public bool OverflowFlag { get; private set; }
        public Word GetResultLo { get; private set; }
        public Word GetResultHi { get; private set; }

        public Alu()
        {
            ZeroFlag = false;
            OverflowFlag = false;
            GetResultLo = new Word((uint) 0);
            GetResultHi = new Word((uint) 0);
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
            try { GetResultLo.Set(checked(arg1.SignedDecimal + arg2.SignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformAddU()
        {
            try { GetResultLo.Set(checked(arg1.UnsignedDecimal + arg2.UnsignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformAnd()
        {
            GetResultLo.Set(arg1.UnsignedDecimal & arg2.UnsignedDecimal);
        }

        private void PerformOr()
        {
            GetResultLo.Set(arg1.UnsignedDecimal | arg2.UnsignedDecimal);
        }

        private void PerformXor()
        {
            GetResultLo.Set(arg1.UnsignedDecimal ^ arg2.UnsignedDecimal);
        }

        private void PerformSub()
        {
            try { GetResultLo.Set(checked(arg1.SignedDecimal - arg2.SignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformSubU()
        {
            try { GetResultLo.Set(checked(arg1.UnsignedDecimal - arg2.UnsignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformSetOnLessThen()
        {
            GetResultLo.Set(arg1.SignedDecimal < arg2.SignedDecimal ? 1 : 0);
        }

        private void PerformSetOnLessThenU()
        {
            if (arg1.UnsignedDecimal < arg2.UnsignedDecimal)
                GetResultLo.Set((UInt32)1);
            else
                GetResultLo.Set((UInt32)0);
        }

        private void PerformSetOnLessThanZ()
        {
            if (arg1.SignedDecimal < 0)
                GetResultLo.Set((UInt32)1);
            else
                GetResultLo.Set((UInt32)0);
        }

        private void PerformShift(bool left)
        {
            UInt32 shiftAmount = InstructionMemory.GetShiftAmount.UnsignedDecimal;
            UInt32 valueToShift = arg2.UnsignedDecimal;
            Shift(left, valueToShift, shiftAmount);
        }

        private void PerformShiftLeft16()
        {
            UInt32 shiftAmount = 16;
            UInt32 valueToShift = arg2.UnsignedDecimal;
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
            GetResultLo.Set(valueToShift);
        }

        private void PerformMult()
        {
            Int64 res = arg1.SignedDecimal * arg2.SignedDecimal;
            GetResultLo.Set((Int32) res);
            GetResultHi.Set((Int32) (res >> 32));
        }

        private void PerformMultU()
        {
            UInt64 res = arg1.UnsignedDecimal * arg2.UnsignedDecimal;
            GetResultLo.Set((uint)res);
            GetResultHi.Set((uint)(res >> 32));
        }

        private void PerformDiv()
        {
            GetResultLo.Set(arg1.UnsignedDecimal / arg2.UnsignedDecimal);
            GetResultHi.Set(arg1.UnsignedDecimal % arg2.UnsignedDecimal);
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
                    InstructionMemory.GetImmediate.SignExtendSigned();
                    arg2 = InstructionMemory.GetImmediate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(GetType().Name + ": AluSrc out of range");
            }
        }

        private void CheckIfResultIsZero()
        {
            if (GetResultLo.UnsignedDecimal == 0)
                ZeroFlag = true;
        }
    }
}
