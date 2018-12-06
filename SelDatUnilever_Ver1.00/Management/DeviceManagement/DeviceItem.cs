using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;

namespace SelDatUnilever_Ver1._00.Management.DeviceManagement
{
    public class DeviceItem
    {
        public enum CommandRequest
        {
            CMD_DATA_ORDER=100,
            CMD_DATA_STATE=101
        }
        public struct OneOrder
        {
            public String BufferID { get; set; }
            public String ProductID { get; set; }
            public Pose   Pose;
        }
        public string DeviceID { get; set; }
        public string CodeID { get; set; }
        public List<OneOrder> OneOrderList { get; set; }
        public CommandRequest pCommandRequest;
        public int OrderedAmount = 0;
        public int DoneAmount = 0;
        public DeviceItem() {
            OneOrderList = new List<OneOrder>();
        }
        public void state()
        {
            switch(pCommandRequest)
            {
                case CommandRequest.CMD_DATA_ORDER:
                    break;
                case CommandRequest.CMD_DATA_STATE:
                    break;
            }
        }
    }
}
