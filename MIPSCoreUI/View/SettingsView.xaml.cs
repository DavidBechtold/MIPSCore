using System.Windows;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.SettingsViewModel;
        }
    }
}
