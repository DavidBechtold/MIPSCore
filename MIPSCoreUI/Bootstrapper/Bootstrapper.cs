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
        public static IMipsViewModel MipsCoreViewModel { get; private set; }
        public static IMipsRegisterViewModel MipsRegisterViewModel { get; private set; }
        public static IMipsViewModel MipsMemoryViewModel { get; private set; }
        public static IMessageBoxService MessageBox { get; private set; }
        public static IOpenFileDialogService OpenFileDialog { get; private set; }
        public static SettingsViewModel SettingsViewModel { get; private set; }

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
            MipsCoreViewModel = new MipsCoreViewModel(Core.ControlUnit, Application.Current.Dispatcher);
            MipsRegisterViewModel = new MipsRegisterViewModel(Core, Application.Current.Dispatcher);
            MipsMemoryViewModel = new MipsMemoryViewModel(Core, Application.Current.Dispatcher);
            MainWindowViewModel = new MainWindowViewModel(Core, MipsCoreViewModel, MipsRegisterViewModel, MipsMemoryViewModel, MessageBox, OpenFileDialog);
            SettingsViewModel = new SettingsViewModel();
        }
    }
}
