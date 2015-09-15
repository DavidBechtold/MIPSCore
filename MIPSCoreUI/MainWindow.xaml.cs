using MIPSCoreUI.Bootstrapper;
using MIPSCoreUI.View;
using MIPSCoreUI.ViewModel;
using System.Windows;
namespace MIPSCoreUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var boot = new CBootstrapper();
            this.DataContext = boot.MainWindowViewModel;
        }
    }
}
