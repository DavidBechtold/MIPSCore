using System;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Threading;
using MIPSCoreUI.Bootstrapper;
using MIPSCoreUI.View;
using MipsCore = MIPSCore.MipsCore;

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

        public void Draw()
        {
            MipsDataMemory = core.DataMemory.Hexdump(0, core.DataMemory.GetLastByteAddress);
            CBootstrapper.AddHighlightedTextToInstructionMemory("", HighlightAction.Clear);
            RefreshInstructionMemory();
            RaisePropertyChanged(() => MipsDataMemory);
        }

        public void RefreshGui()
        {
            MipsDataMemory = core.DataMemory.Hexdump(0, core.DataMemory.GetLastByteAddress);
            //CBootstrapper.AddHighlightedTextToInstructionMemory("", false, true);
            RefreshInstructionMemoryWhileRunning();
            RaisePropertyChanged(() => MipsDataMemory);
        }

        private void RefreshInstructionMemory()
        {
            /* puh ^^ had problems with the performance, so i try to call the func so few as possible */
            // TODO remove this shit with the textblock and make it better!
            var stringToAdd = "";
            var codeCounter = 0;
            for (uint i = 0; i < core.InstructionMemory.GetLastByteAddress; i = i + 4, codeCounter++)
            {
                if (core.InstructionMemory.GetProgramCounter.UnsignedDecimal == i)
                {
                    CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, HighlightAction.AddNormal);
                    stringToAdd = Convert.ToString(i, 16).PadLeft(8, '0').ToUpper() + "   ";
                    stringToAdd += core.InstructionMemory.ReadWord(i).Hexadecimal + "   ";
                    if (codeCounter < core.Code.Count)
                    {
                        if(core.Code.ContainsKey(i))
                            stringToAdd += core.Code[i] + "\n";
                        else
                            stringToAdd += "\n";        
                    }
                    else
                        stringToAdd += "\n";
                    CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, HighlightAction.AddHighlighted);
                    stringToAdd = "";
                    continue;
                }

                stringToAdd += Convert.ToString(i, 16).PadLeft(8, '0').ToUpper() + "   ";
                stringToAdd += core.InstructionMemory.ReadWord(i).Hexadecimal + "   ";
                if(codeCounter < core.Code.Count)
                {
                    if (core.Code.ContainsKey(i))
                        stringToAdd += core.Code[i] + "\n";
                    else
                        stringToAdd += "\n";  
                }
                else
                    stringToAdd += "\n";
            }
            CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, HighlightAction.AddNormal);
        }

        private void RefreshInstructionMemoryWhileRunning()
        {
            var text = CBootstrapper.InstructionMemoryText();
            CBootstrapper.AddHighlightedTextToInstructionMemory("", HighlightAction.Clear);

            var address = core.InstructionMemory.GetProgramCounter.UnsignedDecimal;
            var stringToAdd = Convert.ToString(address, 16).PadLeft(8, '0').ToUpper() + "   ";
            stringToAdd += core.InstructionMemory.ReadWord(address).Hexadecimal + "   ";

            if(core.Code.ContainsKey(address))
                stringToAdd += core.Code[address] + "\n";
            else
                stringToAdd += "\n";   
     
            var place = text.IndexOf(stringToAdd, StringComparison.Ordinal);
            if (place < 0)
                return;
            var textBefore = text.Substring(0, place);
            var textAfter = text.Substring(place + stringToAdd.Length, text.Length - (place+ stringToAdd.Length));

            CBootstrapper.AddHighlightedTextToInstructionMemory(textBefore, HighlightAction.AddNormal);
            CBootstrapper.AddHighlightedTextToInstructionMemory(stringToAdd, HighlightAction.AddHighlighted);
            CBootstrapper.AddHighlightedTextToInstructionMemory(textAfter, HighlightAction.AddNormal);
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
