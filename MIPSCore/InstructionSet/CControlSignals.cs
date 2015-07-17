using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Util;
using System.IO;
using System.Xml.Serialization;

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
        [Text("and")] and,
        [Text("or")] or,
        [Text("add")] add,
        [Text("addu")] addu,
        [Text("xor")] xor,
        [Text("sub")] sub,
        [Text("sub unsigned")] subu,
        [Text("set less than")] setLessThan,
        [Text("set less than unsigned")] setLessThanU,
        [Text("multiply")] mult,
        [Text("multiply unsigned")] multu,
        [Text("divide")] div,
        [Text("shift left")] shiftLeft,
        [Text("shift right")] shiftRight,
        [Text("nor")] nor,
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
        [Text("sign extend enequal")] signExtendUnequal, 
        [Text("sign extend less or equal to zero")] signExtendLessOrEqualZero, 
        [Text("jump")] jump, 
        [Text("register")] register 
    }
    public enum DataMemoryWordSize { 
        [Text("byte")] singleByte, 
        [Text("half word")] halfWord, 
        [Text("word")] word,
    }

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

        private CInstructionSet instructionSet;
        private CInstruction instruction;

        public CControlSignals()
        {
            instructionFormat = InstructionFormat.R;
            pcSource = ProgramCounterSource.programCounter;
            systemcall = false;

            FileStream reader = new FileStream("InstructionSet/instructionSet.xml", FileMode.Open);
            XmlSerializer ser = new XmlSerializer(typeof(CInstructionSet));
            instructionSet = ser.Deserialize(reader) as CInstructionSet;
            reader.Close();
            reader = null;
         
            instruction = new CInstruction();
        }

        protected void prepareControlSignals(CWord opCode, CWord function)
        {
            instruction = instructionSet.getInstruction(opCode, function);

            instructionFormat = instruction.format;
            regDestination = instruction.regDestination;
            aluSource = instruction.aluSource;
            aluControl = instruction.aluControl;
            regWrite = instruction.regWrite;
            memWrite = instruction.memWrite;
            memRead = instruction.memRead;
            regFileInput = instruction.regFileInput;
            pcSource = instruction.pcSource;
            dataMemWordSize = instruction.dataMemWordSize;
            systemcall = instruction.systemcall;
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
            rString += "Instruction:\t\t" + instruction.name + "\n";
            rString += "Instruction Format:\t" + instructionFormat.ToText() + "\n";

            if(instruction.format == InstructionFormat.R)
                rString += "Function:\t\t" + instruction.function + "\n";
            else
                rString += "OpCode:\t\t\t" + instruction.opcode + "\n";
            rString += repeatString("-", 80);
         
            /* REGISTER FILE */
            rString += "Register Destination:\t";
            if (regWrite)
                rString += regDestination.ToText() + "\n";
            else
                rString += "no register gets overwritten\n";

            rString += "Register File Input:\t" + regFileInput.ToText() + "\n";
            rString += repeatString("-", 80);

            /* ALU */
            rString += "ALU Source:\t\t" + aluSource.ToText() + "\n";
            rString += "ALU Control:\t\t" + aluControl.ToText() + "\n";
            rString += repeatString("-", 80);

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
            rString += repeatString("-", 80);

            /* PROGRAMCOUNTER */
            rString += "Program Counter Input:\t" + pcSource.ToText() + "\n";

            return rString;
        }

        public string repeatString(string s, int count)
        {
            string rString = "";
            for (int i = 0; i < count; i++)
                rString += s;
            return rString;
        }
    }
}
