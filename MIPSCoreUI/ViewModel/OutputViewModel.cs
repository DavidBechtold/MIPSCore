using System.Windows.Media;
using Microsoft.Practices.Prism.ViewModel;

namespace MIPSCoreUI.ViewModel
{
    class OutputViewModel : NotificationObject, IMessageableViewModel
    {
        private string outputText;
        private SolidColorBrush outputTextColor;

        public OutputViewModel()
        {
            outputText = "";
            outputTextColor = new SolidColorBrush(Colors.Black);
        }

        public void Refresh()
        {
            RaisePropertyChanged(() => OutputText);
        }

        public void Draw()
        {
            Refresh();
        }

        public void ErrorMessage(string message)
        {
            OutputTextColor = new SolidColorBrush(Colors.Red);
            OutputText = message;
        }

        public void NotificationMessage(string message)
        {
            outputTextColor = new SolidColorBrush(Colors.Black);
            outputText += message;
        }

        public string OutputText
        {
            set { outputText = value; RaisePropertyChanged(() => OutputText); }
            get { return outputText; }
        }

        public SolidColorBrush OutputTextColor
        {
            set { outputTextColor = value; RaisePropertyChanged(() => OutputTextColor); }
            get { return outputTextColor; }
        }
    }
}
