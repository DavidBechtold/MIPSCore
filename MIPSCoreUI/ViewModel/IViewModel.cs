namespace MIPSCoreUI.ViewModel
{
    public interface IViewModel
    {
        void Refresh();
        void Draw();
    }

    public interface IMipsExtendedViewModel : IViewModel
    {
        ValueView Display { get; set; }
    }
}
