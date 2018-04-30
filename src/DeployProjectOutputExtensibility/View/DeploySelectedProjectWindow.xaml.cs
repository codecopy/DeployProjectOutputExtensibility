using Microsoft.VisualStudio.Shell;
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
    public partial class DeploySelectedProjectWindow : System.Windows.Window
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
        private TargetInfo configProject;

        /// <summary>
        /// The is project existing
        /// </summary>
        public bool IsNewProject { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider serviceProvider => this.package;

        /// <summary>
        /// Gets or sets the identifying name of the element. The name provides a reference so that code-behind, such as event handler code, can refer to a markup element after it is constructed during processing by a XAML processor.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>
        /// The output.
        /// </value>
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets the project information.
        /// </summary>
        /// <value>
        /// The project information.
        /// </value>
        public ProjectInfoView ProjectView { get; set; }

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
        /// Initializes a new instance of the <see cref="DeploySelectedProjectWindow"/> class.
        /// </summary>
        public DeploySelectedProjectWindow()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploySelectedProjectWindow"/> class.
        /// </summary>
        /// <param name="inPackage">The in package.</param>
        public DeploySelectedProjectWindow(Package inPackage) : this()
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

            this.ProjectName = this.currentProject.Name;
            this.Output = this.GetProjectOutput();

            this.configProject = this.userMetadata?[this.ProjectName];
            if (this.configProject == null)
            {
                this.IsNewProject = true;
                this.configProject = new TargetInfo
                {
                    Name = this.ProjectName,
                    TargetDir = string.Empty
                };
                this.configProject.UpdateData(this.userMetadata);
            }

            this.ProjectView = new ProjectInfoView(this.configProject);
        }

        /// <summary>
        /// Gets the project output.
        /// </summary>
        /// <returns></returns>
        private string GetProjectOutput()
        {
            var binFolder = string.Empty;
            try
            {
                if (this.currentProject != null && !string.IsNullOrEmpty(this.currentProject.FullName))
                {
                    string fullPath = this.currentProject.Properties.Item("FullPath").Value.ToString();
                    string outputPath = this.currentProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
                    string outputFileName = this.currentProject.Properties.Item("OutputFileName").Value.ToString();


                    string outputDir = Path.Combine(fullPath, outputPath);
                    var folderObj = new DirectoryInfo(outputDir);
                    binFolder = Path.Combine(folderObj.FullName, outputFileName);
                }
            }
            catch (Exception ex)
            {
                this.SetError(ex.Message);
            }
            return binFolder;
        }

        /// <summary>
        /// Deploys the manual on click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void DeployManualOnClick(object sender, RoutedEventArgs e)
        {
            this.ClearErrorAndLog();
            if (this.currentProject == null)
            {
                this.SetError("There is no selected project.");
                return;
            }

            if (File.Exists(this.Output))
            {
                var fileName = Path.GetFileName(this.Output);
                var destDir = this.configProject.AbsoluteDir;
                var destFilePath = Path.Combine(destDir, fileName);

                try
                {
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    File.Copy(this.Output, destFilePath, this.configProject.OverrideMode);
                    this.Log($"Deploy {this.ProjectName} successfully.");
                }
                catch (Exception ex)
                {
                    this.SetError(ex.Message);
                }
            }
            else
            {
                this.Log($"The file is not found.");
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
                this.userMetadata.Targets.Add(this.configProject);
            }

            try
            {
                ConfigurationProvider.Instance.Save();
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
