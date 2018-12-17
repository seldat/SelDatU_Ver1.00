using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using System;
using System.Threading;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SeldatMRMS
{
    public class ProcedureBufferToReturn : ProcedureControlServices
    {
        public struct DataForkBufferToReturn
        {
            public Pose PointCheckInBuffer;
            public Pose PointFrontLineBuffer;
            public PointDetectBranching PointDetectLineBranching;
            public PointDetect PointPickPallet;
            public Pose PointCheckInReturn;
            public Pose PointFrontLineReturn;
            public PointDetect PointDropPallet;
        }
        DataForkBufferToReturn points;
        BufferToReturn StateBufferToReturn;
        Thread ProBuferToReturn;
        RobotUnity robot;
        ResponseCommand resCmd;
        TrafficManagementService Traffic;
        public ProcedureBufferToReturn(RobotUnity robot, DataForkBufferToReturn dataPoints,TrafficManagementService traffiicService) : base(robot, null)
        {
            StateBufferToReturn = BufferToReturn.BUFRET_IDLE;
            this.robot = robot;
            this.points = dataPoints;
            this.Traffic = traffiicService;
        }

        public void Start(String content, BufferToReturn state = BufferToReturn.BUFRET_ROBOT_GOTO_CHECKIN_BUFFER)
        {
            StateBufferToReturn = state;
            ProBuferToReturn = new Thread(this.Procedure);
            ProBuferToReturn.Name = content;
            ProBuferToReturn.Start(this);
        }
        public void Destroy()
        {
            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_RELEASED;
        }
        public void Procedure(object ojb)
        {
            ProcedureBufferToReturn BfToRe = (ProcedureBufferToReturn)ojb;
            RobotUnity rb = BfToRe.robot;
            DataForkBufferToReturn p = BfToRe.points;
            TrafficManagementService Traffic = BfToRe.Traffic;
            while (StateBufferToReturn != BufferToReturn.BUFRET_ROBOT_RELEASED)
            {
                switch (StateBufferToReturn)
                {
                    case BufferToReturn.BUFRET_IDLE:
                        break;
                    case BufferToReturn.BUFRET_ROBOT_GOTO_CHECKIN_BUFFER: // bắt đầu rời khỏi vùng GATE đi đến check in/ đảm bảo check out vùng cổng để robot kế tiếp vào làm việc
                        rb.SendPoseStamped(p.PointCheckInBuffer);
                        StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_CHECKIN_BUFFER;
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_CHECKIN_BUFFER: // doi robot di den khu vuc checkin cua vung buffer
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_ZONE_BUFFER_READY;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_ZONE_BUFFER_READY: // doi khu vuc buffer san sang de di vao
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineBuffer.Position))
                        {
                            rb.SendPoseStamped(p.PointFrontLineBuffer);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_CAME_FRONTLINE_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_FORWARD_DIRECTION);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_POINT_BRANCHING;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_POINT_BRANCHING:
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
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_POINT_BRANCHING;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_CAME_POINT_BRANCHING:  //doi bobot re
                        if ((resCmd == ResponseCommand.RESPONSE_FINISH_TURN_LEFT) || (resCmd == ResponseCommand.RESPONSE_FINISH_TURN_RIGHT))
                        {
                            rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETUP);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_GOTO_PICKUP_PALLET_BUFFER;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_GOTO_PICKUP_PALLET_BUFFER:
                        if (true == rb.CheckPointDetectLine(p.PointPickPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_PICKUP_PALLET_BUFFER;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_PICKUP_PALLET_BUFFER:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            // this.SaveDataToDb(points);
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_GOBACK_FRONTLINE_BUFFER: // đợi
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            rb.SendPoseStamped(p.PointCheckInReturn);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_GOTO_CHECKIN_RETURN;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_GOTO_CHECKIN_RETURN: // dang di
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_CAME_CHECKIN_RETURN;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_CAME_CHECKIN_RETURN: // đã đến vị trí
                        if (false == Traffic.HasRobotUnityinArea(p.PointFrontLineReturn.Position))
                        {
                            rb.SendPoseStamped(p.PointFrontLineReturn);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_GOTO_FRONTLINE_DROPDOWN_PALLET;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_GOTO_FRONTLINE_DROPDOWN_PALLET:
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_CAME_FRONTLINE_DROPDOWN_PALLET;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_CAME_FRONTLINE_DROPDOWN_PALLET:  // đang trong tiến trình dò line và thả pallet
                        rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_PALLETDOWN);
                        StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_POINT_DROP_PALLET;
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_POINT_DROP_PALLET:
                        if (true == rb.CheckPointDetectLine(p.PointDropPallet, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_DROPDOWN_PALLET;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_DROPDOWN_PALLET:
                        if (resCmd == ResponseCommand.RESPONSE_LINEDETECT_PALLETDOWN)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_GOBACK_FRONTLINE);
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_FRONTLINE;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_WAITTING_GOTO_FRONTLINE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOBACK_FRONTLINE)
                        {
                            resCmd = ResponseCommand.RESPONSE_NONE;
                            StateBufferToReturn = BufferToReturn.BUFRET_ROBOT_RELEASED;
                        }
                        break;
                    case BufferToReturn.BUFRET_ROBOT_RELEASED:  // trả robot về robotmanagement để nhận quy trình mới
                        break;
                    default:
                        break;
                }
                Thread.Sleep(5);
            }
            StateBufferToReturn = BufferToReturn.BUFRET_IDLE;
            try
            {
                ProBuferToReturn.Abort();
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
