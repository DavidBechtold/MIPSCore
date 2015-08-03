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
using MIPSCore.InstructionSet;

using MIPSCore.ControlUnit;

namespace MIPSCoreUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private CCoreUI coreUI;
        public event PropertyChangedEventHandler PropertyChanged;
                
        public MainWindow()
        {
            InitializeComponent();
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

        private void ButtonClock_Click(object sender, RoutedEventArgs e)
        {
            coreUI.clock();
        }

    }

    public class CCoreUI: INotifyPropertyChanged
    {
        private CCore core;
        private BackgroundWorker worker;
        private enum Events{clocked, exception, completed};
       
        /* colors */
        private SolidColorBrush instructionMemoryActive;
        private SolidColorBrush registerFileActive;
        private SolidColorBrush aluActive;
        private SolidColorBrush dataMemoryActive;

        /* control line colors */
        private SolidColorBrush controlLineInactive = new SolidColorBrush(Colors.LightBlue);
        private SolidColorBrush controlLineActive = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush branchControlLine;
        private SolidColorBrush jumpControlLine;
        private SolidColorBrush aluOperationControlLine;    
        private SolidColorBrush regFileWriteControlLine;
        private SolidColorBrush dataMemoryControlLine;
        private SolidColorBrush aluSourceControlLine;
        private SolidColorBrush regFileInputControlLine;

        public event PropertyChangedEventHandler PropertyChanged;

        private double adderWidth = 30;
        private double adderHeight = 30;
        private double rectangleHeight = 140;
        private double rectangleWidth = 100;
        private double rectangleSpaceBetween = 50;

        public CCoreUI()
        {
            core = new CCore();
            core.setMode(ExecutionMode.singleStep);
            core.clocked += new EventHandler(clockedEvent);
            core.completed += new EventHandler(completedEvent);
            core.exception += new EventHandler(exceptionEvent);

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork +=new DoWorkEventHandler(worker_doWork);
            worker.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(worker_runWorkerCompleted);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_progressChanged);

            branchControlLine = jumpControlLine = aluOperationControlLine = dataMemoryControlLine = aluSourceControlLine = regFileWriteControlLine = regFileInputControlLine = controlLineInactive;


            core.programObjdump("Testcode//bubblesort.objdump");
            core.startCore();
        }

        public void clock()
        {
            core.singleClock();
        }

        private void clockedEvent(Object obj, EventArgs args)
        {
            /* let the work do from an background worker because the clocked event is not the wpf gui thread */
            if (worker.IsBusy != true)
            {
                worker.RunWorkerAsync();
            }
            


        }

        private void completedEvent(Object obj, EventArgs args)
        {
        }

        private void exceptionEvent(Object obj, EventArgs args)
        {
        }

        private void worker_doWork(Object obj, EventArgs args)
        {
            /* Control Lines */
            if (core.getControlUnit.getAluControl == ALUControl.stall)
                this.ALUOperationControlLine = controlLineInactive;
            else
            {
                this.aluOperationControlLine = new SolidColorBrush(Colors.Blue);
                OnPropertyChanged("ALUOperationControlLine");
            }

            if (core.getControlUnit.getMemRead == true || core.getControlUnit.getMemWrite == true)
                this.DataMemoryControlLine = controlLineActive;
            else
                this.DataMemoryControlLine = controlLineInactive;
        }

        private void worker_runWorkerCompleted(Object obj, EventArgs args)
        {
        }

        private void worker_progressChanged(Object obj, EventArgs args)
        {
        }

        public SolidColorBrush BranchControlLine
        {
            set { branchControlLine = value; OnPropertyChanged("BranchControlLine"); }
            get { return branchControlLine; }
        }

        public SolidColorBrush JumpControlLine
        {
            set { jumpControlLine = value; OnPropertyChanged("JumpControlLine"); }
            get { return jumpControlLine; }
        }

        public SolidColorBrush ALUSourceControlLine
        {
            set { aluSourceControlLine = value; OnPropertyChanged("ALUSourceControlLine"); }
            get { return aluSourceControlLine; }
        }

        public SolidColorBrush ALUOperationControlLine
        {
            set { aluSourceControlLine = value; OnPropertyChanged("ALUOperationControlLine"); }
            get { return aluOperationControlLine; }
        }

        public SolidColorBrush DataMemoryControlLine
        {
            set { dataMemoryControlLine = value; OnPropertyChanged("DataMemoryControlLine"); }
            get { return dataMemoryControlLine; }
        }

        public SolidColorBrush RegFileInputControlLine
        {
            set { regFileInputControlLine = value; OnPropertyChanged("RegFileInputControlLine"); }
            get { return regFileInputControlLine; }
        }

        public SolidColorBrush RegFileWriteControlLine
        {
            set { regFileWriteControlLine = value; OnPropertyChanged("RegFileWriteControlLine"); }
            get { return regFileWriteControlLine; }
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
