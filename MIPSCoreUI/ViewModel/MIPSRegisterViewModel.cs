using System;
using System.Windows.Threading;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.Instruction_Set;
using MIPSCore.Register_File;

namespace MIPSCoreUI.ViewModel
{
    public enum ValueView { HexaDecimal, SignedDecimal, UnsignedDecimal };
    public class MipsRegisterViewModel : NotificationObject, IMipsExtendedViewModel
    {
        private readonly MipsCore core;
        private readonly Dispatcher dispatcher;
        public ValueView Display { get; set; }

        public string MipsRegisters { get; private set; }

        public MipsRegisterViewModel(MipsCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;
            Display = ValueView.SignedDecimal;
        }

        public void Refresh()
        {
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(RefreshGui));
        }

        public void Draw()
        {
            Refresh();
        }

        private void RefreshGui()
        {
            switch (Display)
            {
                case ValueView.HexaDecimal: MipsRegisters = RegisterToHex(); break;
                case ValueView.UnsignedDecimal: MipsRegisters = RegisterToUnsignedDec(); break;
                case ValueView.SignedDecimal: MipsRegisters = RegisterToSignedDec(); break;
            }
            RaisePropertyChanged(() => MipsRegisters);
        }

        private string RegisterToHex()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.Hexadecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.Hexadecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.Hexadecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().Hexadecimal + "\n";
            if (core.ControlUnit.InstructionFormat == InstructionFormat.R)
                result += "\t$rd\t:" + core.RegisterFile.ReadRd().UnsignedDecimal + "\n";
            else
                result += "\t$rd\t:" + 0 + "\n";  
            result += "\t$rt\t:" + core.RegisterFile.ReadRt().Hexadecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.Hexadecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegisterHex(i) + "\n";
            return result;
        }

        private string RegisterToUnsignedDec()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.UnsignedDecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.UnsignedDecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.UnsignedDecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().UnsignedDecimal + "\n";
            if (core.ControlUnit.InstructionFormat == InstructionFormat.R)
                result += "\t$rd\t:" + core.RegisterFile.ReadRd().UnsignedDecimal + "\n";
            else
                result += "\t$rd\t:" + 0 + "\n";    
            result += "\t$rt\t:" + core.RegisterFile.ReadRt().UnsignedDecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.UnsignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegisterUnsigned(i) + "\n";
            return result;
        }

        private string RegisterToSignedDec()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.SignedDecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.SignedDecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.SignedDecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().SignedDecimal + "\n";
            if (core.ControlUnit.InstructionFormat == InstructionFormat.R)
                result += "\t$rd\t:" + core.RegisterFile.ReadRd().UnsignedDecimal + "\n";
            else
                result += "\t$rd\t:" + 0 + "\n";  
            result += "\t$rt\t:" + core.RegisterFile.ReadRt().SignedDecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.SignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegister(i) + "\n";
            return result;
        }
    }
}
