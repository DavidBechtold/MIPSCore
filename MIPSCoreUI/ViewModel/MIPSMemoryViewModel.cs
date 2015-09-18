using System;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.ViewModel
{
    public class MIPSMemoryViewModel : NotificationObject, IMIPSViewModel
    {
        private CCore core;
        private Dispatcher dispatcher;

        public string MIPSInstructionMemory {get; private set;}
        public string MIPSDataMemory { get; private set; }

        public MIPSMemoryViewModel(CCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;

            MIPSInstructionMemory = "";
            MIPSDataMemory = "";
        }

        public void refresh()
        {
            /* invoke the wpf thread */
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                refreshGUI();
            }));
        }

        private void refreshGUI()
        {
            MIPSDataMemory = core.getDataMemory.hexdump(0, core.getDataMemory.getLastByteAddress);
            CBootstrapper.AddHighlightedTextToInstructionMemory("", false, true);
            refreshInstructionMemory();
            RaisePropertyChanged(() => MIPSDataMemory);
        }

        private void refreshInstructionMemory()
        {
            /* puh ^^ had problems with the performance, so i try to call the func so few as possible */
            string stringToAdd = "";
            int codeCounter = 0;
            for (uint i = 0; i < core.getInstructionMemory.getLastByteAddress; i = i + 4, codeCounter++)
            {
                if (core.getInstructionMemory.getProgramCounter.getUnsignedDecimal == i)
                {
                    CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, false, false);
                    stringToAdd = Convert.ToString(i, 16).PadLeft(8, '0').ToUpper() + "   ";
                    stringToAdd += core.getInstructionMemory.readWord(i).getHexadecimal + "   ";
                    if (codeCounter < core.Code.Count)
                        stringToAdd += core.Code[codeCounter] + "\n";
                    else
                        stringToAdd += "\n";
                    CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, true, false);
                    stringToAdd = "";
                    continue;
                }

                stringToAdd += Convert.ToString(i, 16).PadLeft(8, '0').ToUpper() + "   ";
                stringToAdd += core.getInstructionMemory.readWord(i).getHexadecimal + "   ";
                if(codeCounter < core.Code.Count)
                    stringToAdd += core.Code[codeCounter] + "\n";
                else
                    stringToAdd += "\n";
            }
            CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, false, false);
        }

        private void refreshDataMemory()
        {

        }
    }

    public class InstructionEntry
    {
        public string Address { get; set; }
        public string Instruction { get; set; }
        public string Code { get; set; }

        public InstructionEntry(string address, string instruction, string code)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (instruction == null) throw new ArgumentNullException("instruction");
            if (code == null) throw new ArgumentNullException("code");
            Address = address;
            Instruction = instruction;
            Code = code;
        }
    }

    public class DataEntry
    {
        public string Address { get; set; }
        public string Value { get; set; }

        public DataEntry(string address, string value)
        {
            if (address == null) throw new ArgumentNullException("address");
            if (value == null) throw new ArgumentNullException("value");
            Address = address;
            Value = value;
        }
    }
}
