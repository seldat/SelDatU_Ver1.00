using DoorControllerService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static DoorControllerService.DoorService;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SeldatMRMS.Management.DoorServices
{
    public class DoorManagementService
    {
        public ListCollectionView Grouped_PropertiesDoor { get; private set; }
        public List<DoorInfoConfig> PropertiesDoor_List;
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
            PropertiesDoor_List = new List<DoorInfoConfig>();
            Grouped_PropertiesDoor = (ListCollectionView)CollectionViewSource.GetDefaultView(PropertiesDoor_List);
        }

        public void AddConfig()
        {
            DoorInfoConfig ptemp = new DoorInfoConfig();
            ptemp.IdStr = (PropertiesDoor_List.Count + 1);
            ptemp.Ip = "192.168.1.2";
            ptemp.Port = 10001;
            ptemp.infoPallet = "{ \"pallet\":\"null\",\"bay\":1,\"hasSubLine\":\"no\",\"direction\":\"null\",\"row\":0}";
            ptemp.PointFrontLineStr = "1,2,3";
            PropertiesDoor_List.Add(ptemp);
            Grouped_PropertiesDoor.Refresh();
        }
        public void SaveConfig(DataGrid datagrid)
        {
            String path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ConfigCharge.json");
            System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(datagrid.ItemsSource, Formatting.Indented));
        }
        public bool LoadConfigure()
        {
            String path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ConfigCharge.json");
            if (!File.Exists(path))
            {
                File.Create(path);
                return false;
            }
            else
            {
                try
                {
                    String data = File.ReadAllText(path);
                    if (data.Length > 0)
                    {
                        List<DoorInfoConfig> tempPropertiestcharge = JsonConvert.DeserializeObject<List<DoorInfoConfig>>(data);
                        tempPropertiestcharge.ForEach(e => PropertiesDoor_List.Add(e));
                        Grouped_PropertiesDoor.Refresh();
                        return true;
                    }
                }
                catch { }
            }
            return false;
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
                ptemp.Id = (DoorId)double.Parse(row.Field<String>("ID"));
                ptemp.Ip = row.Field<String>("IP");
                ptemp.Port = int.Parse(row.Field<String>("Port"));
                ptemp.PointCheckInGate = new Pose(double.Parse(row.Field<String>("PointCheckInGate").Split(',')[0]),
                                                double.Parse(row.Field<String>("PointCheckInGate").Split(',')[1]),
                                                double.Parse(row.Field<String>("PointCheckInGate").Split(',')[2]));
                ptemp.PointFrontLine = new Pose(double.Parse(row.Field<String>("PointFrontLine").Split(',')[0]),
                                                double.Parse(row.Field<String>("PointFrontLine").Split(',')[1]),
                                                double.Parse(row.Field<String>("PointFrontLine").Split(',')[2]));
                ptemp.infoPallet = row.Field<String>("InfoPallet").ToString();
 
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
