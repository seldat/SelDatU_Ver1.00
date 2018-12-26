using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS.Management.RobotManagent
{
    public class RobotBaseService:TrafficRobotUnity
    {
        public bool SelectedATask { get; set; }
       
        public struct LoadedConfigureInformation
        {
            public bool IsLoadedStatus { get; set; }
            public String ErrorContent { get; set; }
        }
    }
}
