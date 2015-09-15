using System.Windows;
using System.Windows.Controls;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für MIPSRegister.xaml
    /// </summary>
    public partial class MIPSRegister : UserControl
    {
        public MIPSRegister()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = CBootstrapper.MipsRegisterViewModel;
        }
    }
}
