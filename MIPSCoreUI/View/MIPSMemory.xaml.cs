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
        }
    }
}
