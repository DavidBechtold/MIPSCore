using System;
using System.Windows;
using MIPSCoreUI.Bootstrapper;
using System.Windows.Media;
using System.Windows.Documents;

namespace MIPSCoreUI.View
{
    public enum HighlightAction { Clear, AddNormal, AddHighlighted }
    public partial class MipsMemory
    {
        private readonly Run runBackgroundYellow;
        public MipsMemory()
        {
            InitializeComponent();
            runBackgroundYellow = new Run {Background = Brushes.Yellow};
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.MipsMemoryViewModel;
            CBootstrapper.AddHighlightedTextToInstructionMemory = AddTextHighlightedToInstructionMemory;    //TODO make this mvvm
            CBootstrapper.InstructionMemoryText = InstructionMemoryText;
        }

        private void AddTextHighlightedToInstructionMemory(string text, HighlightAction action)
        {
            if (text == null) throw new ArgumentNullException("text");

            switch (action)
            {
                case HighlightAction.Clear: 
                    InstructionMemory.Text = "";
                    break;
                case HighlightAction.AddNormal: 
                    InstructionMemory.Inlines.Add(text);
                    break;
                case HighlightAction.AddHighlighted:
                    runBackgroundYellow.Text = text;
                    InstructionMemory.Inlines.Add(runBackgroundYellow);
                    break;
            }
        }

        public string InstructionMemoryText()
        {
            return InstructionMemory.Text;
        }
    }
}
