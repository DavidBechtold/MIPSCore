using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore;
using MIPSCore.Util;
using MIPSCore.ControlUnit;
using MIPSCore.InstructionSet;

namespace MIPSCore.RegisterFile
{
    public class CRegisterFile
    {
        private CCore core;
        public const UInt16 RegisterNumber_ra = 31;
        public const UInt16 RegisterCount = CMIPSRegister.numberOfRegisters;

        /* register */
        private CMIPSRegister registers;

        public CRegisterFile(CCore core)
        {
            this.core = core;
            registers = new CMIPSRegister();
        }

        public void clock()
        {
            if (core.getControlUnit.getRegWrite)
            {
                //check if the result comes from the data mem or the alu
                switch (core.getControlUnit.getRegisterFileInput)
                {
                    case RegisterFileInput.aluLO:
                        write(core.getAlu.getResultLO);
                        break;
                    case RegisterFileInput.aluHI:
                        write(core.getAlu.getResultHI);
                        break;
                    case RegisterFileInput.dataMemory:
                        write(core.getDataMemory.getLoadedValue);
                        break;
                    case RegisterFileInput.programCounter:
                        write(core.getInstructionMemory.getProgramCounter +  4);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(this.GetType().Name + ": RegisterFileInput out of range");
                }                    
            }
        }

        public void flush()
        {
            registers.flush();
        }

        private void write(CWord word)
        {
            switch(core.getControlUnit.getRegDestination)
            {
                case  RegisterDestination.rd:
                    if (core.getInstructionMemory.getRd.getUnsignedDecimal >= CMIPSRegister.numberOfRegisters)
                        throw new ArgumentOutOfRangeException(this.GetType().Name + ": RD number out of Range");

                    registers.write((ushort) core.getInstructionMemory.getRd.getUnsignedDecimal, word);
                    break;

                case RegisterDestination.rt:
                    if (core.getInstructionMemory.getRt.getUnsignedDecimal >= CMIPSRegister.numberOfRegisters)
                        throw new ArgumentOutOfRangeException(this.GetType().Name + ": RT number out of Range");

                    registers.write((ushort)core.getInstructionMemory.getRt.getUnsignedDecimal, word);
                    break;

                case RegisterDestination.ra:
                    registers.write(RegisterNumber_ra, word);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": Registerdestination out of Range");
            }
        }

        public CWord readRs()
        {
            if(core.getInstructionMemory.getRs.getUnsignedDecimal >= CMIPSRegister.numberOfRegisters)
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": RS number out of Range");
            return registers.readCWord((ushort) core.getInstructionMemory.getRs.getUnsignedDecimal);
        }

        public CWord readRt()
        {
            if (core.getInstructionMemory.getRt.getUnsignedDecimal >= CMIPSRegister.numberOfRegisters)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": RT number out of Range");
            return registers.readCWord((ushort)core.getInstructionMemory.getRt.getUnsignedDecimal);
        }

        public CWord readRd()
        {
            if (core.getInstructionMemory.getRd.getUnsignedDecimal >= CMIPSRegister.numberOfRegisters)
                throw new ArgumentOutOfRangeException(this.GetType().Name + ": RD number out of Range");
            return registers.readCWord((ushort)core.getInstructionMemory.getRd.getUnsignedDecimal);
        }

        public override string ToString()
        {
            return registers.ToString();
        }

        public void initStackPointer()
        {
            registers.write(29, core.getDataMemory.sizeBytes);
        }

        public Int32[] readAllRegister()
        {
            return registers.readRegisters();
        }

        public Int32 readRegister(UInt16 num)
        {
            return registers.readRegister(num);
        }

        public UInt32 readRegisterUnsigned(UInt16 num)
        {
            return registers.readRegisterUnsigned(num);
        }

        public string toStringRegister(UInt16 num)
        {
            return registers.registerToStringDecSigned(num);
        }

        public string toStringRegisterUnsigned(UInt16 num)
        {
            return registers.registerToStringDecUnsigned(num);
        }

        public string toStringRegisterHex(UInt16 num)
        {
            return registers.registerToStringHex(num);
        }
    }
}
