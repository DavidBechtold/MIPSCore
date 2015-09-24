using System;
using System.Collections;
using System.Configuration;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Data_Memory;
using MIPSCore.Instruction_Memory;
using MIPSCore.Register_File;
using MIPSCore.Timing;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace MIPSCore
{
    public enum ExecutionMode { SingleStep, RunToCompletion };

    public class MipsCore : IMipsCore
    {
        public const ushort CCoreWordLength = CWord.wordLength;
        public const ushort RegisterCount = CMIPSRegister.numberOfRegisters;

        public IInstructionMemory InstructionMemory { get; private set; }
        public IControlUnit ControlUnit { get; private set; }
        public IRegisterFile RegisterFile { get; private set; }
        public IAlu Alu { get; private set; }
        public IDataMemory DataMemory { get; private set; }
        public ExecutionMode Mode { get; private set; }

        public event EventHandler Completed;
        public event EventHandler Clocked;
        public event EventHandler Exception;
        
        private const ulong StdCoreFrequencyHz = 100;
        private const MemSize StdInstructionMemorySizeKb = MemSize.Size1Kb;
        private const MemSize StdDataMemorySizeKb = MemSize.Size1Kb;
        private const bool StdInitStackPointer = true;

        private ulong coreFrequencyHz;
        private MemSize instructionMemorySizeKb;
        private MemSize dataMemorySizeKb;
        private bool initStackPointer;
        private bool programmCompleted;

        private readonly IClock clock;
        private readonly MipsProgrammer programmer;
        private string excetpionString;

        public MipsCore()
        {
            /* read config file */
            ReadConfigFile();

            /* bootstrapping */
            InstructionMemory = new InstructionMemory(instructionMemorySizeKb);
            RegisterFile = new RegisterFile();
            ControlUnit = new ControlUnit();
            Alu = new Alu();
            DataMemory = new DataMemory(dataMemorySizeKb);
            clock = new Clock(coreFrequencyHz, ClockTick);
            programmer = new MipsProgrammer(this);

            InstructionMemory.ControlUnit = ControlUnit;
            InstructionMemory.Alu = Alu;
            InstructionMemory.RegisterFile = RegisterFile;
            RegisterFile.InstructionMemory = InstructionMemory;
            RegisterFile.Alu = Alu;
            RegisterFile.DataMemory = DataMemory;
            RegisterFile.ControlUnit = ControlUnit;
            ControlUnit.InstructionMemory = InstructionMemory;
            Alu.InstructionMemory = InstructionMemory;
            Alu.ControlUnit = ControlUnit;
            Alu.RegisterFile = RegisterFile;
            DataMemory.ControlUnit = ControlUnit;
            DataMemory.Alu = Alu;
            DataMemory.RegisterFile = RegisterFile;

            programmCompleted = false;
            excetpionString = "";
            SetMode(ExecutionMode.SingleStep);
        }

        private void InitCore()
        {
            programmCompleted = false;
            excetpionString = "";
            RegisterFile.Flush();
            InstructionMemory.Flush();
            DataMemory.Flush();

            if (initStackPointer)
                RegisterFile.InitStackPointer();
        }

        public void StartCore()
        {
            clock.Start();
        }

        public void StopCore()
        {
            clock.Stop();
        }

        public void ProgramObjdump(string path)
        {
            InitCore();

            try
            {
                programmer.ProgramObjdump(path);
            }
            catch (Exception exeption)
            {
                excetpionString = exeption.ToString();
                if (Exception != null) Exception(this, new EventArgs());
            }
        }

        public void SetMode(ExecutionMode executionMode)
        {
            Mode = executionMode;
            switch (Mode)
            {
                case ExecutionMode.SingleStep: clock.SingleStep = true;  break;
                case ExecutionMode.RunToCompletion: clock.SingleStep = false; break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void SingleClock()
        {  
            clock.Step();
        }

        private void ClockTick(object sender, EventArgs e)
        {
            //for debugging and to avoid race conditions at the beginning stop the clock,
            //if we don't stop the clock here, it can happen that the clock interrupt interrupts the next instructions (specially at debugging)
            if (programmCompleted)
                return;

            clock.Stop();
            try
            {
                InstructionMemory.Clock();
                ControlUnit.Clock();
                Alu.Clock();
                DataMemory.Clock();
                RegisterFile.Clock();
                 
                //call event clocked
                if (Clocked != null) Clocked(this, new EventArgs());

                if (ControlUnit.Systemcall)
                {
                    //Systemcall occur => stop the clock
                    // TODO check if the exit systemcall has occured
                    programmCompleted = true;
                    if (Completed != null) Completed(this, new EventArgs());
                }
                else
                    clock.Start();               
            }
            catch (Exception exeption)
            {
                excetpionString = exeption.ToString();
                if (Exception != null) Exception(this, new EventArgs());
            }
        }
        
        public void ReadConfigFile()
        {
            /* CoreFrequency */
            try { coreFrequencyHz = Convert.ToUInt64(ConfigurationManager.AppSettings["CoreFrequency_Hz"]); }
            catch (FormatException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["CoreFrequency_Hz"] + " can not be converted to a CoreFrequency. Standard value of " + StdCoreFrequencyHz + " Hz is used.");
                coreFrequencyHz = StdCoreFrequencyHz; 
            }
    
            if (coreFrequencyHz == 0)
            {
                Console.WriteLine("Configuration key for the core frequency not found. Standard value of " + StdCoreFrequencyHz + " Hz is used.");
                coreFrequencyHz = StdCoreFrequencyHz; 
            }

            /* InstructionMemorySize */
            try { instructionMemorySizeKb = (MemSize)Enum.Parse(typeof(MemSize), ConfigurationManager.AppSettings["InstructionMemorySize_kB"]); }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Configuration key for the instruction memory size not found. Standard value of " + StdInstructionMemorySizeKb + " kB is used.");
                instructionMemorySizeKb = StdInstructionMemorySizeKb;
            }
            catch (ArgumentException)
            { 
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["InstructionMemorySize_kB"] + " can not be converted to an instruction memory size. Standard value of " + StdInstructionMemorySizeKb + " kB is used.");
                instructionMemorySizeKb = StdInstructionMemorySizeKb; 
            }

            /* DataMemorySize */
            try { dataMemorySizeKb = (MemSize)Enum.Parse(typeof(MemSize), ConfigurationManager.AppSettings["DataMemorySize_kB"]); }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Configuration key for the data memory size not found. Standard value of " + StdDataMemorySizeKb + " kB is used.");
                dataMemorySizeKb = StdDataMemorySizeKb;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["DataMemorySize_kB"] + " can not be converted to an data memory size. Standard value of " + StdDataMemorySizeKb + " kB is used.");
                dataMemorySizeKb = StdDataMemorySizeKb;
            }
            
            /* InitialiseStackPointer */
            try { initStackPointer = Convert.ToBoolean(ConfigurationManager.AppSettings["InitialiseStackPointer"]); }
            catch (FormatException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["InitialiseStackPointer"] + " can not be converted to an boolean value for InitialiseStackPointer. Standard value " + StdInitStackPointer + " is used.");
                initStackPointer = StdInitStackPointer;
            }
        }

        public uint DataMemorySizeBytes()
        {
            return DataMemory.SizeBytes;
        }

        public int ReadWordDataMemory(uint byteAddr)
        {
            return DataMemory.ReadWord(byteAddr).getSignedDecimal;
        }

        public uint ReadWordDataMemoryUnsigned(uint byteAddr)
        {
            return DataMemory.ReadWord(byteAddr).getUnsignedDecimal;
        }

        public int[] ReadAllRegisters()
        {
            return RegisterFile.ReadAllRegister();
        }

        public string ToStringAllRegisters()
        {
            return RegisterFile.ToString();
        }

        public int ReadRegister(ushort number)
        {
            return RegisterFile.ReadRegister(number);
        }

        public string ToStringRegister(ushort number)
        {
            return RegisterFile.ToStringRegister(number);
        }

        public uint ReadRegisterUnsigned(ushort number)
        {
            return RegisterFile.ReadRegisterUnsigned(number);
        }

        public string ToStringRegisterUnsigned(ushort number)
        {
            return RegisterFile.ToStringRegisterUnsigned(number);
        }

        public string ToStringRegisterHex(ushort number)
        {
            return RegisterFile.ToStringRegisterHex(number);
        }

        public string ActualInstruction()
        {
            return InstructionMemory.GetActualInstruction.getHexadecimal;
        }

        public string ProgramCounter()
        {
            return InstructionMemory.GetProgramCounter.getHexadecimal + "";
        }

        public string ReadControlUnitSignals()
        {
            return ControlUnit.ToString();
        }

        public string GetExceptionString()
        {
            return excetpionString;
        }

        public ArrayList Code
        {
            get { return programmer.Code; }
        }
    }
}
