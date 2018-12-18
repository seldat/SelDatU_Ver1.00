using SeldatMRMS;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SelDatUnilever_Ver1._00.Management.TrafficManager
{
   public class TrafficRounterService
    {
        protected List<RobotUnity> RobotUnityListOnTraffic = new List<RobotUnity>();
        public class ZoneRegister
        {
            public String NameID { get; set; }
            public String TypeZone { get; set; }
            public int Index { get; set; }
            public Point Point1 { get; set; }
            public Point Point2 { get; set; }
            public Point Point3 { get; set; }
            public Point Point4{ get; set;}
            public String Detail { get; set; }
            public Point[] GetZone()
            {
                return new Point[4] { Point1, Point2, Point3, Point4 };
            }
        }
        public Dictionary<String, ZoneRegister> ZoneRegisterList = new Dictionary<string, ZoneRegister>();
        public TrafficRounterService() { }
        public void LoadConfigureZone()
        {
            string name = "Area";
            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            "C:\\Users\\luat.tran\\source\\repos\\Unilever\\SelDatUnilever_Ver1.00\\SelDatUnilever_Ver1.00\\Management\\TrafficManager\\Configure.xlsx" +
                            ";Extended Properties='Excel 12.0 XML;HDR=YES;';";
            OleDbConnection con = new OleDbConnection(constr);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            DataTable data = new DataTable();
            sda.Fill(data);
            double x, y;
            foreach (DataRow row in data.Rows)
            {
                ZoneRegister zone = new ZoneRegister();
                zone.NameID = row.Field<string>("Name");
                zone.Index =int.Parse(row.Field<string>("Index"));
                x = double.Parse(row.Field<string>("Point1").Split(',')[0]);
                y = double.Parse(row.Field<string>("Point1").Split(',')[1]);
                zone.Point1 = new Point(x, y);
                x = double.Parse(row.Field<string>("Point2").Split(',')[0]);
                y = double.Parse(row.Field<string>("Point2").Split(',')[1]);
                zone.Point2 = new Point(x, y);
                x = double.Parse(row.Field<string>("Point3").Split(',')[0]);
                y = double.Parse(row.Field<string>("Point3").Split(',')[1]);
                zone.Point3 = new Point(x, y);
                x = double.Parse(row.Field<string>("Point4").Split(',')[0]);
                y = double.Parse(row.Field<string>("Point4").Split(',')[1]);
                zone.Point4 = new Point(x, y);
                zone.Detail = row.Field<string>("Detail_vn");
                ZoneRegisterList.Add(zone.NameID, zone);
            }
            con.Close();
        }
        public int FindIndexZoneRegister(Point p)
        {
            int index = -1;
            foreach(ZoneRegister z in ZoneRegisterList.Values)
            {
                if(ExtensionService.IsInPolygon(z.GetZone(),p))
                {
                    index = z.Index;
                    break;
                }
            }
            return index;
        }
        public int FindAmoutOfRobotUnityinArea(String areaName)
        {
            int amout=0;
            foreach(RobotUnity r in RobotUnityListOnTraffic)
            {

                if (ExtensionService.IsInPolygon(ZoneRegisterList["areaName"].GetZone(), r.properties.pose.Position))
                {
                    amout++;
                }
            }
            return amout;
        }
        public String DetermineRobotUnityinArea(Point position)
        {
            String zoneName = "";
            bool hasRobot = false;
            foreach (var r in ZoneRegisterList.Values) // xác định khu vực đến
            {

                if (ExtensionService.IsInPolygon(r.GetZone(), position))
                {
                    zoneName = r.NameID;
                    break;
                }
            }
            return zoneName;
        }
        public bool HasRobotUnityinArea(Point goal)
        {
            String zoneName = "";
            bool hasRobot = false;
            foreach (var r in ZoneRegisterList.Values) // xác định khu vực đến
            {

                if (ExtensionService.IsInPolygon(r.GetZone(), goal))
                {
                    zoneName = r.NameID;
                    break;
                }
            }
            foreach (RobotUnity r in RobotUnityListOnTraffic) // xác định robot có trong khu vực
            {

                if (ExtensionService.IsInPolygon(ZoneRegisterList[zoneName].GetZone(), r.properties.pose.Position))
                {
                    hasRobot = true;
                }
            }
            return hasRobot;
        }
        public bool HasRobotUnityinArea(String AreaName)
        { 
            bool hasRobot = false;
            foreach (RobotUnity r in RobotUnityListOnTraffic) // xác định robot có trong khu vực
            {

                if (ExtensionService.IsInPolygon(ZoneRegisterList[AreaName].GetZone(), r.properties.pose.Position))
                {
                    hasRobot = true;
                }
            }
            return hasRobot;
        }

    }
}
