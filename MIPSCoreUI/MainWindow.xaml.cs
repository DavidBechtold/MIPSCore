using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MIPSCore;

using MIPSCore.ControlUnit;

namespace MIPSCoreUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CCore core;
        private ControlUnit control;
        public event PropertyChangedEventHandler PropertyChanged;
        public MainWindow()
        {
            InitializeComponent();
            core = new CCore();
            control = new ControlUnit(core.getControlUnit);
            this.DataContext = control;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            control.RegisterWrite = new SolidColorBrush(Colors.Blue);
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void InstructionMemory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string test = "";
        }
    }

    public class ControlUnit: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CControlUnit control;
        private SolidColorBrush registerWrite;
        public ControlUnit(CControlUnit control)
        {
            this.control = control;
            registerWrite = new SolidColorBrush(Colors.Black);
        }

        public string Control
        {
            set
            {
                string test = value;
            }
            get
            {
                return "TEST";
            }
        }

        public SolidColorBrush RegisterWrite
        {
            set
            {
                registerWrite = value;
                OnPropertyChanged("RegisterWrite");
            }
            get
            {
                return registerWrite;
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
