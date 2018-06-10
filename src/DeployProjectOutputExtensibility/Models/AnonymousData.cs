using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TP.AutoDeploy.Models
{
    public class AnonymousData
    {
        [XmlArrayItem("HistoryGroup")]
        public List<HistoryGroup> HistoryGroups { get; set; }

        public AnonymousData()
        {
            this.HistoryGroups = new List<HistoryGroup>();
        }
    }
}
