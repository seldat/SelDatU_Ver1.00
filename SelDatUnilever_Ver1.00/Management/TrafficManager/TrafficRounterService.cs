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
            public String NameID;
            public String TypeZone;
            public int index;
            public Point point1;
            public Point point2;
            public Point point3;
            public Point point4;
            public Point[] GetZone()
            {
                return new Point[4] { point1, point2, point3, point4 };
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
            foreach (DataRow row in data.Rows)
            {
                ZoneRegister zone = new ZoneRegister();
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
                    index = z.index;
                    break;
                }
            }
            return index;
        }
    }
}
