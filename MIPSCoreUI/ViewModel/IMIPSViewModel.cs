namespace MIPSCoreUI.ViewModel
{
    public interface IMipsViewModel
    {
        void Refresh();
        void Draw();
    }

    public interface IMipsRegisterViewModel : IMipsViewModel
    {
        RegisterView Display { get; set; }
    }
}
