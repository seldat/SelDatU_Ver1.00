using DoorControllerService;
using SeldatMRMS.Management;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.DBProcedureService;
using static SelDatUnilever_Ver1._00.Management.DeviceManagement.DeviceItem;

namespace SeldatMRMS
{
    public class RegisterProcedureService
    {
        protected virtual bool Cancel() { return false; }
        public class RegisterProcedureItem
        {
            public OrderItem orderItem;
            public ProcedureControlServices item;
            public ProcedureDataItems procedureDataItems;
            public RobotUnity robot;
            public static bool currentErrorStatus = false;
            public void Start()
            {
                //if(itemProcService != null)
                    //itemProcService.
            }
            public void Stop()
            {
                //if(itemProcService != null)
                //itemProcService.
            }
            public void Dispose()
            {
                //if(itemProcService != null)
                //itemProcService.
            }
        }
        protected List<RegisterProcedureItem> RegisterProcedureItemList = new List<RegisterProcedureItem>();
        public RegisterProcedureService() { }
        public enum ProcedureItemSelected
        {
            PROCEDURE_FORLIFT_TO_BUFFER,
            PROCEDURE_BUFFER_TO_MACHINE,
            PROCEDURE_BUFFER_TO_RETURN,
            PROCEDURE_MACHINE_TO_RETURN,
            PROCEDURE_ROBOT_TO_READY,
            PROCEDURE_ROBOT_TO_CHARGE,
        }
        public void StoreProceduresInDataBase()
        {

        }
        public void RegisteAnItem(ProcedureControlServices item, ProcedureDataItems procedureDataItems, RobotUnity robot)
        {


        }
        protected virtual void ReleaseProcedureItemHandler(Object  item)
        {
           /* Task.Run(() =>
            {
                var element = RegisterProcedureItemList.Find(e => e.item == item);
                element.procedureDataItems.EndTime = DateTime.Now;
                element.procedureDataItems.StatusProcedureDelivered = "OK";
                RegisterProcedureItemList.Remove(element);
            });*/
        }
        protected virtual void ErrorApprearInProcedureItem(ProcedureControlServices item)
        {
            // chờ xử lý // error staus is true;
            // báo sự cố cho lớp robotmanagement // đợi cho chờ xử lý// hủy bỏ quy trình
        }
        protected virtual void AskPriority() { }
    }
}
