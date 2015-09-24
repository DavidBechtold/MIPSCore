﻿using MIPSCoreUI.Bootstrapper;
using System;
using System.Windows;
using System.Windows.Threading;
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
            CBootstrapper.Redraw = Redraw;
            DataContext = CBootstrapper.MainWindowViewModel;
        }

        private void Redraw()
        {
            this.Refresh();
        }
    }

    public static class ExtensionMethods
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
