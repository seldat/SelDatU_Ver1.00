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
        RobotManagementService RobotManagementServiceRegistery { get; set; }
        DoorManagementService DoorManagementServiceRegistery { get; set; }
        ProcedureManagementService ProcedureManagementServiceRegistery { get; set; }
        public UnityManagementService() { }
        public void Initialize()
        {
            RobotManagementServiceRegistery = new RobotManagementService();
            DoorManagementServiceRegistery = new DoorManagementService();
            ProcedureManagementServiceRegistery = new ProcedureManagementService();
        }
        public void Dispose()
        {

        }
    }
}
