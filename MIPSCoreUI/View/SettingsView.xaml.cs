﻿using System.Windows;
using MIPSCoreUI.Bootstrapper;
using MIPSCoreUI.ViewModel;

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

            DataContext = new SettingsViewModel(CBootstrapper.Core);
        }
    }
}
