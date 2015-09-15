using System;
using System.Windows;

namespace MIPSCoreUI.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public void ShowNotification(string message)
        {
            MessageBox.Show(message, "Notification", MessageBoxButton.OK);
        }

        public bool AskForConfirmationYesno(string message, string caption)
        {
            var result = MessageBox.Show(message, caption , MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes;
        }

        public MessageBoxResult AskForConfirmationYesnocancel(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel);
        }
    }
}