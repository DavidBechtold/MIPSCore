using System;
using System.Windows;

namespace MIPSCoreUI.Services
{
    public interface IMessageBoxService
    {
        void ShowNotification(string message);
        bool AskForConfirmationYesno(string message, string caption);
        MessageBoxResult AskForConfirmationYesnocancel(string message, string caption);
    }
}
