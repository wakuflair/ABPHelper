using System;
using System.Collections.Generic;
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

        public abstract bool CanExecute(IDictionary<string, object> parameter);
        public abstract void Execute(IDictionary<string, object> parameter);

        public static MessageBoxResult MessageBox(string message, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.Information, params object[] parameters)
        {
            return System.Windows.MessageBox.Show(string.Format(message, parameters), "ABPHelper", button, icon);
        }
    }
}