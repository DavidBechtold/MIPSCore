namespace MIPSCoreUI.ViewModel
{
    public interface IViewModel
    {
        void Refresh(); // is called when the simulator is running with an program
        void Draw();    // is called when a new program is loaded into the simulator => refresh data structures if needed
    }

    public interface IMipsExtendedViewModel : IViewModel
    {
        ValueView Display { get; set; }
    }

    public interface IMessageableViewModel : IViewModel
    {
        void ErrorMessage(string message);
        void NotificationMessage(string message);
    }
}
