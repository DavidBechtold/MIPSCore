using System;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Instruction_Set;
using MIPSCore.Register_File;
using MIPSCore.Util;
using MIPSCore.Util._Memory;

namespace MIPSCore.Instruction_Memory
{
    public class InstructionMemory : Memory, IInstructionMemory
    {
        private bool firstCommand;
        private bool branchDelaySlot;
        private bool instructionDelayed;
        private Word delayedProgramCounter;


        public IAlu Alu { get; set; }
        public IControlUnit ControlUnit { get; set; }
        public IRegisterFile RegisterFile { get; set; }
        
        public Word GetOpCode { get; private set; }
        public Word GetFunction { get; private set; }
        public Word GetShiftAmount { get; private set; }
        public Word GetImmediate { get; private set; }
        public Word GetRd { get; private set; }
        public Word GetRs { get; private set; }
        public Word GetRt { get; private set; }
        public Word GetJumpTarget { get; private set; }
        public Word GetProgramCounter { get; private set; }
        public Word GetActualInstruction { get; private set; }
        public Word SetAddressOffset { set { Offset = value; } }

        public InstructionMemory(MemorySize size)
            : base(size)
        {
            Flush();
            branchDelaySlot = false;
            instructionDelayed = false;
            delayedProgramCounter = new Word((uint) 0);
        }

        public void Clock()
        {
            // 1.) calculate new programcounter
            CalcProgramCounter();

            // 2.) get instruction from memory to execute
            GetActualInstruction = ReadNextInstuction();

            // TODO make converter class
            // 3.) divide instruction for the register file and the control unit
            GetOpCode = GetActualInstruction.GetSubword(0, 6);

            if (GetOpCode.UnsignedDecimal == 0)
            {
                /* R - Format */
                GetRs = GetActualInstruction.GetSubword(6, 5);
                GetRt = GetActualInstruction.GetSubword(6 + 5, 5);
                GetRd = GetActualInstruction.GetSubword(6 + 5 + 5, 5);
                GetShiftAmount = GetActualInstruction.GetSubword(6 + 5 + 5 + 5, 5);
                GetFunction = GetActualInstruction.GetSubword(32 - 6, 6);
            }
            else
            {
                /* I Format */
                GetImmediate = GetActualInstruction.GetSubword(16, 16);
                GetRs = GetActualInstruction.GetSubword(6, 5);
                GetRd = GetActualInstruction.GetSubword(6 + 5, 5);
                /* J Format */
                GetJumpTarget = GetActualInstruction.GetSubword(6, 26);
            }
        }

        public void BranchDelaySlot(bool branchDelay)
        {
            branchDelaySlot = branchDelay;
        }

        private void CalcProgramCounter()
        {
            if (instructionDelayed)
            {
                instructionDelayed = false;
                GetProgramCounter = delayedProgramCounter;
                return;
            }

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
                    {
                        if (branchDelaySlot)
                        {
                            instructionDelayed = true;
                            delayedProgramCounter = GetProgramCounter + (uint) ((short) GetImmediate.SignedDecimal*4 + 4);
                            GetProgramCounter += 4;
                        }
                        else
                            GetProgramCounter += (uint) ((short) GetImmediate.SignedDecimal*4 + 4);
                    }
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendUnequal:
                    if (!Alu.ZeroFlag)
                    {
                        if (branchDelaySlot)
                        {
                            instructionDelayed = true;
                            delayedProgramCounter = GetProgramCounter + (uint)((short)GetImmediate.SignedDecimal * 4 + 4);
                            GetProgramCounter += 4;
                        }
                        else
                            GetProgramCounter += (uint)((short)GetImmediate.SignedDecimal * 4 + 4);
                    }
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendLessThanZero:
                    if (Alu.GetResultLo.UnsignedDecimal == 1)
                    {
                        if (branchDelaySlot)
                        {
                            instructionDelayed = true;
                            delayedProgramCounter = GetProgramCounter + (uint)((short)GetImmediate.SignedDecimal * 4 + 4);
                            GetProgramCounter += 4;
                        }
                        else
                            GetProgramCounter += (uint)((short)GetImmediate.SignedDecimal * 4 + 4);
                    }
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.SignExtendLessOrEqualZero:
                    if (!Alu.ZeroFlag || Alu.GetResultLo.UnsignedDecimal == 1)
                    {
                        if (branchDelaySlot)
                        {
                            instructionDelayed = true;
                            delayedProgramCounter = GetProgramCounter + (uint)((short)GetImmediate.SignedDecimal * 4 + 4);
                            GetProgramCounter += 4;
                        }
                        else
                            GetProgramCounter += (uint)((short)GetImmediate.SignedDecimal * 4 + 4);
                    }
                    else
                        GetProgramCounter += 4;
                    break;
                case ProgramCounterSource.Jump:
                    if (branchDelaySlot)
                    {
                        instructionDelayed = true;
                        delayedProgramCounter = GetJumpTarget * 4;
                        GetProgramCounter += 4;
                    }
                    else
                        GetProgramCounter = GetJumpTarget*4;
                    break;
                case ProgramCounterSource.Register:
                    if (branchDelaySlot)
                    {
                        instructionDelayed = true;
                        delayedProgramCounter = RegisterFile.ReadRs();
                        GetProgramCounter += 4;
                    }
                    else
                        GetProgramCounter = RegisterFile.ReadRs();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Reset()
        {
            GetOpCode = new Word((uint)0);
            GetFunction = new Word((uint)0);
            GetShiftAmount = new Word((uint)0);
            GetImmediate = new Word((uint)0);
            GetRd = new Word((uint)0);
            GetRs = new Word((uint)0);
            GetRt = new Word((uint)0);
            GetJumpTarget = new Word((uint)0);

            firstCommand = true;
            GetProgramCounter = new Word((uint)0);
            GetActualInstruction = new Word(0);
        }

        public override sealed void Flush()
        {
            Reset();
            base.Flush();
        }

        public bool GetBranchDelaySlot()
        {
            return branchDelaySlot;
        }

        private Word ReadNextInstuction()
        {
            return ReadWord(GetProgramCounter);
        }
    }
}
