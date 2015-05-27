using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Util;

namespace MIPSCore.InstructionSet
{
    class CControlSignals
    {
        public enum RegisterDestination { rt, rd };
        public enum ALUSource { regFile, signExtend }
        public enum ALUControl {
            and             = 0,
            or              = 1,
            add             = 2,
            addu, 
            sub             = 6, 
            setOnLessThan   = 7,
            nor             = 12,
        }
        public enum ProgramCounterSource { signExtend, programCounter }

        /* control signals */
        private RegisterDestination regDestination; //which register will be written
        private ALUSource aluSource;      //alu take the value fom regFile or from the sign extender (immediate cmd)
        private ALUControl aluControl;     //which operation the alu should perform
        private bool regWrite;       //true => write register | false => no register to write (jmp, beq commands)
        private bool memWrite;       //true => write memory
        private bool memRead;        //true => read memory
        private bool memToReg;       //true => write memory content to register
        private ProgramCounterSource pcSource;       //take the source from the programcounter or from the sign extender (jmp,.. instruction)

        public CControlSignals(CWord opCode, CWord function)
        {
            prepareControlSignals(opCode, function);
        }

        private void prepareControlSignals(CWord opCode, CWord function)
        {
            switch (getFormat(opCode))
            {
                case InstructionFormat.R:
                    prepareRFormatControlSignals(function);
                    break;
                case InstructionFormat.I:

                    break;
                case InstructionFormat.J:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": InstuctionFormat out of range");
            }
        }

        public enum InstructionFormat {R, I, J}
        private InstructionFormat getFormat(CWord opCode)
        {
            switch (opCode.getUnsignedDecimal)
            {
                /* R-Format */
                case 0:
                    return InstructionFormat.R;
      
                /* I-Format */
                case 4: //beq
                case 5: //bne
                case 8: //andi
                case 9: //addiu
                case 12://andi
                case 32: //lb: load byte
                case 36: //lbu: load byte unsigned
                case 33: //lh: load half word
                case 37: //lhu: load half word unsigned
                case 35: //lw: load word
                case 13: //ori
                case 40: //sb: store byte
                case 41: //sh: store halfword
                case 10: //slti: set less than imm.
                case 11: //sltiu: set less than imm. unsigned
                case 43: //sw: store word
                case 14: //xori: xor imm.
                    return InstructionFormat.I;
                /* J-Format */
                case 2: //j: jump
                case 3: //jal: jump and link
                    return InstructionFormat.J;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": opCode out of range");
            }
        }

        private void prepareRFormatControlSignals(CWord function)
        {
            regDestination = RegisterDestination.rd;
            aluSource = ALUSource.regFile;  
            pcSource = ProgramCounterSource.programCounter;
            regWrite = true;
            memRead = false;
            memWrite = false;
            memToReg = false;

            // check function 
            switch (function.getUnsignedDecimal)
            {
                case 0: //sll: shift left logical
                case 2: //srl: shift right logicl
                case 24: //mult
                case 25: //multu
                    throw new NotImplementedException("not implemented");
                case 32: //add
                    aluControl = ALUControl.add;
                    break;
                case 33: //addu
                    aluControl = ALUControl.addu;
                    break;
                case 34: //sub
                    aluControl = ALUControl.sub;
                    break;
                case 35: //subu
                    throw new NotImplementedException("not implemented");
                case 36: //and
                    aluControl = ALUControl.and;
                    break;
                case 37: //or
                    aluControl = ALUControl.or;
                    break;
                case 38: //xor
                    throw new NotImplementedException("not implemented");
                case 39: //nor
                    aluControl = ALUControl.nor;
                    break;
                case 42: //slt: set less than
                    aluControl = ALUControl.setOnLessThan;
                    break;
                case 43: //sltu:
                    throw new NotImplementedException("not implemented");
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": opCode out of range");
            }
            
        }
    }
}
