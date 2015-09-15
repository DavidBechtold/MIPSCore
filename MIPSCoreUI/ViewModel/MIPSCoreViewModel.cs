using MIPSCore;
using MIPSCore.ControlUnit;
using MIPSCore.InstructionSet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MIPSCoreUI.ViewModel
{
    public class MIPSCoreViewModel: INotifyPropertyChanged, IMIPSCoreViewModel
    {
        private CControlUnit controlUnit;
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
        private SolidColorBrush jumpLine;
        private SolidColorBrush jumpRegisterLine;
        private SolidColorBrush jumpRegisterAluRead1Line;
        private SolidColorBrush programCounterLine;
        private SolidColorBrush programCounterOrRegisterFileInputLine;
        private SolidColorBrush branchMuxLine;
        private SolidColorBrush jumpMuxLine;
        private SolidColorBrush branchLine;
        private SolidColorBrush immediateOrBranchLine;
        private SolidColorBrush immediateLine;
        private SolidColorBrush instructionMemoryLine;
        private SolidColorBrush registerFileRsLine;
        private SolidColorBrush registerFileRdLine;
        private SolidColorBrush registerFileRtLine;
        private SolidColorBrush registerFileRsRdLine;
        private SolidColorBrush registerFileRsRdRtLine;
        private SolidColorBrush jumpBranchLine;
        private SolidColorBrush aluRead1Line;
        private SolidColorBrush aluRead2Line;
        private SolidColorBrush registerFileRtOutLine;
        private SolidColorBrush dataMemoryOutLine;
        private SolidColorBrush aluResultLine;
        private SolidColorBrush writeAluResultLine;
        private SolidColorBrush writePcToRegisterLine;
        private SolidColorBrush dataMemoryAddressLine;
        private SolidColorBrush registerFileWriteBackLine;

        /* muxes */
        private SolidColorBrush branchMux;
        private SolidColorBrush jumpMux;
        private SolidColorBrush aluSourceMux;
        private SolidColorBrush dataMemoryMux;

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

        public MIPSCoreViewModel(CControlUnit controlUnit)
        {
            if (controlUnit == null) throw new ArgumentNullException("controlUnit");
            this.controlUnit = controlUnit;

            ExecutedInstructionName = "";
            JumpLine = JumpRegisterLine = JumpRegisterAluRead1Line = ProgramCounterLine = 
                BranchLine = ImmediateOrBranchLine = InstructionMemoryLine =
                ProgramCounterOrRegisterFileInputLine = lineInactive;
            RegisterFileRsLine = JumpBranchLine = ImmediateLine = lineInactive;
        
            DataMemoryAddressLine = WritePcToRegisterLine = WriteAluResultLine = AluResultLine = DataMemoryOutLine = DataMemoryMux = AluSourceMux = RegisterFileRtOutLine = BranchMux = JumpMux = BranchMuxLine = JumpMuxLine = AluRead1Line = AluRead2Line = lineInactive;

            RegisterFileWriteBackLine = BranchControlLine = JumpControlLine = ALUOperationControlLine = DataMemoryControlLine = ALUSourceControlLine = RegFileWriteControlLine = RegFileInputControlLine = controlLineInactive;
        }

        public void clocked()
        {
            /* invoke the wpf thread */
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                refreshGUI();
            }));
        }

        private void refreshGUI()
        {
            fillExecutedInstructionGroupBox();

            aluOperationControlLine = controlLineInactive;
            branchControlLine = controlLineInactive;
            jumpControlLine = controlLineInactive;
            aluSourceControlLine = controlLineInactive;
            regFileWriteControlLine = controlLineInactive;

            aluSourceMux = lineInactive;
            jumpMux = lineActive;
            jumpMuxLine = lineActive;
            aluSourceMux = lineActive;
            instructionMemoryLine = lineActive;
            DataMemoryMux = lineInactive;

            jumpLine = lineInactive;
            branchLine = lineInactive;
            immediateOrBranchLine = lineInactive;
            branchMux = lineInactive;
            branchMuxLine = lineInactive;
            programCounterLine = lineInactive;
            programCounterOrRegisterFileInputLine = lineInactive;
            jumpRegisterLine = lineInactive;
            jumpRegisterAluRead1Line = lineInactive;
            jumpBranchLine = lineInactive;
            registerFileRsLine = lineInactive;
            registerFileRdLine = lineInactive;
            registerFileRtLine = lineInactive; 
            registerFileRsRdLine = lineInactive;
            registerFileRsRdRtLine= lineInactive;
            immediateLine = lineInactive;
            aluRead1Line = lineInactive;
            aluRead2Line = lineInactive;
            registerFileRtOutLine = lineInactive;
            dataMemoryOutLine = lineInactive;
            aluResultLine = lineInactive;
            writeAluResultLine = lineInactive;
            writePcToRegisterLine = lineInactive;
            dataMemoryAddressLine = lineInactive;
            registerFileWriteBackLine = lineInactive;

            /* Control Lines */
            if (controlUnit.getAluControl != ALUControl.stall)
            {
                aluOperationControlLine = controlLineActive;
                aluRead1Line = lineActive;
                aluRead2Line = lineActive;
                jumpRegisterAluRead1Line = lineActive;
            }

            if (controlUnit.getRegWrite)
            {
                regFileWriteControlLine = controlLineActive;
                dataMemoryMux = lineActive;
                aluResultLine = lineActive;
                registerFileWriteBackLine = lineActive;

                if (controlUnit.getRegisterFileInput == RegisterFileInput.aluLO)
                {
                    writeAluResultLine = lineActive;
                    
                }
                else if (controlUnit.getRegisterFileInput == RegisterFileInput.programCounter)
                    writePcToRegisterLine = lineActive;
                else if (controlUnit.getRegisterFileInput == RegisterFileInput.dataMemory)
                {
                    dataMemoryAddressLine = lineActive;
                    dataMemoryOutLine = lineActive;
                }
            }

            /* Memory Lines */
            if (controlUnit.getMemRead == true || controlUnit.getMemWrite == true)
            {
                dataMemoryControlLine = controlLineActive;
                dataMemoryAddressLine = lineActive;
                aluResultLine = lineActive;
            }
            else if (controlUnit.getMemRead)
            {
                dataMemoryMux = lineActive;
                dataMemoryOutLine = lineActive;
            }

            


            /* Branch Lines */
            if (controlUnit.getPcSource == ProgramCounterSource.programCounter)
            {
                branchMux = lineActive;
                branchMuxLine = lineActive;
                programCounterLine = lineActive;
                programCounterOrRegisterFileInputLine = lineActive;
            }
            else if (controlUnit.getPcSource == ProgramCounterSource.jump)
            {
                jumpControlLine = controlLineActive;

                jumpLine = lineActive;
                registerFileRsRdLine = lineActive;
                registerFileRsRdRtLine= lineActive;
                jumpBranchLine= lineActive;
                aluSourceMux = lineInactive;

            }
            else if (controlUnit.getPcSource == ProgramCounterSource.register)
            {
                jumpControlLine = controlLineActive;

                jumpRegisterLine = lineActive;
                jumpRegisterAluRead1Line = lineActive;
                aluSourceMux = lineInactive;
            }
            else //branch
            {
                branchControlLine = controlLineActive;

                branchLine = lineActive;
                immediateOrBranchLine = lineActive;
                branchMux = lineActive;
                branchMuxLine = lineActive;
                programCounterOrRegisterFileInputLine = lineActive;
                registerFileRsRdLine = lineActive;
                registerFileRsRdRtLine= lineActive;
                jumpBranchLine= lineActive;
                registerFileRtOutLine = lineActive;
            }

            /* instruction memory lines */
            if (controlUnit.getInstructionFormat == InstructionFormat.R)
            {
                registerFileRsLine = lineActive;
                registerFileRdLine = lineActive;
                registerFileRtLine = lineActive;
                registerFileRsRdLine = lineActive;
                registerFileRsRdRtLine = lineActive;
                registerFileRtOutLine = lineActive;
            }
            else if (controlUnit.getInstructionFormat == InstructionFormat.I && branchLine == lineInactive)
            {
                registerFileRsLine = lineActive;
                registerFileRtLine = lineActive;
                registerFileRsRdLine = lineActive;
                registerFileRsRdRtLine = lineActive;
                jumpBranchLine = lineActive;
                immediateOrBranchLine = lineActive;
                immediateLine = lineActive;
                aluSourceControlLine = controlLineActive;
            }
            
            OnPropertyChanged("ALUOperationControlLine");
            OnPropertyChanged("DataMemoryControlLine");
            OnPropertyChanged("BranchControlLine");
            OnPropertyChanged("JumpControlLine");
            OnPropertyChanged("AluSourceControlLine");
            OnPropertyChanged("RegFileWriteControlLine");

            OnPropertyChanged("BranchMux");
            OnPropertyChanged("JumpMux");
            OnPropertyChanged("JumpLine");
            OnPropertyChanged("ProgramCounterLine");
            OnPropertyChanged("ProgramCounterOrRegisterFileInputLine");
            OnPropertyChanged("JumpRegisterLine");
            OnPropertyChanged("JumpRegisterAluRead1Line");
            OnPropertyChanged("BranchLine");
            OnPropertyChanged("ImmediateOrBranchLine");
            OnPropertyChanged("BranchMuxLine");
            OnPropertyChanged("JumpMuxLine");
            OnPropertyChanged("InstructionMemoryLine");
            OnPropertyChanged("RegisterFileRsLine");
            OnPropertyChanged("RegisterFileRdLine");
            OnPropertyChanged("RegisterFileRtLine");
            OnPropertyChanged("RegisterFileRsRdLine");
            OnPropertyChanged("RegisterFileRsRdRtLine");
            OnPropertyChanged("JumpBranchLine");
            OnPropertyChanged("ImmediateLine");
            OnPropertyChanged("AluRead1Line");
            OnPropertyChanged("AluRead2Line");
            OnPropertyChanged("RegisterFileRtOutLine");
            OnPropertyChanged("AluSourceMux");
            OnPropertyChanged("DataMemoryMux");
            OnPropertyChanged("DataMemoryOutLine");
            OnPropertyChanged("AluResultLine");
            OnPropertyChanged("WriteAluResultLine");
            OnPropertyChanged("WritePcToRegisterLine");
            OnPropertyChanged("DataMemoryAddressLine");
            OnPropertyChanged("RegisterFileWriteBackLine");
        }

        private void fillExecutedInstructionGroupBox()
        {
            executedInstructionName = controlUnit.GetInstructionAssemblerName + ": " + controlUnit.GetInstructionFriendlyName;
            executedInstructionExample = controlUnit.GetInstructionExample;
            executedInstructionMeaning = controlUnit.GetInstructionMeaning;
            executedInstructionFormat = controlUnit.GetInstructionFormat;
            executedInstructionFunction = controlUnit.GetInstructionFunction;
            executedInstructionOpCode = controlUnit.GetInstructionOpCode;
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
        public SolidColorBrush JumpLine
        {
            set { jumpLine = value; OnPropertyChanged("JumpLine"); }
            get { return jumpLine; }
        }

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

        public SolidColorBrush BranchLine
        {
            set { branchLine = value; OnPropertyChanged("BranchLine"); }
            get { return branchLine; }
        }

        public SolidColorBrush ImmediateOrBranchLine
        {
            set { immediateOrBranchLine = value; OnPropertyChanged("ImmediateOrBranchLine"); }
            get { return immediateOrBranchLine; }
        }

        public SolidColorBrush BranchMuxLine
        {
            set { branchMuxLine = value; OnPropertyChanged("BranchMuxLine"); }
            get { return branchMuxLine; }
        }

        public SolidColorBrush JumpMuxLine
        {
            set { jumpMuxLine = value; OnPropertyChanged("JumpMuxLine"); }
            get { return jumpMuxLine; }
        }

        public SolidColorBrush InstructionMemoryLine
        {
            set { instructionMemoryLine = value; OnPropertyChanged("InstructionMemoryLine"); }
            get { return instructionMemoryLine; }
        }

        public SolidColorBrush RegisterFileRsLine
        {
            set { registerFileRsLine = value; OnPropertyChanged("RegisterFileRsLine"); }
            get { return registerFileRsLine; }
        }

        public SolidColorBrush RegisterFileRdLine
        {
            set { registerFileRdLine = value; OnPropertyChanged("RegisterFileRdLine"); }
            get { return registerFileRdLine; }
        }

        public SolidColorBrush RegisterFileRtLine
        {
            set { registerFileRtLine = value; OnPropertyChanged("RegisterFileRtLine"); }
            get { return registerFileRtLine; }
        }

        public SolidColorBrush RegisterFileRsRdLine
        {
            set { registerFileRsRdLine = value; OnPropertyChanged("RegisterFileRsRdLine"); }
            get { return registerFileRsRdLine; }
        }

        public SolidColorBrush RegisterFileRsRdRtLine
        {
            set { registerFileRsRdRtLine = value; OnPropertyChanged("RegisterFileRsRdRtLine"); }
            get { return registerFileRsRdRtLine; }
        }

        public SolidColorBrush JumpBranchLine
        {
            set { jumpBranchLine = value; OnPropertyChanged("JumpBranchLine"); }
            get { return jumpBranchLine; }
        }

        public SolidColorBrush ImmediateLine
        {
            set { immediateLine = value; OnPropertyChanged("ImmediateLine"); }
            get { return immediateLine; }
        }

        public SolidColorBrush AluRead1Line
        {
            set { aluRead1Line = value; OnPropertyChanged("AluRead1Line"); }
            get { return aluRead1Line; }
        }

        public SolidColorBrush AluRead2Line
        {
            set { aluRead2Line = value; OnPropertyChanged("AluRead2Line"); }
            get { return aluRead2Line; }
        }

        public SolidColorBrush RegisterFileRtOutLine
        {
            set { registerFileRtOutLine = value; OnPropertyChanged("RegisterFileRtOutLine"); }
            get { return registerFileRtOutLine; }
        }

        public SolidColorBrush DataMemoryOutLine
        {
            set { dataMemoryOutLine = value; OnPropertyChanged("DataMemoryOutLine"); }
            get { return dataMemoryOutLine; }
        }    

        public SolidColorBrush AluResultLine
        {
            set { aluResultLine = value; OnPropertyChanged("AluResultLine"); }
            get { return aluResultLine; }
        }

        public SolidColorBrush WriteAluResultLine
        {
            set { writeAluResultLine = value; OnPropertyChanged("WriteAluResultLine"); }
            get { return writeAluResultLine; }
        }

        public SolidColorBrush WritePcToRegisterLine
        {
            set { writePcToRegisterLine = value; OnPropertyChanged("WritePcToRegisterLine"); }
            get { return writePcToRegisterLine; }
        }

        public SolidColorBrush DataMemoryAddressLine
        {
            set { dataMemoryAddressLine = value; OnPropertyChanged("DataMemoryAddressLine"); }
            get { return dataMemoryAddressLine; }
        }

        public SolidColorBrush RegisterFileWriteBackLine
        {
            set { registerFileWriteBackLine = value; OnPropertyChanged("RegisterFileWriteBackLine"); }
            get { return registerFileWriteBackLine; }
        }

        /* Muxes */
        public SolidColorBrush BranchMux
        {
            set { branchMux = value; OnPropertyChanged("BranchMux"); }
            get { return branchMux; }
        }

        public SolidColorBrush JumpMux
        {
            set { jumpMux = value; OnPropertyChanged("JumpMux"); }
            get { return jumpMux; }
        }

        public SolidColorBrush AluSourceMux
        {
            set { aluSourceMux = value; OnPropertyChanged("AluSourceMux"); }
            get { return aluSourceMux; }
        }

        public SolidColorBrush DataMemoryMux
        {
            set { dataMemoryMux = value; OnPropertyChanged("DataMemoryMux"); }
            get { return dataMemoryMux; }
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
