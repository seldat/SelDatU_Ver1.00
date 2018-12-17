using SeldatMRMS.Management.RobotManagent;
using SelDatUnilever_Ver1._00.Management.ChargerCtrl;
using System;
using System.Diagnostics;
using System.Threading;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;
using static SelDatUnilever_Ver1._00.Management.ChargerCtrl.ChargerCtrl;
using static SelDatUnilever_Ver1._00.Management.ComSocket.RouterComPort;

namespace SeldatMRMS
{
    public class ProcedureRobotToCharger : ProcedureControlServices
    {

        Thread ProRobotToCharger;
        RobotUnity robot;
        ResponseCommand resCmd;
        RobotGoToCharge StateRobotToCharge;
        ChargerCtrl chargerCtrl;
        DataReceive batLevel;
        DataReceive statusCharger;
        const UInt32 TIME_OUT_WAIT_STATE = 60000;
        const UInt32 TIME_OUT_ROBOT_RECONNECT_SERVER = 180000;

        public byte getBatteryLevel(){
            return batLevel.data[0];
        }
        public byte getStatusCharger(){
            return statusCharger.data[0];
        }
        public ProcedureRobotToCharger(RobotUnity robot,ChargerManagementService charger,ChargerId id) : base(robot, null)
        {
            StateRobotToCharge = RobotGoToCharge.ROBCHAR_IDLE;
            batLevel = new DataReceive();
            statusCharger = new DataReceive();
            this.robot = robot;
            ChargerId id_t = id;
            switch (id_t)
            {
                case ChargerId.CHARGER_ID_1:
                    chargerCtrl = charger.ChargerStation_1;
                    break;
                case ChargerId.CHARGER_ID_2:
                    chargerCtrl = charger.ChargerStation_2;
                    break;
                case ChargerId.CHARGER_ID_3:
                    chargerCtrl = charger.ChargerStation_3;
                    break;
                case ChargerId.CHARGER_ID_4:
                    chargerCtrl = charger.ChargerStation_4;
                    break;
                default: break;
            }
        }
        public void Start(String content, RobotGoToCharge state = RobotGoToCharge.ROBCHAR_CHARGER_CHECKSTATUS)
        {
            StateRobotToCharge = state;
            ProRobotToCharger = new Thread(this.Procedure);
            ProRobotToCharger.Name = content;
            ProRobotToCharger.Start(this);
        }
        public void Destroy()
        {
            StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_RELEASED;
        }
        public void Procedure(object ojb)
        {
            ProcedureRobotToCharger RbToChar = (ProcedureRobotToCharger)ojb;
            RobotUnity rb = RbToChar.robot;
            while (StateRobotToCharge != RobotGoToCharge.ROBCHAR_ROBOT_RELEASED)
            {
                switch (StateRobotToCharge)
                {
                    case RobotGoToCharge.ROBCHAR_IDLE: break;
                    case RobotGoToCharge.ROBCHAR_CHARGER_CHECKSTATUS:
                        if(true == chargerCtrl.WaitState(ChargerState.ST_READY,TIME_OUT_WAIT_STATE)){
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_ALLOW_CUTOFF_POWER_ROBOT;
                        }
                        break; //kiểm tra kết nối và trạng thái sạc
                    case RobotGoToCharge.ROBCHAR_ROBOT_GOTO_CHARGER:
                        rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_CHARGEAREA);
                        break;
                    case RobotGoToCharge.ROBCHAR_ROBOT_START_CHARGE:
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOTO_POSITION)
                        {
                            chargerCtrl.StartCharge();
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_WAITTING_ROBOT_CONTACT_CHARGER;
                        }
                        break;
                    case RobotGoToCharge.ROBCHAR_WAITTING_ROBOT_CONTACT_CHARGER: 
                        if (true == chargerCtrl.WaitState(ChargerState.ST_CONTACT_GOOD,TIME_OUT_WAIT_STATE))
                        {
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_ALLOW_CUTOFF_POWER_ROBOT;
                        }
                        else if (true == chargerCtrl.WaitState(ChargerState.ST_CONTACT_FAIL,TIME_OUT_WAIT_STATE))
                        {
                            while(true){
                                Thread.Sleep(1000);
                            }
                        }
                        break; //robot tiep xuc tram sac        
                    case RobotGoToCharge.ROBCHAR_ROBOT_ALLOW_CUTOFF_POWER_ROBOT:
                        rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_TURNOFF_PC);
                        StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_WAITTING_CUTOFF_POWER_PC;
                        break; //cho phép cắt nguồn robot
                    case RobotGoToCharge.ROBCHAR_ROBOT_WAITTING_CUTOFF_POWER_PC:
                        if(true != rb.properties.IsConnected){
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_WAITTING_CHARGEBATTERY;    
                        }
                        break;
                    case RobotGoToCharge.ROBCHAR_WAITTING_CHARGEBATTERY:
                         if (true == chargerCtrl.WaitChargeFull(ref batLevel,ref statusCharger))
                        {
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_FINISHED_CHARGEBATTERY;
                        }
                        break; //dợi charge battery và thông tin giao tiếp server và trạm sạc
                    case RobotGoToCharge.ROBCHAR_FINISHED_CHARGEBATTERY:
                        StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_WAITING_RECONNECTING;    
                        break; //Hoàn Thành charge battery và thông tin giao tiếp server và trạm sạc
                    case RobotGoToCharge.ROBCHAR_ROBOT_WAITING_RECONNECTING:
                        if(true == CheckReconnectServer(TIME_OUT_ROBOT_RECONNECT_SERVER)){
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_STATUS_GOOD_OPERATION;  
                        }
                        else{
                            StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_STATUS_BAD_OPERATION;
                        }
                        break; //Robot mở nguồng và đợi connect lại
                    case RobotGoToCharge.ROBCHAR_ROBOT_STATUS_GOOD_OPERATION: 
                        StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_RELEASED;
                        break;
                    case RobotGoToCharge.ROBCHAR_ROBOT_STATUS_BAD_OPERATION: 
                        StateRobotToCharge = RobotGoToCharge.ROBCHAR_ROBOT_RELEASED;
                        break;
                    case RobotGoToCharge.ROBCHAR_ROBOT_RELEASED:

