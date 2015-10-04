using System.Windows;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für MIPSRegister.xaml
    /// </summary>
    public partial class RegisterView
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.MipsRegisterViewModel;
        }
    }
}
