using System;
using System.Timers;

namespace MIPSCore.Timing
{
    public class Clock: IClock, IDisposable
    {
        private readonly EventHandler callback;
        private readonly Timer timer;
        private ulong frequencyHz;

        public Clock(ulong frequencyHz, EventHandler callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            this.frequencyHz = frequencyHz;
            this.callback = callback;
            timer = new Timer();
            timer.Elapsed += TimerCallback;
            Init();
        }

        private void Init()
        {
            if (frequencyHz == 0)
            {
                timer.Interval = 0;
                return;
            }
            var frequencyMs = 1.0 / frequencyHz * 1E3;
            SingleStep = false;
            timer.Interval = frequencyMs;
        }

        public void Step()
        {
            timer.Start();
        }

        public void Start()
        {
            if(!SingleStep)
                timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public bool SingleStep { get; set; }

        private void TimerCallback(object obj, ElapsedEventArgs target)
        {
            if (SingleStep)
                timer.Stop();

            callback.Invoke("Timer", new EventArgs());
        }

        public ulong FrequencyHz { get { return frequencyHz; } set { frequencyHz = value; Init(); } }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
