using System;
using System.Windows;
using MIPSCoreUI.Bootstrapper;
using System.Windows.Media;
using System.Windows.Documents;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für MIPSMemory.xaml
    /// </summary>
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
        }

        private void AddTextHighlightedToInstructionMemory(string text, bool highlight, bool clear)
        {
            if (text == null) throw new ArgumentNullException("text");

            if (clear)
                InstructionMemory.Text = "";

            if (highlight)
            {
                runBackgroundYellow.Text = text;
                InstructionMemory.Inlines.Add(runBackgroundYellow);
            }
            else
                InstructionMemory.Inlines.Add(text);
        }
    }
}
