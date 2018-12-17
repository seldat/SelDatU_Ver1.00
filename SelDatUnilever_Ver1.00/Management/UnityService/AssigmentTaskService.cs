using SeldatMRMS.Management.RobotManagent;
using SelDatUnilever_Ver1._00.Management.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDatUnilever_Ver1._00.Management.UnityService
{
    public class AssigmentTaskService:TaskRounterService
    {
       
        private RobotUnity robotTemp;
        public AssigmentTaskService() { }
        public void FinishTask(String deviceId)
        {
            var item = deviceRegistrationService.GetDeviceItemList().Find(e => e.deviceID == deviceId);
            item.RemoveFirstOrder();
        }
        public async Task Process()
        {
            switch (processAssignAnTaskState)
            {
                case ProcessAssignAnTask.PROC_IDLE:
                    break;
                case ProcessAssignAnTask.PROC_REQUEST_ROBOT:
                    robotTemp = robotManageService.GetRobotNotAssignTask();
                    if(robotTemp!=null)
                    {
                        processAssignAnTaskState = ProcessAssignAnTask.PROC_REQUEST_TASK;
                        break;
                    }                
                    Task.Delay(10).Wait();
                    break;
                case ProcessAssignAnTask.PROC_REQUEST_TASK:


                    break;
                case ProcessAssignAnTask.PROC_SORT_TASK:
                    MoveElementToEnd();
                    break;
            }
        }
    }
}
