using System.Windows;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    public partial class LedsArrayView
    {
        public LedsArrayView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.LedsViewModel;
        }
    }
}
