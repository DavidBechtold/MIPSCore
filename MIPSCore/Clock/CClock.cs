using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MIPSCore.Clock
{
    public class CClock
    {
        private UInt64 frequency_Hz;
        private double frequency_ns;
        private double frequency_ms;
        private EventHandler callback;
        private Timer timer;

        public CClock(UInt64 frequency_Hz, EventHandler callback)
        {
            if (frequency_Hz == 0)
                throw new ArgumentException("Class Clock: Constructor: Argument frequency_Hz must be greater than zero.");
            this.frequency_Hz = frequency_Hz;
            this.frequency_ns = 1 / frequency_Hz * 1E9;
            this.frequency_ms = 1 / frequency_Hz * 1E3;
            this.callback = callback;

            timer = new Timer(frequency_ms);
            timer.Elapsed += new ElapsedEventHandler(timerCallback);
        }

        public void start()
        {
            timer.Start();
        }

        public void stop()
        {
            timer.Stop();
        }

        private void timerCallback(object obj, ElapsedEventArgs target)
        {
            Console.WriteLine("Callback");
            callback.Invoke("Timer", new EventArgs());
        }
    }
}
