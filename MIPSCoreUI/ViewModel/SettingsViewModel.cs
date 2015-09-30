using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore.Util._Memory;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.ViewModel
{
    public class SettingsViewModel : NotificationObject
    {
        private int textMemorySizeIndex;
        private int dataMemorySizeIndex;

        public ulong FrequencyHz { get; set; }
        public DelegateCommand Apply { get; set; }
        public ObservableCollection<MemorySize> TextMemorySize { get; set; }
        public ObservableCollection<MemorySize> DataMemorySize { get; set; } 

        public SettingsViewModel()
        {
            FrequencyHz = CBootstrapper.Core.FrequencyHz;
            Apply = new DelegateCommand(OnApply);
            TextMemorySize = new ObservableCollection<MemorySize>();
            DataMemorySize = new ObservableCollection<MemorySize>();

            InitCollection(TextMemorySize);
            InitCollection(DataMemorySize);
            textMemorySizeIndex = (int) CBootstrapper.Core.InstructionMemory.Size - 1;
            dataMemorySizeIndex = (int) CBootstrapper.Core.DataMemory.Size - 1;
        }

        private void InitCollection(ICollection<MemorySize> collection)
        {
            foreach (MemorySize item in Enum.GetValues(typeof(MemorySize)))
                collection.Add(item);
        }   

        private void OnApply()
        {
            CBootstrapper.Core.FrequencyHz = FrequencyHz;
            CBootstrapper.Core.DataMemorySize(DataMemorySize[dataMemorySizeIndex]);
            CBootstrapper.Core.TextMemorySize(DataMemorySize[textMemorySizeIndex]);
        }

        public int TextMemorySizeIndex
        {
            get { return textMemorySizeIndex; }
            set { textMemorySizeIndex = value; }
        }

        public int DataMemorySizeIndex
        {
            get { return dataMemorySizeIndex; }
            set { dataMemorySizeIndex = value; }
        }
        
    }
}