using PropertyChanged;
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
    /// Interaction logic for ProjectInfo.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class ProjectInfoView : UserControl
    {
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
        /// The project info
        /// </summary>
        public ProjectInfo Project { get; }

        public ProjectInfoView()
        {
            InitializeComponent();
        }

        public ProjectInfoView(ProjectInfo project) : this()
        {
            this.DataContext = this;
            this.Project = project;
            this.ProjectName = project.ProjectName;
            this.Output = project.Output;
        }
    }
}
