﻿using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using System.Windows.Controls;
using EnvDTE80;
using System.IO;
using TP.AutoDeploy.Configuration;
using TP.AutoDeploy.Models;
using Microsoft.VisualStudio;
using PropertyChanged;
using TP.AutoDeploy.Manager;
using TP.AutoDeploy.Extension;

namespace TP.AutoDeploy.View
{
    /// <summary>
    /// Interaction logic for DeploySelectedProject.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class DeploySingleProjectView : System.Windows.Window
    {
        #region Variables

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// The current project
        /// </summary>
        private Project currentProject;

        /// <summary>
        /// The user metadata
        /// </summary>
        private UserMetadata userMetadata;

        /// <summary>
        /// The configuration project
        /// </summary>
        private TargetInfo deployTarget;

        /// <summary>
        /// The is project existing
        /// </summary>
        public bool IsNewProject { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the deploy target information.
        /// </summary>
        /// <value>
        /// The deploy target information.
        /// </value>
        public TargetInfo DeployTargetInfo => this.deployTarget;

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider serviceProvider => this.package;

        /// <summary>
        /// Gets or sets the project information.
        /// </summary>
        /// <value>
        /// The project information.
        /// </value>
        public ProjectDeploymentView ProjectDeployment { get; set; }

        /// <summary>
        /// Selected project info
        /// </summary>
        public ProjectInfoView ProjectInfo { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has error; otherwise, <c>false</c>.
        /// </value>
        public bool HasError { get; set; } = false;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploySingleProjectView"/> class.
        /// </summary>
        public DeploySingleProjectView()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploySingleProjectView"/> class.
        /// </summary>
        /// <param name="inPackage">The in package.</param>
        public DeploySingleProjectView(Package inPackage) : this()
        {
            this.package = inPackage;
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public void LoadData()
        {
            this.currentProject = SolutionManager.Instance.GetActivatedProject();
            this.userMetadata = ConfigurationProvider.Instance.UserMetadata;

            var projectInfo = new ProjectInfo(this.currentProject);
            this.ProjectInfo = new ProjectInfoView(projectInfo);

            this.deployTarget = this.userMetadata?[this.ProjectInfo.ProjectName];
            if (this.deployTarget == null)
            {
                this.IsNewProject = true;
                this.deployTarget = new TargetInfo
                {
                    Name = this.ProjectInfo.ProjectName,
                    TargetDir = string.Empty
                };
                this.deployTarget.UpdateData(this.userMetadata);
            }

            this.ProjectDeployment = new ProjectDeploymentView(this.deployTarget);
        }

        /// <summary>
        /// Deploys the manual on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DeployManualOnClick(object sender, RoutedEventArgs e)
        {
            this.ClearErrorAndLog();

            string message;
            if (this.ProjectInfo.Project.Deploy(this.deployTarget, out message))
            {
                this.Log(message);
            }
            else
            {
                this.SetError(message);
            }
        }

        /// <summary>
        /// Saves the on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveOnClick(object sender, RoutedEventArgs e)
        {
            this.ClearErrorAndLog();

            if (this.IsNewProject)
            {
                this.userMetadata.Targets.Add(this.deployTarget);
            }

            try
            {
                ConfigurationProvider.Instance.Save();
                this.IsNewProject = false;
            }
            catch (Exception ex)
            {
                this.SetError(ex.Message);
            }
        }

        /// <summary>
        /// Clears the error.
        /// </summary>
        private void ClearErrorAndLog()
        {
            this.Error = string.Empty;
            this.HasError = false;
        }

        /// <summary>
        /// Sets the error.
        /// </summary>
        /// <param name="message">The message.</param>
        private void SetError(string message)
        {
            this.Error = message;
            this.HasError = true;
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void Log(string message)
        {
            this.Error = message;
            this.HasError = false;
        }
    }
}
