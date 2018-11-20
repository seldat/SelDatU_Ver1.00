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
                this.OnMainRoad = 0;
                this.OnAuthorizedPriorityProcedure = false;
            }
            public int OnMainRoad { get; set; } //  Index on Road;
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
        private TrafficBehaviorState TrafficBehaviorStateTracking;
        public TrafficRobotUnity() : base() { }
        public void RegisteRobotInAvailable(List<RobotUnity> RobotUnitylist)
        {
            this.RobotUnitylist = RobotUnitylist;
            TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_NOTOUCH;
        }
        public bool CheckIntersection()
        {
            return FindHeaderIntersectsFullRiskArea(this.properties.pose.Position);
        }
        public bool CheckSafeDistance() // KIểm tra khoản cách an toàn/ nếu đang trong vùng close với robot khác thì giảm tốc độ, chuyển sang chế độ dò risk area
        {
            bool iscloseDistance = false;
            foreach(RobotUnity r in RobotUnitylist)
            {
                iscloseDistance = r.FindHeaderIsCloseRiskArea(this.properties.pose.Position);
                if (iscloseDistance)
                {
                    SetSpeed(RobotSpeedLevel.ROBOT_SPEED_SLOW);
                    // reduce speed robot control
                    break;
                }
            }
            return iscloseDistance;
        }
        public void DetectTouchedPosition() // determine traffic state
        {
            foreach (RobotUnity r in RobotUnitylist)
            {
                if (r.FindHeaderIntersectsRiskAreaHeader(this.properties.pose.Position))
                {
                    TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_HEADER;
                    break;
                }
                else if (r.FindHeaderIntersectsRiskAreaTail(this.properties.pose.Position))
                {
                    TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_TAIL;
                    break;
                }
                else if (r.FindHeaderIntersectsRiskAreaRightSide(this.properties.pose.Position))
                {
                    TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_SIDE;
                    break;
                }
                else if (r.FindHeaderIntersectsRiskAreaLeftSide(this.properties.pose.Position))
                {
                    TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_SIDE;
                    break;
                }
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
                if(CheckIntersection())
                {
                    DetectTouchedPosition();
                    TrafficBehavior();
                }
            }
            else
            {
                TrafficBehaviorStateTracking = TrafficBehaviorState.HEADER_TOUCH_NOTOUCH;
                TrafficBehavior();
            }
        }
        public void DetermineOperatingArea()
        {
            // dùng tọa độ xá định khu vực hoạt động
            // cập nhật chỉ số ưu tiên
        }
    }
}
