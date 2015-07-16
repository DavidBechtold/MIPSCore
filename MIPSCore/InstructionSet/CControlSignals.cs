using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Util;

namespace MIPSCore.InstructionSet
{
    public enum InstructionFormat {
        [Text("R")] R,
        [Text("I")] I,
        [Text("J")] J
    }
    public enum RegisterDestination { 
        [Text("rt")] rt,
        [Text("rd")] rd, 
        [Text("ra")] ra 
    }
    public enum ALUSource { 
        [Text("register file")] regFile, 
        [Text("sign extend")] signExtend 
    }
    public enum ALUControl {
        [Text("and 0")] and = 0,
        [Text("or 1")] or = 1,
        [Text("add 2")] add = 2,
        [Text("addu ?")] addu,
        [Text("sub 6")] sub = 6,
        [Text("set on less than 7")] setOnLessThan = 7,
        [Text("multiply 9")] mult,
        [Text("divide 10")] div,
        [Text("shift left 3")] shiftLeft,
        [Text("shift right 4")] shiftRight,
        [Text("nor 12")] nor = 12,
        [Text("stall")] stall,
    }
    public enum RegisterFileInput { 
        [Text("alu low register")] aluLO, 
        [Text("alu high register")] aluHI,
        [Text("data memory")] dataMemory,
        [Text("program counter")] programCounter
    }
    public enum ProgramCounterSource { 
        [Text("program counter")] programCounter, 
        [Text("sign extend equal")] signExtendEqual, 
        [Text("sign extend unequal")] signExtendUnequal, 
        [Text("sign extend less or equal zero")] signExtendLessOrEqualZero, 
        [Text("jump")] jump, 
        [Text("register")] register 
    }
    public enum DataMemoryWordSize { 
        [Text("byte")] singleByte, 
        [Text("half word")] halfWord, 
        [Text("word")] word 
    }

    /*public enum IInstructions
    {
        [Text("beq: branch on equal")] beq = 4, 
        [Text("bne: branch on not equal")] bne = 5, 
        [Text("blez: branch on less than or equal to zero")] blez = 6,
        [Text("addi: addition imm.")] addi = 8, 
        [Text("addiu: addition imm. unsigned")] addiu = 9, 
        [Text("slti: set less than imm.")] slti = 10, 
        [Text("sltiu: set less than imm. unsigned")] sltiu = 11,
        [Text("andi: and imm.")] andi = 12,
        [Text("ori: or imm.")] ori = 13, 
        [Text("xori: xor imm.")] xori = 14,
        [Text("lb: load byte")] lb = 32,
        [Text("lh: load halfword")] lh = 33,
        [Text("lw: load word")] lw = 35,
        [Text("lbu: load byte unsigned")] lbu = 36,
        [Text("lhu: load halfword unsigned")] lhu = 37,
        [Text("sb: store byte")] sb = 40,
        [Text("sh: store halfword")] sh = 41,
        [Text("sw: store word")] sw = 43,
    }*/

    public class CControlSignals
    {
        
        /* control signals */
        private InstructionFormat instructionFormat;
        private RegisterDestination regDestination; //which register will be written
        private ALUSource aluSource;                //alu take the value fom regFile or from the sign extender (immediate cmd)
        private ALUControl aluControl;              //which operation the alu should perform
        private bool regWrite;                      //true => write register | false => no register to write (jmp, beq commands)
        private bool memWrite;                      //true => write memory
        private bool memRead;                       //true => read memory
        private RegisterFileInput regFileInput;
        private ProgramCounterSource pcSource;      //take the source from the programcounter or from the sign extender (jmp,.. instruction)
        private DataMemoryWordSize dataMemWordSize; //how much data must be read from the data memory
        private bool systemcall;                    //a systemcall occur
        string instruction;

        public CControlSignals()
        {
            instructionFormat = InstructionFormat.R;
            pcSource = ProgramCounterSource.programCounter;
            systemcall = false;
            instruction = "";
        }

        protected void prepareControlSignals(CWord opCode, CWord function)
        {
            switch (getFormat(opCode))
            {
                case InstructionFormat.R:
                    prepareRFormatControlSignals(function);
                    break;
                case InstructionFormat.I:
                    prepareIFormatControlSignals(opCode);
                    break;
                case InstructionFormat.J:
                    prepareJFormatControlSignals(opCode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": InstuctionFormat out of range");
            }
        }

