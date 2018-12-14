using SeldatMRMS.Management.RobotManagent;
using SelDatUnilever_Ver1._00.Management.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SelDatUnilever_Ver1._00.Management.DeviceManagement.DeviceItem;

namespace SelDatUnilever_Ver1._00.Management.UnityService
{
   public class TaskRounterService
    {
        public enum ProcessAssignAnTask
        {
            PROC_IDLE,
            PROC_REQUEST_ROBOT,
            PROC_REQUEST_TASK,
            PROC_SORT_TASK
        }
        public event Action<bool> FinishTaskCallBack;
        protected ProcessAssignAnTask processAssignAnTaskState;
        public RobotManagementService robotManageService;
        public DeviceRegistrationService deviceRegistrationService;
        protected void RegistryService(RobotManagementService robotManageService)
        {
            this.robotManageService = robotManageService;
        }
        protected void RegistryService(DeviceRegistrationService deviceRegistrationService)
        {
            this.deviceRegistrationService = deviceRegistrationService;
        }
        public TaskRounterService() {
            processAssignAnTaskState = ProcessAssignAnTask.PROC_IDLE;
        }
        public void MoveElementToEnd()
        {
            var element = deviceRegistrationService.GetDeviceItemList()[0];
            deviceRegistrationService.GetDeviceItemList().RemoveAt(0);
            deviceRegistrationService.GetDeviceItemList().Add(element);
        }
        public OneOrder Gettask()
        {
            OneOrder item = deviceRegistrationService.GetDeviceItemList()[0].GetOrder();
            return item;
        }

   }
}
