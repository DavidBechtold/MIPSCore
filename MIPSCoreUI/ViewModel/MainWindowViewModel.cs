using System;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Util;
using MIPSCoreUI.Services;
using MIPSCoreUI.View;
using MipsCore = MIPSCore.MipsCore;

namespace MIPSCoreUI.ViewModel
{
    public class MainWindowViewModel :NotificationObject
    {
        private readonly MipsCore core;
        private readonly IControlUnit controlUnit;
        private readonly IAlu alu;
        private readonly IMipsViewModel mipsCoreViewModel;
        private readonly IMipsExtendedViewModel mipsRegisterViewModel;
        private readonly IMipsExtendedViewModel mipsMemoryViewModel;
        private readonly IMessageBoxService messageBox;
        private readonly IOpenFileDialogService openFileDialog;

        public DelegateCommand Clock { get; private set; }
        public DelegateCommand Run { get; private set; }
        public DelegateCommand Stop { get; private set; }
        public DelegateCommand LoadFile { get; private set; }
        public DelegateCommand ViewHexadecimal { get; private set; }
        public DelegateCommand ViewSignedDecimal { get; private set; }
        public DelegateCommand ViewUnsignedDecimal { get; private set; }
        public DelegateCommand Settings { get; private set; }

        /* executed command */
        private string executedInstructionName;
        private string executedInstructionExample;
        private string executedInstructionMeaning;
        private string executedInstructionFormat;
        private string executedInstructionFunction;
        private string executedInstructionOpCode;
        private string excecutedInstructionAluOperation;

        /* state register */
        private readonly SolidColorBrush stateRegisterActive;
        private readonly SolidColorBrush stateRegisterInactive;
        private SolidColorBrush stateRegisterSignFlag;
        private SolidColorBrush stateRegisterZeroFlag;
        private SolidColorBrush stateRegisterOverflowFlag;
        private SolidColorBrush stateRegisterCarryFlag;

        public MainWindowViewModel(MipsCore core, IMipsViewModel mipsCoreViewModel, IMipsExtendedViewModel mipsRegisterViewModel, IMipsExtendedViewModel mipsMemoryViewModel, IMessageBoxService messageBox, IOpenFileDialogService openFileDialog)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsCoreViewModel");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsRegisterViewModel");
            if (messageBox == null) throw new ArgumentNullException("messageBox");
            if (openFileDialog == null) throw new ArgumentNullException("openFileDialog");

            this.core = core;
            controlUnit = core.ControlUnit;
            alu = core.Alu;
            this.mipsCoreViewModel = mipsCoreViewModel;
            this.mipsRegisterViewModel = mipsRegisterViewModel;
            this.mipsMemoryViewModel = mipsMemoryViewModel;
            this.messageBox = messageBox;
            this.openFileDialog = openFileDialog;

            core.Clocked += Clocked;
            core.Completed += Completed;
            core.Exception += Exception;

            /* install delegates für command bindings */
            Clock = new DelegateCommand(OnClock);
            Run = new DelegateCommand(OnRun);
            Stop = new DelegateCommand(OnStop);
            LoadFile = new DelegateCommand(OnLoadFile);
            ViewHexadecimal = new DelegateCommand(() => OnViewRegister(ValueView.HexaDecimal));
            ViewSignedDecimal = new DelegateCommand(() => OnViewRegister(ValueView.SignedDecimal));
            ViewUnsignedDecimal = new DelegateCommand(() => OnViewRegister(ValueView.UnsignedDecimal));
            Settings = new DelegateCommand(OnSettings);

