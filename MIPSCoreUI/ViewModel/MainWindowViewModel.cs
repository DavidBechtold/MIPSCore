using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore;
using MIPSCore.ALU;
using MIPSCore.Control_Unit;
using MIPSCore.Util;
using MIPSCore.Util.MIPSEventArgs;
using MIPSCoreUI.Services;
using MIPSCoreUI.View;
using MipsCore = MIPSCore.MipsCore;
using System.Windows.Threading;

namespace MIPSCoreUI.ViewModel
{
    public class MainWindowViewModel :NotificationObject
    {
        private readonly MipsCore core;
        private readonly IControlUnit controlUnit;
        private readonly IAlu alu;
        private readonly IViewModel mipsCoreViewModel;
        private readonly IMipsExtendedViewModel mipsRegisterViewModel;
        private readonly IMipsExtendedViewModel mipsMemoryViewModel;
        private readonly IViewModel ledsViewModel;
        private readonly IMessageableViewModel outputViewModel;
        private readonly IMessageBoxService messageBox;
        private readonly IOpenFileDialogService openFileDialog;
        private readonly DispatcherTimer loadingAnimationTimer;
        private bool isBusy;
        private int loadingAnimationStep;
        private string loadingAnimationFileName;
        private string busyStatusText;
        private string currentLoadedFilePath;
        private Action<string> currentLoadAction;
        private bool currentLoadRefreshOutput;

        public DelegateCommand Clock { get; private set; }
        public DelegateCommand Run { get; private set; }
        public DelegateCommand Stop { get; private set; }
        public DelegateCommand Reset { get; private set; }
        public DelegateCommand LoadCFile { get; private set; }
        public DelegateCommand LoadFile { get; private set; }
        public DelegateCommand LoadAsmFile { get; private set; }
        public DelegateCommand SaveFile { get; private set; }
        public DelegateCommand ViewHexadecimal { get; private set; }
        public DelegateCommand ViewSignedDecimal { get; private set; }
        public DelegateCommand ViewUnsignedDecimal { get; private set; }
        public DelegateCommand ViewVersion { get; private set; }
        public DelegateCommand Settings { get; private set; }
        public DelegateCommand Exit { get; private set; }
        public DelegateCommand ReloadCurrentFile { get; private set; }

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

        public MainWindowViewModel(MipsCore core, IViewModel mipsCoreViewModel, IMipsExtendedViewModel mipsRegisterViewModel, IMipsExtendedViewModel mipsMemoryViewModel, 
            IViewModel ledsViewModel, IMessageBoxService messageBox, IOpenFileDialogService openFileDialog, IMessageableViewModel outputViewModel)
        {
            if (core == null) throw new ArgumentNullException("core");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsCoreViewModel");
            if (mipsCoreViewModel == null) throw new ArgumentNullException("mipsRegisterViewModel");
            if (ledsViewModel == null) throw new ArgumentNullException("ledsViewModel");
            if (messageBox == null) throw new ArgumentNullException("messageBox");
            if (openFileDialog == null) throw new ArgumentNullException("openFileDialog");

            this.core = core;
            controlUnit = core.ControlUnit;
            alu = core.Alu;
            this.mipsCoreViewModel = mipsCoreViewModel;
            this.mipsRegisterViewModel = mipsRegisterViewModel;
            this.mipsMemoryViewModel = mipsMemoryViewModel;
            this.ledsViewModel = ledsViewModel;
            this.messageBox = messageBox;
            this.openFileDialog = openFileDialog;
            this.outputViewModel = outputViewModel;

            core.Clocked += Clocked;
            core.Completed += Completed;
            core.Exception += Exception;
            core.Notification += Notification;

            /* install delegates für command bindings */
            Clock = new DelegateCommand(OnClock);
            Run = new DelegateCommand(OnRun);
            Stop = new DelegateCommand(OnStop);
            Reset = new DelegateCommand(OnReset);
            LoadCFile = new DelegateCommand(OnLoadCFile);
            LoadFile = new DelegateCommand(OnLoadFile);
            LoadAsmFile = new DelegateCommand(OnLoadAsmFile);
            SaveFile = new DelegateCommand(OnSaveFile);
            ViewHexadecimal = new DelegateCommand(() => OnViewRegister(ValueView.HexaDecimal));
            ViewSignedDecimal = new DelegateCommand(() => OnViewRegister(ValueView.SignedDecimal));
            ViewUnsignedDecimal = new DelegateCommand(() => OnViewRegister(ValueView.UnsignedDecimal));
            ViewVersion = new DelegateCommand(() => OnVersionView());
            Settings = new DelegateCommand(OnSettings);
            Exit = new DelegateCommand(OnExit);
            ReloadCurrentFile = new DelegateCommand(OnReloadCurrentFile);

            /* state register */
            stateRegisterActive = new SolidColorBrush(Colors.DeepSkyBlue);
            stateRegisterInactive = new SolidColorBrush(Colors.White);

            loadingAnimationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
            loadingAnimationTimer.Tick += LoadingAnimationTick;
            BusyStatusText = "Datei wird geladen...";
        }

        private void Clocked(object sender, EventArgs args)
        {
            FillExecutedInstructionGroupBox();
            StateRegisterRefresh();
            mipsCoreViewModel.Refresh();
            mipsRegisterViewModel.Refresh();
            mipsMemoryViewModel.Refresh();
            ledsViewModel.Refresh();
            outputViewModel.Refresh();
        }

