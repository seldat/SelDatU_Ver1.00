using Newtonsoft.Json.Linq;
using SelDatUnilever_Ver1._00.Communication;
using SelDatUnilever_Ver1._00.Communication.HttpServerRounter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SelDatUnilever_Ver1._00.Management.DeviceManagement
{
    public class DeviceRegistrationService:HttpServer
    {
       private List<DeviceItem> deviceItemList { get; set; }
    
     public DeviceRegistrationService(int port):base(port)
       {
            deviceItemList = new List<DeviceItem>();
        }
       public void RemoveDeviceItem(String deviceID)
       {
            if (deviceItemList.Count > 0)
            {
                deviceItemList.RemoveAt(deviceItemList.FindIndex(e => e.deviceID == deviceID));
            }
       }
        public int HasDeviceItemAt(String deviceID)
        {
            return deviceItemList.FindIndex(e=>e.deviceID==deviceID);
        }
        public DeviceItem FindDeviceItem(String deviceID)
        {
            return deviceItemList.Find(e => e.deviceID == deviceID);
        }
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            String data = inputData.ReadToEnd();
            JObject results = JObject.Parse(data);
             String deviceID = (String)results["userName"];
             if (HasDeviceItemAt(deviceID) >= 0)
             {
                 FindDeviceItem(deviceID).rounter(data);
             }
             else
             {
                 DeviceItem deviceItem = new DeviceItem();
                 deviceItem.ParseData(data);
                 deviceItemList.Add(deviceItem);
             }
            p.writeSuccess();
        }
        public List<DeviceItem> GetDeviceItemList()
        {
            return deviceItemList;
        }
    }
}
