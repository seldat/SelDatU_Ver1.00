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
            CMD_DATA_ORDER_BUFFERTOMACHINE=100,
            CMD_DATA_ORDER_RETURN = 100,
            CMD_DATA_ORDER_FORKLIFT = 101,
            CMD_DATA_STATE =102
        }
        public class ForkLiftToBufferTask
        {
            public Pose CheckInPose;
            public LinePose LineAtGate; // line tại cổng
            public LinePose MainLineInBuffer; // line dò trong buffer đến subLine
            public LinePose SubLineInBuffer; // line dò pallet
        }
        public class BufferToMachineTask
        {
            public Pose CheckInPoseBuffer;
            public Pose PosePalletAtMachine;
            public LinePose MainLineInBuffer; // line dò trong buffer đến subLine
            public LinePose SubLineInBuffer; // line dò pallet
        }
        public class PalletReturnAtMachine
        {
            public Pose CheckInPoseBuffer;
            public Pose CheckInPoseReturn;
            public Pose PosePalletAtMachine;
            public LinePose MainLineInReturn; // line dò trong buffer đến subLine
            public LinePose SubLineInReturn; // line dò pallet
        }
        public class OneOrder : CollectionDataService
        {
            
            public String PalletID;
            public String BufferID { get; set; }
            public String ProductID { get; set; }
            public String TypeRequest; // FL: ForkLift// BM: BUFFER MACHINE // PR: Pallet return
            public ForkLiftToBufferTask  ForkLiftToBufferTaskRequested;
            public BufferToMachineTask   BufferToMachineTaskRequested;
            public PalletReturnAtMachine PalletReturnAtMachineRequested;
            public long   datetime;
            public bool   status = false; // chua hoan thanh
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
        public void ParseDataOfBufferMachine(String json)
        {
            JObject results = JObject.Parse(json);
            string deviceID = (string)results["DeviceID"];
            foreach (var result in results["Tasks"])
            {
                OneOrder order = new OneOrder();
                order.PalletID= (string)result["PalletID"];
                order.BufferID= (string)result["BufferID"];
                order.ProductID = (string)result["ProductID"];
                //order.PosePalletAtMachine=new Pose() { Position = new System.Windows.Point((double)result["Position"]["X"], (double)result["Position"]["Y"]),AngleW= (double)result["Position"]["Angle"] };
                OneOrderList.Add(order);
            }
                

        }
        public void ParseDataForkLift(String json)
        {
            JObject results = JObject.Parse(json);
        }

    }
}
