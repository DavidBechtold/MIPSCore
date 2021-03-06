﻿using System.Windows;
using MIPSCoreUI.Bootstrapper;

namespace MIPSCoreUI.View
{
    public enum HighlightAction { Clear, AddNormal, AddHighlighted }
    public partial class MemoryView
    {
        public MemoryView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = CBootstrapper.MipsMemoryViewModel;
        }
    }
}
