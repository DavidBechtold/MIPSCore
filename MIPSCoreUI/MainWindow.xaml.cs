using MIPSCoreUI.Bootstrapper;
using System.Windows;

namespace MIPSCoreUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CBootstrapper.Init();
            DataContext = CBootstrapper.MainWindowViewModel;
        }
    }
}
