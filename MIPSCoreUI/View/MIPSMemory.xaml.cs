using System.Windows;
using System.Windows.Controls;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für MIPSMemory.xaml
    /// </summary>
    public partial class MIPSMemory : UserControl
    {
        public MIPSMemory()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = CBootstrapper.MipsMemoryViewModel;
        }
    }
}
