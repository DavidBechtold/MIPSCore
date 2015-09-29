namespace MIPSCore.Timing
{
    public interface IClock
    {
        void Step();
        void Start();
        void Stop();
        bool SingleStep { get; set; }
        ulong FrequencyHz { get; set; }
    }
}