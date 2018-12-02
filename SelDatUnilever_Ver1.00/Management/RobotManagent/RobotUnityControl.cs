using SeldatMRMS.Communication;
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
    public class RobotUnityControl:RosSocket
    {
        public event Action<Pose, Object> PoseCallBack;
        public event Action<Communication.Message> ZoneCallBack;
        public event Action<Int32> FinishStatesCallBack;
        public class Pose
        {
            public Pose() // Angle gốc
            {
                
            }
            public Pose(Point p,double AngleW) // Angle gốc
            {
                this.Position = p;
                this.AngleW = AngleW;
           }
           public Pose(double X,double Y, double AngleW) // Angle gốc
           {
                this.Position = new Point(X,Y);
                this.AngleW = AngleW;
           }
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
            public String CurrentLevel;
            public bool CriticalLevel;

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
        }
        ParamsRosSocket paramsRosSocket;
        public PropertiesRobotUnity properties;
        protected virtual void SupervisorTraffic() { }
        public RobotUnityControl()
        {
            properties.pose = new Pose();
            properties.DistanceIntersection = 40;
        }
        public void createRosTerms()
        {
            int subscription_robotInfo = this.Subscribe("/amcl_pose", "geometry_msgs/PoseWithCovarianceStamped", AmclPoseHandler);
            paramsRosSocket.publication_ctrlrobotdriving = this.Advertise("/ctrlRobotDriving", "std_msgs/Int32");
            int subscription_finishedStates = this.Subscribe("/finishedStates", "std_msgs/Int32", FinishedStatesHandler);
            paramsRosSocket.publication_robotnavigation = this.Advertise("/robot_navigation", "geometry_msgs/PoseStamped");
            paramsRosSocket.publication_checkAliveTimeOut = this.Advertise("/checkAliveTimeOut", "std_msgs/String"); 
            paramsRosSocket.publication_linedetectionctrl = this.Advertise("/linedetectionctrl", "std_msgs/Int32");
        }
        private void AmclPoseHandler(Communication.Message message)
        {
            GeometryPoseWithCovarianceStamped standardString = (GeometryPoseWithCovarianceStamped)message;
            double posX = (double)standardString.pose.pose.position.x;
            double posY = (double)standardString.pose.pose.position.y;
            double posThetaZ = (double)standardString.pose.pose.orientation.z;
            double posThetaW = (double)standardString.pose.pose.orientation.w;
            double posTheta = (double)2 * Math.Atan2(posThetaZ, posThetaW);
            properties.pose.Position = new Point(posX,posY);
            properties.pose.AngleW = posTheta;
            PoseCallBack(properties.pose, this);
        }
        private void FinishedStatesHandler(Communication.Message message)
        {
            StandardInt32 standard = (StandardInt32)message;
            FinishStatesCallBack(standard.data);
        }
        protected override void OnClosedEvent(object sender, CloseEventArgs e) {
            properties.IsConnected = false;
            base.OnClosedEvent(sender,e);
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
        protected override void OnOpenedEvent() {
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
