using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.AutoDeploy.Models
{
    public interface IToolSetting
    {
        int MaxHistory { get; set; }
    }
}
