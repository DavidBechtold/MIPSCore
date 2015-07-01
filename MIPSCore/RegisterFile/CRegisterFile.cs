using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Core;
using MIPSCore.Util;
using MIPSCore.ControlUnit;
using MIPSCore.InstructionSet;

namespace MIPSCore.RegisterFile
{
    public class CRegisterFile
    {
        private CCore core;

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
                if (core.getControlUnit.getMemToReg)
                    write(core.getDataMemory.getLoadedValue);
                else
                    write(core.getAlu.getResult);
            }
        }



        public void clearAll()
        {

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
    }
}
