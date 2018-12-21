using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using System;
using System.Threading;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SeldatMRMS
{
    public class ProcedureMachineToReturn : ProcedureControlServices
    {
        public struct DataMachineToReturn
        {
            public Pose PointFrontLineMachine;
            public PointDetect PointPickPallet;
            public Pose PointCheckInReturn;
            public Pose PointFrontLineReturn;
            public PointDetect PointDropPallet;
        }
        DataMachineToReturn points;
        MachineToReturn StateMachineToReturn;
        Thread ProMachineToReturn;
        RobotUnity robot;
        ResponseCommand resCmd;
        TrafficManagementService Traffic;
        public ProcedureMachineToReturn(RobotUnity robot,TrafficManagementService traffiicService) : base(robot, null)
        {
            StateMachineToReturn = MachineToReturn.MACRET_IDLE;
            this.robot = robot;
            this.points = new DataMachineToReturn();
            this.Traffic = traffiicService;
        }

        public void Start(MachineToReturn state = MachineToReturn.MACRET_ROBOT_GOTO_FRONTLINE_MACHINE)
        {
            StateMachineToReturn = state;
            ProMachineToReturn = new Thread(this.Procedure);
            ProMachineToReturn.Start(this);
        }
        public void Destroy()
        {
            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_RELEASED;
        }
        public void Procedure(object ojb)
        {
            ProcedureMachineToReturn BfToRe = (ProcedureMachineToReturn)ojb;
            RobotUnity rb = BfToRe.robot;
            DataMachineToReturn p = BfToRe.points;
            TrafficManagementService Traffic = BfToRe.Traffic;
            while (StateMachineToReturn != MachineToReturn.MACRET_ROBOT_RELEASED)
            {
                switch (StateMachineToReturn)
                {
                    case MachineToReturn.MACRET_IDLE:
                        break;
                    case MachineToReturn.MACRET_ROBOT_GOTO_FRONTLINE_MACHINE: // doi khu vuc buffer san sang de di vao
                        rb.SendPoseStamped(p.PointFrontLineMachine);
                        StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_CAME_FRONTLINE_MACHINE;
                        break;
                    case MachineToReturn.MACRET_ROBOT_WAITTING_CAME_FRONTLINE_MACHINE:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETUP);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_PICKUP_PALLET_MACHINE;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_GOTO_PICKUP_PALLET_MACHINE:
                        if (true == rb.CheckPointDetectLine(p.PointPickPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_PICKUP_PALLET_MACHINE;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_WAITTING_PICKUP_PALLET_MACHINE:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETUP)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            this.UpdatePalletState(PalletStatus.F);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_GOBACK_FRONTLINE_MACHINE;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_WAITTING_GOBACK_FRONTLINE_MACHINE: // đợi
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendPoseStamped(p.PointCheckInReturn);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_GOTO_CHECKIN_RETURN;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_GOTO_CHECKIN_RETURN: // dang di
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_CAME_CHECKIN_RETURN;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_CAME_CHECKIN_RETURN: // đã đến vị trí
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineReturn.Position))
                        {
                            rb.SendPoseStamped(p.PointFrontLineReturn);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_GOTO_FRONTLINE_DROPDOWN_PALLET;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_GOTO_FRONTLINE_DROPDOWN_PALLET:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_CAME_FRONTLINE_DROPDOWN_PALLET;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_CAME_FRONTLINE_DROPDOWN_PALLET:  // đang trong tiến trình dò line và thả pallet
                        rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETDOWN);
                        StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_GOTO_POINT_DROP_PALLET;
                        break;
                    case MachineToReturn.MACRET_ROBOT_WAITTING_GOTO_POINT_DROP_PALLET:
                        if (true == rb.CheckPointDetectLine(p.PointDropPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_DROPDOWN_PALLET;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_WAITTING_DROPDOWN_PALLET:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            this.UpdatePalletState(PalletStatus.W);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_WAITTING_GOTO_FRONTLINE;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_WAITTING_GOTO_FRONTLINE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateMachineToReturn = MachineToReturn.MACRET_ROBOT_RELEASED;
                        }
                        break;
                    case MachineToReturn.MACRET_ROBOT_RELEASED:  // trả robot về robotmanagement để nhận quy trình mới
                        break;
                    default:
                        break;
                }
                Thread.Sleep(5);
            }
            StateMachineToReturn = MachineToReturn.MACRET_IDLE;
        }
        public override void FinishStatesCallBack(Int32 message)
        {
            this.resCmd = (ResponseCommand)message;
        }
    }
}
