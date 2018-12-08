using Newtonsoft.Json.Linq;
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
        public enum TabletConTrol
        {
            TABLET_MACHINE=10000,
            TABLET_FORKLIFT=10001
        }
        public enum CommandRequest
        {
            CMD_DATA_ORDER_MACHINE=100,
            CMD_DATA_ORDER_FORKLIFT = 101,
            CMD_DATA_STATE =102
        }
        public class OneOrder
        {
            public String BufferID { get; set; }
            public String ProductID { get; set; }
            public Pose   Pose;
            public bool status = false; // chua hoan thanh
        }
        public string DeviceID { get; set; } // dia chi Emei
        public string CodeID { get; set; }
        public List<OneOrder> OneOrderList { get; set; }
        public int OrderedAmount = 0;
        public int DoneAmount = 0;
        public DeviceItem() {
            OneOrderList = new List<OneOrder>();
        }
        public void state(CommandRequest pCommandRequest,String data)
        {
            switch(pCommandRequest)
            {
                case CommandRequest.CMD_DATA_ORDER_MACHINE:
                    ParseDataMachine(data);
                    break;
                case CommandRequest.CMD_DATA_ORDER_FORKLIFT:
                    break;
                case CommandRequest.CMD_DATA_STATE:
                    break;
            }
        }
        public void RemoveFirstOrder()
        {
            if(OneOrderList.Count>0)
            {
                OneOrderList.RemoveAt(0);
            }
        }
        public void AddOrder(OneOrder hasOrder)
        {
            OneOrderList.Add(hasOrder);
        }
        public OneOrder GetOrder()
        {
            if (OneOrderList.Count > 0)
            {
                return OneOrderList[0];
            }
            return null;
        }
        public void ClearOrderList()
        {
            if(OneOrderList.Count>0)
            {
                OneOrderList.Clear();
            }
        }
        public void rounter(String data)
        {

        }
        public void ParseDataMachine(String json)
        {
            JObject results = JObject.Parse(json);
        }
        public void ParseDataForkLift(String json)
        {
            JObject results = JObject.Parse(json);
        }

    }
}
