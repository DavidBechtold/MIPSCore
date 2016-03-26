using System;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Data_Memory;
using MIPSCore.Instruction_Memory;
using MIPSCore.Instruction_Set;
using MIPSCore.Util;

namespace MIPSCore.Register_File
{
    public class RegisterFile : IRegisterFile
    {
        public const ushort RegisterNumberRa = 31;
        public const ushort RegisterCount = MipsRegister.NumberOfRegisters;

        public IAlu Alu { set; get; }
        public IDataMemory DataMemory { get; set; }
        public IControlUnit ControlUnit { set; get; }
        public IInstructionMemory InstructionMemory { set; get; }

        /* register */
        private readonly MipsRegister registers;

        public RegisterFile()
        {
            registers = new MipsRegister();
        }

        public void Clock()
        {
            if (!ControlUnit.RegisterWrite) return;
            //check if the result comes from the data mem or the alu
            switch (ControlUnit.RegisterFileInput)
            {
                case RegisterFileInput.AluLo:
                    Write(Alu.GetResultLo);
                    break;
                case RegisterFileInput.AluHi:
                    Write(Alu.GetResultHi);
                    break;
                case RegisterFileInput.DataMemory:
                    Write(DataMemory.LoadedValue);
                    break;
                case RegisterFileInput.ProgramCounter:
                    Write(InstructionMemory.GetProgramCounter + 4);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Flush()
        {
            registers.Flush();
        }

        private void Write(Word word)
        {
            switch(ControlUnit.RegisterDestination)
            {
                case  RegisterDestination.Rd:
                    if (InstructionMemory.GetRd.UnsignedDecimal >= MipsRegister.NumberOfRegisters)
                        throw new ArgumentOutOfRangeException();

                    registers.Write((ushort)InstructionMemory.GetRd.UnsignedDecimal, word);
                    break;

                case RegisterDestination.Rt:
                    if (InstructionMemory.GetRt.UnsignedDecimal >= MipsRegister.NumberOfRegisters)
                        throw new ArgumentOutOfRangeException();

                    registers.Write((ushort)InstructionMemory.GetRt.UnsignedDecimal, word);
                    break;

                case RegisterDestination.Ra:
                    registers.Write(RegisterNumberRa, word);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Word ReadRs()
        {
            if(InstructionMemory.GetRs.UnsignedDecimal >= MipsRegister.NumberOfRegisters)
                    throw new ArgumentOutOfRangeException();
            return registers.ReadCWord((ushort) InstructionMemory.GetRs.UnsignedDecimal);
        }

        public Word ReadRt()
        {
            if (InstructionMemory.GetRt.UnsignedDecimal >= MipsRegister.NumberOfRegisters)
                throw new ArgumentOutOfRangeException();
            return registers.ReadCWord((ushort)InstructionMemory.GetRt.UnsignedDecimal);
        }

        public Word ReadRd()
        {
            if (InstructionMemory.GetRd.UnsignedDecimal >= MipsRegister.NumberOfRegisters)
                throw new ArgumentOutOfRangeException();
            return registers.ReadCWord((ushort)InstructionMemory.GetRd.UnsignedDecimal);
        }

        public override string ToString()
        {
            return registers.ToString();
        }

        public void InitStackAndGlobalPointer()
        {
            registers.Write(29, DataMemory.SizeBytes);          //sp
            //registers.Write(28, DataMemory.SizeBytes / 2);    //gp
        }

        public int[] ReadAllRegister()
        {
            return registers.ReadRegisters();
        }

        public int ReadRegister(ushort num)
        {
            return registers.ReadRegister(num);
        }

        public uint ReadRegisterUnsigned(ushort num)
        {
            return registers.ReadRegisterUnsigned(num);
        }

        public string ToStringRegister(ushort num)
        {
            return registers.RegisterToStringDecSigned(num);
        }

        public string ToStringRegisterUnsigned(ushort num)
        {
            return registers.RegisterToStringDecUnsigned(num);
        }

        public string ToStringRegisterHex(ushort num)
        {
            return registers.RegisterToStringHex(num);
        }
    }
}
