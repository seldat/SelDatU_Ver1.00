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
            public Pose PointCheckInGate;
            public Pose PointOfGate;
            public Pose PointFrontLineGate;
            public PointDetect PointPickPalletIn;
            public Pose PointCheckInBuffer;
            public Pose PointFrontLineBuffer;
            public PointDetectBranching PointDetectLineBranching;
            public PointDetect PointDropPallet;
        }
        DataForkLiftToBuffer points;
        ForkLiftToBuffer StateForkLiftToBuffer;
        Thread ProForkLiftToBuffer;
        RobotUnity robot;
        DoorService door;
        ResponseCommand resCmd;
        TrafficManagementService Traffic;
        const UInt32 TIME_OUT_OPEN_DOOR = 600000;/* ms */
        const UInt32 TIME_OUT_CLOSE_DOOR = 600000;/* ms */

        public ProcedureForkLiftToBuffer(RobotUnity robot, DoorManagementService doorservice, DataForkLiftToBuffer dataPoints,TrafficManagementService traffiicService) : base(robot, doorservice.DoorMezzamineUpBack)
        {
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_IDLE;
            resCmd = ResponseCommand.RESPONSE_NONE;
            this.robot = robot;
            this.points = dataPoints;
            this.door = doorservice.DoorMezzamineUpBack;
            this.Traffic = traffiicService;
        }
        public void Start(String content, ForkLiftToBuffer state = ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_GATE)
        {
            StateForkLiftToBuffer = state;
            ProForkLiftToBuffer = new Thread(this.Procedure);
            ProForkLiftToBuffer.Name = content;
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
            DataForkLiftToBuffer p = FlToBuf.points;
            DoorService ds = FlToBuf.door;
            TrafficManagementService Traffic = FlToBuf.Traffic;
            while (StateForkLiftToBuffer != ForkLiftToBuffer.FORBUF_ROBOT_RELEASED)
            {
                switch (StateForkLiftToBuffer)
                {
                    case ForkLiftToBuffer.FORBUF_IDLE:
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_GATE: //gui toa do di den khu vuc checkin cong
                        rb.SendPoseStamped(p.PointCheckInGate);
                        StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_CHECKIN_GATE;
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_CHECKIN_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_CHECKIN_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_CAME_CHECKIN_GATE: // đã đến vị trí, kiem tra va cho khu vuc cong san sang de di vao.
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineBuffer.Position))
                        {
                            rb.SendPoseStamped(p.PointOfGate);
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
                        ds.Open(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK);
                        StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_OPEN_DOOR;
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_OPEN_DOOR:  //doi mo cong
                        if (true == ds.WaitOpen(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK, TIME_OUT_OPEN_DOOR))
                        {
                           StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_OPEN_DOOR_SUCCESS;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_OPEN_DOOR_SUCCESS: // mo cua thang cong ,gui toa do line de robot di vao gap hang
                        rb.SendPoseStamped(p.PointFrontLineGate);
                        StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CAME_FRONTLINE_PALLET_IN;
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CAME_FRONTLINE_PALLET_IN:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_FRONTLINE_PALLET_IN;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_CAME_FRONTLINE_PALLET_IN:
                        rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETUP);
                        StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_PALLET_IN;
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_PALLET_IN:
                        if (true == rb.CheckPointDetectLine(p.PointPickPalletIn, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_PICKUP_PALLET_IN;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_PICKUP_PALLET_IN: // doi robot gap hang
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETUP)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_GATE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendPoseStamped(p.PointOfGate);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOOUT_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOOUT_GATE: // doi robot di ra khoi cong
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            ds.Close(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CLOSE_GATE;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CLOSE_GATE: // doi dong cong.
                        if (true == ds.WaitClose(DoorService.DoorId.DOOR_MEZZAMINE_UP_BACK,TIME_OUT_CLOSE_DOOR))
                        {
                           rb.SendPoseStamped(p.PointCheckInBuffer);
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
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineBuffer.Position))
                        {
                            rb.SendPoseStamped(p.PointFrontLineBuffer);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_FORWARD_DIRECTION);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_POINT_BRANCHING;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOTO_POINT_BRANCHING:
                        if (true == rb.CheckPointDetectLine(p.PointDetectLineBranching.xy, rb))
                        {
                            if (p.PointDetectLineBranching.brDir == BrDirection.DIR_LEFT)
                            {
                                rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_TURN_LEFT);
                            }
                            else if (p.PointDetectLineBranching.brDir == BrDirection.DIR_RIGHT)
                            {
                                rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_TURN_RIGHT);
                            }
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_CAME_POINT_BRANCHING;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_CAME_POINT_BRANCHING:  //doi bobot re
                        if ((resCmd == ResponseCommand.RESPONSE_FINISH_TURN_LEFT) || (resCmd == ResponseCommand.RESPONSE_FINISH_TURN_RIGHT))
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETDOWN);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_GOTO_DROPDOWN_PALLET_BUFFER;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_DROPDOWN_PALLET_BUFFER:
                        if (true == rb.CheckPointDetectLine(p.PointDropPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_DROPDOWN_PALLET_BUFFER;
                        }
                        break;
                    case ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_DROPDOWN_PALLET_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            this.SaveDataToDb(points);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER;
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

                        break;
                    default: break;
                }
                Thread.Sleep(5);
            }
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_IDLE;
            try
            {
                ProForkLiftToBuffer.Abort();
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
