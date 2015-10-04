using System;
using System.Windows.Media;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore.Data_Memory;

namespace MIPSCoreUI.ViewModel
{
    public class LedsViewModel : NotificationObject, IViewModel
    {
        public const int LedCount = 32;
        private readonly IDataMemory dataMemory;
        private SolidColorBrush[] leds;
        private readonly SolidColorBrush ledInactive = new SolidColorBrush(Colors.DimGray);
        private readonly SolidColorBrush ledActive = new SolidColorBrush(Colors.DarkRed);
        private uint dataWordAddress;

        public LedsViewModel(IDataMemory dataMemory)
        {
            if (dataMemory == null) throw new ArgumentNullException("dataMemory");
            this.dataMemory = dataMemory;

            dataWordAddress = 0;

            Leds = new SolidColorBrush[LedCount];
            for (var i = 0; i < LedCount; i++)
                Leds[i] = ledInactive;
        }

        public void Refresh()
        {
            if (DataWordAddress > dataMemory.GetLastByteAddress - 4)
                return;
            var word = dataMemory.ReadWord(DataWordAddress).UnsignedDecimal;
            for (var i = 0; i < LedCount; i++)
            {
                var test = (uint) Math.Pow(2, i);
                var test1 = word & test;
                if (test1 > 0)
                    Leds[i] = ledActive;
                else
                    Leds[i] = ledInactive;
            }
            RaisePropertyChanged(() => Leds);
        }

        public void Draw()
        {
            Refresh();
        }

        public SolidColorBrush[] Leds
        {
            get { return leds; }
            private set { leds = value; RaisePropertyChanged(() => Leds); }
        }

        public uint DataWordAddress
        {
            get { return dataWordAddress; }
            set { dataWordAddress = value; Refresh(); }
        }
    }
}