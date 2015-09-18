using System;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.RegisterFile;

namespace MIPSCoreUI.ViewModel
{
    public enum RegisterView { HexaDecimal, SignedDecimal, UnsignedDecimal };
    public class MIPSRegisterViewModel : NotificationObject, IMIPSRegisterViewModel
    {
        private CCore core;
        private Dispatcher dispatcher;
        public RegisterView Display { get; set; }

        public string MIPSRegisters { get; private set; }



        public MIPSRegisterViewModel(CCore core, Dispatcher dispatcher)
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
            string result = "\t$pc\t:" + core.getInstructionMemory.getProgramCounter.getHexadecimal + "\n";
            result += "\t$lo\t:" + core.getAlu.getResultLO.getHexadecimal + "\n";
            result += "\t$hi\t:" + core.getAlu.getResultHI.getHexadecimal + "\n";
            result += "\t$rs\t:" + core.getRegisterFile.readRs().getHexadecimal + "\n";
            result += "\t$rd\t:" + core.getRegisterFile.readRd().getHexadecimal + "\n";
            result += "\t$rt\t:" + core.getRegisterFile.readRs().getHexadecimal + "\n";
            result += "\t$imm\t:" + core.getInstructionMemory.getImmediate.getHexadecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < CRegisterFile.RegisterCount; i++)
                result += core.getRegisterFile.toStringRegisterHex(i) + "\n";
            return result;
        }

        private string registerToUnsignedDec()
        {
            string result = "\t$pc\t:" + core.getInstructionMemory.getProgramCounter.getUnsignedDecimal + "\n";
            result += "\t$lo\t:" + core.getAlu.getResultLO.getUnsignedDecimal + "\n";
            result += "\t$hi\t:" + core.getAlu.getResultHI.getUnsignedDecimal + "\n";
            result += "\t$rs\t:" + core.getRegisterFile.readRs().getUnsignedDecimal + "\n";
            result += "\t$rd\t:" + core.getRegisterFile.readRd().getUnsignedDecimal + "\n";
            result += "\t$rt\t:" + core.getRegisterFile.readRs().getUnsignedDecimal + "\n";
            result += "\t$imm\t:" + core.getInstructionMemory.getImmediate.getUnsignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < CRegisterFile.RegisterCount; i++)
                result += core.getRegisterFile.toStringRegisterUnsigned(i) + "\n";
            return result;
        }

        private string registerToSignedDec()
        {
            string result = "\t$pc\t:" + core.getInstructionMemory.getProgramCounter.getSignedDecimal + "\n";
            result += "\t$lo\t:" + core.getAlu.getResultLO.getSignedDecimal + "\n";
            result += "\t$hi\t:" + core.getAlu.getResultHI.getSignedDecimal + "\n";
            result += "\t$rs\t:" + core.getRegisterFile.readRs().getSignedDecimal + "\n";
            result += "\t$rd\t:" + core.getRegisterFile.readRd().getSignedDecimal + "\n";
            result += "\t$rt\t:" + core.getRegisterFile.readRs().getSignedDecimal + "\n";
            result += "\t$imm\t:" + core.getInstructionMemory.getImmediate.getSignedDecimal + "\n";
            result += "\n";
            for (ushort i = 0; i < CRegisterFile.RegisterCount; i++)
                result += core.getRegisterFile.toStringRegister(i) + "\n";
            return result;
        }
    }
}
