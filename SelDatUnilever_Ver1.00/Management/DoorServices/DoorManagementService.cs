using DoorControllerService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS.Management.DoorServices
{
    public class DoorManagementService
    {
        public DoorMezzamineUp DoorMezzamineUpUnity { get; set; }
        public DoorMezzamineReturn DoorMezzamineReturnUnity { get; set; }
        public DoorElevator DoorElevatorUnity { get; set; }
        public DoorManagementService() {
            DoorMezzamineUpUnity = new DoorMezzamineUp();
            DoorMezzamineReturnUnity = new DoorMezzamineReturn();
            DoorElevatorUnity = new DoorElevator();
        }
        public void LoadDoorConfigure()
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
            foreach (DataRow row in data.Rows) { }
            con.Close();
        }
        public void ResetAllDoors()
        {

        }
        public void DisposeAllDoors()
        {

        }
    }
}
