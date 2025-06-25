using System;
using System.Xml.Serialization;
using MIPSCore.Util;

namespace MIPSCore.Instruction_Set
{
    [XmlRoot("CInstructionSet")]
    public class InstructionSet
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

        private bool CheckOpCodeAndRd(int i, Word opcode, Word rd)
        {
            return (Instructions[i].Opcode == opcode.UnsignedDecimal) && (Instructions[i].Rd == rd.UnsignedDecimal);
        }

        public CInstruction GetInstruction(Word opcode, Word function, Word rd)
        {   
            
            switch (opcode.UnsignedDecimal)
            {
                case 0:
                    /* R Format */
                    for (var i = 0; i < Instructions.Length; i++)
                        if (CheckFunction(i, function))
                            return Instructions[i];
                    break;
                case 1:
                    /* I Format special branch */
                    for (var i = 0; i < Instructions.Length; i++)
                        if (CheckOpCodeAndRd(i, opcode, rd))
                            return Instructions[i];
                    break;
                default:
                    /* I / J Format */ 
                    for (var i = 0; i < Instructions.Length; i++)
                        if (CheckOpCode(i, opcode))
                            return Instructions[i];
                    break;
            }
            throw new ArgumentOutOfRangeException("Unbekannter Befehl");
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

        [XmlElement("rd")] 
        public UInt16 Rd;

        [XmlElement("regWrite")]
        public bool RegWrite;

        [XmlElement("regDestination")]
        public RegisterDestination RegDestination;

        [XmlElement("regFileInput")]
        public RegisterFileInput RegFileInput;

        [XmlElement("aluSource1")]
        public AluSource1 AluSource1;

        [XmlElement("aluSource2")]
        public AluSource2 AluSource2;

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
            Rd = 0;
            RegWrite = false;
            RegDestination = RegisterDestination.Rd;
            RegFileInput = RegisterFileInput.AluLo;
            AluSource1 = AluSource1.Rs;
            AluSource2 = AluSource2.Rt;
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
