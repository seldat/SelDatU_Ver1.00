using SeldatMRMS;
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
        protected ProcedureManagementService procedureService;
        public RobotManagementService robotManageService;
        
        public List<DeviceItem> deviceItemsList;
        public void RegistryService(RobotManagementService robotManageService)
        {
            this.robotManageService = robotManageService;
        }
        public void RegistryService(ProcedureManagementService procedureService)
        {
            this.procedureService = procedureService;
        }
        public void RegistryService(List<DeviceItem> deviceItemsList)
        {
            this.deviceItemsList = deviceItemsList;
        }
        public TaskRounterService() {
            processAssignAnTaskState = ProcessAssignAnTask.PROC_IDLE;
        }
        public void MoveElementToEnd()
        {
            var element = deviceItemsList[0];
            deviceItemsList.RemoveAt(0);
            deviceItemsList.Add(element);
        }
        public OrderItem Gettask()
        {
            OrderItem item = deviceItemsList[0].GetOrder();
            return item;
        }

   }
}
