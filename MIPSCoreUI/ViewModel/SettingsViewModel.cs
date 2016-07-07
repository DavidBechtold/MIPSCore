using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.Util._Memory;

namespace MIPSCoreUI.ViewModel
{
    public class SettingsViewModel : NotificationObject
    {
        private readonly MipsCore core;
        private bool branchDelaySlot;
        public ulong FrequencyHz { get; set; }
        public DelegateCommand Apply { get; set; }
        public ObservableCollection<MemorySize> TextMemorySize { get; set; }
        public ObservableCollection<MemorySize> DataMemorySize { get; set; }
        public int TextMemorySizeIndex { get; set; }
        public int DataMemorySizeIndex { get; set; }
        public bool BranchDelaySlot { get { return branchDelaySlot;} set{ branchDelaySlot = value; RaisePropertyChanged(() => BranchDelaySlot); }}

        public SettingsViewModel(MipsCore core)
        {
            if (core == null) throw new ArgumentNullException("core");
            this.core = core;

            FrequencyHz = core.FrequencyHz;
            Apply = new DelegateCommand(OnApply);
            TextMemorySize = new ObservableCollection<MemorySize>();
            DataMemorySize = new ObservableCollection<MemorySize>();

            InitCollection(TextMemorySize);
            InitCollection(DataMemorySize);
            TextMemorySizeIndex = (int) core.InstructionMemory.Size;
            DataMemorySizeIndex = (int) core.DataMemory.Size;
            branchDelaySlot = core.GetBranchDelaySlot();
        }

        private void InitCollection(ICollection<MemorySize> collection)
        {
            foreach (MemorySize item in Enum.GetValues(typeof(MemorySize)))
                collection.Add(item);
        }   

        private void OnApply()
        {
            core.FrequencyHz = FrequencyHz;
            core.DataMemorySize(DataMemorySize[DataMemorySizeIndex]);
            core.TextMemorySize(DataMemorySize[TextMemorySizeIndex]);
            core.SetBranchDelaySlot(branchDelaySlot);
        }
    }
}