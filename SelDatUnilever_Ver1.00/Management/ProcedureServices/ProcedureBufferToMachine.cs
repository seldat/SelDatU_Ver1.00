using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using System;
using System.Threading;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SeldatMRMS
{
    public class ProcedureBufferToMachine : ProcedureControlServices
    {
        public class DataBufferToMachine
        {
            private ProcedureBufferToMachine prBM;
            public DataBufferToMachine(ProcedureBufferToMachine prBM) { this.prBM = prBM; }
            public Pose PointCheckInBuffer;
            public Pose PointFrontLineBuffer;
            public PointDetectBranching PointDetectLineBranching;
            public PointDetect PointPickPallet;
            public Pose PointCheckInMachine;
            public Pose PointFrontLineMachine;
            public PointDetect PointDropPallet;
        }
        DataBufferToMachine points;
        BufferToMachine StateBufferToMachine;
        Thread ProBuferToMachine;
        RobotUnity robot;
        ResponseCommand resCmd;
        TrafficManagementService Traffic;
        public ProcedureBufferToMachine(RobotUnity robot,TrafficManagementService traffiicService) : base(robot, null)
        {
            StateBufferToMachine = BufferToMachine.BUFMAC_IDLE;
            this.robot = robot;
            this.points = new DataBufferToMachine(this);
            this.Traffic = traffiicService;
        }

        public void Start(BufferToMachine state = BufferToMachine.BUFMAC_ROBOT_GOTO_CHECKIN_BUFFER)
        {
            StateBufferToMachine = state;
            ProBuferToMachine = new Thread(this.Procedure);
            ProBuferToMachine.Start(this);
        }
        public void Destroy()
        {
            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_RELEASED;
        }
        public void Procedure(object ojb)
        {
            ProcedureBufferToMachine BfToMa = (ProcedureBufferToMachine)ojb;
            RobotUnity rb = BfToMa.robot;
            DataBufferToMachine p = BfToMa.points;
            TrafficManagementService Traffic = BfToMa.Traffic;
            while (StateBufferToMachine != BufferToMachine.BUFMAC_ROBOT_RELEASED)
            {
                switch (StateBufferToMachine)
                {
                    case BufferToMachine.BUFMAC_IDLE:
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_GOTO_CHECKIN_BUFFER: // bắt đầu rời khỏi vùng GATE đi đến check in/ đảm bảo check out vùng cổng để robot kế tiếp vào làm việc
                        rb.SendPoseStamped(p.PointCheckInBuffer());
                        StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_CHECKIN_BUFFER;
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_CHECKIN_BUFFER: // doi robot di den khu vuc checkin cua vung buffer
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_ZONE_BUFFER_READY;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_ZONE_BUFFER_READY: // doi khu vuc buffer san sang de di vao
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineBuffer.Position))
                        {
                            rb.SendPoseStamped(p.PointFrontLineBuffer);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_FORWARD_DIRECTION);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_POINT_BRANCHING;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_POINT_BRANCHING:
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
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_POINT_BRANCHING;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_CAME_POINT_BRANCHING:  //doi bobot re
                        if ((resCmd == ResponseCommand.RESPONSE_FINISH_TURN_LEFT) || (resCmd == ResponseCommand.RESPONSE_FINISH_TURN_RIGHT))
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETUP);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_GOTO_PICKUP_PALLET_BUFFER;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_GOTO_PICKUP_PALLET_BUFFER:
                        if (true == rb.CheckPointDetectLine(p.PointPickPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_PICKUP_PALLET_BUFFER;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_PICKUP_PALLET_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETUP)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            this.UpdatePalletState(PalletStatus.F);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER: // đợi
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendPoseStamped(p.PointFrontLineMachine);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_GOTO_FRONTLINE_DROPDOWN_PALLET;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_GOTO_FRONTLINE_DROPDOWN_PALLET:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_CAME_FRONTLINE_DROPDOWN_PALLET;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_CAME_FRONTLINE_DROPDOWN_PALLET:  // đang trong tiến trình dò line và thả pallet
                        rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETDOWN);
                        StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_POINT_DROP_PALLET;
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_POINT_DROP_PALLET:
                        if (true == rb.CheckPointDetectLine(p.PointDropPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_DROPDOWN_PALLET;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_DROPDOWN_PALLET:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            this.UpdatePalletState(PalletStatus.W);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_FRONTLINE;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_WAITTING_GOTO_FRONTLINE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateBufferToMachine = BufferToMachine.BUFMAC_ROBOT_RELEASED;
                        }
                        break;
                    case BufferToMachine.BUFMAC_ROBOT_RELEASED:  // trả robot về robotmanagement để nhận quy trình mới
                        break;
                    default:
                        break;
                }
                Thread.Sleep(5);
            }
            StateBufferToMachine = BufferToMachine.BUFMAC_IDLE;
        }
        public override void FinishStatesCallBack(Int32 message)
        {
            this.resCmd = (ResponseCommand)message;
        }
    }
}
