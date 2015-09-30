using System.Windows;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    public partial class LedsArray
    {
        public LedsArray()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.LedsViewModel;
        }
    }
}
