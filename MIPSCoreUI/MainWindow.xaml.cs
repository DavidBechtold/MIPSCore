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
using System.Windows.Threading;

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
        private enum Events{clocked, exception, completed};
       
        /* executed command */
        private string executedInstructionName;
        private string executedInstructionExample;
        private string executedInstructionMeaning;
        private string executedInstructionFormat;
        private string executedInstructionFunction;
        private string executedInstructionOpCode;
        
        /* colors */
        private SolidColorBrush instructionMemoryActive;
        private SolidColorBrush registerFileActive;
        private SolidColorBrush aluActive;
        private SolidColorBrush dataMemoryActive;

        /* lines */
        private SolidColorBrush lineInactive = new SolidColorBrush(Colors.Black);
        private SolidColorBrush lineActive = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush jumpRegisterLine;
        private SolidColorBrush jumpRegisterAluRead1Line;
        private SolidColorBrush programCounterLine;
        private SolidColorBrush programCounterOrRegisterFileInputLine;

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

            ExecutedInstructionName = "";

            jumpRegisterLine = JumpRegisterAluRead1Line = ProgramCounterLine = ProgramCounterOrRegisterFileInputLine = lineInactive;


            branchControlLine = jumpControlLine = aluOperationControlLine = dataMemoryControlLine = aluSourceControlLine = regFileWriteControlLine = regFileInputControlLine = controlLineInactive;

            core.programObjdump("C://Users//david//Dropbox//Bachelor Arbeit//MIPSCore//SystemTest//Testcode//bubblesort.objdump");
            core.startCore();
        }

        public void clock()
        {
            core.singleClock();
        }

        private void clockedEvent(Object obj, EventArgs args)
        {
            /* invoke the wpf thread */
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                refreshGUI();
            }));
        }

        private void completedEvent(Object obj, EventArgs args)
        {
        }

        private void exceptionEvent(Object obj, EventArgs args)
        {
        }

        private void refreshGUI()
        {
            fillExecutedInstructionGroupBox();
            
            /* Control Lines */
            if (core.getControlUnit.getAluControl == ALUControl.stall)
                aluOperationControlLine = controlLineInactive;
            else
                aluOperationControlLine = controlLineActive;

            /* Memory Lines */
            if (core.getControlUnit.getMemRead == true || core.getControlUnit.getMemWrite == true)
                dataMemoryControlLine = controlLineActive;
            else
                dataMemoryControlLine = controlLineInactive;

            /* Branch Lines */
            if (core.getControlUnit.getPcSource == ProgramCounterSource.programCounter)
            {
                branchControlLine = controlLineInactive;
                jumpControlLine = controlLineInactive;

                programCounterLine = lineActive;
                programCounterOrRegisterFileInputLine = lineActive;
                jumpRegisterLine = lineInactive;
                JumpRegisterAluRead1Line = lineInactive;
            }
            else if (core.getControlUnit.getPcSource == ProgramCounterSource.jump)
            {
                branchControlLine = controlLineInactive;
                jumpControlLine = controlLineActive;

                programCounterLine = lineInactive;
                programCounterOrRegisterFileInputLine = lineInactive;
                jumpRegisterLine = lineInactive;
                JumpRegisterAluRead1Line = lineInactive;
            }
            else if (core.getControlUnit.getPcSource == ProgramCounterSource.register)
            {
                branchControlLine = controlLineInactive;
                jumpControlLine = controlLineActive;

                programCounterLine = lineInactive;
                programCounterOrRegisterFileInputLine = lineInactive;
                jumpRegisterLine = lineActive;
                JumpRegisterAluRead1Line = lineActive;
            }
            else
            {
                branchControlLine = controlLineActive;
                jumpControlLine = controlLineInactive;

                programCounterLine = lineInactive;
                programCounterOrRegisterFileInputLine = lineInactive;
                jumpRegisterLine = lineInactive;
                JumpRegisterAluRead1Line = lineInactive;
            }

            OnPropertyChanged("ALUOperationControlLine");
            OnPropertyChanged("DataMemoryControlLine");
            OnPropertyChanged("BranchControlLine");
            OnPropertyChanged("JumpControlLine");

            OnPropertyChanged("ProgramCounterLine");
            OnPropertyChanged("ProgramCounterOrRegisterFileInputLine");
            OnPropertyChanged("JumpRegisterLine");
            OnPropertyChanged("JumpRegisterAluRead1Line");
        }

        private void fillExecutedInstructionGroupBox()
        {
            executedInstructionName = core.getControlUnit.GetInstructionAssemblerName + ": " + core.getControlUnit.GetInstructionFriendlyName;
            executedInstructionExample = core.getControlUnit.GetInstructionExample;
            executedInstructionMeaning = core.getControlUnit.GetInstructionMeaning;
            executedInstructionFormat = core.getControlUnit.GetInstructionFormat;
            executedInstructionFunction = core.getControlUnit.GetInstructionFunction;
            executedInstructionOpCode = core.getControlUnit.GetInstructionOpCode;
            OnPropertyChanged("ExecutedInstructionName");
            OnPropertyChanged("ExecutedInstructionExample");
            OnPropertyChanged("ExecutedInstructionMeaning");
            OnPropertyChanged("ExecutedInstructionFormat");
            OnPropertyChanged("ExecutedInstructionFunction");
            OnPropertyChanged("ExecutedInstructionOpCode");
        }
        
        /* Executed Command */
        public String ExecutedInstructionName
        {
            set { executedInstructionName = value; OnPropertyChanged("ExecutedCommandName"); }
            get { return executedInstructionName; }
        }

        public String ExecutedInstructionExample
        {
            set { executedInstructionExample = value; OnPropertyChanged("ExecutedInstructionExample"); }
            get { return executedInstructionExample; }
        }

        public String ExecutedInstructionMeaning
        {
            set { executedInstructionMeaning = value; OnPropertyChanged("ExecutedInstructionMeaning"); }
            get { return executedInstructionMeaning; }
        }

        public String ExecutedInstructionFormat
        {
            set { executedInstructionFormat = value; OnPropertyChanged("ExecutedInstructionFormat"); }
            get { return executedInstructionFormat; }
        }

        public String ExecutedInstructionFunction
        {
            set { executedInstructionFunction = value; OnPropertyChanged("ExecutedInstructionFunction"); }
            get { return executedInstructionFunction; }
        }

        public String ExecutedInstructionOpCode
        {
            set { executedInstructionOpCode = value; OnPropertyChanged("ExecutedInstructionOpCode"); }
            get { return executedInstructionOpCode; }
        }

        /* Lines */
        public SolidColorBrush JumpRegisterLine
        {
            set { jumpRegisterLine = value; OnPropertyChanged("JumpRegisterLine"); }
            get { return jumpRegisterLine; }
        }

        public SolidColorBrush JumpRegisterAluRead1Line
        {
            set { jumpRegisterAluRead1Line = value; OnPropertyChanged("JumpRegisterAluRead1Line"); }
            get { return jumpRegisterAluRead1Line; }
        }

        public SolidColorBrush ProgramCounterLine
        {
            set { programCounterLine = value; OnPropertyChanged("ProgramCounterLine"); }
            get { return programCounterLine; }
        }

        public SolidColorBrush ProgramCounterOrRegisterFileInputLine
        {
            set { programCounterOrRegisterFileInputLine = value; OnPropertyChanged("ProgramCounterOrRegisterFileInputLine"); }
            get { return programCounterOrRegisterFileInputLine; }
        }

        /* Control Lines */
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
