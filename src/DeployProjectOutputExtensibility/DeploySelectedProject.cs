using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TP.AutoDeploy.Helper;
using TP.AutoDeploy.Manager;
using TP.AutoDeploy.View;

namespace TP.AutoDeploy
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DeploySelectedProject
    {
        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4d4b75df-8678-4ffa-898d-8556ec90ddcf");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deploy.SelectedProject"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private DeploySelectedProject(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            var commandService = this.serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, 0x0100);
                var command = new OleMenuCommand(this.OnDeploySelectedProject, menuCommandID);
                commandService.AddCommand(command);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DeploySelectedProject Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider serviceProvider => VSContext.ServiceProvider;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new DeploySelectedProject(package);
            SolutionManager.Instance.Initialize(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void OnDeploySelectedProject(object sender, EventArgs e)
        {
            if (this.CheckCondition())
            {
                var view = new DeploySelectedProjectWindow(this.package);
                view.LoadData();
                view.ShowDialog();
            }
        }

        /// <summary>
        /// Checks the condition.
        /// </summary>
        /// <returns></returns>
        private bool CheckCondition()
        {
            var currentProject = SolutionManager.Instance.GetActivatedProject();
            if (currentProject == null)
            {
                VSUIHelper.ShowMessageBox("There is no selected project.", OLEMSGICON.OLEMSGICON_WARNING);
                return false;
            }

            return true;
        }
    }
}
