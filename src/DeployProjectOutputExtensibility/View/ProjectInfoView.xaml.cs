using System;
using System.Collections.Generic;
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
using TP.AutoDeploy.Models;

namespace TP.AutoDeploy.View
{
    /// <summary>
    /// Interaction logic for ProjectInfoView.xaml
    /// </summary>
    
    public partial class ProjectInfoView : UserControl
    {
        /// <summary>
        /// Gets or sets the project information.
        /// </summary>
        /// <value>
        /// The project information.
        /// </value>
        public TargetInfo ProjectInfo { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInfoView"/> class.
        /// </summary>
        public ProjectInfoView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectInfoView" /> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public ProjectInfoView(TargetInfo project) : this()
        {
            this.ProjectInfo = project;
        }
    }
}
