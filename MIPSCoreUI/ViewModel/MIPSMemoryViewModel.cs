using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using MIPSCoreUI.Bootstrapper;
using MipsCore = MIPSCore.MipsCore;

namespace MIPSCoreUI.ViewModel
{
    public class MipsMemoryViewModel : NotificationObject, IMipsViewModel
    {
        private readonly MipsCore core;
        private readonly Dispatcher dispatcher;
        private int oldProgramCounter;
        private int oldDataChangedAddress;

        public ObservableCollection<ListTextDto> List { get; private set; }
        public ObservableCollection<DataTextDto> DataList { get; private set; }
        public string MipsDataMemory { get; private set; }
        public ListTextDto SelectedItem { get; set; }
        public DelegateCommand AddBreakpoint { get; set; }
        public int SelectedMemoryIndex { get; set; }

        private SolidColorBrush lineNotActiveColor;

        public MipsMemoryViewModel(MipsCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;

            MipsDataMemory = "";

            List = new ObservableCollection<ListTextDto>();
            DataList = new ObservableCollection<DataTextDto>();
            AddBreakpoint = new DelegateCommand(OnAddBreakpoint);
            oldProgramCounter = 0;
            oldDataChangedAddress = 0;
            lineNotActiveColor = new SolidColorBrush(Colors.White);
        }

        public void Refresh()
        {
            /* invoke the wpf thread */
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(RefreshGui));
        }

        public void Draw()
        {
            DrawDataMemory();
            DrawInstructionMemory();
            RaisePropertyChanged(() => MipsDataMemory);
        }

        public void RefreshGui()
        {
                RefreshInstructionMemory();
                RefreshDataMemory();
        }

        private void OnAddBreakpoint()
        {
            if (List.Count <= 0 || oldProgramCounter >= List.Count) return;

            if (List[SelectedItem.Number].BreakpointVisible)
                List[SelectedItem.Number].BreakpointVisible = false;
            else
                List[SelectedItem.Number].BreakpointVisible = true;
            //TODO Add breakpoint to the mips core
            List[SelectedItem.Number].Changed();
        }

        private void DrawInstructionMemory()
        {
            List.Clear();
            var instructionMemory = core.InstructionMemory;
            var codeDict = core.Code;
            var codeCounter = 0;
            for (uint i = 0; i < instructionMemory.GetLastByteAddress; i = i + 4, codeCounter++)
            {
                var code = "";
                var instruction = instructionMemory.ReadWord(i).Hexadecimal;
                if ((codeCounter < codeDict.Count) && codeDict.ContainsKey(i))
                    code += codeDict[i];
                List.Add(new ListTextDto(i, codeCounter, instruction, code, lineNotActiveColor));
            }
        }

        private void DrawDataMemory()
        {
            DataList.Clear();
            var dataMemory = core.DataMemory;
            var codeCounter = 0;
            for (uint i = 0; i < dataMemory.GetLastByteAddress; i = i + 4, codeCounter++)
            {
                var value = dataMemory.ReadWord(i).Hexadecimal;
                DataList.Add(new DataTextDto(i, codeCounter, value, lineNotActiveColor));
            }
        }

        private void RefreshDataMemory()
        {
            if (DataList.Count <= 0) return;
            var dataMemory = core.DataMemory;
            foreach (var address in dataMemory.ChangedWordAddresses)
            {
                DataList[(int)address / 4].Value = dataMemory.ReadWord(address).Hexadecimal;
                DataList[(int)address / 4].Changed();
            }

            if (dataMemory.LastChangedAddress.Changed)
            {
                DataList[oldDataChangedAddress / 4].Background = lineNotActiveColor;
                DataList[oldDataChangedAddress / 4].Changed();
                oldDataChangedAddress = (int) dataMemory.LastChangedAddress.Address;
                DataList[oldDataChangedAddress / 4].Background = new SolidColorBrush(Colors.DarkGray);
                DataList[oldDataChangedAddress / 4].Changed();
            }
        }

        private void RefreshInstructionMemory()
        {
            if (List.Count <= 0) return;
            List[oldProgramCounter / 4].Background = lineNotActiveColor;
            List[oldProgramCounter / 4].Changed();
            oldProgramCounter = CBootstrapper.Core.InstructionMemory.GetProgramCounter.SignedDecimal;
            List[oldProgramCounter / 4].Background = new SolidColorBrush(Colors.DarkGray);
            List[oldProgramCounter / 4].Changed();
        }
    }

    public class ListTextDto : NotificationObject
    {
        public string Instruction { get; set; }
        public string Code { get; set; }
        public uint Address { get; set; }
        public int Number { get; set; }
        public bool BreakpointVisible { get; set; }

        public ListTextDto()
        {
            Address = 0;
            Number = 0;
            Instruction = "";
            Code = "";
            Text = "";
            BreakpointVisible = false;
            Background = new SolidColorBrush(Colors.White);
        }

        public void Changed()
        {
            Text = ToString();
            RaisePropertyChanged(() => Text);
            RaisePropertyChanged(() => Background);
            RaisePropertyChanged(() => BreakpointVisible);
        }

        public ListTextDto(uint address, int number, string instruction, string code, SolidColorBrush background)
        {
            if (instruction == null) throw new ArgumentNullException("instruction");
            if (code == null) throw new ArgumentNullException("code");
            if (background == null) throw new ArgumentNullException("background");
            Address = address;
            Number = number;
            Instruction = instruction;
            Code = code;
            Background = background;
            Text = ToString();
            Changed();
        }

        public SolidColorBrush Background { get; set; }

        public string Text { get; set; }

        public override sealed string ToString()
        {
            return Convert.ToString(Address, 16).PadLeft(8, '0').ToUpper() + "   " + Instruction + "   " + Code;
        }
    }

    public class DataTextDto : NotificationObject
    {
        public string Value { get; set; }
        public uint Address { get; set; }
        public int Number { get; set; }

        public DataTextDto()
        {
            Address = 0;
            Number = 0;
            Value = "";
            Text = "";
            Background = new SolidColorBrush(Colors.White);
        }

        public void Changed()
        {
            Text = ToString();
            RaisePropertyChanged(() => Text);
            RaisePropertyChanged(() => Background);
        }

        public DataTextDto(uint address, int number, string value, SolidColorBrush background)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (background == null) throw new ArgumentNullException("background");
            Address = address;
            Number = number;
            Value = value;
            Background = background;
            Text = ToString();
            Changed();
        }

        public SolidColorBrush Background { get; set; }

        public string Text { get; set; }

        public override sealed string ToString()
        {
            return Convert.ToString(Address, 16).PadLeft(8, '0').ToUpper() + "   " + Value;
        }
    }
}
