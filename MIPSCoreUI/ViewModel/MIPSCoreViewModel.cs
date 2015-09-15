using System;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using MIPSCore.ControlUnit;
using MIPSCore.InstructionSet;

namespace MIPSCoreUI.ViewModel
{
    public class MIPSCoreViewModel : NotificationObject, IMIPSViewModel
    {
        private CControlUnit controlUnit;
        private Dispatcher dispatcher;
 
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

        private double adderWidth = 30;
        private double adderHeight = 30;
        private double rectangleHeight = 140;
        private double rectangleWidth = 100;
        private double rectangleSpaceBetween = 50;

        public MIPSCoreViewModel(CControlUnit controlUnit, Dispatcher dispatcher)
        {
            if (controlUnit == null) throw new ArgumentNullException("controlUnit");
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            this.controlUnit = controlUnit;
            this.dispatcher = dispatcher;

            JumpLine = JumpRegisterLine = JumpRegisterAluRead1Line = ProgramCounterLine = 
                BranchLine = ImmediateOrBranchLine = InstructionMemoryLine =
                ProgramCounterOrRegisterFileInputLine = lineInactive;
            RegisterFileRsLine = JumpBranchLine = ImmediateLine = lineInactive;
        
            DataMemoryAddressLine = WritePcToRegisterLine = WriteAluResultLine = AluResultLine = DataMemoryOutLine = DataMemoryMux = AluSourceMux = RegisterFileRtOutLine = BranchMux = JumpMux = BranchMuxLine = JumpMuxLine = AluRead1Line = AluRead2Line = lineInactive;

            RegisterFileWriteBackLine = BranchControlLine = JumpControlLine = ALUOperationControlLine = DataMemoryControlLine = ALUSourceControlLine = RegFileWriteControlLine = RegFileInputControlLine = controlLineInactive;
        }

