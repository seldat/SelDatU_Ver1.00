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
            TABLET_MACHINE = 10000,
            TABLET_FORKLIFT = 10001
        }
        public enum CommandRequest
        {
            CMD_DATA_ORDER_BUFFERTOMACHINE = 100,
            CMD_DATA_ORDER_RETURN = 100,
            CMD_DATA_ORDER_FORKLIFT = 101,
            CMD_DATA_STATE = 102
        }

        public class OneOrder : CollectionDataService
        {
            public String palletID;
            public String bufferID { get; set; }
            public String productID { get; set; }
            public String typeRequest; // FL: ForkLift// BM: BUFFER MACHINE // PR: Pallet return
            public long datetime;
            public bool status = false; // chua hoan thanh
        }
        public string deviceID { get; set; } // dia chi Emei
        public string codeID { get; set; }
        public List<OneOrder> oneOrderList { get; set; }
        public int orderedAmount = 0;
        public int doneAmount = 0;
        public DeviceItem()
        {
            oneOrderList = new List<OneOrder>();
        }
        public void state(CommandRequest pCommandRequest, String data)
        {
            switch (pCommandRequest)
            {
                case CommandRequest.CMD_DATA_ORDER_BUFFERTOMACHINE:
                    break;
                case CommandRequest.CMD_DATA_ORDER_FORKLIFT:
                    break;
                case CommandRequest.CMD_DATA_STATE:
                    break;
            }
        }
        public void RemoveFirstOrder()
        {
            if (oneOrderList.Count > 0)
            {
                oneOrderList.RemoveAt(0);
            }
        }
        public void AddOrder(OneOrder hasOrder)
        {
            oneOrderList.Add(hasOrder);
        }
        public OneOrder GetOrder()
        {
            if (oneOrderList.Count > 0)
            {
                return oneOrderList[0];
            }
            return null;
        }
        public void ClearOrderList()
        {
            if (oneOrderList.Count > 0)
            {
                oneOrderList.Clear();
            }
        }
        public void rounter(String data)
        {

        }
        public void ParseDataOfBufferMachine(String json)
        {
            JObject results = JObject.Parse(json);
            string deviceID = (string)results["DeviceID"];
            foreach (var result in results["Tasks"])
            {
                OneOrder order = new OneOrder();
                order.palletID = (string)result["PalletID"];
                order.bufferID = (string)result["BufferID"];
                order.productID = (string)result["ProductID"];
                //order.PosePalletAtMachine=new Pose() { Position = new System.Windows.Point((double)result["Position"]["X"], (double)result["Position"]["Y"]),AngleW= (double)result["Position"]["Angle"] };
            }
        }

    }
}
