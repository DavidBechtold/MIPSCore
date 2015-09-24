using System;
using System.Timers;

namespace MIPSCore.Timing
{
    public class Clock: IClock
    {
        private readonly EventHandler callback;
        private readonly Timer timer;

        public Clock(ulong frequencyHz, EventHandler callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");
            if (frequencyHz == 0) throw new ArgumentException("Class Clock: Constructor: Argument frequency_Hz must be greater than zero.");
            var frequencyMs = 1.0 / frequencyHz * 1E3;
            this.callback = callback;
            SingleStep = false;

            timer = new Timer(frequencyMs);
            timer.Elapsed += TimerCallback;
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
    }
}
