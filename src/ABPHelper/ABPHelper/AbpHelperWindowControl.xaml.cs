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
        public static IServiceProvider ServiceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpHelperWindowControl"/> class.
        /// </summary>
        public AbpHelperWindowControl(IServiceProvider serviceProvider)
        {
            this.InitializeComponent();
            ServiceProvider = serviceProvider;
        }

        private void Generate_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}