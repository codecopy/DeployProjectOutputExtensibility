using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TP.AutoDeploy.Configuration;
using TP.AutoDeploy.Extension;
using TP.AutoDeploy.Models;

namespace TP.AutoDeploy.View
{
    /// <summary>
    /// Interaction logic for ProjectInfoView.xaml
    /// </summary>
    
    public partial class ProjectDeploymentView : UserControl
    {
        /// <summary>
        /// Gets or sets the project information.
        /// </summary>
        /// <value>
        /// The project information.
        /// </value>
        public TargetInfo ProjectInfo { get; set; }

        /// <summary>
        /// Gets or sets the recent dirs.
        /// </summary>
        /// <value>
        /// The recent dirs.
        /// </value>
        public ObservableCollection<string> RecentDirs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDeploymentView"/> class.
        /// </summary>
        public ProjectDeploymentView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDeploymentView" /> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public ProjectDeploymentView(TargetInfo project) : this()
        {
            this.ProjectInfo = project;
            var userMetadata = ConfigurationProvider.Instance.UserMetadata;
            var recentLocations = userMetadata.AnonymousData.GetRecentLocations(project.Name);
            this.RecentDirs = new ObservableCollection<string>(recentLocations);
        }
    }
}