        private InstructionFormat getFormat(CWord opCode)
        {
            switch (opCode.getUnsignedDecimal)
            {
                /* R-Format */
                case 0:
                    instructionFormat = InstructionFormat.R;
                    return InstructionFormat.R;
      
                /* I-Format */
                case 4: //beq
                case 5: //bne
                case 6: //blez: Branch on less than or equal to zero
                case 8: //addi
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
                    instructionFormat = InstructionFormat.I;
                    return InstructionFormat.I;
                /* J-Format */
                case 2: //j: jump
                case 3: //jal: jump and link
                    instructionFormat = InstructionFormat.J;
                    return InstructionFormat.J;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": opCode " + opCode.getUnsignedDecimal + " out of range");
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
            regFileInput = RegisterFileInput.aluLO;   //save the result from the alu to the register
            systemcall = false;

            // check function 
            switch (function.getUnsignedDecimal)
            {
                case 0: //sll: shift left logical
                    instruction = "sll: shift left logical";
                    aluControl = ALUControl.shiftLeft;
                    break;
                case 2: //srl: shift right logicl
                    instruction = "srl: shift right logical";
                    aluControl = ALUControl.shiftRight;
                    break;
                case 8: //jr: jump register
                    instruction = "jr: jump register";
                    memRead = false;
                    memWrite = false;
                    regWrite = false;
                    aluControl = ALUControl.stall;
                    pcSource = ProgramCounterSource.register;
                    break;
                case 12: //systemcall
                    instruction = "systemcall";
                    regWrite = false;
                    systemcall = true;
                    aluControl = ALUControl.stall;
                    break;
                case 16: //mfhi: move from high
                    instruction = "mfhi: move from high";
                    regFileInput = RegisterFileInput.aluHI;     //read from hi result
                    aluControl = ALUControl.stall;
                    break;
                case 24: //mult
                case 25: //multu
                    throw new NotImplementedException("not implemented");
                case 32: //add
                    instruction = "add: addition";
                    aluControl = ALUControl.add;
                    break;
                case 33: //addu
                    instruction = "addu: addition unsigned";
                    aluControl = ALUControl.addu;
                    break;
                case 34: //sub
                    instruction = "sub: subtraction";
                    aluControl = ALUControl.sub;
                    break;
                case 35: //subu
                    throw new NotImplementedException("not implemented");
                case 36: //and
                    instruction = "and";
                    aluControl = ALUControl.and;
                    break;
                case 37: //or
                    instruction = "or";
                    aluControl = ALUControl.or;
                    break;
                case 38: //xor
                    instruction = "xor";
                    throw new NotImplementedException("not implemented");
                case 39: //nor
                    instruction = "nor";
                    aluControl = ALUControl.nor;
                    break;
                case 42: //slt: set less than
                    instruction = "slt: set less than";
                    aluControl = ALUControl.setOnLessThan;
                    break;
                case 43: //sltu:
                    throw new NotImplementedException("not implemented");
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": function out of range");
            }
        }

