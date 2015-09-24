using System;
using System.Windows.Threading;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.Register_File;

namespace MIPSCoreUI.ViewModel
{
    public enum RegisterView { HexaDecimal, SignedDecimal, UnsignedDecimal };
    public class MipsRegisterViewModel : NotificationObject, IMipsRegisterViewModel
    {
        private readonly MipsCore core;
        private readonly Dispatcher dispatcher;
        public RegisterView Display { get; set; }

        public string MipsRegisters { get; private set; }

        public MipsRegisterViewModel(MipsCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;
            Display = RegisterView.SignedDecimal;
        }

        public void Refresh()
        {
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(RefreshGui));
        }

        private void RefreshGui()
        {
            switch (Display)
            {
                case RegisterView.HexaDecimal: MipsRegisters = RegisterToHex(); break;
                case RegisterView.UnsignedDecimal: MipsRegisters = RegisterToUnsignedDec(); break;
                case RegisterView.SignedDecimal: MipsRegisters = RegisterToSignedDec(); break;
            }
            RaisePropertyChanged(() => MipsRegisters);
        }

        private string RegisterToHex()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.Hexadecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.Hexadecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.Hexadecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().Hexadecimal + "\n";
            result += "\t$rd\t:" + core.RegisterFile.ReadRd().Hexadecimal + "\n";
            result += "\t$rt\t:" + core.RegisterFile.ReadRs().Hexadecimal + "\n";
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
            result += "\t$rd\t:" + core.RegisterFile.ReadRd().UnsignedDecimal + "\n";
            result += "\t$rt\t:" + core.RegisterFile.ReadRs().UnsignedDecimal + "\n";
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
            result += "\t$rd\t:" + core.RegisterFile.ReadRd().SignedDecimal + "\n";
            result += "\t$rt\t:" + core.RegisterFile.ReadRs().SignedDecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.SignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegister(i) + "\n";
            return result;
        }
    }
}