        public void refresh()
        {
            /* invoke the wpf thread */
            dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                refreshGUI();
            }));
        }

        private void refreshGUI()
        {
            string test;
            if (controlUnit.GetInstructionAssemblerName == "jr")
                test = "";
            ALUOperationControlLine = controlLineInactive;
            BranchControlLine = controlLineInactive;
            JumpControlLine = controlLineInactive;
            ALUSourceControlLine = controlLineInactive;
            RegFileWriteControlLine = controlLineInactive;
            DataMemoryControlLine = controlLineInactive;

            AluSourceMux = lineInactive;
            JumpMux = lineActive;
            JumpMuxLine = lineActive;
            AluSourceMux = lineActive;
            InstructionMemoryLine = lineActive;
            DataMemoryMux = lineInactive;

            JumpLine = lineInactive;
            BranchLine = lineInactive;
            ImmediateOrBranchLine = lineInactive;
            BranchMux = lineInactive;
            BranchMuxLine = lineInactive;
            ProgramCounterLine = lineInactive;
            ProgramCounterOrRegisterFileInputLine = lineInactive;
            JumpRegisterLine = lineInactive;
            JumpRegisterAluRead1Line = lineInactive;
            JumpBranchLine = lineInactive;
            RegisterFileRsLine = lineInactive;
            RegisterFileRdLine = lineInactive;
            RegisterFileRtLine = lineInactive; 
            RegisterFileRsRdLine = lineInactive;
            RegisterFileRsRdRtLine= lineInactive;
            ImmediateLine = lineInactive;
            AluRead1Line = lineInactive;
            AluRead2Line = lineInactive;
            RegisterFileRtOutLine = lineInactive;
            DataMemoryOutLine = lineInactive;
            AluResultLine = lineInactive;
            WriteAluResultLine = lineInactive;
            WritePcToRegisterLine = lineInactive;
            DataMemoryAddressLine = lineInactive;
            RegisterFileWriteBackLine = lineInactive;

            /* Control Lines */
            if (controlUnit.getAluControl != ALUControl.stall)
            {
                ALUOperationControlLine = controlLineActive;
                AluRead1Line = lineActive;
                AluRead2Line = lineActive;
                JumpRegisterAluRead1Line = lineActive;
            }

            if (controlUnit.getRegWrite)
            {
                RegFileWriteControlLine = controlLineActive;
                DataMemoryMux = lineActive;
                AluResultLine = lineActive;
                RegisterFileWriteBackLine = lineActive;

                if (controlUnit.getRegisterFileInput == RegisterFileInput.aluLO)
                {
                    WriteAluResultLine = lineActive;
                    
                }
                else if (controlUnit.getRegisterFileInput == RegisterFileInput.programCounter)
                    WritePcToRegisterLine = lineActive;
                else if (controlUnit.getRegisterFileInput == RegisterFileInput.dataMemory)
                {
                    DataMemoryAddressLine = lineActive;
                    DataMemoryOutLine = lineActive;
                }
            }

            /* Memory Lines */
            if (controlUnit.getMemRead == true || controlUnit.getMemWrite == true)
            {
                DataMemoryControlLine = controlLineActive;
                DataMemoryAddressLine = lineActive;
                AluResultLine = lineActive;
            }
            else if (controlUnit.getMemRead)
            {
                DataMemoryMux = lineActive;
                DataMemoryOutLine = lineActive;
            }

            /* Branch Lines */
            if (controlUnit.getPcSource == ProgramCounterSource.programCounter)
            {
                BranchMux = lineActive;
                BranchMuxLine = lineActive;
                ProgramCounterLine = lineActive;
                ProgramCounterOrRegisterFileInputLine = lineActive;
            }
            else if (controlUnit.getPcSource == ProgramCounterSource.jump)
            {
                JumpControlLine = controlLineActive;

                JumpLine = lineActive;
                RegisterFileRsRdLine = lineActive;
                RegisterFileRsRdRtLine= lineActive;
                JumpBranchLine= lineActive;
                AluSourceMux = lineInactive;

            }
            else if (controlUnit.getPcSource == ProgramCounterSource.register)
            {
                JumpControlLine = controlLineActive;

                JumpRegisterLine = lineActive;
                JumpRegisterAluRead1Line = lineActive;
                AluSourceMux = lineInactive;
            }
            else //branch
            {
                BranchControlLine = controlLineActive;

                BranchLine = lineActive;
                ImmediateOrBranchLine = lineActive;
                BranchMux = lineActive;
                BranchMuxLine = lineActive;
                ProgramCounterOrRegisterFileInputLine = lineActive;
                RegisterFileRsRdLine = lineActive;
                RegisterFileRsRdRtLine= lineActive;
                JumpBranchLine= lineActive;
                RegisterFileRtOutLine = lineActive;
            }

            /* instruction memory lines */
            if (controlUnit.getInstructionFormat == InstructionFormat.R)
            {
                RegisterFileRsLine = lineActive;
                RegisterFileRdLine = lineActive;
                RegisterFileRtLine = lineActive;
                RegisterFileRsRdLine = lineActive;
                RegisterFileRsRdRtLine = lineActive;
                if (JumpRegisterLine != lineActive)
                    RegisterFileRtOutLine = lineActive;
            }
            else if (controlUnit.getInstructionFormat == InstructionFormat.I && BranchLine == lineInactive)
            {
                RegisterFileRsLine = lineActive;
                RegisterFileRtLine = lineActive;
                RegisterFileRsRdLine = lineActive;
                RegisterFileRsRdRtLine = lineActive;
                JumpBranchLine = lineActive;
                ImmediateOrBranchLine = lineActive;
                ImmediateLine = lineActive;
                ALUSourceControlLine = controlLineActive;
            }
        }

        /* Lines */
        public SolidColorBrush JumpLine
        {
            set { jumpLine = value; RaisePropertyChanged(() => JumpLine); }
            get { return jumpLine; }
        }

        public SolidColorBrush JumpRegisterLine
        {
            set { jumpRegisterLine = value; RaisePropertyChanged(() => JumpRegisterLine); }
            get { return jumpRegisterLine; }
        }

        public SolidColorBrush JumpRegisterAluRead1Line
        {
            set { jumpRegisterAluRead1Line = value; RaisePropertyChanged(() => JumpRegisterAluRead1Line); }
            get { return jumpRegisterAluRead1Line; }
        }

        public SolidColorBrush ProgramCounterLine
        {
            set { programCounterLine = value; RaisePropertyChanged(() => ProgramCounterLine); }
            get { return programCounterLine; }
        }

        public SolidColorBrush ProgramCounterOrRegisterFileInputLine
        {
            set { programCounterOrRegisterFileInputLine = value; RaisePropertyChanged(() => ProgramCounterOrRegisterFileInputLine); }
            get { return programCounterOrRegisterFileInputLine; }
        }

        public SolidColorBrush BranchLine
        {
            set { branchLine = value; RaisePropertyChanged(() => BranchLine); }
            get { return branchLine; }
        }

        public SolidColorBrush ImmediateOrBranchLine
        {
            set { immediateOrBranchLine = value; RaisePropertyChanged(() => ImmediateOrBranchLine); }
            get { return immediateOrBranchLine; }
        }

        public SolidColorBrush BranchMuxLine
        {
            set { branchMuxLine = value; RaisePropertyChanged(() => BranchMuxLine); }
            get { return branchMuxLine; }
        }

        public SolidColorBrush JumpMuxLine
        {
            set { jumpMuxLine = value; RaisePropertyChanged(() => JumpMuxLine); }
            get { return jumpMuxLine; }
        }

        public SolidColorBrush InstructionMemoryLine
        {
            set { instructionMemoryLine = value; RaisePropertyChanged(() => InstructionMemoryLine); }
            get { return instructionMemoryLine; }
        }

        public SolidColorBrush RegisterFileRsLine
        {
            set { registerFileRsLine = value; RaisePropertyChanged(() => RegisterFileRsLine); }
            get { return registerFileRsLine; }
        }

        public SolidColorBrush RegisterFileRdLine
        {
            set { registerFileRdLine = value; RaisePropertyChanged(() => RegisterFileRdLine); }
            get { return registerFileRdLine; }
        }

        public SolidColorBrush RegisterFileRtLine
        {
            set { registerFileRtLine = value; RaisePropertyChanged(() => RegisterFileRtLine); }
            get { return registerFileRtLine; }
        }

        public SolidColorBrush RegisterFileRsRdLine
        {
            set { registerFileRsRdLine = value; RaisePropertyChanged(() => RegisterFileRsRdLine); }
            get { return registerFileRsRdLine; }
        }

        public SolidColorBrush RegisterFileRsRdRtLine
        {
            set { registerFileRsRdRtLine = value; RaisePropertyChanged(() => RegisterFileRsRdRtLine); }
            get { return registerFileRsRdRtLine; }
        }

        public SolidColorBrush JumpBranchLine
        {
            set { jumpBranchLine = value; RaisePropertyChanged(() => JumpBranchLine); }
            get { return jumpBranchLine; }
        }

        public SolidColorBrush ImmediateLine
        {
            set { immediateLine = value; RaisePropertyChanged(() => ImmediateLine); }
            get { return immediateLine; }
        }

        public SolidColorBrush AluRead1Line
        {
            set { aluRead1Line = value; RaisePropertyChanged(() => AluRead1Line); }
            get { return aluRead1Line; }
        }

        public SolidColorBrush AluRead2Line
        {
            set { aluRead2Line = value; RaisePropertyChanged(() => AluRead2Line); }
            get { return aluRead2Line; }
        }

        public SolidColorBrush RegisterFileRtOutLine
        {
            set { registerFileRtOutLine = value; RaisePropertyChanged(() => RegisterFileRtOutLine); }
            get { return registerFileRtOutLine; }
        }

        public SolidColorBrush DataMemoryOutLine
        {
            set { dataMemoryOutLine = value; RaisePropertyChanged(() => DataMemoryOutLine); }
            get { return dataMemoryOutLine; }
        }    

        public SolidColorBrush AluResultLine
        {
            set { aluResultLine = value; RaisePropertyChanged(() => AluResultLine); }
            get { return aluResultLine; }
        }

        public SolidColorBrush WriteAluResultLine
        {
            set { writeAluResultLine = value; RaisePropertyChanged(() => WriteAluResultLine); }
            get { return writeAluResultLine; }
        }

        public SolidColorBrush WritePcToRegisterLine
        {
            set { writePcToRegisterLine = value; RaisePropertyChanged(() => WritePcToRegisterLine); }
            get { return writePcToRegisterLine; }
        }

        public SolidColorBrush DataMemoryAddressLine
        {
            set { dataMemoryAddressLine = value; RaisePropertyChanged(() => DataMemoryAddressLine); }
            get { return dataMemoryAddressLine; }
        }

        public SolidColorBrush RegisterFileWriteBackLine
        {
            set { registerFileWriteBackLine = value; RaisePropertyChanged(() => RegisterFileWriteBackLine); }
            get { return registerFileWriteBackLine; }
        }

        /* Muxes */
        public SolidColorBrush BranchMux
        {
            set { branchMux = value; RaisePropertyChanged(() => BranchMux); }
            get { return branchMux; }
        }

        public SolidColorBrush JumpMux
        {
            set { jumpMux = value; RaisePropertyChanged(() => JumpMux); }
            get { return jumpMux; }
        }

        public SolidColorBrush AluSourceMux
        {
            set { aluSourceMux = value; RaisePropertyChanged(() => AluSourceMux); }
            get { return aluSourceMux; }
        }

        public SolidColorBrush DataMemoryMux
        {
            set { dataMemoryMux = value; RaisePropertyChanged(() => DataMemoryMux); }
            get { return dataMemoryMux; }
        }

        /* Control Lines */
        public SolidColorBrush BranchControlLine
        {
            set { branchControlLine = value; RaisePropertyChanged(() => BranchControlLine); }
            get { return branchControlLine; }
        }

        public SolidColorBrush JumpControlLine
        {
            set { jumpControlLine = value; RaisePropertyChanged(() => JumpControlLine); }
            get { return jumpControlLine; }
        }

        public SolidColorBrush ALUSourceControlLine
        {
            set { aluSourceControlLine = value; RaisePropertyChanged(() => ALUSourceControlLine); }
            get { return aluSourceControlLine; }
        }

        public SolidColorBrush ALUOperationControlLine
        {
            set { aluOperationControlLine = value; RaisePropertyChanged(() => ALUOperationControlLine); }
            get { return aluOperationControlLine; }
        }

        public SolidColorBrush DataMemoryControlLine
        {
            set { dataMemoryControlLine = value; RaisePropertyChanged(() => DataMemoryControlLine); }
            get { return dataMemoryControlLine; }
        }

        public SolidColorBrush RegFileInputControlLine
        {
            set { regFileInputControlLine = value; RaisePropertyChanged(() => RegFileInputControlLine); }
            get { return regFileInputControlLine; }
        }

        public SolidColorBrush RegFileWriteControlLine
        {
            set { regFileWriteControlLine = value; RaisePropertyChanged(() => RegFileWriteControlLine); }
            get { return regFileWriteControlLine; }
        }

        public double RectangleWidth
        {
            get { return rectangleWidth; }
            set { rectangleWidth = value; RaisePropertyChanged(() => RectangleWidth); }
        }

        public double RectangleHeight 
        {
            get {return rectangleHeight;}
            set { rectangleHeight = value; RaisePropertyChanged(() => RectangleHeight); }
        }
    }
}
