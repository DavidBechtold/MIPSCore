using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using MIPSCore.Util;

namespace MIPSCore.InstructionSet
{
    [XmlRoot("CInstructionSet")]
    public class CInstructionSet
    {
        [XmlElement("CInstruction")]
        public CInstruction[] instructions;

        public int Length
        {
            get
            {
                return instructions.Length;
            }
        }

        private bool checkFunction(int i, CWord function)
        {
            return instructions[i].function == function.getUnsignedDecimal;
        }

        private bool checkOpCode(int i, CWord opcode)
        {
            return instructions[i].opcode == opcode.getUnsignedDecimal;
        }

        public CInstruction getInstruction(CWord opcode, CWord function)
        {
            if (opcode.getUnsignedDecimal == 0)
            {
                /* R Format */
                for (int i = 0; i < instructions.Length; i++)
                    if (checkFunction(i, function))
                        return instructions[i];
            }
            else
            {
                /* I / J Format */
                for (int i = 0; i < instructions.Length; i++)
                 if(checkOpCode(i, opcode))
                     return instructions[i];
            }

            throw new ArgumentOutOfRangeException(this.GetType().Name + ": no instruction for opcode " + opcode.getUnsignedDecimal + " and function " + function.getUnsignedDecimal + " found");
        }
    }

    public class CInstruction
    {
        [XmlElement("name")]
        public string name;

        [XmlElement("assembler")]
        public string assembler;

        [XmlElement("meaning")]
        public string meaning;

        [XmlElement("format")]
        public InstructionFormat format;

        [XmlElement("opcode")]
        public UInt16 opcode;

        [XmlElement("function")]
        public UInt16 function;

        [XmlElement("regWrite")]
        public bool regWrite;

        [XmlElement("regDestination")]
        public RegisterDestination regDestination;

        [XmlElement("regFileInput")]
        public RegisterFileInput regFileInput;

        [XmlElement("aluSource")]
        public ALUSource aluSource;

        [XmlElement("aluControl")]
        public ALUControl aluControl;

        [XmlElement("memWrite")]
        public bool memWrite;

        [XmlElement("memRead")]
        public bool memRead;

        [XmlElement("memWordSize")]
        public DataMemoryWordSize dataMemWordSize;
  
        [XmlElement("pcSource")]
        public ProgramCounterSource pcSource;

        [XmlElement("systemcall")]
        public bool systemcall;

        public CInstruction()
        {
            name = "";
            format = InstructionFormat.R;
            opcode = 0;
            function = 0;
            regWrite = false;
            regDestination = RegisterDestination.rd;
            regFileInput = RegisterFileInput.aluLO;
            aluSource = ALUSource.regFile;
            aluControl = ALUControl.add;
            memWrite = false;
            memRead = false;
            dataMemWordSize = DataMemoryWordSize.word;
            pcSource = ProgramCounterSource.programCounter;
            systemcall = false;
        }

        public override string ToString()
        {
            return "";
        }

        
    }
    
}
