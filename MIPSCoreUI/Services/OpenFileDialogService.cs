using Microsoft.Win32;

namespace MIPSCoreUI.Services
{
    class DialogOpenFileDialogService : IOpenFileDialogService
    {
        private readonly OpenFileDialog dialog;
        public DialogOpenFileDialogService()
        {
            dialog = new OpenFileDialog();
        }

        public bool OpenFileDialog()
        {
            var showDialog = dialog.ShowDialog();
            return showDialog != null && showDialog.Value;
        }

        public void SetFilter(string filterString)
        {
            dialog.Filter = filterString;
        }

        public string GetFileName()
        {
            return dialog.FileName;
        }
    }
}
