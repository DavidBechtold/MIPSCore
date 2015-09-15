using MIPSCore;
using MIPSCoreUI.Services;
using MIPSCoreUI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace MIPSCoreUI.View
{
    class MainWindowViewModel :NotificationObject
    {
        private CCore core;
        private IMIPSCoreViewModel mipsCoreViewModel;
        private IMessageBoxService messageBox;
        private IOpenFileDialogService openFileDialog;

        public DelegateCommand Clock { get; private set; }

        public MainWindowViewModel(CCore core, IMIPSCoreViewModel mipsCoreViewModel, IMessageBoxService messageBox, IOpenFileDialogService openFileDialog)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsCoreViewModel");
            if (messageBox == null) throw new ArgumentNullException("messageBox");
            if (openFileDialog == null) throw new ArgumentNullException("openFileDialog");

            this.core = core;
            this.mipsCoreViewModel = mipsCoreViewModel;
            this.messageBox = messageBox;
            this.openFileDialog = openFileDialog;

            core.clocked += clocked;
            core.completed += completed;
            core.exception += exception;

            /* install delegates für command bindings */
            Clock = new DelegateCommand(() => OnClock());

        }

        private void clocked(Object sender, EventArgs args)
        {
            mipsCoreViewModel.clocked();
        }

        private void completed(Object sender, EventArgs args)
        {
        }

        private void exception(Object sender, EventArgs args)
        {
            messageBox.ShowNotification(args.ToString());
        }

        public void OnClock()
        {
            string test = "";
        }
    }
}