            /* state register */
            stateRegisterActive = new SolidColorBrush(Colors.DeepSkyBlue);
            stateRegisterInactive = new SolidColorBrush(Colors.White);
        }

        private void Clocked(object sender, EventArgs args)
        {
            FillExecutedInstructionGroupBox();
            StateRegisterRefresh();
            mipsCoreViewModel.Refresh();
            mipsRegisterViewModel.Refresh();
            mipsMemoryViewModel.Refresh();
        }

        private void Completed(object sender, EventArgs args)
        {
            core.SetMode(ExecutionMode.SingleStep);
        }

        private void Exception(object sender, EventArgs args)
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

            core.StopCore();
            core.SetMode(ExecutionMode.SingleStep);
            core.ProgramObjdump(openFileDialog.GetFileName());
            core.StartCore();
            mipsRegisterViewModel.Draw();
            mipsMemoryViewModel.Draw();
        }

        private void OnSettings()
        {
            var window = new SettingsWindow();
            window.ShowDialog();
            mipsMemoryViewModel.Draw();
        }

        private void OnViewRegister(ValueView view)
        {
            mipsRegisterViewModel.Display = view;
            mipsMemoryViewModel.Display = view;
            mipsRegisterViewModel.Refresh();
            mipsMemoryViewModel.Draw();
        }

        private void FillExecutedInstructionGroupBox()
        {
            ExecutedInstructionName = controlUnit.GetInstructionAssemblerName + ": " + controlUnit.GetInstructionFriendlyName;
            ExecutedInstructionExample = controlUnit.GetInstructionExample;
            ExecutedInstructionMeaning = controlUnit.GetInstructionMeaning;
            ExecutedInstructionFormat = controlUnit.GetInstructionFormat;
            ExecutedInstructionFunction = controlUnit.GetInstructionFunction;
            ExecutedInstructionOpCode = controlUnit.GetInstructionOpCode;
            ExcecutedInstructionAluOperation = controlUnit.AluControl.ToText();
        }

        private void StateRegisterRefresh()
        {
            StateRegisterZeroFlag = stateRegisterInactive;
            StateRegisterOverflowFlag = stateRegisterInactive;
            StateRegisterCarryFlag = stateRegisterInactive;
            StateRegisterSignFlag = stateRegisterInactive;

            if (alu.ZeroFlag)
                StateRegisterZeroFlag = stateRegisterActive;
            if (alu.OverflowFlag)
                StateRegisterOverflowFlag = stateRegisterActive;
            if (alu.CarryFlag)
                StateRegisterCarryFlag = stateRegisterActive;
            if (alu.SignFlag)
                StateRegisterSignFlag = stateRegisterActive;
        }

        /* Executed Command */
        public string ExecutedInstructionName
        {
            set { executedInstructionName = value; RaisePropertyChanged(() => ExecutedInstructionName); }
            get { return executedInstructionName; }
        }

        public string ExecutedInstructionExample
        {
            set { executedInstructionExample = value; RaisePropertyChanged(() => ExecutedInstructionExample); }
            get { return executedInstructionExample; }
        }

        public string ExecutedInstructionMeaning
        {
            set { executedInstructionMeaning = value; RaisePropertyChanged(() => ExecutedInstructionMeaning); }
            get { return executedInstructionMeaning; }
        }

        public string ExecutedInstructionFormat
        {
            set { executedInstructionFormat = value; RaisePropertyChanged(() => ExecutedInstructionFormat); }
            get { return executedInstructionFormat; }
        }

        public string ExecutedInstructionFunction
        {
            set { executedInstructionFunction = value; RaisePropertyChanged(() => ExecutedInstructionFunction); }
            get { return executedInstructionFunction; }
        }

        public string ExecutedInstructionOpCode
        {
            set { executedInstructionOpCode = value; RaisePropertyChanged(() => ExecutedInstructionOpCode); }
            get { return executedInstructionOpCode; }
        }
        public string ExcecutedInstructionAluOperation
        {
            set { excecutedInstructionAluOperation = value; RaisePropertyChanged(() => ExcecutedInstructionAluOperation); }
            get { return excecutedInstructionAluOperation; }
        }

        public SolidColorBrush StateRegisterSignFlag
        {
            set { stateRegisterSignFlag = value; RaisePropertyChanged(() => StateRegisterSignFlag); }
            get { return stateRegisterSignFlag; }
        }

        public SolidColorBrush StateRegisterZeroFlag
        {
            set { stateRegisterZeroFlag = value; RaisePropertyChanged(() => StateRegisterZeroFlag); }
            get { return stateRegisterZeroFlag; }
        }

        public SolidColorBrush StateRegisterOverflowFlag
        {
            set { stateRegisterOverflowFlag = value; RaisePropertyChanged(() => StateRegisterOverflowFlag); }
            get { return stateRegisterOverflowFlag; }
        }

        public SolidColorBrush StateRegisterCarryFlag
        {
            set { stateRegisterCarryFlag = value; RaisePropertyChanged(() => StateRegisterCarryFlag); }
            get { return stateRegisterCarryFlag; }
        }
    }
}
