using System;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Documents;
using System.Collections.ObjectModel;

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
            MIPSInstructionMemory = core.getInstructionMemory.hexdump(0, core.getInstructionMemory.getLastByteAddress);

            RaisePropertyChanged(() => MIPSInstructionMemory);
            RaisePropertyChanged(() => MIPSDataMemory);
        }
    }
}
