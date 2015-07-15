﻿using System;
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
using System.Text.RegularExpressions;
using System.Configuration;

namespace MIPSCore.Core
{
    public enum ExecutionMode { singleStep, runToCompletion };

    public class CCore : ICore
    {
        public const UInt16 CCoreWordLength = CWord.wordLength;
        public const UInt16 RegisterCount = CMIPSRegister.numberOfRegisters;
        
        private const UInt64 stdCoreFrequency_Hz = 100;
        private const MemSize stdInstructionMemorySize_kB = MemSize.Size_1kB;
        private const MemSize stdDataMemorySize_kB = MemSize.Size_1kB;
        private const bool stdInitStackPointer = true;

        private UInt64 coreFrequency_Hz;
        private MemSize instructionMemorySize_kB;
        private MemSize dataMemorySize_kB;
        private bool initStackPointer;
        private bool programmCompleted;

        private CClock clock;
        private CInstructionMemory instructionMemory;
        private CRegisterFile registerFile;
        private CALU alu;
        private CControlUnit controlUnit;
        private CDataMemory dataMemory;

        public event EventHandler completed;
        public event EventHandler clocked;

        public CCore()
        {
            /* read config file */
            readConfigFile();

            instructionMemory = new CInstructionMemory(this, instructionMemorySize_kB);
            registerFile = new CRegisterFile(this);
            alu = new CALU(this);
            controlUnit = new CControlUnit(this);
            dataMemory = new CDataMemory(this, dataMemorySize_kB);
            clock = new CClock(coreFrequency_Hz, clockTick);
            programmCompleted = false;
        }

        private void initCore()
        {
            programmCompleted = false;
            registerFile.flush();

            if (initStackPointer)
                registerFile.initStackPointer();
        }

        public void startCore()
        {
            clock.start();
        }

        public void stopCore()
        {
            clock.stop();
        }

        public void programCore(string path)
        {
            initCore();

            string[] code = System.IO.File.ReadAllLines(path);

            if (code.Length >= instructionMemory.getSize)
                throw new IndexOutOfRangeException("Codelength is greater than " + instructionMemory.getSize + ".");

            for (UInt32 i = 0; i < code.Length; i++)
                instructionMemory.programWord(new CWord(Convert.ToUInt32(code[i], 16)), i * 4);
        }

        public void programObjdump(string path)
        {
            initCore();

            string strCode = System.IO.File.ReadAllText(path);
            UInt32 codeCounter = 0;

            Regex rgx = new Regex("([0-9]|[a-f])+(?=\\:)|(?!\t{1})([0-9]|[a-f]){8}(?= \t)", RegexOptions.IgnoreCase);
            MatchCollection match = rgx.Matches(strCode);
            rgx = null;
            strCode = null;

            if (match.Count / 2 * 4 >= instructionMemory.getSize)
                throw new IndexOutOfRangeException("Codelength is greater than " + instructionMemory.getSize + ".");

            UInt32 address = 0;
            UInt32 instruction = 0;
            foreach (Match codeMatch in match)
            {
                string stringMatch = codeMatch.Value;
                
                if (codeCounter % 2 == 0)
                    address = Convert.ToUInt32(stringMatch, 16);
                else
                {
                    instruction = Convert.ToUInt32(stringMatch, 16);
                    instructionMemory.programWord(new CWord((UInt32)instruction), address);
                }
                codeCounter++;
            }
        }

        public void setMode(ExecutionMode mode)
        {
            switch (mode)
            {
                case ExecutionMode.singleStep: clock.SingleStep = true;  break;
                case ExecutionMode.runToCompletion: clock.SingleStep = false; break;
                default: throw new ArgumentOutOfRangeException(this.GetType().Name + ": Executionmode out of range");
            }
        }

        public void singleStep()
        {  
            clock.step();
        }

        private void clockTick(object sender, EventArgs e)
        {
            //for debugging and to avoid race conditions at the beginning stop the clock,
            //if we don't stop the clock here, it can happen that the clock interrupt interrupts the next instructions (specially at debugging)

            if (programmCompleted)
                return;

            clock.stop();
            try
            {
                instructionMemory.clock();
                controlUnit.clock();
                alu.clock();
                dataMemory.clock();
                registerFile.clock();

                //call event clocked
                clocked(this, new EventArgs());

                if (controlUnit.getSystemcall)
                {
                    //Systemcall occur => stop the clock
                    // TODO check if the exit systemcall has occured
                    programmCompleted = true;
                    completed(this, new EventArgs());
                }
                else
                    clock.start();               
            }
            catch (Exception exeption)
            {
                //Console.WriteLine(exeption.ToString());
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

        public void readConfigFile()
        {
            /* CoreFrequency */
            try { coreFrequency_Hz = Convert.ToUInt64(ConfigurationManager.AppSettings["CoreFrequency_Hz"]); }
            catch (FormatException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["CoreFrequency_Hz"] + " can not be converted to a CoreFrequency. Standard value of " + stdCoreFrequency_Hz + " Hz is used.");
                coreFrequency_Hz = stdCoreFrequency_Hz; 
            }
    
            if (coreFrequency_Hz == 0)
            {
                Console.WriteLine("Configuration key for the core frequency not found. Standard value of " + stdCoreFrequency_Hz + " Hz is used.");
                coreFrequency_Hz = stdCoreFrequency_Hz; 
            }

            /* InstructionMemorySize */
            try { instructionMemorySize_kB = (MemSize)Enum.Parse(typeof(MemSize), ConfigurationManager.AppSettings["InstructionMemorySize_kB"]); }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Configuration key for the instruction memory size not found. Standard value of " + stdInstructionMemorySize_kB + " kB is used.");
                instructionMemorySize_kB = stdInstructionMemorySize_kB;
            }
            catch (ArgumentException)
            { 
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["InstructionMemorySize_kB"] + " can not be converted to an instruction memory size. Standard value of " + stdInstructionMemorySize_kB + " kB is used.");
                instructionMemorySize_kB = stdInstructionMemorySize_kB; 
            }

            /* DataMemorySize */
            try { dataMemorySize_kB = (MemSize)Enum.Parse(typeof(MemSize), ConfigurationManager.AppSettings["DataMemorySize_kB"]); }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Configuration key for the data memory size not found. Standard value of " + stdDataMemorySize_kB + " kB is used.");
                dataMemorySize_kB = stdDataMemorySize_kB;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["DataMemorySize_kB"] + " can not be converted to an data memory size. Standard value of " + stdDataMemorySize_kB + " kB is used.");
                dataMemorySize_kB = stdDataMemorySize_kB;
            }
            
            /* InitialiseStackPointer */
            try { initStackPointer = Convert.ToBoolean(ConfigurationManager.AppSettings["InitialiseStackPointer"]); }
            catch (FormatException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["InitialiseStackPointer"] + " can not be converted to an boolean value for InitialiseStackPointer. Standard value " + stdInitStackPointer + " is used.");
                initStackPointer = stdInitStackPointer;
            }
        }

        public string readRegister(UInt16 number)
        {
            return registerFile.registerToString(number);
        }

        public string readRegisterUnsigned(UInt16 number)
        {
            return registerFile.registerToStringUnsigned(number);
        }

        public string readRegisterHex(UInt16 number)
        {
            return registerFile.registerToStringHex(number);
        }

        public string actualInstruction()
        {
            return instructionMemory.getActualInstruction.getHexadecimal;
        }

        public string programCounter()
        {
            return instructionMemory.getProgramCounter.getUnsignedDecimal + "";
        }

        public string readControlUnitSignals()
        {
            return controlUnit.ToString();
        }
    }
}
