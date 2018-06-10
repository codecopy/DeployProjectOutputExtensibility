using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TP.AutoDeploy.Models
{
    public class HistoryGroup
    {
        public string Name { get; set; }

        [XmlArrayItem("History")]
        public List<string> Histories { get; set; }

        public HistoryGroup()
        {
            this.Histories = new List<string>();
        }
    }
}
