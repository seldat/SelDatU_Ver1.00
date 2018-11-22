using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS.Management.RobotManagent
{
    class RobotManagementService
    {
        Dictionary<String,RobotUnity> RobotUnityRegistedList = new Dictionary<string, RobotUnity>();
        //readonly Dictionary<String, RobotUnity> RobotUnityRegistedList = new Dictionary<string, RobotUnity>();
        Dictionary<String, RobotUnity> RobotUnityTaskedList = new Dictionary<string, RobotUnity>();
        public RobotManagementService() { }
        public void LoadRobotUnityConfigure()
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
                RobotUnity robot = new RobotUnity();
                robot.Initialize(row);
                RobotUnityRegistedList.Add(robot.properties.NameID,robot);
            }
            con.Close();
        }
        public void AddATaskToRobot()
        {
            foreach (var item in RobotUnityRegistedList.Values)
            {
                if(item.SelectedATask)
                {
                    RobotUnityTaskedList.Add(item.properties.NameID,item);
                }
            }
        }
        public void ReleaseRobotFromATask(String NameID)
        {
            RobotUnityTaskedList.Remove(NameID);
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
        public void VerifyRobotUnity()
        {

        }

    }
}
