using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDatUnilever_Ver1._00.Management.DeviceManagement
{
    public class CollectionDataService
    {
        protected virtual void ReceiveResponseHandler(String msg) { }
        protected BridgeClientRequest ClientRequest;
        public CollectionDataService() {
            ClientRequest = new BridgeClientRequest();
            ClientRequest.ReceiveResponseHandler += ReceiveResponseHandler;
        }
    }
}
