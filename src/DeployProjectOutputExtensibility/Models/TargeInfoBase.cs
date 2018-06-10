using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TP.AutoDeploy.Extension;

namespace TP.AutoDeploy.Models
{
    [ImplementPropertyChanged]
    public class TargetInfoBase : ICloneable
    {
        private TargetInfoBase parentObject;

        /// <summary>
        /// The parent object
        /// </summary>
        [XmlIgnore]
        [AlsoNotifyFor("AbsoluteDir")]
        public TargetInfoBase ParentObject
        {
            get { return this.parentObject; }
            set
            {
                this.parentObject = value;
                this.Parent = value?.Name;
            }
        }

        /// <summary>
        /// Gets or sets the common target.
        /// </summary>
        /// <value>
        /// The common target.
        /// </value>
        [XmlIgnore]
        public ObservableCollection<TargetInfoBase> CommonTarget { get; set; }

        /// <summary>
        /// Gets or sets the absolute dir.
        /// </summary>
        /// <value>
        /// The absolute dir.
        /// </value>
        [XmlIgnore]
        public string AbsoluteDir => this.GetAbsolute();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TargetInfoBase"/> is inherit.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inherit; otherwise, <c>false</c>.
        /// </value>
        [AlsoNotifyFor("AbsoluteDir")]
        public bool Inherit { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [AlsoNotifyFor("AbsoluteDir")]
        public string Parent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the target dir.
        /// </summary>
        /// <value>
        /// The target dir.
        /// </value>
        [AlsoNotifyFor("AbsoluteDir")]
        public string TargetDir { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fixed target dir.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fixed target dir; otherwise, <c>false</c>.
        /// </value>
        [AlsoNotifyFor("AbsoluteDir")]
        public bool IsFixedTargetDir { get; set; }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return (TargetInfoBase) this.MemberwiseClone();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
