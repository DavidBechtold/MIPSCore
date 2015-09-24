using System;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using System.Windows.Threading;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.ViewModel
{
    public class MipsMemoryViewModel : NotificationObject, IMipsViewModel
    {
        private readonly MipsCore core;
        private readonly Dispatcher dispatcher;

        public string MipsInstructionMemory {get; private set;}
        public string MipsDataMemory { get; private set; }

        public MipsMemoryViewModel(MipsCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;

            MipsInstructionMemory = "";
            MipsDataMemory = "";
        }

        public void Refresh()
        {
            /* invoke the wpf thread */
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(RefreshGui));
        }

        private void RefreshGui()
        {
            MipsDataMemory = core.DataMemory.Hexdump(0, core.DataMemory.GetLastByteAddress);
            CBootstrapper.AddHighlightedTextToInstructionMemory("", false, true);
            RefreshInstructionMemory();
            RaisePropertyChanged(() => MipsDataMemory);
        }

        private void RefreshInstructionMemory()
        {
            /* puh ^^ had problems with the performance, so i try to call the func so few as possible */
            string stringToAdd = "";
            int codeCounter = 0;
            for (uint i = 0; i < core.InstructionMemory.GetLastByteAddress; i = i + 4, codeCounter++)
            {
                if (core.InstructionMemory.GetProgramCounter.UnsignedDecimal == i)
                {
                    CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, false, false);
                    stringToAdd = Convert.ToString(i, 16).PadLeft(8, '0').ToUpper() + "   ";
                    stringToAdd += core.InstructionMemory.ReadWord(i).Hexadecimal + "   ";
                    if (codeCounter < core.Code.Count)
                        stringToAdd += core.Code[codeCounter] + "\n";
                    else
                        stringToAdd += "\n";
                    CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, true, false);
                    stringToAdd = "";
                    continue;
                }

                stringToAdd += Convert.ToString(i, 16).PadLeft(8, '0').ToUpper() + "   ";
                stringToAdd += core.InstructionMemory.ReadWord(i).Hexadecimal + "   ";
                if(codeCounter < core.Code.Count)
                    stringToAdd += core.Code[codeCounter] + "\n";
                else
                    stringToAdd += "\n";
            }
            CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, false, false);
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
