using System;
using System.Windows;

namespace ABPHelper.Helper
{
    public abstract class HelperBase
    {
        protected readonly IServiceProvider ServiceProvider;

        protected HelperBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public abstract void Execute();

        protected MessageBoxResult MessageBox(string message, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information)
        {
            return System.Windows.MessageBox.Show(message, "ABPHelper", button, icon);
        }
    }
}