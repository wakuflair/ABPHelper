//------------------------------------------------------------------------------
// <copyright file="AbpHelperWindowControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ABPHelper.Helper;
using Microsoft.VisualStudio.Shell;

namespace ABPHelper
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AbpHelperWindowControl.
    /// </summary>
    public partial class AbpHelperWindowControl : UserControl
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpHelperWindowControl"/> class.
        /// </summary>
        public AbpHelperWindowControl(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        private void Generate_OnClick(object sender, RoutedEventArgs e)
        {
            var helper = new AddNewServiceMethodHelper(_serviceProvider);
            var parameter = new Dictionary<string, object>();
            var names = Regex.Split(txtNames.Text, @"\r\n");

            if (HelperBase.MessageBox("{0} method{1} will be generated. OK?", MessageBoxButton.OKCancel, MessageBoxImage.Question, names.Length, (names.Length > 1 ? "s" : string.Empty)) == MessageBoxResult.Cancel)
            {
                return;
            }
            parameter["names"] = names;
            parameter["async"] = chkAsync.IsChecked;

            if (helper.CanExecute(parameter))
            {
                helper.Execute(parameter);
            }
        }
    }
}