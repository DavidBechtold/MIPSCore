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

        private bool singleStep;

        public CClock(UInt64 frequency_Hz, EventHandler callback)
        {
            if (frequency_Hz == 0)
                throw new ArgumentException("Class Clock: Constructor: Argument frequency_Hz must be greater than zero.");
            this.frequency_Hz = frequency_Hz;
            this.frequency_ns = 1.0 / frequency_Hz * 1E9;
            this.frequency_ms = 1.0 / frequency_Hz * 1E3;
            this.callback = callback;
            this.singleStep = false;

            timer = new Timer(frequency_ms);
            timer.Elapsed += new ElapsedEventHandler(timerCallback);
        }

        public void step()
        {
            timer.Start();
        }

        public void start()
        {
            if(!singleStep)
                timer.Start();
        }

        public void stop()
        {
            timer.Stop();
        }

        private void timerCallback(object obj, ElapsedEventArgs target)
        {
            if (singleStep)
                timer.Stop();

            callback.Invoke("Timer", new EventArgs());
        }

        public bool SingleStep
        {
            get
            {
                return singleStep;
            }
            set
            {
                singleStep = value;
            }
        }
    }
}
