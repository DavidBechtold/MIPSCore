using MIPSCore;
using MIPSCoreUI.Services;
using MIPSCoreUI.ViewModel;
using System;
using System.Windows;
using MipsCore = MIPSCore.MipsCore;

namespace MIPSCoreUI.Bootstrapper
{
    public static class CBootstrapper
    {
        public static MipsCore Core { get; private set; }
        public static MainWindowViewModel MainWindowViewModel { get; private set; }
        public static IViewModel MipsCoreViewModel { get; private set; }
        public static IMipsExtendedViewModel MipsRegisterViewModel { get; private set; }
        public static IMipsExtendedViewModel MipsMemoryViewModel { get; private set; }
        public static IMessageBoxService MessageBox { get; private set; }
        public static IOpenFileDialogService OpenFileDialog { get; private set; }
        public static SettingsViewModel SettingsViewModel { get; private set; }
        public static IViewModel LedsViewModel { get; private set; }
        public static IMessageableViewModel OutputViewModel { get; private set; }

        public static void Init()
        {
            /* init services */
            MessageBox = new MessageBoxService();
            OpenFileDialog = new DialogOpenFileDialogService();

            /* init core */
            try { Core = new MipsCore(); }
            catch (Exception e) { MessageBox.ShowNotification(e.ToString()); }
            Core.SetMode(ExecutionMode.SingleStep);

            /* init viewmodels */
            MipsCoreViewModel = new CoreViewModel(Core.ControlUnit, Application.Current.Dispatcher);
            MipsRegisterViewModel = new RegisterViewModel(Core, Application.Current.Dispatcher);
            MipsMemoryViewModel = new MemoryViewModel(Core, Application.Current.Dispatcher);
            SettingsViewModel = new SettingsViewModel(Core);
            LedsViewModel = new LedsViewModel(Core.DataMemory);
            OutputViewModel = new OutputViewModel();
            MainWindowViewModel = new MainWindowViewModel(Core, MipsCoreViewModel, MipsRegisterViewModel, MipsMemoryViewModel, LedsViewModel, MessageBox, OpenFileDialog, OutputViewModel);
        }
    }
}
