namespace MIPSCoreUI.ViewModel
{
    public interface IMipsViewModel
    {
        void Refresh();
        void Draw();
    }

    public interface IMipsExtendedViewModel : IMipsViewModel
    {
        ValueView Display { get; set; }
    }
}
