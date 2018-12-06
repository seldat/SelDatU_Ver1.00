using SelDatUnilever_Ver1._00.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDatUnilever_Ver1._00.Management.DeviceManagement
{
    public abstract class DeviceControlService:HttpServer
    {
       Dictionary<String, DeviceItem> DeviceItemList = new Dictionary<string, DeviceItem>();
       public DeviceControlService(int port):base(port)
       {

       }
       public void ParseInfomation()
       {

       }
       public void RemoveDeviceItem(String NameKey)
       {
            DeviceItemList.Remove(NameKey);
       }
    }
}
