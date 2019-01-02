using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;

namespace SeldatMRMS.Management.RobotManagent
{
    public class RobotManagementService
    {
        public Int32 AmountofRobotUnity = 3;
        public class ResultRobotReady
        {
            public RobotUnity robot;
            public bool onReristryCharge = false;

        }
        public ListCollectionView Grouped_PropertiesRobotUnity { get; private set; }
        public List<PropertiesRobotUnity> PropertiesRobotUnity_List;
        Dictionary<String,RobotUnity>  RobotUnityRegistedList = new Dictionary<string, RobotUnity>();
        Dictionary<String, RobotUnity> RobotUnityWaitTaskList = new Dictionary<string, RobotUnity>();
        Dictionary<String, RobotUnity> RobotUnityReadyList = new Dictionary<string, RobotUnity>();
        public RobotManagementService() {
            //LoadRobotUnityConfigure();
          
            PropertiesRobotUnity_List = new List<PropertiesRobotUnity>();
            Grouped_PropertiesRobotUnity = (ListCollectionView)CollectionViewSource.GetDefaultView(PropertiesRobotUnity_List);
         //   LoadConfigure();
        }
        public void Add_PropertiesRobotUnity()
        {
            if (PropertiesRobotUnity_List.Count < AmountofRobotUnity)
            {
                PropertiesRobotUnity newItem = new PropertiesRobotUnity();
                newItem.NameID = Guid.NewGuid().ToString();
                PropertiesRobotUnity_List.Add(newItem);
                Grouped_PropertiesRobotUnity.Refresh();
            }
            else
            {
                MessageBox.Show(SelDatUnilever_Ver1._00.Properties.Resources.Error_Add_RobotUnity);
            }
        }
        public void SaveConfig(DataGrid datagrid)
        {
                String path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ConfigRobot.json");
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(datagrid.ItemsSource, Formatting.Indented));   
        }
        public bool LoadConfigure()
        {
            String path= Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ConfigRobot.json");
            if(!File.Exists(path))
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
                        List<PropertiesRobotUnity> tempPropertiestRobotList = JsonConvert.DeserializeObject<List<PropertiesRobotUnity>>(data);
                        tempPropertiestRobotList.ForEach(e => PropertiesRobotUnity_List.Add(e));
                        Grouped_PropertiesRobotUnity.Refresh();
                        return true;
                    }                   
                }
                catch { }
            }
            return false;
        }
        public void LoadRobotUnityConfigure()
        {
            string name = "Robot";
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
            foreach (DataRow row in data.Rows)
            {
                RobotUnity robot = new RobotUnity();
                //robot.Initialize(row);
                robot.properties.NameID = "Robot1";
                RobotUnityRegistedList.Add(robot.properties.NameID, robot);
                AddRobotUnityReadyList(robot);
            }
            con.Close();
        }
        public void AddRobotUnityWaitTaskList(RobotUnity robot)
        {
           RobotUnityWaitTaskList.Add(robot.properties.NameID,robot);
        }
        public void RemoveRobotUnityWaitTaskList(String NameID)
        {
            RobotUnityWaitTaskList.Remove(NameID);
        }
        public void DestroyAllRobotUnity()
        {
            foreach (var item in RobotUnityRegistedList.Values)
            {
                item.Dispose();
            }
            RobotUnityRegistedList.Clear();
        }
        public void DestroyRobotUnity(String NameID)
        {
            if(RobotUnityRegistedList.ContainsKey(NameID))
            {
                RobotUnity robot = RobotUnityRegistedList[NameID];
                robot.Dispose();
                RobotUnityRegistedList.Remove(NameID);
            }
   
        }
        public ResultRobotReady GetRobotUnityWaitTaskItem0()
        {
            ResultRobotReady result = null;
            if (RobotUnityWaitTaskList.Count > 0)
            {
                RobotUnity robot = RobotUnityWaitTaskList.ElementAt(0).Value;
                if (robot.getBattery())
                {
                    RemoveRobotUnityWaitTaskList(robot.properties.NameID);
                }
                result = new ResultRobotReady() { robot = robot, onReristryCharge = robot.getBattery() };
            }
            return result;
        }
        public void AddRobotUnityReadyList(RobotUnity robot)
        {
            RobotUnityReadyList.Add(robot.properties.NameID,robot);
        }
        
        public ResultRobotReady GetRobotUnityReadyItem0()
        {
            ResultRobotReady result = null;
            if (RobotUnityReadyList.Count > 0)
            {
                RobotUnity robot = RobotUnityReadyList.ElementAt(0).Value;
                if(robot.getBattery())
                {
                    RemoveRobotUnityReadyList(robot.properties.NameID);
                }
                result = new ResultRobotReady() {robot=robot, onReristryCharge=robot.getBattery()};
            }
            return result;
        }
        public void RemoveRobotUnityReadyList(String nameID)
        {
            RobotUnityReadyList.Remove(nameID);
        }
    }
}
