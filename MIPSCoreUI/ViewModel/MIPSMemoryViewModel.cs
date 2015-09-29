﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
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

        private ObservableCollection<ListTextDto> list;
        private int oldProgramCounter;
        public ListTextDto SelectedItem { get; set; }
        public DelegateCommand AddBreakpoint { get; set; }

        public MipsMemoryViewModel(MipsCore core, Dispatcher dispatcher)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (dispatcher == null) throw new ArgumentException("dispatcher");
            this.core = core;
            this.dispatcher = dispatcher;

            MipsInstructionMemory = "";
            MipsDataMemory = "";

            List = new ObservableCollection<ListTextDto>();
            AddBreakpoint = new DelegateCommand(OnAddBreakpoint);
            oldProgramCounter = 0;
        }

        public void Refresh()
        {
            /* invoke the wpf thread */
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(RefreshGui));
        }

        public void Draw()
        {
            MipsDataMemory = core.DataMemory.Hexdump(0, core.DataMemory.GetLastByteAddress);
            DrawInstructionMemory();
            RaisePropertyChanged(() => MipsDataMemory);
        }

        public void RefreshGui()
        {
            MipsDataMemory = core.DataMemory.Hexdump(0, core.DataMemory.GetLastByteAddress);
            RefreshInstructionMemory();
            RaisePropertyChanged(() => MipsDataMemory);
        }

        public ObservableCollection<ListTextDto> List
        {
            get { return list; }
            private set { list = value; RaisePropertyChanged(() => List); }
        }

        private void OnAddBreakpoint()
        {
            if (List.Count > 0 && oldProgramCounter < List.Count)
            {
                List[SelectedItem.Number].Background = new SolidColorBrush(Colors.Yellow);
                List[SelectedItem.Number].Changed();
            }
        }

        private void DrawInstructionMemory()
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

        private void RefreshInstructionMemory()
        {
            if (List.Count > 0)
            {
                List[oldProgramCounter / 4].Background = new SolidColorBrush(Colors.White);
                List[oldProgramCounter / 4].Changed();
                oldProgramCounter = CBootstrapper.Core.InstructionMemory.GetProgramCounter.SignedDecimal;
                List[oldProgramCounter / 4].Background = new SolidColorBrush(Colors.DarkGray);
                List[oldProgramCounter / 4].Changed();
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
