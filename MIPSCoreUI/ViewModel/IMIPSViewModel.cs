namespace MIPSCoreUI.ViewModel
{
    public interface IMipsViewModel
    {
        void Refresh();
    }

    public interface IMipsRegisterViewModel : IMipsViewModel
    {
        RegisterView Display { get; set; }
    }
}
