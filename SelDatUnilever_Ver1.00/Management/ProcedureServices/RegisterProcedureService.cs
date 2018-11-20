using DoorControllerService;
using SeldatMRMS.Management;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.DBProcedureService;

namespace SeldatMRMS
{
    public class RegisterProcedureService
    {
        protected virtual bool Cancel() { return false; }
         public class RegisterProcedureItem
        {
            public ProcedureControlServices item;
            public ProcedureDataItems procedureDataItems;
            public RobotUnity robot;
            public static bool currentErrorStatus=false;
        }
        protected List<RegisterProcedureItem> RegisterProcedureItemList=new List<RegisterProcedureItem>();
        public RegisterProcedureService() {}
        public String GetAIDProcedure()
        {
            return "1234";
        }
        public enum ProcedureItemSelected
        {
            PROCEDURE_FORLIFT_TO_BUFFER,
            PROCEDURE_BUFFER_TO_MACHINE,
            PROCEDURE_BUFFER_TO_HOPPER,
        }
        public void StoreProceduresInDataBase()
        {

        }
        public void RegisteAnItem(ProcedureControlServices item, ProcedureDataItems procedureDataItems, RobotUnity robot,DoorService door)
        {
            RegisterProcedureItem itemprocedure = new RegisterProcedureItem() { item = item,robot=robot,procedureDataItems=procedureDataItems };
            item.ReleaseProcedureHandler += ReleaseProcedureItemHandler;
            RegisterProcedureItemList.Add(itemprocedure);
        }
        protected virtual void ReleaseProcedureItemHandler(ProcedureControlServices item)
        {
            Task.Run(() =>
            {
                var element = RegisterProcedureItemList.Find(e => e.item == item);
                element.procedureDataItems.EndTime = DateTime.Now;
                element.procedureDataItems.DeliveryInfo = item.DeliveryInfo;
                element.procedureDataItems.StatusProcedureDelivered = "OK";
                // gui den database
                RegisterProcedureItemList.Remove(element);
            });
        }
        protected virtual void ErrorApprearInProcedureItem(ProcedureControlServices item)
        {
            // chờ xử lý // error staus is true;
            // báo sự cố cho lớp robotmanagement // đợi cho chờ xử lý// hủy bỏ quy trình
        }
        protected virtual void AskPriority() { }
    }
}
