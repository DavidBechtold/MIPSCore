using System;
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
        public bool SignFlag { get; private set; }
        public bool CarryFlag { get; private set; }
        public Word GetResultLo { get; private set; }
        public Word GetResultHi { get; private set; }

        public Alu()
        {
            ZeroFlag = false;
            OverflowFlag = false;
            GetResultLo = new Word((uint) 0);
            GetResultHi = new Word((uint) 0);
            arg1 = new Word(0);
            arg2 = new Word(0);
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
                case AluControl.SetLessThanEqualZero: PerformSetOnLessThanEqualZ(); break;
                case AluControl.SetGreaterThanZero: PerformSetOnGreaterThanZ(); break;
                case AluControl.SetGreaterEqualZero: PerformSetGreaterEqualZero(); break;
                case AluControl.Mult:           PerformMult();          break;
                case AluControl.Multu:          PerformMultU();         break;
                case AluControl.Div:            PerformDiv();           break;
                case AluControl.ShiftLeft:      PerformShift(true);     break;
                case AluControl.ShiftRight:     PerformShift(false);    break;
                case AluControl.ShiftRightArithmetic: PerformShiftArithmetic(false); break;
                case AluControl.ShiftLeft16:    PerformShiftLeft16();   break;
                case AluControl.Nor:
                    throw new NotImplementedException();
                
                case AluControl.Stall:  return;
                default:
                    throw new ArgumentOutOfRangeException(GetType().Name + ": AluControl out of range");
            }
            SetFlags();
        }

        private void PerformAdd()
        {
            try { GetResultLo.Set(checked(arg1.SignedDecimal + arg2.SignedDecimal)); }
            catch (OverflowException) { OverflowFlag = true; }
        }

        private void PerformAddU()
        {
            GetResultLo.Set((uint) (arg1.SignedDecimal + arg2.SignedDecimal));
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
            GetResultLo.Set((uint)(arg1.SignedDecimal - arg2.SignedDecimal));
        }

        private void PerformSetOnLessThen()
        {
            GetResultLo.Set(arg1.SignedDecimal < arg2.SignedDecimal ? 1 : 0);
        }

        private void PerformSetOnLessThenU()
        {
            if (arg1.UnsignedDecimal < arg2.UnsignedDecimal)
                GetResultLo.Set((uint)1);
            else
                GetResultLo.Set((uint)0);
        }

        private void PerformSetOnLessThanZ()
        {
            if (arg1.SignedDecimal < 0)
                GetResultLo.Set((uint)1);
            else
                GetResultLo.Set((uint)0);
        }

        private void PerformSetOnLessThanEqualZ()
        {
            if (arg1.SignedDecimal <= 0)
                GetResultLo.Set((uint)1);
            else
                GetResultLo.Set((uint)0);
        }

        private void PerformSetOnGreaterThanZ()
        {
            if (arg1.SignedDecimal > 0)
                GetResultLo.Set((uint)1);
            else
                GetResultLo.Set((uint)0);
        }

        private void PerformSetGreaterEqualZero()
        {
            if (arg1.SignedDecimal >= 0)
                GetResultLo.Set((uint)1);
            else
                GetResultLo.Set((uint)0);
        }

        private void PerformShift(bool left)
        {
            Shift(left,  false);
        }

        private void PerformShiftLeft16()
        {
            arg1 = new Word((uint) 16);
            Shift(true, false);
        }

        private void PerformShiftArithmetic(bool left)
        {
            Shift(left, true);
        }

        private void Shift(bool left, bool arithmetic)
        {
            uint shiftAmount = arg1.UnsignedDecimal;
            uint valueToShift = arg2.UnsignedDecimal;
            for (var i = 0; i < shiftAmount; i++)
            {
                if (left)
                {
                    CarryFlag = (valueToShift & 0x80000000) == 0x80000000;
                    valueToShift = valueToShift << 1;
                    if (arithmetic && CarryFlag)
                        valueToShift |= 0x00000001;
                }
                else
                {
                    CarryFlag = (valueToShift & 0x00000001) == 0x00000001;
                    valueToShift = valueToShift >> 1;
                    if (arithmetic && CarryFlag)
                        valueToShift |= 0x80000000;
                }
            }
            GetResultLo.Set(valueToShift);
        }

        private void PerformMult()
        {
            long res = (long)arg1.SignedDecimal * (long)arg2.SignedDecimal;
            GetResultLo.Set((int) res);
            GetResultHi.Set((int) (res >> 32));
        }

        private void PerformMultU()
        {
            ulong res = (ulong)arg1.UnsignedDecimal * (ulong)arg2.UnsignedDecimal;
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
            SignFlag = false;
            CarryFlag = false;
            
            switch (ControlUnit.AluSource1)
            {
                case AluSource1.Rs: arg1 = RegisterFile.ReadRs(); break;
                case AluSource1.RsSignExtend: arg1 = RegisterFile.ReadRs().SignExtend(); break;
                case AluSource1.RsSignExtendZero: arg1 = RegisterFile.ReadRs().SignExtendZero(); break;
                case AluSource1.Rs40: arg1 = new Word(((uint)RegisterFile.ReadRs().UnsignedDecimal & 0x0000001F)); break;
                case AluSource1.Shamt: arg1= new Word((InstructionMemory.GetShiftAmount.UnsignedDecimal)); break;
                default: throw new ArgumentOutOfRangeException();
            }

            switch (ControlUnit.AluSource2)
            {
                case AluSource2.Rd: arg2 = RegisterFile.ReadRd(); break;
                case AluSource2.RdSignExtend: arg2 = RegisterFile.ReadRd().SignExtend(); break;
                case AluSource2.RdSignExtendZero: arg2 = RegisterFile.ReadRd().SignExtendZero(); break;
                case AluSource2.Rt: arg2 = RegisterFile.ReadRt(); break;
                case AluSource2.RtSignExtend: arg2 = RegisterFile.ReadRt().SignExtend(); break;
                case AluSource2.RtSignExtendZero: arg2 = RegisterFile.ReadRt().SignExtendZero(); break;
                case AluSource2.ImmSignExtend: arg2 = InstructionMemory.GetImmediate.SignExtend(); break;
                case AluSource2.ImmSignExtendZero: arg2 = InstructionMemory.GetImmediate.SignExtendZero(); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void SetFlags()
        {
            if (GetResultLo.UnsignedDecimal == 0)
                ZeroFlag = true;
            if (GetResultLo.SignedDecimal < 0)
                SignFlag = true;
        }
    }
}
