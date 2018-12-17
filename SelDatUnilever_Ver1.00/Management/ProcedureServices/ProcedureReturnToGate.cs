using DoorControllerService;
using SeldatMRMS.Management.DoorServices;
using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using System;
using System.Threading;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SeldatMRMS
{

    public class ProcedureReturnToGate : ProcedureControlServices
    {
        public struct DataReturnToGate
        {
            
            public Pose PointCheckInReturn;
            public Pose PointFrontLineReturn;
            public Pose PointFrontLineGate;
            public PointDetect PointPickPallet;
            public Pose PointCheckInGate;
            public Pose PointOfGate;
            public PointDetect PointDropPallet;
        }
        DataReturnToGate points;
        ReturnToGate StateReturnToGate;
        Thread ProReturnToGate;
        RobotUnity robot;
        DoorManagementService door;
        ResponseCommand resCmd;
        TrafficManagementService Traffic;
        const UInt32 TIME_OUT_OPEN_DOOR = 600000;/* ms */
        const UInt32 TIME_OUT_CLOSE_DOOR = 600000;/* ms */

        public ProcedureReturnToGate(RobotUnity robot, DoorManagementService doorservice, DataReturnToGate dataPoints,TrafficManagementService traffiicService) : base(robot, doorservice.DoorMezzamineReturnBack)
        {
            StateReturnToGate = ReturnToGate.RETGATE_IDLE;
            resCmd = ResponseCommand.RESPONSE_NONE;
            this.robot = robot;
            this.points = dataPoints;
            this.door = doorservice;
            this.Traffic = traffiicService;
        }
        public void Start(String content, ReturnToGate state = ReturnToGate.RETGATE_ROBOT_WAITTING_GOTO_CHECKIN_RETURN)
        {
            StateReturnToGate = state;
            ProReturnToGate = new Thread(this.Procedure);
            ProReturnToGate.Name = content;
            ProReturnToGate.Start(this);
        }
        public void Destroy()
        {
            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_RELEASED;
        }
        public void Procedure(object ojb)
        {
            ProcedureReturnToGate FlToBuf = (ProcedureReturnToGate)ojb;
            RobotUnity rb = FlToBuf.robot;
            DataReturnToGate p = FlToBuf.points;
            DoorService ds = FlToBuf.door.DoorMezzamineReturnBack;
            TrafficManagementService Traffic = FlToBuf.Traffic;
            while (StateReturnToGate != ReturnToGate.RETGATE_ROBOT_RELEASED)
            {
                switch (StateReturnToGate)
                {
                    case ReturnToGate.RETGATE_IDLE:
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_GOTO_CHECKIN_RETURN: // doi robot di den khu vuc checkin cua vung buffer
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_ZONE_RETURN_READY;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_ZONE_RETURN_READY: // doi khu vuc buffer san sang de di vao
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineReturn.Position))
                        {
                            rb.SendPoseStamped(p.PointFrontLineReturn);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_CAME_FRONTLINE_RETURN;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_CAME_FRONTLINE_RETURN:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETUP);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_GOTO_PICKUP_PALLET_RETURN;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_GOTO_PICKUP_PALLET_RETURN:
                        if (true == rb.CheckPointDetectLine(p.PointPickPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_PICKUP_PALLET_RETURN;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_PICKUP_PALLET_RETURN:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETUP)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            this.SaveDataToDb(points);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_GOBACK_FRONTLINE_RETURN;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_GOBACK_FRONTLINE_RETURN: // đợi
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_GOTO_CHECKIN_GATE;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_GOTO_CHECKIN_GATE: //gui toa do di den khu vuc checkin cong
                        rb.SendPoseStamped(p.PointCheckInGate);
                        StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_GOTO_CHECKIN_GATE;
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_GOTO_CHECKIN_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_CAME_CHECKIN_GATE;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_CAME_CHECKIN_GATE: // đã đến vị trí, kiem tra va cho khu vuc cong san sang de di vao.
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineReturn.Position))
                        {
                            rb.SendPoseStamped(p.PointOfGate);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_GOTO_GATE;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_GOTO_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_CAME_GATE_POSITION;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_CAME_GATE_POSITION: // da den khu vuc cong , gui yeu cau mo cong.
                        ds.Open(DoorService.DoorId.DOOR_MEZZAMINE_RETURN_BACK);
                        StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_OPEN_DOOR;
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_OPEN_DOOR:  //doi mo cong
                        if (true == ds.WaitOpen(DoorService.DoorId.DOOR_MEZZAMINE_RETURN_BACK,TIME_OUT_OPEN_DOOR))
                        {
                           StateReturnToGate = ReturnToGate.RETGATE_ROBOT_OPEN_DOOR_SUCCESS;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_OPEN_DOOR_SUCCESS: // mo cua thang cong ,gui toa do line de robot di vao
                        rb.SendPoseStamped(p.PointFrontLineGate);
                        StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_CAME_FRONTLINE_POSITION_PALLET_RETURN;
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_CAME_FRONTLINE_POSITION_PALLET_RETURN:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_CAME_FRONTLINE_POSITION_PALLET_RETURN;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_CAME_FRONTLINE_POSITION_PALLET_RETURN:
                        rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETDOWN);
                        StateReturnToGate = ReturnToGate.RETGATE_ROBOT_GOTO_POSITION_PALLET_RETURN;
                        break;
                    case ReturnToGate.RETGATE_ROBOT_GOTO_POSITION_PALLET_RETURN:
                        if (true == rb.CheckPointDetectLine(p.PointDropPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_DROPDOWN_PALLET_RETURN;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_DROPDOWN_PALLET_RETURN: // doi robot gap hang
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_GOBACK_FRONTLINE_GATE;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_GOBACK_FRONTLINE_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendPoseStamped(p.PointOfGate);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_GOOUT_GATE;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_GOOUT_GATE: //doi robot di ra khoi cong
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            ds.Close(DoorService.DoorId.DOOR_MEZZAMINE_RETURN_BACK);
                            StateReturnToGate = ReturnToGate.RETGATE_ROBOT_WAITTING_CLOSE_GATE;
                        }
                        break;
                    case ReturnToGate.RETGATE_ROBOT_WAITTING_CLOSE_GATE: // doi dong cong.
                        if (true == ds.WaitClose(DoorService.DoorId.DOOR_MEZZAMINE_RETURN_BACK,TIME_OUT_CLOSE_DOOR))
                        {
                           StateReturnToGate = ReturnToGate.RETGATE_ROBOT_RELEASED;
                        }
                        break;
                    
                    case ReturnToGate.RETGATE_ROBOT_RELEASED: // trả robot về robotmanagement để nhận quy trình mới

                        break;
                    default: break;
                }
                Thread.Sleep(5);
            }
            StateReturnToGate = ReturnToGate.RETGATE_IDLE;
            try
            {
                ProReturnToGate.Abort();
            }
            catch (System.Exception)
            {
                Console.WriteLine("faillllllllllllllllllllll");
                throw;
            }
        }

        public override void FinishStatesCallBack(Int32 message)
        {
            this.resCmd = (ResponseCommand)message;
        }
    }
}
