namespace MIPSCoreUI.ViewModel
{
    public interface IMIPSViewModel
    {
        void refresh();
    }

    public interface IMIPSRegisterViewModel : IMIPSViewModel
    {
        RegisterView Display { get; set; }
    }
}