        private void Completed(object sender, EventArgs args)
        {
            core.SetMode(ExecutionMode.SingleStep);
        }

        private void Exception(object sender, EventArgs args)
        {
            MIPSEventArgs mips_args = args as MIPSEventArgs;
            if (mips_args != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    outputViewModel.ErrorMessage(mips_args.Message);
                    outputViewModel.Draw();
                });
            }
        }

        private void Notification(object sender, EventArgs args)
        {
            MIPSEventArgs mips_args = args as MIPSEventArgs;
            if (mips_args != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    outputViewModel.NotificationMessage(mips_args.Message);
                    outputViewModel.Draw();
                });
            }
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

        private void OnReset()
        {
            core.ResetCore();
            mipsRegisterViewModel.Draw();
            mipsMemoryViewModel.Draw();
            ledsViewModel.Draw();
            outputViewModel.Draw();
        }

        private async void OnLoadFile()
        {
            await LoadProgramAsync("Object Dumps (*.objdump)|*.objdump|All files (*.*)|*.*", core.ProgramObjdump, true);
        }

        private async void OnLoadCFile()
        {
            await LoadProgramAsync("C Files (*.c)|*.c", core.ProgramC);
        }

        private async void OnLoadAsmFile()
        {
            await LoadProgramAsync("C Files (*.s)|*.asm", core.ProgramAssembler);
        }

        private async void OnReloadCurrentFile()
        {
            if (string.IsNullOrWhiteSpace(currentLoadedFilePath) || currentLoadAction == null)
            {
                messageBox.ShowNotification("Es ist keine Datei geladen.");
                return;
            }

            await LoadProgramFromFileAsync(currentLoadedFilePath, currentLoadAction, currentLoadRefreshOutput);
        }

        private async Task LoadProgramAsync(string filter, Action<string> loadAction, bool refreshOutput = false)
        {
            openFileDialog.SetFilter(filter);
            if (!openFileDialog.OpenFileDialog())
                return;

            string fileName = openFileDialog.GetFileName();
            await LoadProgramFromFileAsync(fileName, loadAction, refreshOutput);
        }

        private async Task LoadProgramFromFileAsync(string fileName, Action<string> loadAction, bool refreshOutput = false)
        {
            SetCurrentLoadedProgram(fileName, loadAction, refreshOutput);
            StartLoadingAnimation(fileName);
            try
            {
                await Task.Run(() =>
                {
                    core.StopCore();
                    core.SetMode(ExecutionMode.SingleStep);
                    loadAction(fileName);
                    core.StartCore();
                });

                mipsRegisterViewModel.Draw();
                mipsMemoryViewModel.Draw();
                ledsViewModel.Draw();

                if (refreshOutput)
                    outputViewModel.Refresh();
            }
            catch (Exception ex)
            {
                messageBox.ShowNotification(string.Format("Fehler beim Laden: {0}", ex.Message));
            }
            finally
            {
                StopLoadingAnimation();
            }
        }

        private void SetCurrentLoadedProgram(string fileName, Action<string> loadAction, bool refreshOutput)
        {
            currentLoadedFilePath = fileName;
            currentLoadAction = loadAction;
            currentLoadRefreshOutput = refreshOutput;
        }

        private void StartLoadingAnimation(string fileName)
        {
            loadingAnimationFileName = Path.GetFileName(fileName);
            loadingAnimationStep = 0;
            UpdateBusyStatusText();
            IsBusy = true;
            loadingAnimationTimer.Start();
        }

        private void StopLoadingAnimation()
        {
            loadingAnimationTimer.Stop();
            IsBusy = false;
            BusyStatusText = "Datei wird geladen...";
        }

        private void LoadingAnimationTick(object sender, EventArgs e)
        {
            loadingAnimationStep++;
            UpdateBusyStatusText();
        }

        private void UpdateBusyStatusText()
        {
            var dots = string.Empty;
            var dotsCount = (loadingAnimationStep % 4) + 1;
            for (var i = 0; i < dotsCount; i++)
            {
                dots += " .";
            }

            BusyStatusText = string.Format("File \"{0}\" wird geladen{1}", loadingAnimationFileName, dots);
        }

        private void OnSaveFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Object Dumps (*.objdump)|*.objdump|All files (*.*)|*.*",
                Title = "Datei speichern",
                DefaultExt = "objdump",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                try
                {
                    // Hier sollte der Core den aktuellen Speicherinhalt oder das Programm speichern
                    File.WriteAllText(filePath, core.programmedFile);

                    messageBox.ShowNotification($"Datei erfolgreich gespeichert: {filePath}");
                }
                catch (Exception ex)
                {
                    messageBox.ShowNotification($"Fehler beim Speichern: {ex.Message}");
                }
            }
        }


        private void OnSettings()
        {
            var window = new SettingsView();
            window.ShowDialog();
            mipsMemoryViewModel.Draw();
        }

        private void OnExit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnViewRegister(ValueView view)
        {
            mipsRegisterViewModel.Display = view;
            mipsMemoryViewModel.Display = view;
            mipsRegisterViewModel.Refresh();
            mipsMemoryViewModel.Draw();
        }

        private void OnVersionView()
        {
            VersionView versionView = new VersionView();
            versionView.ShowDialog();
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

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public string BusyStatusText
        {
            get { return busyStatusText; }
            set
            {
                busyStatusText = value;
                RaisePropertyChanged(() => BusyStatusText);
            }
        }
    }
}