        private void prepareIFormatControlSignals(CWord opCode)
        {
            regDestination = RegisterDestination.rt;
            aluSource = ALUSource.signExtend;
            systemcall = false;
           
            /* TODO set this per opcode */
            pcSource = ProgramCounterSource.programCounter;
            
            // check function 
            switch (opCode.getUnsignedDecimal)
            {
                case 4: //beq
                    instruction = "beq: branch on equal";
                    regWrite = false;   //don't write value back to the register file
                    memRead = false;    //no need to read from data memory
                    memWrite = false;   //no need to write data to the data memory
                    pcSource = ProgramCounterSource.signExtendEqual; //it's a branch command => if result are equal jump
                    aluSource = ALUSource.regFile;
                    aluControl = ALUControl.sub;
                    break;
                case 5: //bne
                    instruction = "bne: branch on not equal";
                    regWrite = false;   //don't write value back to the register file
                    memRead = false;    //no need to read from data memory
                    memWrite = false;   //no need to write data to the data memory
                    pcSource = ProgramCounterSource.signExtendUnequal; //it's a branch command => if result are unequal jump
                    aluSource = ALUSource.regFile;
                    aluControl = ALUControl.sub;
                    break;
                case 6: //blez: Branch on less than or equal to zero
                    instruction = "blez: branch on less than or equal to zero";
                    regWrite = false;   //don't write value back to the register file
                    memRead = false;    //no need to read from data memory
                    memWrite = false;   //no need to write data to the data memory
                    pcSource = ProgramCounterSource.signExtendLessOrEqualZero; //it's a branch command => if result are unequal jump
                    aluSource = ALUSource.regFile;
                    aluControl = ALUControl.setOnLessThan;
                    break; 
                case 8: //addi
                    regWrite = true;                        //write result back to the register file
                    memRead = false;                        //no need to read from data memory
                    memWrite = false;                       //no need to write data to the data memory
                    regFileInput = RegisterFileInput.aluLO;   //save the result from the alu to the register
                    aluControl = ALUControl.add;
                    break;
                case 9: //addiu
                    regWrite = true;                        //write result back to the register file
                    memRead = false;                        //no need to read from data memory
                    memWrite = false;                       //no need to write data to the data memory
                    regFileInput = RegisterFileInput.aluLO;   //save the result from the alu to the register
                    aluControl = ALUControl.addu;
                    break;
                case 12://andi
                    throw new NotImplementedException();
                case 32: //lb: load byte
                    regWrite = true;                                // need to write data back to the register file
                    memRead = true;                                 //read value from data memory
                    regFileInput = RegisterFileInput.dataMemory;    //save the result from the data memory to the register                        
                    aluControl = ALUControl.add;                    //add baseregister to the offset
                    dataMemWordSize = DataMemoryWordSize.word;      //write a word size to the datamemory
                    break;
                case 36: //lbu: load byte unsigned
                    dataMemWordSize = DataMemoryWordSize.singleByte;
                    break;
                case 33: //lh: load half word
                    dataMemWordSize = DataMemoryWordSize.halfWord;
                    break;
                case 37: //lhu: load half word unsigned
                case 35: //lw: load word
                    regWrite = true;                                //no need to write data back to the register file
                    memRead = true;                                 //read value from data memory
                    memWrite = false;                               
                    regFileInput = RegisterFileInput.dataMemory;    //save the result from the data memory to the register    
                    aluControl = ALUControl.add;                    //add baseregister to the offset
                    dataMemWordSize = DataMemoryWordSize.word;      //write a word size to the datamemory
                    break;
                case 13: //ori
                case 40: //sb: store byte
                case 41: //sh: store halfword
                    throw new NotImplementedException();
                case 10: //slti: set less than imm.
                    regWrite = true;
                    memRead = false;
                    memWrite = false;
                    regFileInput = RegisterFileInput.aluLO;   //save the result from the alu to the register
                    aluControl = ALUControl.setOnLessThan;
                    break;
                case 11: //sltiu: set less than imm. unsigned
                    throw new NotImplementedException();
                case 43: //sw: store word
                    regWrite = false;                           //no need to write data back to the register file
                    memWrite = true;                            //we need to write to the data memory
                    aluControl = ALUControl.add;                //add baseregister to the offset
                    dataMemWordSize = DataMemoryWordSize.word;  //write a word size to the datamemory
                    break;
                case 14: //xori: xor imm.
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": opCode out of range");
            }
        }

        private void prepareJFormatControlSignals(CWord opCode)
        {
            pcSource = ProgramCounterSource.jump;   //take the jump address for the source of the pc
            aluSource = ALUSource.regFile;
            aluControl = ALUControl.stall;
            memRead = false;
            memWrite = false;
            systemcall = false;
            
            switch (opCode.getUnsignedDecimal)
            {
                case 2: //j: jump
                    regWrite = false;   //no need to write a register
                    break;
                case 3: //jal: jump and link
                    regWrite = true;                            //need to write the program counter to $ra
                    regDestination = RegisterDestination.ra;    //save the program counter to the ra register
                    regFileInput = RegisterFileInput.programCounter; 
                    break;
                default:
                    throw new ArgumentOutOfRangeException(this.GetType().Name + ": opCode out of range");
            }
        }

        public InstructionFormat getInstructionFormat { get { return instructionFormat; } }
        public RegisterDestination getRegDestination { get { return regDestination; } }
        public ALUSource getAluSource { get { return aluSource; } }
        public ALUControl getAluControl { get { return aluControl; } }
        public bool getRegWrite { get { return regWrite; } }
        public bool getMemWrite { get { return memWrite; } }
        public bool getMemRead { get { return memRead; } }
        public RegisterFileInput getRegisterFileInput { get { return regFileInput; } }
        public ProgramCounterSource getPcSource { get { return pcSource; } }
        public DataMemoryWordSize getDataMemoryWordSize { get { return dataMemWordSize; } }
        public bool getSystemcall { get { return systemcall; } }

        public override string ToString()
        {
            string rString = "";

            /* INSTRUCTION */
            rString += "Instruction Format:\t" + instructionFormat.ToText() + "\n";
            
            /* REGISTER FILE */
            rString += "Register Destination:\t";
            if (regWrite)
                rString += regDestination.ToText() + "\n";
            else
                rString += "no register gets overwritten\n";

            rString += "Register File Input:\t" + regFileInput.ToText() + "\n";

            /* ALU */
            rString += "ALU Source:\t\t" + aluSource.ToText() + "\n";
            rString += "ALU Control:\t\t" + aluControl.ToText() + "\n";
            
            /* DATA MEMORY */
            rString += "Data Memory Write:\t";
            if (memWrite)
                rString += "yes \n";
            else
                rString += "no \n";

            rString += "Data Memory Read:\t";
            if (memRead)
                rString += "yes \n";
            else
                rString += "no \n";

            rString += "Data Memory Size:\t" + dataMemWordSize.ToText() + "\n";

            /* PROGRAMCOUNTER */
            rString += "Program Counter Input:\t" + pcSource.ToText() + "\n";

            return rString;
        }
    }
}
