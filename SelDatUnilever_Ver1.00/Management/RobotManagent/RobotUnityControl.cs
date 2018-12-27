using SeldatMRMS.Communication;
using SelDatUnilever_Ver1._00.Management.ChargerCtrl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using WebSocketSharp;

namespace SeldatMRMS.Management.RobotManagent
{

    public class RobotUnityControl : RosSocket
    {
        public event Action<int> FinishStatesCallBack;
        public event Action<Pose, Object> PoseHandler;

        private const float delBatterry = 5;
        public class Pose
        {
            public Pose(Point p, double AngleW) // Angle gốc
            {
                this.Position = p;
                this.AngleW = AngleW;
            }
            public Pose(double X, double Y, double AngleW) // Angle gốc
            {
                this.Position = new Point(X, Y);
                this.AngleW = AngleW;
           }
            public Pose() { }
           public void Destroy() // hủy vị trí robot để robot khác có thể làm việc trong quá trình detect
            {
                this.Position = new Point(-1000, -1000);
                this.AngleW = 0;
            }
           public Point Position { get; set; }
           public double AngleW { get; set; } // radian
        }
        public enum RobotSpeedLevel
        {
            ROBOT_SPEED_NORMAL = 100,
            ROBOT_SPEED_SLOW = 50,
            ROBOT_SPEED_STOP = 0,
        }
        public bool getBattery()
        {
            return properties.BatteryReadyWork;
        }

        public struct PropertiesRobotUnity
        {
            [CategoryAttribute("ID Settings"), DescriptionAttribute("Name of the customer")]
            public String NameID;
            public double DistanceIntersection { get; set; }
            public Pose pose{ get; set; }
            public String URL;
            public bool IsConnected { get; set; }
            public double L1 { get; set;}
            public double L2 { get; set;}
            public double WS { get; set; }
            public double Width {get; set;}
            public double Length {get; set;}
            public double Height { get; set; }
            [CategoryAttribute("Laser"), DescriptionAttribute("Name of the customer")]
            public String LaserOperation;
            [CategoryAttribute("Battery"), DescriptionAttribute("Name of the customer")]
            public float BatteryLevelRb;
            public float BatteryLowLevel;
            public bool BatteryReadyWork;
            public ChargerCtrl.ChargerId chargeID;

        }

        public enum RequestCommandLineDetect
        {
            REQUEST_CHARGECTRL_CANCEL = 1201,
            REQUEST_LINEDETECT_PALLETUP=1203,
            REQUEST_LINEDETECT_PALLETDOWN=1204,

            REQUEST_LINEDETECT_CHARGEAREA=1206,  
            REQUEST_RETURN_LINE_CHARGEARE  = 1207,  
            REQUEST_LINEDETECT_READYAREA=1208,
        }

        public enum RequestCommandPosPallet{
            REQUEST_LINEDETECT_COMING_POSITION = 1205,
            REQUEST_TURN_LEFT = 1210,
            REQUEST_TURN_RIGHT = 1211,
            REQUEST_FORWARD_DIRECTION = 1212,
            REQUEST_GOBACK_FRONTLINE = 1213,
            REQUEST_TURNOFF_PC = 1214
        }

        public enum ResponseCommand
        {
            RESPONSE_NONE = 0,
            RESPONSE_LASER_CAME_POINT = 2000,
            RESPONSE_LINEDETECT_PALLETUP = 3203,
            RESPONSE_LINEDETECT_PALLETDOWN = 3204,
            RESPONSE_FINISH_GOTO_POSITION = 3205,
            RESPONSE_FINISH_DETECTLINE_CHARGEAREA = 3206,
            RESPONSE_FINISH_RETURN_LINE_CHARGEAREA = 3207,
            RESPONSE_FINISH_TURN_LEFT = 3210,
            RESPONSE_FINISH_TURN_RIGHT = 3211,
            RESPONSE_FINISH_GOBACK_FRONTLINE = 3213,
            RESPONSE_ERROR = 3215
        }

