using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.AutoDeploy.View;

namespace TP.AutoDeploy.Models
{
    public class ToolSetting : IToolSetting
    {
        /// <summary>
        /// Gets or sets the maximum history.
        /// </summary>
        /// <value>
        /// The maximum history.
        /// </value>
        public int MaxHistory { get; set; } = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSetting"/> class.
        /// </summary>
        public ToolSetting()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSetting"/> class.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        public ToolSetting(IToolSetting rawData)
        {
            if (rawData == null)
            {
                return;
            }
            this.MaxHistory = rawData.MaxHistory;
        }
    }
}
