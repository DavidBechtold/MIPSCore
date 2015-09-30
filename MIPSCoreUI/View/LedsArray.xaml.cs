using System.Windows;
using System.Windows.Controls;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für LedArray.xaml
    /// </summary>
    public partial class LedsArray : UserControl
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
