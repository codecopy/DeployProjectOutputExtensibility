﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using TP.AutoDeploy.View;

namespace TP.AutoDeploy
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "3.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionPage), "Deploy Definition", "General", 113, 114, true)]
    [Guid(DeploySelectedProjectPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class DeploySelectedProjectPackage : Package
    {
        /// <summary>
        /// Deploy.SelectedProjectPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "770c6009-b7ea-4235-b693-bff1afb6063d";

        /// <summary>
        /// The installation path
        /// </summary>
        private string installationPath;

        /// <summary>
        /// The reference assemblies
        /// </summary>
        private List<string> refAssemblies = new List<string>()
        {
            "DotNetKit.Wpf.AutoCompleteComboBox"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Deploy.SelectedProject"/> class.
        /// </summary>
        public DeploySelectedProjectPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            this.installationPath = Path.GetDirectoryName(this.GetType().Assembly.Location);
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        /// <summary>
        /// Called when [assembly resolve].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var requestAss = args.Name.Split(',')[0];
            var isRefAss = this.refAssemblies.Any(ass => string.Equals(ass, requestAss));
            if (isRefAss)
            {
                try
                {
                    return Assembly.LoadFrom(Path.Combine(this.installationPath, $"{requestAss}.dll"));
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            VSContext.ServiceProvider = this;
            DeploySelectedProject.Initialize(this);
            base.Initialize();
        }

        #endregion
    }
}
