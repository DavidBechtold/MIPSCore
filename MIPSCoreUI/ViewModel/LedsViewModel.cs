using System;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore.Data_Memory;

namespace MIPSCoreUI.ViewModel
{
    public class LedsViewModel : NotificationObject, IViewModel
    {
        public const int LedCount = 32;
        public readonly string Activate = "Activate";
        public readonly string Deactivate = "Deactivate";
        private readonly IDataMemory dataMemory;
        private SolidColorBrush[] leds;
        private string activateDeactivateLedsContent;
        private readonly SolidColorBrush ledInactive = new SolidColorBrush(Colors.DimGray);
        private readonly SolidColorBrush ledActive = new SolidColorBrush(Colors.DarkRed);
        private uint dataWordAddress;

        public DelegateCommand ActivateDeactivateLeds { get; private set; }
        
        public LedsViewModel(IDataMemory dataMemory)
        {
            if (dataMemory == null) throw new ArgumentNullException("dataMemory");
            this.dataMemory = dataMemory;

            dataWordAddress = dataMemory.GetLastByteAddress / 2 + 1;
            activateDeactivateLedsContent = Activate;
            ActivateDeactivateLeds = new DelegateCommand(OnActivateDeactivateLeds);

            Leds = new SolidColorBrush[LedCount];
            for (var i = 0; i < LedCount; i++)
                Leds[i] = ledInactive;
        }

        private void OnActivateDeactivateLeds()
        {
            ActivateDeactivateLedsContent = ActivateDeactivateLedsContent == Deactivate ? Activate : Deactivate;
            Refresh();
        }

        public void Refresh()
        {
            if (ActivateDeactivateLedsContent != Deactivate) return;

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

        public string ActivateDeactivateLedsContent
        {
            get { return activateDeactivateLedsContent;}
            set { activateDeactivateLedsContent = value; RaisePropertyChanged(() => ActivateDeactivateLedsContent); }
        }

        public uint DataWordAddress
        {
            get { return dataWordAddress; }
            set { dataWordAddress = value; RaisePropertyChanged(() => DataWordAddress); }
        }
    }
}