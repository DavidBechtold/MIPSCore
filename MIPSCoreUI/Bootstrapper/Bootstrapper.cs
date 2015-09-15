using MIPSCore;
using MIPSCoreUI.Services;
using MIPSCoreUI.View;
using MIPSCoreUI.ViewModel;
using System.Windows;

namespace MIPSCoreUI.Bootstrapper
{
    public static class CBootstrapper
    {
        public static CCore Core { get; private set; }
        public static MainWindowViewModel MainWindowViewModel { get; private set; }
        public static IMIPSViewModel MipsCoreViewModel { get; private set; }
        public static IMIPSViewModel MipsRegisterViewModel { get; private set; }
        public static IMIPSViewModel MipsMemoryViewModel { get; private set; }
        public static IMessageBoxService MessageBox { get; private set; }
        public static IOpenFileDialogService OpenFileDialog { get; private set; }

        public static void Init()
        {
            /* init core */
            Core = new CCore();
            Core.setMode(ExecutionMode.singleStep);

            /* init services */
            MessageBox = new MessageBoxService();
            OpenFileDialog = new DialogOpenFileDialogService();

            /* init viewmodels */
            MipsCoreViewModel = new MIPSCoreViewModel(Core.getControlUnit, Application.Current.Dispatcher);
            MipsRegisterViewModel = new MIPSRegisterViewModel(Core, Application.Current.Dispatcher);
            MipsMemoryViewModel = new MIPSMemoryViewModel(Core, Application.Current.Dispatcher);
            MainWindowViewModel = new MainWindowViewModel(Core, MipsCoreViewModel, MipsRegisterViewModel, MipsMemoryViewModel, MessageBox, OpenFileDialog);
        }
    }
}
