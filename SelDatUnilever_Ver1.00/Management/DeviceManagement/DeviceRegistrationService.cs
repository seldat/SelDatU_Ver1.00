using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication;
using SelDatUnilever_Ver1._00.Communication.HttpServerRounter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDatUnilever_Ver1._00.Management.DeviceManagement
{
    public abstract class DeviceRegistrationService:HttpServer
    {
       public List<DeviceItem> DeviceItemList { get; set; }
       public DeviceRegistrationService(int port):base(port)
       {
            DeviceItemList = new List<DeviceItem>();
        }
       public void RemoveDeviceItem(String deviceID)
       {
            if (DeviceItemList.Count > 0)
            {
                DeviceItemList.RemoveAt(DeviceItemList.FindIndex(e => e.DeviceID == deviceID));
            }
       }
        public int HasDeviceItemAt(String deviceID)
        {
            return DeviceItemList.FindIndex(e=>e.DeviceID==deviceID);
        }
        public DeviceItem FindDeviceItem(String deviceID)
        {
            return DeviceItemList.Find(e => e.DeviceID == deviceID);
        }
        public override async Task handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            String data = inputData.ReadToEnd();
            JObject results = JObject.Parse(data);
            String deviceID = (String)results["DeviceID"];
            if (HasDeviceItemAt(deviceID) >= 0)
            {
                FindDeviceItem(deviceID).rounter(data);
            }
            else
            {
                DeviceItem deviceItem = new DeviceItem();
                deviceItem.rounter(data);
                DeviceItemList.Add(deviceItem);
            }
        }
    }
}
