//------------------------------------------------------------------------------
// <copyright file="AbpHelperWindow.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using EnvDTE;
using EnvDTE80;

namespace ABPHelper
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("cff10a46-bdbf-4562-9002-04e671268596")]
    public sealed class AbpHelperWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbpHelperWindow"/> class.
        /// </summary>
        public AbpHelperWindow() : base(null)
        {
            this.Caption = "AbpHelper";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new AbpHelperWindowControl(this);
        }
    }
}
