using System;
using System.Xml.Serialization;
using MIPSCore.Util;

namespace MIPSCore.Instruction_Set
{
    [XmlRoot("CInstructionSet")]
    public class CInstructionSet
    {
        [XmlElement("CInstruction")]
        public CInstruction[] Instructions;
        public int Length { get { return Instructions.Length; } }

        private bool CheckFunction(int i, Word function)
        {
            return Instructions[i].Function == function.UnsignedDecimal;
        }

        private bool CheckOpCode(int i, Word opcode)
        {
            return Instructions[i].Opcode == opcode.UnsignedDecimal;
        }

        public CInstruction GetInstruction(Word opcode, Word function)
        {
            if (opcode.UnsignedDecimal == 0)
            {
                /* R Format */
                for (var i = 0; i < Instructions.Length; i++)
                    if (CheckFunction(i, function))
                        return Instructions[i];
            }
            else
            {
                /* I / J Format */
                for (var i = 0; i < Instructions.Length; i++)
                 if(CheckOpCode(i, opcode))
                     return Instructions[i];
            }

            throw new ArgumentOutOfRangeException();
        }
    }

    public class CInstruction
    {
        [XmlElement("name")]
        public string Name;

        [XmlElement("assembler")]
        public string Assembler;

        [XmlElement("example")]
        public string Example;

        [XmlElement("meaning")]
        public string Meaning;

        [XmlElement("format")]
        public InstructionFormat Format;

        [XmlElement("opcode")]
        public UInt16 Opcode;

        [XmlElement("function")]
        public UInt16 Function;

        [XmlElement("regWrite")]
        public bool RegWrite;

        [XmlElement("regDestination")]
        public RegisterDestination RegDestination;

        [XmlElement("regFileInput")]
        public RegisterFileInput RegFileInput;

        [XmlElement("aluSource")]
        public AluSource AluSource;

        [XmlElement("aluControl")]
        public AluControl AluControl;

        [XmlElement("memWrite")]
        public bool MemWrite;

        [XmlElement("memRead")]
        public bool MemRead;

        [XmlElement("memWordSize")]
        public DataMemoryWordSize DataMemWordSize;

        [XmlElement("memSignExtend")]
        public bool MemSignExtend;
  
        [XmlElement("pcSource")]
        public ProgramCounterSource PcSource;

        [XmlElement("systemcall")]
        public bool Systemcall;

        public CInstruction()
        {
            Name = "";
            Format = InstructionFormat.R;
            Opcode = 0;
            Function = 0;
            RegWrite = false;
            RegDestination = RegisterDestination.Rd;
            RegFileInput = RegisterFileInput.AluLo;
            AluSource = AluSource.RegFile;
            AluControl = AluControl.Add;
            MemWrite = false;
            MemRead = false;
            DataMemWordSize = DataMemoryWordSize.Word;
            MemSignExtend = false;
            PcSource = ProgramCounterSource.ProgramCounter;
            Systemcall = false;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