                        break; // trả robot về robotmanagement để nhận quy trình mới
                }
                Thread.Sleep(5);
            }
            StateRobotToCharge = RobotGoToCharge.ROBCHAR_IDLE;
            try
            {
                ProRobotToCharger.Abort();
            }
            catch (System.Exception)
            {
                Console.WriteLine("faillllllllllllllllllllll");
                throw;
            }
        }

        private bool CheckReconnectServer(UInt32 timeOut)
        {
            bool result = true;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do
            {
                Thread.Sleep(1000);
                if(sw.ElapsedMilliseconds > timeOut){
                    result = false;
                    break;
                }
            } while (true != robot.properties.IsConnected);
            sw.Stop();
            return result;
        }
        public override void FinishStatesCallBack(Int32 message)
        {
            this.resCmd = (ResponseCommand)message;
        }
    }
    public class ProcedureRobotToReady : ProcedureControlServices
    {
        public struct DataRobotToReady
        {
            public Pose PointFrontLineReadyStation;
            public PointDetect PointPrepareStop;
        }

        DataRobotToReady points;
        Thread ProRobotToReady;
        RobotUnity robot;
        ResponseCommand resCmd;
        RobotGoToReady StateRobotGoToReady;
        public ProcedureRobotToReady(RobotUnity robot, DataRobotToReady dataPoints) : base(robot, null)
        {
            StateRobotGoToReady = RobotGoToReady.ROBREA_IDLE;
            this.robot = robot;
            this.points = dataPoints;
        }
        public void Start(String content, RobotGoToReady state = RobotGoToReady.ROBREA_ROBOT_GOTO_FRONTLINE_READYSTATION)
        {
            StateRobotGoToReady = state;
            ProRobotToReady = new Thread(this.Procedure);
            ProRobotToReady.Name = content;
            ProRobotToReady.Start(this);
        }
        public void Destroy()
        {
            StateRobotGoToReady = RobotGoToReady.ROBREA_ROBOT_RELEASED;
        }

        public void Procedure(object ojb)
        {
            ProcedureRobotToReady RbToRd = (ProcedureRobotToReady)ojb;
            RobotUnity rb = RbToRd.robot;
            DataRobotToReady p = RbToRd.points;
            while (StateRobotGoToReady != RobotGoToReady.ROBREA_ROBOT_RELEASED)
            {
                switch (StateRobotGoToReady)
                {
                    case RobotGoToReady.ROBREA_IDLE:
                        break;
                    case RobotGoToReady.ROBREA_ROBOT_GOTO_FRONTLINE_READYSTATION: // ROBOT cho tiến vào vị trí đầu line charge su dung laser
                        rb.SendPoseStamped(p.PointFrontLineReadyStation);
                        StateRobotGoToReady = RobotGoToReady.ROBREA_ROBOT_WAITTING_GOTO_READYSTATION;
                        break;
                    case RobotGoToReady.ROBREA_ROBOT_WAITTING_GOTO_READYSTATION: // Robot dang di toi dau line ready station
                        if (resCmd == ResponseCommand.RESPONSE_LASER_CAME_POINT)
                        {
                            rb.SendCmdLineDetectionCtrl(RequestCommandLineDetect.REQUEST_LINEDETECT_READYAREA);
                            StateRobotGoToReady = RobotGoToReady.ROBREA_ROBOT_WAIITNG_DETECTLINE_TO_READYSTATION;
                        }
                        break;
                    case RobotGoToReady.ROBREA_ROBOT_WAIITNG_DETECTLINE_TO_READYSTATION: // đang đợi dò line để đến vị trí line trong buffer
                        if (true == rb.CheckPointDetectLine(p.PointPrepareStop, rb))
                        {
                            rb.SendCmdPosPallet(RequestCommandPosPallet.REQUEST_LINEDETECT_COMING_POSITION);
                            StateRobotGoToReady = RobotGoToReady.ROBREA_ROBOT_WAITTING_CAME_POSITION_READYSTATION;
                        }
                        break;
                    case RobotGoToReady.ROBREA_ROBOT_WAITTING_CAME_POSITION_READYSTATION: // đến vị trả robot về robotmanagement để nhận quy trình mới
                        if (resCmd == ResponseCommand.RESPONSE_FINISH_GOTO_POSITION)
                        {
                            StateRobotGoToReady = RobotGoToReady.ROBREA_ROBOT_RELEASED;
                        }
                        break;
                    case RobotGoToReady.ROBREA_ROBOT_RELEASED:
                        break;
                }
                Thread.Sleep(5);
            }
            StateRobotGoToReady = RobotGoToReady.ROBREA_IDLE;
            try
            {
                ProRobotToReady.Abort();
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
