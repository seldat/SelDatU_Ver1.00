using SeldatMRMS.Management.DoorServices;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS.Management.UnityService
{
    public class UnityManagementService
    {
        RobotManagementService RobotManagementServiceRegister { get; set; }
        DoorManagementService DoorManagementServiceRegister { get; set; }
        ProcedureManagementService ProcedureManagementServiceRegister { get; set; }
        public UnityManagementService() { }
        public void Initialize()
        {
            RobotManagementServiceRegister = new RobotManagementService();
            DoorManagementServiceRegister = new DoorManagementService();
            ProcedureManagementServiceRegister = new ProcedureManagementService();
        }
        public void Dispose()
        {

        }
    }
}
