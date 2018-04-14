using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.AutoDeploy.Models
{
    [ImplementPropertyChanged]
    public class TargetInfo : TargetInfoBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether [override mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [override mode]; otherwise, <c>false</c>.
        /// </value>
        public bool OverrideMode { get; set; } = true;
    }
}
