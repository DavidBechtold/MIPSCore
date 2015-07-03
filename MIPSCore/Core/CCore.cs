using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore.Clock;
using MIPSCore.Util;
using MIPSCore.ALU;
using MIPSCore.InstructionMemory;
using MIPSCore.ControlUnit;
using MIPSCore.RegisterFile;
using MIPSCore.DataMemory;

namespace MIPSCore.Core
{
    public class CCore
    {
        public const UInt16 CCoreWordLength = CWord.wordLength;

        private CClock clock;
        private CInstructionMemory instructionMemory;
        private CRegisterFile registerFile;
        private CALU alu;
        private CControlUnit controlUnit;
        private CDataMemory dataMemory;

        public CCore()
        {
            instructionMemory = new CInstructionMemory(this, MemSize.Size_1kB);
            registerFile = new CRegisterFile(this);
            alu = new CALU(this);
            controlUnit = new CControlUnit(this);
            dataMemory = new CDataMemory(this, MemSize.Size_1kB);
            clock = new CClock(1, clockTick);
        }

        public void startCore()
        {
            clock.start();
        }

        public void programCore(string path)
        {
            string[] code = System.IO.File.ReadAllLines(path);

            if (code.Length >= instructionMemory.getSize)
                throw new IndexOutOfRangeException("Codelength is greater than " + instructionMemory.getSize + ".");

            for (UInt32 i = 0; i < code.Length; i++)
                instructionMemory.programWord(new CWord(Convert.ToUInt32(code[i], 16)), i * 4);
        }

        public void programObjdump(string path)
        {
            string[] code = System.IO.File.ReadAllLines(path);
            UInt32 codeCounter = 0;

            for (UInt32 i = 0; i < code.Length; i++)
            {
                if (code[i].Contains(":\t"))
                {
                    if (code.Length >= instructionMemory.getSize)
                        throw new IndexOutOfRangeException("Codelength is greater than " + instructionMemory.getSize + ".");
                    string substring = code[i].Substring(code[i].IndexOf(':') + 2, 8);

                    try
                    {
                        instructionMemory.programWord(new CWord(Convert.ToUInt32(substring, 16)), codeCounter++ * 4);
                    }
                    catch
                    {
                        throw new ArithmeticException("Bei der Zeile: " + code[i] + " : konnte der Hexwert nicht gelesen werden");
                    }
                }

            }

        }

        private void clockTick(object sender, EventArgs e)
        {
            //for debugging and to avoid race conditions at the beginning stop the clock,
            //if we don't stop the clock here, it can happen that the clock interrupt interrupts the next instructions (specially at debugging)

            //for debugging clock all components in one clock
            clock.stop();
            try
            {
                instructionMemory.clock();
                controlUnit.clock();
                alu.clock();
                dataMemory.clock();
                registerFile.clock();
                clock.start();
            }
            catch (Exception exeption)
            {
                Console.WriteLine(exeption.ToString());
            }
        }

        public CInstructionMemory getInstructionMemory
        {
            get
            {
                return instructionMemory;
            }
        }

        public CControlUnit getControlUnit
        {
            get
            {
                return controlUnit;
            }
        }

        public CRegisterFile getRegisterFile
        {
            get
            {
                return registerFile;
            }
        }

        public CALU getAlu
        {
            get
            {
                return alu;
            }
        }

        public CDataMemory getDataMemory
        {
            get
            {
                return dataMemory;
            }
        }
    }
}
