using SeldatMRMS.Management.RobotManagent;
using SelDatUnilever_Ver1._00.Management.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.RegisterProcedureService;
using static SelDatUnilever_Ver1._00.Management.DeviceManagement.DeviceItem;

namespace SelDatUnilever_Ver1._00.Management.UnityService
{
    public class AssigmentTaskService:TaskRounterService
    {
       
        private RobotUnity robotTemp;
        public AssigmentTaskService() { }
        public void FinishTask(String deviceId)
        {
            var item = deviceItemsList.Find(e => e.deviceID == deviceId);
            item.RemoveFirstOrder();
        }
        public async Task Process()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    switch (processAssignAnTaskState)
                    {
                        case ProcessAssignAnTask.PROC_IDLE:
                            break;
                        case ProcessAssignAnTask.PROC_REQUEST_ROBOT:
                            robotTemp = robotManageService.getRobotUnityTask();
                            if (robotTemp != null)
                            {
                                processAssignAnTaskState = ProcessAssignAnTask.PROC_REQUEST_TASK;
                                break;
                            }
                            Task.Delay(10).Wait();
                            break;
                        case ProcessAssignAnTask.PROC_REQUEST_TASK:
                            OrderItem orderItem = Gettask();
                            if (orderItem != null)
                            {
                                if(orderItem.typeReq==DeviceItem.TyeRequest.TYPEREQUEST_FORLIFT_TO_BUFFER)
                                {
                                    procedureService.Register(ProcedureItemSelected.PROCEDURE_FORLIFT_TO_BUFFER, robotTemp, orderItem);
                                }
                                else if (orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_BUFFER_TO_MACHINE)
                                {
                                    procedureService.Register(ProcedureItemSelected.PROCEDURE_BUFFER_TO_MACHINE, robotTemp, orderItem);
                                }
                                else if (orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_MACHINE_TO_RETURN)
                                {
                                    procedureService.Register(ProcedureItemSelected.PROCEDURE_MACHINE_TO_RETURN, robotTemp, orderItem);
                                }
                                else if(orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_BUFFER_TO_RETURN)
                                {
                                    procedureService.Register(ProcedureItemSelected.PROCEDURE_BUFFER_TO_RETURN, robotTemp, orderItem);
                                }
                                // procedure;
                            }
                            processAssignAnTaskState = ProcessAssignAnTask.PROC_SORT_TASK;
                            break;
                        case ProcessAssignAnTask.PROC_SORT_TASK:
                            MoveElementToEnd();
                            break;
                    }
                    Task.Delay(10).Wait();
                }
             });
        }
    }
}
