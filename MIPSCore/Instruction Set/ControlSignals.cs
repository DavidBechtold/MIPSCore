using System.IO;
using System.Xml.Serialization;
using MIPSCore.Util;

namespace MIPSCore.Instruction_Set
{
    public enum InstructionFormat {
        [Text("R")] R,
        [Text("I")] I,
        [Text("J")] J
    }
    public enum RegisterDestination { 
        [Text("Rt")] Rt,
        [Text("Rd")] Rd, 
        [Text("Ra")] Ra 
    }
    public enum AluSource { 
        [Text("Rt")] Rt,
        [Text("Rt sign extend")]  RtSignExtend,
        [Text("Rt sign extend zero")] RtSignExtendZero,
        [Text("Rd")] Rd,
        [Text("Rd sign extend")] RdSignExtend,
        [Text("Rd sign extend zero")] RdSignExtendZero,
        [Text("Immediate sign extend")] ImmSignExtend,
        [Text("Immediate sign extend zero")] ImmSignExtendZero
    }
    public enum AluControl {
        [Text("And")] And,
        [Text("Or")] Or,
        [Text("Add")] Add,
        [Text("Addu")] Addu,
        [Text("Xor")] Xor,
        [Text("Sub")] Sub,
        [Text("Sub unsigned")] Subu,
        [Text("Set less than")] SetLessThan,
        [Text("Set less than unsigned")] SetLessThanU,
        [Text("set less than zero")] SetLessThanZero,
        [Text("set less than zero")] SetGreaterEqualZero,
        [Text("Multiply")] Mult,
        [Text("Multiply unsigned")] Multu,
        [Text("Divide")] Div,
        [Text("Shift left")] ShiftLeft,
        [Text("Shift right")] ShiftRight,
        [Text("Shift 16 left")] ShiftLeft16,
        [Text("Nor")] Nor,
        [Text("Stall")] Stall,
    }
    public enum RegisterFileInput { 
        [Text("alu low register")] AluLo, 
        [Text("alu high register")] AluHi,
        [Text("data memory")] DataMemory,
        [Text("program counter")] ProgramCounter
    }
    public enum ProgramCounterSource { 
        [Text("program counter")] ProgramCounter, 
        [Text("sign extend equal")] SignExtendEqual, 
        [Text("sign extend unequal")] SignExtendUnequal,
        [Text("sign extend on less than zero")]SignExtendLessThanZero, 
        [Text("sign extend on less or equal than zero")] SignExtendLessOrEqualZero, 
        [Text("jump")] Jump, 
        [Text("register")] Register 
    }
    public enum DataMemoryWordSize { 
        [Text("byte")] SingleByte, 
        [Text("half word")] HalfWord, 
        [Text("word")] Word,
    }

    public class ControlSignals: IControlSignals
    {
        public string GetInstructionFriendlyName { get; private set; }
        public string GetInstructionAssemblerName { get; private set; }
        public string GetInstructionExample { get; private set; }
        public string GetInstructionMeaning { get; private set; }
        public string GetInstructionFormat { get; private set; }
        public string GetInstructionOpCode { get; private set; }
        public string GetInstructionFunction { get; private set; }

        public InstructionFormat InstructionFormat { get; private set; }
        public RegisterDestination RegisterDestination { get; private set; }
        public AluSource AluSource { get; private set; }
        public AluControl AluControl { get; private set; }
        public bool RegisterWrite { get; private set; }
        public bool MemoryWrite { get; private set; }
        public bool MemoryRead { get; private set; }
        public bool MemorySignExtend { get; private set; }
        public RegisterFileInput RegisterFileInput { get; private set; }
        public ProgramCounterSource ProgramCounterSource { get; private set; }
        public DataMemoryWordSize DataMemoryWordSize { get; private set; }
        public bool Systemcall { get; private set; }

        private readonly CInstructionSet instructionSet;
        private CInstruction instruction;

        public ControlSignals()
        {
            InstructionFormat = InstructionFormat.R;
            ProgramCounterSource = ProgramCounterSource.ProgramCounter;
            Systemcall = false;

            if(!File.Exists("Instruction Set//instructionSet.xml"))
                throw new FileNotFoundException("File \"Instruction Set/instructionSet.xml\" not found.");

            var reader = new FileStream("Instruction Set//instructionSet.xml", FileMode.Open);
            var ser = new XmlSerializer(typeof(CInstructionSet));
            instructionSet = ser.Deserialize(reader) as CInstructionSet;
            reader.Close();
            instruction = new CInstruction();
        }

        protected void PrepareControlSignals(Word opCode, Word function, Word rd)
        {
            instruction = instructionSet.GetInstruction(opCode, function, rd);

            GetInstructionFriendlyName = instruction.Name;
            GetInstructionAssemblerName = instruction.Assembler;
            GetInstructionExample = instruction.Example;
            GetInstructionMeaning = instruction.Meaning;
            GetInstructionFormat = instruction.Format.ToText();
            GetInstructionOpCode = instruction.Opcode.ToString();
            GetInstructionFunction = instruction.Function.ToString();

            InstructionFormat = instruction.Format;
            RegisterDestination = instruction.RegDestination;
            AluSource = instruction.AluSource;
            AluControl = instruction.AluControl;
            RegisterWrite = instruction.RegWrite;
            MemoryWrite = instruction.MemWrite;
            MemoryRead = instruction.MemRead;
            MemorySignExtend = instruction.MemSignExtend;
            RegisterFileInput = instruction.RegFileInput;
            ProgramCounterSource = instruction.PcSource;
            DataMemoryWordSize = instruction.DataMemWordSize;
            Systemcall = instruction.Systemcall;
        }

        public override string ToString()
        {
            var rString = "";

            /* INSTRUCTION */
            rString += "Instruction:\t\t" + instruction.Name + "\n";
            rString += "Instruction Format:\t" + InstructionFormat.ToText() + "\n";

            if(instruction.Format == InstructionFormat.R)
                rString += "Function:\t\t" + instruction.Function + "\n";
            else
                rString += "OpCode:\t\t\t" + instruction.Opcode + "\n";
            rString += RepeatString("-", 80);
         
            /* REGISTER FILE */
            rString += "Register Destination:\t";
            if (RegisterWrite)
                rString += RegisterDestination.ToText() + "\n";
            else
                rString += "no register gets overwritten\n";

            rString += "Register File Input:\t" + RegisterFileInput.ToText() + "\n";
            rString += RepeatString("-", 80);

            /* ALU */
            rString += "ALU Source:\t\t" + AluSource.ToText() + "\n";
            rString += "ALU Control:\t\t" + AluControl.ToText() + "\n";
            rString += RepeatString("-", 80);

            /* DATA MEMORY */
            rString += "Data Memory Write:\t";
            if (MemoryWrite)
                rString += "yes \n";
            else
                rString += "no \n";

            rString += "Data Memory Read:\t";
            if (MemoryRead)
                rString += "yes \n";
            else
                rString += "no \n";
            rString += "Data Memory Size:\t" + DataMemoryWordSize.ToText() + "\n";
            
            rString += "Data Memory Sign Extend:\t";
            if (MemorySignExtend)
                rString += "yes \n";
            else
                rString += "no \n";

            rString += RepeatString("-", 80);
            /* PROGRAMCOUNTER */
            rString += "Program Counter Input:\t" + ProgramCounterSource.ToText() + "\n";

            return rString;
        }

        private string RepeatString(string s, int count)
        {
            var rString = "";
            for (var i = 0; i < count; i++)
                rString += s;
            return rString;
        }
    }
}
