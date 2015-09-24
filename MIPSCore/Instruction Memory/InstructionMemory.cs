using System;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.InstructionSet;
using MIPSCore.Register_File;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace MIPSCore.Instruction_Memory
{
    public class InstructionMemory : Memory, IInstructionMemory
    {
        private bool firstCommand;

        public IAlu Alu { get; set; }
        public IControlUnit ControlUnit { get; set; }
        public IRegisterFile RegisterFile { get; set; }
        
        public CWord GetOpCode { get; private set; }
        public CWord GetFunction { get; private set; }
        public CWord GetShiftAmount { get; private set; }
        public CWord GetImmediate { get; private set; }
        public CWord GetRd { get; private set; }
        public CWord GetRs { get; private set; }
        public CWord GetRt { get; private set; }
        public CWord GetJumpTarget { get; private set; }
        public CWord GetProgramCounter { get; private set; }
        public CWord GetActualInstruction { get; private set; }
        public CWord SetAddressOffset { set { Offset = value; } }

        public InstructionMemory(MemSize size)
            : base(size)
        {
            Flush();
        }

        public void Clock()
        {
            // 1.) calculate new programcounter
            CalcProgramCounter();

            // 2.) get instruction from memory to execute
            GetActualInstruction = ReadNextInstuction();

            // TODO make converter class
            // 3.) divide instruction for the register file and the control unit
            GetOpCode = GetActualInstruction.getSubword(0, 6);
            GetRs = GetActualInstruction.getSubword(6, 5);
            GetRt = GetActualInstruction.getSubword(6 + 5, 5);

            /* R Format */
            GetFunction = GetActualInstruction.getSubword(32 - 6, 6);
            GetShiftAmount = GetActualInstruction.getSubword(6 + 5 + 5 + 5, 5);
            GetRd = GetActualInstruction.getSubword(6 + 5 + 5, 5);

            /* I Format */
            GetImmediate = GetActualInstruction.getSubword(16, 16);

            /* J Format */
            GetJumpTarget = GetActualInstruction.getSubword(6, 26);
        }

        private void CalcProgramCounter()
        {
            switch (ControlUnit.ProgramCounterSource)
            {
                case ProgramCounterSource.ProgramCounter:
                    if (firstCommand)
                        firstCommand = false;
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendEqual:
                    if (Alu.ZeroFlag)
                        GetProgramCounter += GetImmediate * 4 + 4;
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendUnequal:
                    if (!Alu.ZeroFlag)
                        GetProgramCounter += GetImmediate * 4 + 4;
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendLessThanZero:
                    if (Alu.GetResultLo.getUnsignedDecimal == 1)
                        GetProgramCounter += GetImmediate * 4 + 4;
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendLessOrEqualZero:
                    if (!Alu.ZeroFlag || Alu.GetResultLo.getUnsignedDecimal == 1)
                        GetProgramCounter += GetImmediate * 4 + 4;
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.Jump:
                    GetProgramCounter = GetJumpTarget * 4;
                    break;
                case ProgramCounterSource.Register:
                    GetProgramCounter = RegisterFile.ReadRs();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override sealed void Flush()
        {
            GetOpCode = new CWord((uint)0);
            GetFunction = new CWord((uint)0);
            GetShiftAmount = new CWord((uint)0);
            GetImmediate = new CWord((uint)0);
            GetRd = new CWord((uint)0);
            GetRs = new CWord((uint)0);
            GetRt = new CWord((uint)0);
            GetJumpTarget = new CWord((uint)0);

            firstCommand = true;
            GetProgramCounter = new CWord((uint)0);
            GetActualInstruction = new CWord(0);

            base.Flush();
        }

        private CWord ReadNextInstuction()
        {
            return ReadWord(GetProgramCounter);
        }
    }
}
