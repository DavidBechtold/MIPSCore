using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        public event EventHandler Notification;

        private readonly bool initStackAndGlobalPointer;
        private bool programmCompleted;

        private readonly IClock clock;
        private readonly MipsProgrammer programmer;
        private string excetpionString;
        private string notificationMessage;

        private readonly List<uint> breakpoints;

        public string programmedFile { get; private set; }

        public MipsCore()
        {
            /* read config file */
            //ReadConfigFile();

            ulong coreFrequencyHz = 100;
            var instructionMemorySizeKb = MemorySize.Size4Kb;
            var dataMemorySizeKb = MemorySize.Size4Kb;
            initStackAndGlobalPointer = true;

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

            programmedFile = "";
        }

        private void InitCore()
        {
            programmCompleted = false;
            excetpionString = "";
            RegisterFile.Flush();
            InstructionMemory.Flush();
            DataMemory.Flush();

            if (initStackAndGlobalPointer)
                RegisterFile.InitStackAndGlobalPointer();

            /*var compiler = new Compiler();
            string[] files = new string[1];
            files[0] = @"C:\test.c";
            programmer.ProgramObjdump(compiler.Compile(files));*/
        }

        public void StartCore()
        {
            clock.Start();
        }

        public void StopCore()
        {
            clock.Stop();
            SetMode(ExecutionMode.SingleStep);
        }

        public void ResetCore()
        {
            StopCore();
            if (programmedFile.Length == 0)
                return;

            try
            {
                InitCore();
                programmer.Program(programmedFile);
            }
            catch (Exception exeption)
            {
                excetpionString = exeption.ToString();
                if (Exception != null) Exception(this, new EventArgs());
            }
        }

        public void ProgramC(string file)
        {
            InitCore();

            try
            {
                var compiler = new Compiler();
                var files = new string[1];
                files[0] = file;
                programmedFile = compiler.Compile(files);
                programmer.Program(programmedFile);
            }
            catch (Exception exeption)
            {
                excetpionString = exeption.Message;
                if (Exception != null) Exception(this, new EventArgs());
            }
        }

        public void ProgramAssembler(string file)
        {
            InitCore();

            try
            {
                var compiler = new Compiler();
                var files = new string[1];
                files[0] = file;
                programmedFile = compiler.CompileAssembler(files);
                programmer.Program(programmedFile);
            }
            catch (Exception exeption)
            {
                excetpionString = exeption.Message;
                if (Exception != null) Exception(this, new EventArgs());
            }
        }

        public void ProgramObjdump(string path)
        {
            InitCore();

            try
            {
                programmedFile = System.IO.File.ReadAllText(path);
                programmer.Program(programmedFile);
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
            if (programmCompleted)
                return;

            //for debugging and to avoid race conditions at the beginning stop the clock,
            //if we don't stop the clock here, it can happen that the clock interrupt interrupts the next instructions (specially at debugging)
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
                    // read the $v0 register to get the syscall
                    int v0 = RegisterFile.ReadRegister(2);
                    switch (v0)
                    {
                        case 1: // Print integer
                            int value = RegisterFile.ReadRegister(4); // $a0
                            notificationMessage = $"{value}\n";
                            Notification(this, new EventArgs());
                            break;

                        case 4: // Print string
                            uint address = RegisterFile.ReadRegisterUnsigned(4); // $a0
                            notificationMessage = DataMemory.ReadNullTerminatedString(address) + "\n";
                            Notification(this, new EventArgs());
                            break;

                        case 10: // Exit
                            programmCompleted = true;
                            if (Completed != null) Completed(this, new EventArgs());
                            break;
                        default:
                            throw new ArgumentException($"Unbekannter Syscall Code ($v0): {v0}");
                    }
                    //Systemcall occur => stop the clock
                    // TODO check if the exit systemcall has occured
                    
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

        public string GetNotificationMessage()
        {
            return notificationMessage;
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

        public uint GetTextSegmentEndAddress()
        {
            return programmer.TextSegmentEndAddress;
        }

        /*public void ReadConfigFile()
        {
            // CoreFrequency
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

            // InstructionMemorySize 
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

            // DataMemorySize 
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
            
            // InitialiseStackPointer 
            try { initStackPointer = Convert.ToBoolean(ConfigurationManager.AppSettings["InitialiseStackPointer"]); }
            catch (FormatException)
            {
                Console.WriteLine("Configuration Value " + ConfigurationManager.AppSettings["InitialiseStackPointer"] + " can not be converted to an boolean value for InitialiseStackPointer. Standard value " + StdInitStackPointer + " is used.");
                initStackPointer = StdInitStackPointer;
            }
        }*/
    }
}
