namespace MIPSCoreUI.Services
{
    public interface IOpenFileDialogService
    {
        bool OpenFileDialog();
        void SetFilter(string filterString);
        string GetFileName();
    }
}
