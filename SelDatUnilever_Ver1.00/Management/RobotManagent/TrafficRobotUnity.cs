using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SeldatMRMS.Management
{
    public class TrafficRobotUnity:RobotUnityService
    {
        public class PriorityLevel
        {
            public PriorityLevel()
            {
                this.IndexOnMainRoad = 0;
                this.OnAuthorizedPriorityProcedure = false;
            }
            public int IndexOnMainRoad { get; set; } //  Index on Road;
            public bool OnAuthorizedPriorityProcedure { get; set; }

        }
        public enum TrafficBehaviorState
        {
            HEADER_TOUCH_TAIL,
            HEADER_TOUCH_HEADER,
            HEADER_TOUCH_SIDE,
            HEADER_TOUCH_NOTOUCH
        }
        private List<RobotUnity> RobotUnitylist;
        private Dictionary<String,RobotUnity> RobotUnityRiskList=new Dictionary<string, RobotUnity>();
        private TrafficBehaviorState TrafficBehaviorStateTracking;
        public TrafficRobotUnity() : base() { }
        public PriorityLevel PrioritLevelRegister;
        public void RegisteRobotInAvailable(List<RobotUnity> RobotUnitylist)
        {
            this.RobotUnitylist = RobotUnitylist;
            TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_NOTOUCH;
            PrioritLevelRegister = new PriorityLevel();
        }
        public RobotUnity CheckIntersection()
        {
            RobotUnity robot = null;
            if(RobotUnityRiskList.Count>0)
            {
                foreach(RobotUnity r in RobotUnityRiskList.Values)
                {
                   bool onTouch= r.FindHeaderIntersectsFullRiskArea(this.TopHeader()) | r.FindHeaderIntersectsFullRiskArea(this.MiddleHeader()) |r. FindHeaderIntersectsFullRiskArea(this.BottomHeader());
                    if(onTouch)
                    {
                        Console.WriteLine(this.properties.NameID+" co su va cham voi "+ r.properties.NameID);
                        robot = r;
                        break;
                    }
                }
            }
            return robot;
        }
        public bool CheckSafeDistance() // KIểm tra khoản cách an toàn/ nếu đang trong vùng close với robot khác thì giảm tốc độ, chuyển sang chế độ dò risk area
        {
            bool iscloseDistance = false;
            foreach(RobotUnity r in RobotUnitylist)
            {
              //  bool onFound = r.FindHeaderIsCloseRiskArea(this.TopHeader())| r.FindHeaderIsCloseRiskArea(this.BottomHeader())| r.FindHeaderIsCloseRiskArea(this.MiddleHeader());
                bool onFound =  r.FindHeaderIsCloseRiskArea(this.MiddleHeader());

                if (onFound)
                {
                   
                    // if robot in list is near but add in risk list robot

                    SetSpeed(RobotSpeedLevel.ROBOT_SPEED_SLOW);
                    if(!RobotUnityRiskList.ContainsKey(r.properties.NameID) && !r.properties.NameID.Equals(this.properties.NameID))
                    {
                        Console.WriteLine(this.properties.NameID+" khoan cach gan " + r.properties.NameID);
                        RobotUnityRiskList.Add(r.properties.NameID,r);
                       
                    }
                    // reduce speed robot control
                    iscloseDistance = true;
                }
                else
                {
                    // if robot in list is far but before registe in list, must remove in list
                    RemoveRiskList(r.properties.NameID);
                }
            }
            return iscloseDistance;
        }
        public void RemoveRiskList(String NameID)
        {
            if(RobotUnityRiskList.ContainsKey(NameID))
            {
                RobotUnityRiskList.Remove(NameID);
            }
        }
        public void DetectTouchedPosition(RobotUnity robot) // determine traffic state
        {
            /*if (robot.FindHeaderIntersectsRiskAreaHeader(this.TopHeader()) || robot.FindHeaderIntersectsRiskAreaHeader(this.MiddleHeader())|| robot.FindHeaderIntersectsRiskAreaHeader(this.BottomHeader()))
            {
                Console.WriteLine(this.properties.NameID +" Đa co va cham voi vùng Header "+ robot.properties.NameID);
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_HEADER;
            }
            if (robot.FindHeaderIntersectsRiskAreaTail(this.TopHeader())|| robot.FindHeaderIntersectsRiskAreaTail(this.MiddleHeader())|| robot.FindHeaderIntersectsRiskAreaTail(this.BottomHeader()))
            {
                 Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Tail " + robot.properties.NameID);
                 TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_TAIL;
            }
            if (robot.FindHeaderIntersectsRiskAreaRightSide(this.TopHeader())|| robot.FindHeaderIntersectsRiskAreaRightSide(this.MiddleHeader())|| robot.FindHeaderIntersectsRiskAreaRightSide(this.BottomHeader()))
            {
                Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Right Side " + robot.properties.NameID);
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_SIDE;
            }
            if (robot.FindHeaderIntersectsRiskAreaLeftSide(this.TopHeader()) || robot.FindHeaderIntersectsRiskAreaLeftSide(this.MiddleHeader()) || robot.FindHeaderIntersectsRiskAreaLeftSide(this.BottomHeader()))
            {
                 Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Left Side " + robot.properties.NameID);
                 TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_SIDE;
            }*/
            if ( robot.FindHeaderIntersectsRiskAreaHeader(this.MiddleHeader()))
            {
                Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Header " + robot.properties.NameID);
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_HEADER;
            }
            if (robot.FindHeaderIntersectsRiskAreaTail(this.MiddleHeader()))
            {
                Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Tail " + robot.properties.NameID);
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_TAIL;
            }
            if ( robot.FindHeaderIntersectsRiskAreaRightSide(this.MiddleHeader()) )
            {
                Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Right Side " + robot.properties.NameID);
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_SIDE;
            }
            if (robot.FindHeaderIntersectsRiskAreaLeftSide(this.MiddleHeader()) )
            {
                Console.WriteLine(this.properties.NameID + " Đa co va cham voi vùng Left Side " + robot.properties.NameID);
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_SIDE;
            }


        }
        public void TrafficBehavior()
        {
            switch(TrafficBehaviorStateTracking)
            {
                case TrafficBehaviorState.HEADER_TOUCH_NOTOUCH:
                    SetSpeed(RobotSpeedLevel.ROBOT_SPEED_NORMAL);
                    // robot speed normal;
                    break;
                case TrafficBehaviorState.HEADER_TOUCH_HEADER:
                    // Find condition priority
                    // index level of road
                    // procedure Flag is set
                    break;
                case TrafficBehaviorState.HEADER_TOUCH_TAIL:
                    SetSpeed(RobotSpeedLevel.ROBOT_SPEED_STOP);
                    // robot stop
                    break;
                case TrafficBehaviorState.HEADER_TOUCH_SIDE:
                    SetSpeed(RobotSpeedLevel.ROBOT_SPEED_STOP);
                    break;

            }
        }
        protected override void SupervisorTraffic() {
            if(CheckSafeDistance())
            {
                RobotUnity robot = CheckIntersection();
                if (robot != null)
                {
                    DetectTouchedPosition(robot);
                    TrafficBehavior();
                }
            }
            else
            {
                if(RobotUnityRiskList.Count>0)
                {
                    RobotUnityRiskList.Clear();
                }
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_NOTOUCH;
                TrafficBehavior();
            }
        }
    }
}
