using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TP.AutoDeploy.Extension;

namespace TP.AutoDeploy.Models
{
    public class UserMetadata
    {
        /// <summary>
        /// Gets or sets the common target.
        /// </summary>
        /// <value>
        /// The common target.
        /// </value>
        [XmlArrayItem("TargetBase")]
        public List<TargetInfoBase> CommonTarget {get;set;}

        /// <summary>
        /// Gets or sets the projects.
        /// </summary>
        /// <value>
        /// The projects.
        /// </value>
        [XmlArrayItem("Target")]
        public List<TargetInfo> Targets { get; set; }

        /// <summary>
        /// Gets or sets the anonymous data.
        /// </summary>
        /// <value>
        /// The anonymous data.
        /// </value>
        public AnonymousData AnonymousData { get; set; }

        /// <summary>
        /// Gets the <see cref="TargetInfo"/> with the specified project name.
        /// </summary>
        /// <value>
        /// The <see cref="TargetInfo"/>.
        /// </value>
        /// <param name="targetName">Name of the project.</param>
        /// <returns></returns>
        public TargetInfo this[string targetName]
        {
            get
            {
                return this.Targets?.FirstOrDefault(pr => pr.Name.Equals(targetName));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMetadata"/> class.
        /// </summary>
        public UserMetadata()
        {
            this.Targets = new List<TargetInfo>();
            this.CommonTarget = new List<TargetInfoBase>();
            this.AnonymousData = new AnonymousData();
        }

        /// <summary>
        /// Updates the data.
        /// </summary>
        public void UpdateData()
        {
            foreach (var target in this.CommonTarget)
            {
                target.UpdateData(this);
            }

            foreach (var target in this.Targets)
            {
                target.UpdateData(this);
            }
        }
    }
}
