using SeldatMRMS;
using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
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
        public enum ProcessAssignAnTaskWait
        {
            PROC_IDLE = 0,
            PROC_CHECK_HAS_ANTASK,
            PROC_ASSIGN_ANTASK,
            PROC_GET_ANROBOT_IN_WAITTASKLIST,
            PROC_CHECK_ROBOT_BATTERYLEVEL,
            PROC_SET_TRAFFIC_RISKAREA_ON,
            PROC_CHECK_ROBOT_OUTSIDEREADY,
        }
        protected enum ProcessAssignTaskReady
        {
            PROC_IDLE = 0,
            PROC_CHECK_HAS_ANTASK,
            PROC_ASSIGN_ANTASK,
            PROC_GET_ANROBOT_INREADYLIST,
            PROC_CHECK_ROBOT_BATTERYLEVEL,
            PROC_SET_TRAFFIC_RISKAREA_ON,
            PROC_CHECK_ROBOT_OUTSIDEREADY,
        }
        protected ProcessAssignTaskReady processAssignTaskReady;
        public event Action<bool> FinishTaskCallBack;
        protected ProcessAssignAnTaskWait processAssignAnTaskWait;
        protected ProcedureManagementService procedureService;
        public RobotManagementService robotManageService;
        public TrafficManagementService trafficService;
        public List<DeviceItem> deviceItemsList;
        public void RegistryService(RobotManagementService robotManageService)
        {
            this.robotManageService = robotManageService;
        }
        public void RegistryService(TrafficManagementService trafficService)
        {
            this.trafficService = trafficService;
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
            //processAssignAnTaskState = ProcessAssignAnTask.PROC_IDLE;
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