        public virtual void updateparams(){}
        public virtual void OnOccurencyTrigger() { }
        public virtual void OnBatteryLowTrigger() { }
        public struct ParamsRosSocket
        {
            public int publication_RobotInfo;
            public int publication_RobotParams;
            public int publication_ServerRobotCtrl;
            public int publication_CtrlRobotHardware;
            public int publication_DriveRobot;
            public int publication_BatteryRegister;
            public int publication_EmergencyRobot;
            public int publication_ctrlrobotdriving;
            public int publication_robotnavigation;
            public int publication_linedetectionctrl;
            public int publication_checkAliveTimeOut;
            public int publication_postPallet;
            public int publication_cmdAreaPallet;
            public int publication_finishedStates;
            public int publication_batteryvol;
        }
        ParamsRosSocket paramsRosSocket;
        public PropertiesRobotUnity properties;
        protected virtual void SupervisorTraffic() { }
        public RobotUnityControl()
        {
            properties.pose = new Pose();
            properties.DistanceIntersection = 40;
            properties.BatteryLowLevel = 25;
            properties.BatteryReadyWork = true;
        }
        public void createRosTerms()
        {
            int subscription_robotInfo = this.Subscribe("/amcl_pose", "geometry_msgs/PoseWithCovarianceStamped", AmclPoseHandler);
            paramsRosSocket.publication_ctrlrobotdriving = this.Advertise("/ctrlRobotDriving", "std_msgs/Int32");
            int subscription_finishedStates = this.Subscribe("/finishedStates", "std_msgs/Int32", FinishedStatesHandler);
            paramsRosSocket.publication_finishedStates = this.Advertise("/finishedStates", "std_msgs/Int32");

            paramsRosSocket.publication_robotnavigation = this.Advertise("/robot_navigation", "geometry_msgs/PoseStamped");
            paramsRosSocket.publication_checkAliveTimeOut = this.Advertise("/checkAliveTimeOut", "std_msgs/String");
            paramsRosSocket.publication_linedetectionctrl = this.Advertise("/linedetectionctrl", "std_msgs/Int32");
            paramsRosSocket.publication_postPallet = this.Advertise("/pospallet", "std_msgs/Int32");
            paramsRosSocket.publication_cmdAreaPallet = this.Advertise("/cmdAreaPallet", "std_msgs/String");
            paramsRosSocket.publication_batteryvol = this.Subscribe("/battery_vol", "std_msgs/Float32", BatteryVolHandler);
        }

        private void BatteryVolHandler(Communication.Message message)
        {
            StandardFloat32 batVal = (StandardFloat32)message;
            properties.BatteryLevelRb = batVal.data;
            if(properties.BatteryReadyWork == true){
                if(properties.BatteryLevelRb < properties.BatteryLowLevel){
                    properties.BatteryReadyWork = false;
                }
            }
            else{
                if(properties.BatteryLevelRb > (properties.BatteryLowLevel + delBatterry)){
                    properties.BatteryReadyWork = true;
                }
            }
        }

        private void AmclPoseHandler(Communication.Message message)
        {
            GeometryPoseWithCovarianceStamped standardString = (GeometryPoseWithCovarianceStamped)message;
            double posX = (double)standardString.pose.pose.position.x;
            double posY = (double)standardString.pose.pose.position.y;
            double posThetaZ = (double)standardString.pose.pose.orientation.z;
            double posThetaW = (double)standardString.pose.pose.orientation.w;
            double posTheta = (double)2 * Math.Atan2(posThetaZ, posThetaW);
            properties.pose.Position = new Point(posX, posY);
            properties.pose.AngleW = posTheta;
            PoseHandler(properties.pose, this);
        }
        private void FinishedStatesHandler(Communication.Message message)
        {
            StandardInt32 standard = (StandardInt32)message;
            ///MessageBox.Show(standard.data+"");
			FinishStatesCallBack(standard.data);

        }

        public void FinishedStatesPublish(int message)
        {
            StandardInt32 msg = new StandardInt32();
            msg.data = message;
            this.Publish(paramsRosSocket.publication_finishedStates, msg);

        }

        protected override void OnClosedEvent(object sender, CloseEventArgs e) {
            properties.IsConnected = false;
            base.OnClosedEvent(sender,e);
        }
        public void UpdateRiskAraParams(double L1,double L2,double WS, double distanceIntersection)
        {
            properties.L1 = L1;
            properties.L2 = L2;
            properties.WS = WS;
            properties.DistanceIntersection = distanceIntersection;
        }
        public void SendPoseStamped(Pose pose)
        {
            GeometryPoseStamped data=new GeometryPoseStamped();
            data.pose.position.x =(float)pose.Position.X;
            data.pose.position.y = (float)pose.Position.Y;
            data.pose.position.z = 0;
            double theta = pose.AngleW;
            data.pose.orientation.z = (float)Math.Sin(theta / 2);
            data.pose.orientation.w = (float)Math.Cos(theta / 2);
            this.Publish(paramsRosSocket.publication_robotnavigation,data);

        }
        public void SetSpeed(RobotSpeedLevel robotspeed )
        {
            StandardInt32 msg = new StandardInt32();
            msg.data = Convert.ToInt32(robotspeed);
            this.Publish(paramsRosSocket.publication_ctrlrobotdriving,msg);
        }

        public void SendCmdLineDetectionCtrl(RequestCommandLineDetect cmd){
            StandardInt32 msg = new StandardInt32();
            msg.data = Convert.ToInt32(cmd);
            this.Publish(paramsRosSocket.publication_linedetectionctrl,msg);
        }

        public void SendCmdPosPallet(RequestCommandPosPallet cmd){
            StandardInt32 msg = new StandardInt32();
            msg.data = Convert.ToInt32(cmd);
            this.Publish(paramsRosSocket.publication_linedetectionctrl, msg);
        }
        public void SendCmdAreaPallet(String cmd)
        {
            StandardString msg = new StandardString();
            msg.data = cmd;
            this.Publish(paramsRosSocket.publication_cmdAreaPallet, msg);
        }

        protected override void OnOpenedEvent()
        {
            properties.IsConnected = true;
            Console.WriteLine("connected");
            createRosTerms();
        }
        public override void Dispose()
        {
            properties.pose.Destroy();
            base.Dispose();
        }
    }
}
