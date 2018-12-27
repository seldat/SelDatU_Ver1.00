﻿using SeldatMRMS.Management.RobotManagent;
using SelDatUnilever_Ver1._00.Management.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.Management.RobotManagent.RobotManagementService;
using static SeldatMRMS.RegisterProcedureService;
using static SelDatUnilever_Ver1._00.Management.DeviceManagement.DeviceItem;

namespace SelDatUnilever_Ver1._00.Management.UnityService
{
    public class AssigmentTaskService:TaskRounterService
    {
        
        public AssigmentTaskService() { }
        public void FinishTask(String userName)
        {
            var item = deviceItemsList.Find(e => e.userName == userName);
            item.RemoveFirstOrder();
        }
        public void Start()
        {
            Alive = true;
            processAssignAnTaskWait = ProcessAssignAnTaskWait.PROC_ANY_GET_ANROBOT_IN_WAITTASKLIST;
            processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_GET_ANROBOT_INREADYLIST;
            AssignTask();
            AssignTaskAtReady();
        }
        public void Dispose()
        {
            Alive = false;
        }
        public void AssignTask()
        {
           Task.Run(() =>
            {
                OrderItem orderItem = null;
                RobotUnity robot = null;
                while (Alive)
                {
                    //Console.WriteLine(processAssignAnTaskWait);
                    switch (processAssignAnTaskWait)
                    {
                        case ProcessAssignAnTaskWait.PROC_ANY_IDLE:
                            break;
                        case ProcessAssignAnTaskWait.PROC_ANY_GET_ANROBOT_IN_WAITTASKLIST:

                            ResultRobotReady result = robotManageService.GetRobotUnityWaitTaskItem0();
                            if (result != null)
                            {
                                robot = result.robot;
                                if (result.onReristryCharge)
                                {
                                    // registry charge procedure
                                    procedureService.Register(ProcedureItemSelected.PROCEDURE_ROBOT_TO_CHARGE, robot, null);
                                }
                                else
                                {
                                    processAssignAnTaskWait = ProcessAssignAnTaskWait.PROC_ANY_CHECK_HAS_ANTASK;
                                    
                                }
                            }
                            break;
                        case ProcessAssignAnTaskWait.PROC_ANY_CHECK_HAS_ANTASK:
                            orderItem = Gettask();
                            if (orderItem != null)
                            {
                                processAssignAnTaskWait = ProcessAssignAnTaskWait.PROC_ANY_ASSIGN_ANTASK;
                            }
                            else
                            {
                                processAssignAnTaskWait = ProcessAssignAnTaskWait.PROC_ANY_GET_ANROBOT_IN_WAITTASKLIST;
                            }
                            break;
                        case ProcessAssignAnTaskWait.PROC_ANY_ASSIGN_ANTASK:
                            SelectProcedureItem(robot, orderItem);
                            MoveElementToEnd(); // sort Task List
                            // xoa khoi list cho
                            robotManageService.RemoveRobotUnityWaitTaskList(robot.properties.NameID);
                            processAssignAnTaskWait = ProcessAssignAnTaskWait.PROC_ANY_GET_ANROBOT_IN_WAITTASKLIST;
                            break;

                    }
                    Task.Delay(100).Wait();
                }
             });
        }
        public void SelectProcedureItem(RobotUnity robot,OrderItem orderItem)
        {
            if (orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_FORLIFT_TO_BUFFER)
            {
                procedureService.Register(ProcedureItemSelected.PROCEDURE_FORLIFT_TO_BUFFER, robot, orderItem);
            }
            else if (orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_BUFFER_TO_MACHINE)
            {
                procedureService.Register(ProcedureItemSelected.PROCEDURE_BUFFER_TO_MACHINE, robot, orderItem);
            }
            else if (orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_MACHINE_TO_RETURN)
            {
                procedureService.Register(ProcedureItemSelected.PROCEDURE_MACHINE_TO_RETURN, robot, orderItem);
            }
            else if (orderItem.typeReq == DeviceItem.TyeRequest.TYPEREQUEST_BUFFER_TO_RETURN)
            {
                procedureService.Register(ProcedureItemSelected.PROCEDURE_BUFFER_TO_RETURN, robot, orderItem);
            }
            // procedure;
        }
        public void AssignTaskAtReady()
        {
            OrderItem orderItem=null;
            RobotUnity robot = null;
            Task.Run(() =>
            {
                while (Alive)
                {
                    //Console.WriteLine(processAssignTaskReady);
                    switch (processAssignTaskReady)
                    {
                        case ProcessAssignTaskReady.PROC_READY_IDLE:
                            break;
                        case ProcessAssignTaskReady.PROC_READY_GET_ANROBOT_INREADYLIST:

                            ResultRobotReady result = robotManageService.GetRobotUnityReadyItem0();
                            if(result!=null)
                            {
                                robot = result.robot;
                                if(result.onReristryCharge)
                                {
                                    // registry charge procedure
                                    procedureService.Register(ProcedureItemSelected.PROCEDURE_ROBOT_TO_CHARGE, robot, null);
                                }
                                else
                                {
                                    processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_CHECK_HAS_ANTASK;
                                }
                            }
                            break;
                        case ProcessAssignTaskReady.PROC_READY_CHECK_HAS_ANTASK:
                            orderItem = Gettask();
                            if (orderItem != null)
                            {
                                Console.WriteLine(processAssignTaskReady);
                                processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_SET_TRAFFIC_RISKAREA_ON;
                            }
                            else
                            {
                                processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_GET_ANROBOT_INREADYLIST;
                            }
                            break;
                        case ProcessAssignTaskReady.PROC_READY_ASSIGN_ANTASK:
                            Console.WriteLine(processAssignTaskReady);
                            SelectProcedureItem(robot, orderItem);
                            MoveElementToEnd(); // sort Task List
                            processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_CHECK_ROBOT_OUTSIDEREADY;
                            break;
                        case ProcessAssignTaskReady.PROC_READY_SET_TRAFFIC_RISKAREA_ON:
                            robot.TurnOnSupervisorTraffic(true);
                            processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_ASSIGN_ANTASK;
                            break;
                        case ProcessAssignTaskReady.PROC_READY_CHECK_ROBOT_OUTSIDEREADY:

                            // kiem tra robot tai vung ready
                            if(!trafficService.RobotIsInArea("",robot.properties.pose.Position))
                            {
                                // xoa khoi list cho
                                robotManageService.RemoveRobotUnityReadyList(robot.properties.NameID);
                                processAssignTaskReady = ProcessAssignTaskReady.PROC_READY_GET_ANROBOT_INREADYLIST;
                            }

                            break;
                    }
                    Task.Delay(100).Wait();
                }
            });
        }

    }
}
