using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS.Management.RobotManagent
{
    public class RobotUnity:RobotBaseService
    {
        public LoadedConfigureInformation loadConfigureInformation;
        public RobotUnity()
        {

        }
        public void Initialize(DataRow row)
        {
            try
            {
                properties.NameID =row.Field<string>("Name ID");
                properties.URL = row.Field<string>("URL");
                properties.Width = double.Parse(row.Field<string>("Width"));
                properties.Height = double.Parse(row.Field<string>("Height"));
                properties.Length = double.Parse(row.Field<string>("Length"));
                properties.L1= double.Parse(row.Field<string>("L1"));
                properties.L2 = double.Parse(row.Field<string>("L2"));
                properties.WS = double.Parse(row.Field<string>("WS"));
                properties.DistanceIntersection = double.Parse(row.Field<string>("Distance Intersection"));
                // double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]);
                loadConfigureInformation.IsLoadedStatus = true;
            }
            catch {
                loadConfigureInformation.IsLoadedStatus = false;
                }
        }
        
    }
}
