using MIPSCore;
using MIPSCoreUI.Services;
using MIPSCoreUI.View;
using MIPSCoreUI.ViewModel;

namespace MIPSCoreUI.Bootstrapper
{
    class CBootstrapper
    {
        public CCore Core { get; private set; }
        public MainWindowViewModel MainWindowViewModel { get; private set; }
        public IMIPSCoreViewModel MipsCoreViewModel { get; private set; }
        public IMessageBoxService MessageBox {get; private set; }
        public IOpenFileDialogService OpenFileDialog { get; private set; }

        public CBootstrapper()
        {
            /* init core */
            Core = new CCore();
            Core.setMode(ExecutionMode.singleStep);

            Core.programObjdump("C://Users//david//Dropbox//Bachelor Arbeit//MIPSCore//SystemTest//Testcode//bubblesort.objdump");
            Core.startCore();

            /* init services */
            MessageBox = new MessageBoxService();
            OpenFileDialog = new DialogOpenFileDialogService();

            /* init viewmodels */
            MipsCoreViewModel = new MIPSCoreViewModel(Core.getControlUnit);
            MainWindowViewModel = new MainWindowViewModel(Core, MipsCoreViewModel, MessageBox, OpenFileDialog);
        }
    }
}
