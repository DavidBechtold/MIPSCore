using MIPSCore;
using MIPSCoreUI.Services;
using MIPSCoreUI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore.Control_Unit;
using MIPSCore.Util;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    public class MainWindowViewModel :NotificationObject
    {
        private MipsCore core;
        private IControlUnit controlUnit;
        private IMIPSViewModel mipsCoreViewModel;
        private IMIPSRegisterViewModel mipsRegisterViewModel;
        private IMIPSViewModel mipsMemoryViewModel;
        private IMessageBoxService messageBox;
        private IOpenFileDialogService openFileDialog;

        public DelegateCommand Clock { get; private set; }
        public DelegateCommand Run { get; private set; }
        public DelegateCommand Stop { get; private set; }
        public DelegateCommand LoadFile { get; private set; }
        public DelegateCommand ViewRegisterHex { get; private set; }
        public DelegateCommand ViewRegisterSignedDecimal { get; private set; }
        public DelegateCommand ViewRegisterUnsignedDecimal { get; private set; }

        /* executed command */
        private string executedInstructionName;
        private string executedInstructionExample;
        private string executedInstructionMeaning;
        private string executedInstructionFormat;
        private string executedInstructionFunction;
        private string executedInstructionOpCode;
        private string excecutedInstructionAluOperation;

        public MainWindowViewModel(MipsCore core, IMIPSViewModel mipsCoreViewModel, IMIPSRegisterViewModel mipsRegisterViewModel, IMIPSViewModel mipsMemoryViewModel, IMessageBoxService messageBox, IOpenFileDialogService openFileDialog)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsCoreViewModel");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsRegisterViewModel");
            if (messageBox == null) throw new ArgumentNullException("messageBox");
            if (openFileDialog == null) throw new ArgumentNullException("openFileDialog");

            this.core = core;
            controlUnit = core.ControlUnit;
            this.mipsCoreViewModel = mipsCoreViewModel;
            this.mipsRegisterViewModel = mipsRegisterViewModel;
            this.mipsMemoryViewModel = mipsMemoryViewModel;
            this.messageBox = messageBox;
            this.openFileDialog = openFileDialog;

            core.Clocked += clocked;
            core.Completed += completed;
            core.Exception += exception;

            /* install delegates für command bindings */
            Clock = new DelegateCommand(() => OnClock());
            Run = new DelegateCommand(() => OnRun());
            Stop = new DelegateCommand(() => OnStop());
            LoadFile = new DelegateCommand(() => OnLoadFile());
            ViewRegisterHex = new DelegateCommand(() => ViewRegister(RegisterView.HexaDecimal));
            ViewRegisterSignedDecimal = new DelegateCommand(() => ViewRegister(RegisterView.SignedDecimal));
            ViewRegisterUnsignedDecimal = new DelegateCommand(() => ViewRegister(RegisterView.UnsignedDecimal)); 
        }

        private void clocked(Object sender, EventArgs args)
        {
            fillExecutedInstructionGroupBox();
            mipsCoreViewModel.refresh();
            mipsRegisterViewModel.refresh();
            mipsMemoryViewModel.refresh();

            if (core.Mode == ExecutionMode.RunToCompletion)
                CBootstrapper.Redraw();
        }

        private void completed(Object sender, EventArgs args)
        {
            core.SetMode(ExecutionMode.SingleStep);
        }

        private void exception(Object sender, EventArgs args)
        {
            messageBox.ShowNotification(core.GetExceptionString());
        }

        private void OnClock()
        {
            core.SingleClock();
        }

        private void OnRun()
        {
            core.SetMode(ExecutionMode.RunToCompletion);
            core.SingleClock();
        }

        private void OnStop()
        {
            core.SetMode(ExecutionMode.SingleStep);
        }

        private void OnLoadFile()
        {
            openFileDialog.SetFilter("Object Dumps (*.objdump)|*.objdump|All files (*.*)|*.*");
            if (!openFileDialog.OpenFileDialog())
                return;

            core.ProgramObjdump(openFileDialog.GetFileName());
            core.StartCore();
            mipsRegisterViewModel.refresh();
            mipsMemoryViewModel.refresh();
        }

        private void ViewRegister(RegisterView view)
        {
            mipsRegisterViewModel.Display = view;
            mipsRegisterViewModel.refresh();
        }

        private void fillExecutedInstructionGroupBox()
        {
            ExecutedInstructionName = controlUnit.GetInstructionAssemblerName + ": " + controlUnit.GetInstructionFriendlyName;
            ExecutedInstructionExample = controlUnit.GetInstructionExample;
            ExecutedInstructionMeaning = controlUnit.GetInstructionMeaning;
            ExecutedInstructionFormat = controlUnit.GetInstructionFormat;
            ExecutedInstructionFunction = controlUnit.GetInstructionFunction;
            ExecutedInstructionOpCode = controlUnit.GetInstructionOpCode;
            ExcecutedInstructionAluOperation = controlUnit.AluControl.ToText();
        }

        /* Executed Command */
        public String ExecutedInstructionName
        {
            set { executedInstructionName = value; RaisePropertyChanged(() => ExecutedInstructionName); }
            get { return executedInstructionName; }
        }

        public String ExecutedInstructionExample
        {
            set { executedInstructionExample = value; RaisePropertyChanged(() => ExecutedInstructionExample); }
            get { return executedInstructionExample; }
        }

        public String ExecutedInstructionMeaning
        {
            set { executedInstructionMeaning = value; RaisePropertyChanged(() => ExecutedInstructionMeaning); }
            get { return executedInstructionMeaning; }
        }

        public String ExecutedInstructionFormat
        {
            set { executedInstructionFormat = value; RaisePropertyChanged(() => ExecutedInstructionFormat); }
            get { return executedInstructionFormat; }
        }

        public String ExecutedInstructionFunction
        {
            set { executedInstructionFunction = value; RaisePropertyChanged(() => ExecutedInstructionFunction); }
            get { return executedInstructionFunction; }
        }

        public String ExecutedInstructionOpCode
        {
            set { executedInstructionOpCode = value; RaisePropertyChanged(() => ExecutedInstructionOpCode); }
            get { return executedInstructionOpCode; }
        }
        public String ExcecutedInstructionAluOperation
        {
            set { excecutedInstructionAluOperation = value; RaisePropertyChanged(() => ExcecutedInstructionAluOperation); }
            get { return excecutedInstructionAluOperation; }
        }
        
    }
}
