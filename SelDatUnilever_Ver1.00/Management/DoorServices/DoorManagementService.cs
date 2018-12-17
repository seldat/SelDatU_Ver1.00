using DoorControllerService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using static DoorControllerService.DoorService;

namespace SeldatMRMS.Management.DoorServices
{
    public class DoorManagementService
    {
        private List<DoorInfoConfig> DoorInfoConfigList;
        public DoorService DoorMezzamineUpBack;
        public DoorService DoorMezzamineUpFront;
        public DoorService DoorMezzamineReturnBack;
        public DoorService DoorMezzamineReturnFront;
        public DoorElevator DoorElevator;

        public DoorManagementService(){
            LoadDoorConfigure();
            DoorMezzamineUpBack = new DoorService(DoorInfoConfigList[0]);
            DoorMezzamineUpFront = new DoorService(DoorInfoConfigList[1]);
            DoorMezzamineReturnBack = new DoorService(DoorInfoConfigList[2]);
            DoorMezzamineReturnFront = new DoorService(DoorInfoConfigList[3]);
        }
        public void LoadDoorConfigure()
        {
            string name = "Door";
            String path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Configure.xlsx");
            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            path +
                            ";Extended Properties='Excel 12.0 XML;HDR=YES;';";
            OleDbConnection con = new OleDbConnection(constr);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            con.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            DataTable data = new DataTable();
            sda.Fill(data);
            DoorInfoConfigList = new List<DoorInfoConfig>();
            foreach (DataRow row in data.Rows)
            {
                DoorInfoConfig ptemp = new DoorInfoConfig();
                ptemp.id = row.Field<DoorId>("ID");
                ptemp.ip = row.Field<String>("IP");
                ptemp.port = row.Field<Int32>("Port");
                DoorInfoConfigList.Add(ptemp);
            }
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
