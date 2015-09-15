using System;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.RegisterFile;

namespace MIPSCoreUI.ViewModel
{
    public class MIPSRegisterViewModel : NotificationObject, IMIPSViewModel
    {
        private CCore core;
        private Dispatcher dispatcher;
        public bool DisplayHexadecimal { get; set; }

        public string MIPSRegisters { get; private set; }

        public MIPSRegisterViewModel(CCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;
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

            if (DisplayHexadecimal)
                MIPSRegisters = registerToHex();
            else
                MIPSRegisters = registerToUnsignedDec();
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
            result += "\n";
            for (ushort i = 0; i < CRegisterFile.RegisterCount; i++)
                result += core.getRegisterFile.toStringRegisterUnsigned(i) + "\n";
            return result;
        }
    }
}
