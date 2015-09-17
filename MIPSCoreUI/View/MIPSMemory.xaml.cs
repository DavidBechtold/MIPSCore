using System;
using System.Windows;
using System.Windows.Controls;
using MIPSCoreUI.Bootstrapper;
using System.Windows.Media;
using System.Windows.Documents;



namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für MIPSMemory.xaml
    /// </summary>
    public partial class MIPSMemory : UserControl
    {
        private Run runBackgroundYellow;
        private Run runBackgroundWhite;
        public MIPSMemory()
        {
            InitializeComponent();
            runBackgroundYellow = new Run();
            runBackgroundWhite = new Run();
            runBackgroundYellow.Background = Brushes.Yellow;
            runBackgroundWhite.Background = Brushes.White; 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = CBootstrapper.MipsMemoryViewModel;
            CBootstrapper.AddHighlightedTextToInstructionMemory = addTextHighlightedToInstructionMemory;    //TODO make this mvvm
        }

        private void addTextHighlightedToInstructionMemory(string text, bool highlight, bool clear)
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
            {
                InstructionMemory.Inlines.Add(text);
            }
        }
    }
}
