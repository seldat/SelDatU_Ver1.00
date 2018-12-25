using DoorControllerService;
using SeldatMRMS.Management;
using SeldatMRMS.Management.DoorServices;
using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SeldatMRMS
{

    public class ProcedureForkLiftToBuffer : ProcedureControlServices
    {
        public struct DataForkLiftToBuffer
        {
            // public Pose PointCheckInGate;
            // public Pose PointOfGate;
            // public Pose PointFrontLineGate;
            // public PointDetect PointPickPalletIn;
            // public Pose PointCheckInBuffer;
            // public Pose PointFrontLineBuffer;
            // public PointDetectBranching PointDetectLineBranching;
            // public PointDetect PointDropPallet;
        }
        // DataForkLiftToBuffer points;
        ForkLiftToBuffer StateForkLiftToBuffer;
        Thread ProForkLiftToBuffer;
        RobotUnity robot;
        DoorService door;
        ResponseCommand resCmd;
        TrafficManagementService Traffic;
        const UInt32 TIME_OUT_OPEN_DOOR = 600000;/* ms */
        const UInt32 TIME_OUT_CLOSE_DOOR = 600000;/* ms */
        public override event Action<ProcedureControlServices> ReleaseProcedureHandler;
        public ProcedureForkLiftToBuffer(RobotUnity robot, DoorManagementService doorservice, TrafficManagementService traffiicService) : base(robot)
        {
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_IDLE;
            resCmd = ResponseCommand.RESPONSE_NONE;
            this.robot = robot;
            // this.points = new DataForkLiftToBuffer();
            door = doorservice.DoorMezzamineUpBack;
            // this.points.PointFrontLineGate = this.door.config.PointFrontLine;
            // this.points.PointPickPalletIn = this.door.config.PointOfPallet;
            this.Traffic = traffiicService;

        }
        public void Start(ForkLiftToBuffer state = ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_GATE)
        {
            StateForkLiftToBuffer = state;
            ProForkLiftToBuffer = new Thread(this.Procedure);
            ProForkLiftToBuffer.Start(this);
        }
        public void Destroy()
        {
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_RELEASED;
        }
        public void Procedure(object ojb)
        {
            ProcedureForkLiftToBuffer FlToBuf = (ProcedureForkLiftToBuffer)ojb;
            RobotUnity rb = FlToBuf.robot;
            // DataForkLiftToBuffer p = FlToBuf.points;
            DoorService ds = FlToBuf.door;
            TrafficManagementService Traffic = FlToBuf.Traffic;
            while (StateForkLiftToBuffer != ForkLiftToBuffer.FORBUF_ROBOT_RELEASED)
            {
                switch (StateForkLiftToBuffer)
                {
                    case ForkLiftToBuffer.FORBUF_IDLE:
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_GATE: //gui toa do di den khu vuc checkin cong
                        if (Traffic.RobotIsInArea("OPA4", rb.properties.pose.Position))
                        {
                            rb.SendPoseStamped(ds.config.PointFrontLine);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_CHECKIN_GATE;
                        }
                        else
                        {
                            rb.SendPoseStamped(ds.config.PointCheckInGate);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_CHECKIN_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_CHECKIN_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_CHECKIN_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_CAME_CHECKIN_GATE: // đã đến vị trí, kiem tra va cho khu vuc cong san sang de di vao.
                        if (false == Traffic.HasRobotUnityinArea(ds.config.PointFrontLine.Position))
                        {
                            rb.SendPoseStamped( ds.config.PointFrontLine);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_GATE_POSITION;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_CAME_GATE_POSITION: // da den khu vuc cong , gui yeu cau mo cong.
                        if (ds.Open(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK))
                        {
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_OPEN_DOOR;
                        }
                        else
                        {

                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_OPEN_DOOR:  //doi mo cong
                        if (true == ds.WaitOpen(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK, TIME_OUT_OPEN_DOOR))
                        {
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_OPEN_DOOR_SUCCESS;
                        }
                        else
                        {

                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_OPEN_DOOR_SUCCESS: // mo cua thang cong ,gui toa do line de robot di vao gap hang
                        // rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETUP);
                        rb.SendCmdAreaPallet(ds.config.infoPallet);
                        StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_PICKUP_PALLET_IN;
                        break;
                    // case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_PALLET_IN:
                    //     if (true == rb.CheckPointDetectLine(ds.config.P, rb))
                    //     {
                    //         rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                    //         StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_PICKUP_PALLET_IN;
                    //     }
                    //     break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_PICKUP_PALLET_IN: // doi robot gap hang
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETUP)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            // FlToBuf.UpdatePalletState(PalletStatus.F);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_GATE;
                        }
                        else if(resCmd == ResponseCommand.RESPONSE_ERROR){
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_RELEASED;    
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            if (ds.Close(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK))
                            {
                                StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CLOSE_GATE;
                            }
                            else
                            {

                            }
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CLOSE_GATE: // doi dong cong.
                        if (true == ds.WaitClose(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK, TIME_OUT_CLOSE_DOOR))
                        {
                            rb.SendPoseStamped(FlToBuf.GetCheckInBuffer());
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_CHECKIN_BUFFER;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_CHECKIN_BUFFER: // doi robot di den khu vuc checkin cua vung buffer
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_ZONE_BUFFER_READY;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_ZONE_BUFFER_READY: // doi khu vuc buffer san sang de di vao
                        if (false == Traffic.HasRobotUnityinArea(FlToBuf.GetFrontLineBuffer().Position))
                        {
                            rb.SendPoseStamped(FlToBuf.GetFrontLineBuffer());
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdAreaPallet(FlToBuf.GetInfoOfPalletBuffer());
                            // rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_FORWARD_DIRECTION);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_DROPDOWN_PALLET_BUFFER;
                        }
                        break;
                    // case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_POINT_BRANCHING:
                    //     if (true == rb.CheckPointDetectLine(FlToBuf.GetPointDetectBranching().xy, rb))
                    //     {
                    //         if (FlToBuf.GetPointDetectBranching().brDir == BrDirection.DIR_LEFT)
                    //         {
                    //             rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_TURN_LEFT);
                    //         }
                    //         else if (FlToBuf.GetPointDetectBranching().brDir == BrDirection.DIR_RIGHT)
                    //         {
                    //             rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_TURN_RIGHT);
                    //         }
                    //         StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_POINT_BRANCHING;
                    //     }
                    //     break;
                    // case ForkLiftToBuffer.FORBUF_ROBOT_CAME_POINT_BRANCHING:  //doi bobot re
                    //     if ((resCmd == ResponseCommand.RESPONSE_FINISH_TURN_LEFT) || (resCmd == ResponseCommand.RESPONSE_FINISH_TURN_RIGHT))
                    //     {
                    //         resCmd = ResponseCommand.RESPONSE_NONE;
                    //         rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETDOWN);
                    //         StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_GOTO_DROPDOWN_PALLET_BUFFER;
                    //     }
                    //     break;
                    // case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_DROPDOWN_PALLET_BUFFER:
                    //     if (true == rb.CheckPointDetectLine(FlToBuf.GetPointPallet(), rb))
                    //     {
                    //         rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                    //         StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_DROPDOWN_PALLET_BUFFER;
                    //     }
                    //     break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_DROPDOWN_PALLET_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            FlToBuf.UpdatePalletState(PalletStatus.W);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER;
                        }
                        else if(resCmd == ResponseCommand.RESPONSE_ERROR){
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_RELEASED;    
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER: // đợi
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_RELEASED;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_RELEASED: // trả robot về robotmanagement để nhận quy trình mới
                        ReleaseProcedureHandler(this);
                        break;
                    default: break;
                }
                Thread.Sleep(5);
            }
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_IDLE;
        }
        public override void FinishStatesCallBack(Int32 message)
        {
            this.resCmd = (ResponseCommand)message;
        }
    }
}
