using MIPSCoreUI.Bootstrapper;
using System.Windows;
using System.Windows.Input;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class MipsCore
    {
        public MipsCore()
        {
            InitializeComponent();
        }

        private void InstructionMemory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.MipsCoreViewModel;
        }
    }
}
