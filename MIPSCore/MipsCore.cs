using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public const ushort CCoreWordLength = Word.WordLength;
        public const ushort RegisterCount = MipsRegister.NumberOfRegisters;

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
        private const MemorySize StdInstructionMemorySizeKb = MemorySize.Size1Kb;
        private const MemorySize StdDataMemorySizeKb = MemorySize.Size1Kb;
        private const bool StdInitStackPointer = true;

        private ulong coreFrequencyHz;
        private MemorySize instructionMemorySizeKb;
        private MemorySize dataMemorySizeKb;
        private bool initStackPointer;
        private bool programmCompleted;

        private readonly IClock clock;
        private readonly MipsProgrammer programmer;
        private string excetpionString;

        private readonly List<uint> breakpoints;

        private string programmedFilePath;

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
            breakpoints = new List<uint>();

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

            programmedFilePath = "";
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

        public void ResetCore()
        {
            if (programmedFilePath.Length == 0)
                return;

            ProgramObjdump(programmedFilePath);
        }

        public void ProgramObjdump(string path)
        {
            InitCore();

            try
            {
                programmer.ProgramObjdump(path);
                programmedFilePath = path;
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

        public void SetBranchDelaySlot(bool branchDelay)
        {
            InstructionMemory.BranchDelaySlot(branchDelay);
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
           
                //check breakpoints
                if (Mode != ExecutionMode.RunToCompletion) return;
                foreach (var breakpoint in breakpoints.Where(breakpoint => InstructionMemory.GetProgramCounter.UnsignedDecimal == breakpoint))
                {
                    clock.Stop();
                    SetMode(ExecutionMode.SingleStep);
                }
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
            try { instructionMemorySizeKb = (MemorySize)Enum.Parse(typeof(MemorySize), ConfigurationManager.AppSettings["InstructionMemorySize_kB"]); }
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
            try { dataMemorySizeKb = (MemorySize)Enum.Parse(typeof(MemorySize), ConfigurationManager.AppSettings["DataMemorySize_kB"]); }
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
            return DataMemory.ReadWord(byteAddr).SignedDecimal;
        }

        public uint ReadWordDataMemoryUnsigned(uint byteAddr)
        {
            return DataMemory.ReadWord(byteAddr).UnsignedDecimal;
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
            return InstructionMemory.GetActualInstruction.Hexadecimal;
        }

        public string ProgramCounter()
        {
            return InstructionMemory.GetProgramCounter.Hexadecimal + "";
        }

        public string ReadControlUnitSignals()
        {
            return ControlUnit.ToString();
        }

        public string GetExceptionString()
        {
            return excetpionString;
        }

        public Dictionary<uint, string> Code
        {
            get { return programmer.Code; }
        }

        public ulong FrequencyHz
        {
            get { return clock.FrequencyHz; }
            set { clock.FrequencyHz = value; }
        }

        public void DataMemorySize(MemorySize size)
        {
            DataMemory.SetSize(size);
        }

        public void TextMemorySize(MemorySize size)
        {
            InstructionMemory.SetSize(size);
            Code.Clear();
        }

        public void AddBreakpoint(uint address)
        {
            breakpoints.Add(address);
        }

        public void RemoveBreakpoint(uint address)
        {
            if (breakpoints.Contains(address))
                breakpoints.Remove(address);
        }

        public void RemoveAllBreakpoints()
        {
            breakpoints.Clear();
        }

        public bool GetBranchDelaySlot()
        {
            return InstructionMemory.GetBranchDelaySlot();
        }
    }
}
