//------------------------------------------------------------------------------
// <copyright file="AbpHelperWindowControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


using System;
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

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var helper = new AddNewServiceMethodHelper(_serviceProvider);
            helper.Execute();
        }
    }
}