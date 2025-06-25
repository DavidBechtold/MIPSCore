using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für VersionView.xaml
    /// </summary>
    public partial class VersionView : Window
    {
        public VersionView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           Label_VersionView.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
