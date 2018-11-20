using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SeldatMRMS
{
    public class DBProcedureService
    {
        public DBProcedureService() { }
        public struct RobotSensor
        {
            public bool sensor1 { get; set; }
            public bool sensor2 { get; set; }
            public bool sensor3 { get; set; }
        }
        public class RobotTrackingInform
        {
            public String CurrentProcedure;
            public Point location;
            public String status; // Finish/ Error
            public RobotSensor CurrentRobotSensor;
        }
        public struct ProcedureDataItems
        {
            public String ProcedureID { get; set; }
            public String OperationType { get; set; }
            public String RobotTaskID { get; set; }
            public DateTime StartTaskTime { get; set; }
            public String ProductName { get; set; }
            public String ProductCode { get; set; }
            public String StationInfo { get; set; } //[name station]-[line number]-[pallet in line]
            public String DeliveryInfo { get; set; } //[Machine]-[Name];[Hopper];[Gate];[Return]-[name pallet in return]
            public DateTime EndTime { get; set; }
            public String StatusProcedureDelivered { get; set; }
            public String ErrorStatusID { get; set; } // if have
        }
        public struct RobotDataItems
        {
            public String RobotTaskID;
            public RobotSensor RobotSensorStatus;
            public double LevelBatteryStart;
            public double LevelBatteryEnd;
            public List <RobotTrackingInform> RobotTrackingInformList;
        }
    }
}
