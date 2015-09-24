using System;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.Register_File;

namespace MIPSCoreUI.ViewModel
{
    public enum RegisterView { HexaDecimal, SignedDecimal, UnsignedDecimal };
    public class MIPSRegisterViewModel : NotificationObject, IMIPSRegisterViewModel
    {
        private MipsCore core;
        private Dispatcher dispatcher;
        public RegisterView Display { get; set; }

        public string MIPSRegisters { get; private set; }



        public MIPSRegisterViewModel(MipsCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;
            Display = RegisterView.SignedDecimal;
        }

        public void refresh()
        {
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                refreshGUI();
            }));
        }

        private void refreshGUI()
        {
            switch (Display)
            {
                case RegisterView.HexaDecimal: MIPSRegisters = registerToHex(); break;
                case RegisterView.UnsignedDecimal: MIPSRegisters = registerToUnsignedDec(); break;
                case RegisterView.SignedDecimal: MIPSRegisters = registerToSignedDec(); break;
            }
            RaisePropertyChanged(() => MIPSRegisters);
        }

        private string registerToHex()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.getHexadecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.getHexadecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.getHexadecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().getHexadecimal + "\n";
            result += "\t$rd\t:" + core.RegisterFile.ReadRd().getHexadecimal + "\n";
            result += "\t$rt\t:" + core.RegisterFile.ReadRs().getHexadecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.getHexadecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegisterHex(i) + "\n";
            return result;
        }

        private string registerToUnsignedDec()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.getUnsignedDecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.getUnsignedDecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.getUnsignedDecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().getUnsignedDecimal + "\n";
            result += "\t$rd\t:" + core.RegisterFile.ReadRd().getUnsignedDecimal + "\n";
            result += "\t$rt\t:" + core.RegisterFile.ReadRs().getUnsignedDecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.getUnsignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegisterUnsigned(i) + "\n";
            return result;
        }

        private string registerToSignedDec()
        {
            string result = "\t$pc\t:" + core.InstructionMemory.GetProgramCounter.getSignedDecimal + "\n";
            result += "\t$lo\t:" + core.Alu.GetResultLo.getSignedDecimal + "\n";
            result += "\t$hi\t:" + core.Alu.GetResultHi.getSignedDecimal + "\n";
            result += "\t$rs\t:" + core.RegisterFile.ReadRs().getSignedDecimal + "\n";
            result += "\t$rd\t:" + core.RegisterFile.ReadRd().getSignedDecimal + "\n";
            result += "\t$rt\t:" + core.RegisterFile.ReadRs().getSignedDecimal + "\n";
            result += "\t$imm\t:" + core.InstructionMemory.GetImmediate.getSignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < RegisterFile.RegisterCount; i++)
                result += core.RegisterFile.ToStringRegister(i) + "\n";
            return result;
        }
    }
}
