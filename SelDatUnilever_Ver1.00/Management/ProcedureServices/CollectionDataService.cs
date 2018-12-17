using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using SelDatUnilever_Ver1._00.Management.ProcedureServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SelDatUnilever_Ver1._00.Management.DeviceManagement.DeviceItem;

namespace SelDatUnilever_Ver1
{

    public class DataPallet
    {
        public DataPallet() { }
        public bool hasMainLine { get; set; }
        public double ThresholdDetectsMaker_MainLine;
        public double ThresholdDetectsMaker_SubLine;
        public double rot = 1; // 1 : CCW / -1: CW
    }
    public class CollectionDataService
    {
        public enum PalletStatus
        {
            F=200, // Free pallet
            W=201 // Have Pallet
        }
        //public int planID { get; set; }
        // public int productID { get; set; }
        // public int productDetailID { get; set; }
        public OrderItem order;
        //public String typeRequest; // FL: ForkLift// BM: BUFFER MACHINE // PR: Pallet return
        //public String activeDate;
       // public int timeWorkID;
        public List<Pose> checkInBuffer=new List<Pose>();
        public DataPallet dataPalletItem;
        protected BridgeClientRequest clientRequest;
        public CollectionDataService() {
            clientRequest = new BridgeClientRequest();
            clientRequest.ReceiveResponseHandler += ReceiveResponseHandler;
        }
        public CollectionDataService(OrderItem order)
        {
            this.order = order;
            clientRequest = new BridgeClientRequest();
            clientRequest.ReceiveResponseHandler += ReceiveResponseHandler;
        }
        public void AssignAnOrder(OrderItem order)
        {
            this.order = order;
        }
        public String RequestDataProcedure(String dataReq)
        {
            //String url = "http://localhost:8081/robot/rest/plan/getListPlanPallet";
            String url = "http://localhost:8080";
            var data =clientRequest.PostCallAPI(url, dataReq);
            if(data.Result!=null)
            {
                return data.Result;
            }
            return null;
        }
        public void GetCheckIn()
        {
            JArray results = JArray.Parse(order.dataRequest);
            foreach (var result in results)
            {
                int temp_productDetailID = (int)result["productDetailId"];
                if (temp_productDetailID == order.productDetailID)
                {
                    var bufferResults = result["buffers"];
                    var checkinResults = bufferResults[0]["bufferCheckIn"];
                    foreach (var checkinResult in checkinResults)
                    {
                        double x = (double)checkinResult["X"];
                        double y = (double)checkinResult["Y"];
                        double angle = (double)checkinResult["A"];
                        Pose poseTemp = new Pose(x, y, angle * Math.PI / 180.0);
                        checkInBuffer.Add(poseTemp);
                        break;
                    }
                }
            }
        }
        public void GetDataPallet()
        {
            JArray results = JArray.Parse(order.dataRequest);
            foreach (var result in results)
            {
                int temp_productDetailID = (int)result["productDetailId"];
                if (temp_productDetailID == order.productDetailID)
                {
                    var bufferResults = result["buffers"];
                    var palletResults = bufferResults[0]["pallets"];
                    order.palletId = (int)palletResults[0]["palletId"];
                    order.updUsrId = (int)palletResults[0]["updUsrId"];
                    var dataPalletItemResults = palletResults[0]["dataPallet"];
                    bool dataPalletItem_hasMainLine = (bool)dataPalletItemResults["hasMainLine"];
                    double dataPalletItem_rot = (double)dataPalletItemResults["rot"];
                    double dataPalletItem_mainThreshold = (double)dataPalletItemResults["mainThreshold"];
                    double dataPalletItem_subThreshold = (double)dataPalletItemResults["subThreshold"];
                    dataPalletItem = new DataPallet() { hasMainLine = dataPalletItem_hasMainLine,rot=dataPalletItem_rot,ThresholdDetectsMaker_MainLine=dataPalletItem_mainThreshold,ThresholdDetectsMaker_SubLine=dataPalletItem_subThreshold };
                    break;
                }
            }
        }
        public void UpdatePalletState(PalletStatus palletStatus)
        {
            String url = "http://192.168.1.17:8081/robot/rest/plan/updatePalletStatus";
            dynamic product = new JObject();
            product.palletId = order.palletId;
            product.palletStatus = palletStatus.ToString();
            product.updUsrId = order.updUsrId;
            var data=clientRequest.PostCallAPI(url, product.ToString());
            if (data.Result == null)
            {
                ErrorHandler(ProcedureMessages.ProcMessage.MESSAGE_ERROR_UPDATE_PALLETSTATUS);
            }
         }
        protected virtual void ReceiveResponseHandler(String msg) { }
        protected virtual void ErrorHandler(ProcedureMessages.ProcMessage procMessage) { }
    }
}
