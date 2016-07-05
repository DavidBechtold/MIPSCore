using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCoreUI.Bootstrapper;
using MIPSCoreUI.ViewModel;

namespace MIPSCoreUI.View
{
    public class MemoryListViewModel : NotificationObject, IViewModel
    {
        private readonly Dispatcher dispatcher;
        private ObservableCollection<ListTextDto> list;
        private int oldProgramCounter;
        public ListTextDto SelectedItem { get; set; }
        public DelegateCommand AddBreakpoint { get; set; }

        public MemoryListViewModel(Dispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            this.dispatcher = dispatcher;
            List = new ObservableCollection<ListTextDto>();
            AddBreakpoint = new DelegateCommand(OnAddBreakpoint);
            oldProgramCounter = 0;
        }

        private void OnAddBreakpoint()
        {
            if (List.Count > 0 && oldProgramCounter < List.Count)
            {
                List[SelectedItem.Number].Background = new SolidColorBrush(Colors.Yellow);
                List[SelectedItem.Number].Changed();
            }
        }

        public ObservableCollection<ListTextDto> List
        {
            get { return list; }
            private set { list = value; RaisePropertyChanged(() => List); }
        }

        public void Refresh()
        {
            /* invoke the wpf thread */
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(RefreshGui));
        }

        public void RefreshGui()
        {
            List[oldProgramCounter / 4].Background = new SolidColorBrush(Colors.White);
            List[oldProgramCounter / 4].Changed();
            oldProgramCounter = CBootstrapper.Core.InstructionMemory.GetProgramCounter.SignedDecimal;
            List[oldProgramCounter / 4].Background = new SolidColorBrush(Colors.DarkGray);
            List[oldProgramCounter / 4].Changed();
        }

        public void Draw()
        {
            List.Clear();
            var instructionMemory = CBootstrapper.Core.InstructionMemory;
            var codeDict = CBootstrapper.Core.Code;
            var codeCounter = 0;
            for (uint i = 0; i < instructionMemory.GetLastByteAddress; i = i + 4, codeCounter++)
            {
                var code = "";
                var instruction = instructionMemory.ReadWord(i).Hexadecimal;
                if ((codeCounter < codeDict.Count) && codeDict.ContainsKey(i))
                        code += codeDict[i];
                List.Add(new ListTextDto(i, codeCounter, instruction, code, new SolidColorBrush(Colors.White)));
            }
        }
    }

    public class ListTextDto : NotificationObject
    {
        public string Instruction { get; set; }
        public string Code { get; set; }
        public uint Address { get; set; }
        public int Number { get; set; }

        public ListTextDto()
        {
            Address = 0;
            Number = 0;
            Instruction = "";
            Code = "";
            Text = "";
            Background = new SolidColorBrush(Colors.White);
        }

        public void Changed()
        {
            Text = ToString();
            RaisePropertyChanged(() => Text);
            RaisePropertyChanged(() => Background);
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
        }

        public SolidColorBrush Background { get; set; }

        public string Text { get; set; }

        public override sealed string ToString()
        {
            return Convert.ToString(Address, 16).PadLeft(8, '0').ToUpper() + "   " + Instruction + "   " + Code;
        }
    }
}