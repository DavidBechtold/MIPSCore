using MIPSCoreUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MIPSCoreUI.View
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    public partial class MIPSCore : UserControl
    {
        public MIPSCore()
        {
            InitializeComponent();
        }

        private void InstructionMemory_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string test = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
