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
        public event PropertyChangedEventHandler PropertyChanged;
        private CCoreUI coreUI;        
       
        public MainWindow()
        {
            InitializeComponent();
            core = new CCore();
            coreUI = new CCoreUI();
            this.DataContext = coreUI;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //control.RegisterWrite = new SolidColorBrush(Colors.Blue);
            //coreUI.
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;  
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void InstructionMemory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string test = "";
        }

    }

    public class CCoreUI: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private CControlUnit control;
        private SolidColorBrush registerWrite;

        private double adderWidth = 30;
        private double adderHeight = 30;
        private double rectangleHeight = 140;
        private double rectangleWidth = 100;
        private double rectangleSpaceBetween = 50;

        public CCoreUI()
        {
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

        public double RectangleWidth
        {
            get { return rectangleWidth; }
            set { rectangleWidth = value;  OnPropertyChanged("RectangleWidth"); }
        }

        public double RectangleHeight 
        {
            get {return rectangleHeight;}
            set { rectangleHeight = value;  OnPropertyChanged("RectangleHeight"); }
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
