using SeldatMRMS.Management.DoorServices;
using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using SelDatUnilever_Ver1._00.Management.DeviceManagement;
using SelDatUnilever_Ver1._00.Management.UnityService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS.Management.UnityService
{
    public class UnityManagementService
    {
        RobotManagementService robotManagementServiceRegistery { get; set; }
        DoorManagementService doorManagementServiceRegistery { get; set; }
        ProcedureManagementService procedureManagementServiceRegistery { get; set; }
        TrafficManagementService trafficManagementService { get; set; }
        AssigmentTaskService assigmentTaskService { get; set; }
        DeviceRegistrationService deviceRegistrationService { get; set; }
        public UnityManagementService() { }
        public void Initialize()
        {
            robotManagementServiceRegistery = new RobotManagementService();
            doorManagementServiceRegistery = new DoorManagementService();
            procedureManagementServiceRegistery = new ProcedureManagementService();
            trafficManagementService = new TrafficManagementService();
            deviceRegistrationService = new DeviceRegistrationService(11000);
            assigmentTaskService = new AssigmentTaskService();
            trafficManagementService = new TrafficManagementService();
            assigmentTaskService.RegistryService(robotManagementServiceRegistery);
            assigmentTaskService.RegistryService(procedureManagementServiceRegistery);
            assigmentTaskService.RegistryService(deviceRegistrationService.GetDeviceItemList());
            assigmentTaskService.RegistryService(trafficManagementService);
            procedureManagementServiceRegistery.RegistryService(trafficManagementService);
            procedureManagementServiceRegistery.RegistryService(robotManagementServiceRegistery);
        }
        public void Dispose()
        {

        }
    }
}
