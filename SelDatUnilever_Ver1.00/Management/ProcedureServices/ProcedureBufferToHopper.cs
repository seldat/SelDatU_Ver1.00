using DoorControllerService;
using SeldatMRMS.Management;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS
{
    class ProcedureBufferToHopper : ProcedureControlServices
    {
        public ProcedureBufferToHopper(RobotUnity robot) : base(robot, null) { }
    }
}
