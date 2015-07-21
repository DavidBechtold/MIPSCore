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

using System.Configuration;

namespace MIPSCore
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
        private CMIPSProgrammer programmer;

        public event EventHandler completed;
        public event EventHandler clocked;
        public event EventHandler exception;
        private string excetpionString;

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
            programmer = new CMIPSProgrammer(this);
            programmCompleted = false;
            excetpionString = "";
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

        public void programObjdump(string path)
        {
            initCore();

            try
            {
                programmer.programObjdump(path);
            }
            catch (Exception exeption)
            {
                excetpionString = exeption.ToString();
                exception(this, new EventArgs());
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

        public void singleClock()
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
                excetpionString = exeption.ToString();
                exception(this, new EventArgs());
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

        public UInt32 dataMemorySizeBytes()
        {
            return dataMemory.sizeBytes;
        }

        public Int32 readWordDataMemory(UInt32 byteAddr)
        {
            return dataMemory.readWord(byteAddr).getSignedDecimal;
        }

        public UInt32 readWordDataMemoryUnsigned(UInt32 byteAddr)
        {
            return dataMemory.readWord(byteAddr).getUnsignedDecimal;
        }

        public Int32[] readAllRegisters()
        {
            return registerFile.readAllRegister();
        }

        public string toStringAllRegisters()
        {
            return registerFile.ToString();
        }

        public Int32 readRegister(UInt16 number)
        {
            return registerFile.readRegister(number);
        }

        public string toStringRegister(UInt16 number)
        {
            return registerFile.toStringRegister(number);
        }

        public UInt32 readRegisterUnsigned(UInt16 number)
        {
            return registerFile.readRegisterUnsigned(number);
        }

        public string toStringRegisterUnsigned(UInt16 number)
        {
            return registerFile.toStringRegisterUnsigned(number);
        }

        public string toStringRegisterHex(UInt16 number)
        {
            return registerFile.toStringRegisterHex(number);
        }

        public string actualInstruction()
        {
            return instructionMemory.getActualInstruction.getHexadecimal;
        }

        public string programCounter()
        {
            return instructionMemory.getProgramCounter.getHexadecimal + "";
        }

        public string readControlUnitSignals()
        {
            return controlUnit.ToString();
        }

        public string getExceptionString()
        {
            return excetpionString;
        }
    }
}
