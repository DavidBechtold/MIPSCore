using MIPSCoreUI.Bootstrapper;
using MIPSCoreUI.View;
using MIPSCoreUI.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;
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
            CBootstrapper.Init();
            CBootstrapper.Redraw = redraw;
            this.DataContext = CBootstrapper.MainWindowViewModel;
        }

        private void redraw()
        {
            this.Refresh();
        }
    }

    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
