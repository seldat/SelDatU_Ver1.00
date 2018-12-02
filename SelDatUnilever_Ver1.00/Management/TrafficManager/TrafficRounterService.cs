using SeldatMRMS;
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
    class TrafficRounterService
    {
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
            string name = "Sheet1";
            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            "C:\\Users\\Charlie\\Desktop\\test.xls" +
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
                zone.TypeZone = row.Field<string>("TypeZone");
                zone.Index = int.Parse(row.Field<string>("Index"));
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
                zone.Detail = row.Field<string>("Detail_en");
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
    }
}
