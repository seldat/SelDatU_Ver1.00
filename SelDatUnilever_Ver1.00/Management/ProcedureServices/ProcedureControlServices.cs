using DoorControllerService;
using SeldatMRMS.Management;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeldatMRMS
{
    public class ProcedureControlServices:ControlService
    {
        public String ProcedureID { get; set; }
        public String DeliveryInfo { get; set; }
        public struct ContentDatabase {}
        public virtual event Action<ProcedureControlServices> ReleaseProcedureHandler;
        public virtual event Action<ProcedureControlServices> ErrorProcedureHandler;
        public virtual ContentDatabase RequestDataFromDataBase() { return new ContentDatabase(); }
        public ProcedureControlServices(RobotUnity robot, DoorService doorService):base(robot,doorService) {}
        protected enum ForkLiftToBuffer
        {
            FORBUF_IDLE,
            FORBUF_ROBOT_GOTO_CHECKIN_POSITION, // vị trí check in liệu có quy trình nào tại cổng
            FORBUF_ROBOT_CAME_CHECKIN_POSITION, // đã đến vị trí
            FORBUF_ROBOT_GOINTO_WAITINGZONE, // vào vùng chờ
            FORBUF_ROBOT_CAME_WAITINGZONE,  // Waiitng tại vùng chờ
            FORBUF_ROBOT_GOTO_READYZONE_GATE, // đến vùng sẵng sáng trước cổng
            FORBUF_ROBOT_CAME_READYZONE_GATE, // đến vùng trước cổng
            FORBUF_ROBOT_GATE_CHECKSENSOR,  // kiểm tra trạng thái sensors

            FORBUF_GATE_READYOPEN, // Cổng làm việc tốt / và gửi yêu cầu Mở cổng
            FORBUF_GATE_FINISHED_READYOPEN, // Hoàn Thành Mở cổng
            FORBUF_GATE_ERRORSENSOR_OPEN,

            FORBUF_GATE_READYCLOSE, // Cổng làm việc tốt / và gửi yêu cầu đóng cổng
            FORBUF_GATE_FINISHED_READYCLOSE, // Hoàn Thành đóng cổng
            FORBUF_GATE_ERRORSENSOR_CLOSE,

            FORBUF_ROBOT_DETECTLINE_PICKUP_PALLET,  // cho phép dò line và lấy pallet
            FORBUF_ROBOT_WAITING_DETECTLINE_PICKUP_PALLET, // đang trong tiến trình dò line và pick up pallet
            FORBUF_ROBOT_FINISED_DETECTLINE_PICKUP_PALLET, // hoàn thành dò line và pick up pallet

            FORBUF_ROBOT_GOBACK_READYZONE_GATE,
            FORBUF_ROBOT_CAME_GOBACK_READYZONE_GATE,

            FORBUF_ROBOT_GOTO_CHECKIN_BUFFER, // bắt đầu rời khỏi vùng GATE đi đến check in/ đảm bảo check out vùng cổng để robot kế tiếp vào làm việc
            FORBUF_ROBOT_FINISHED_GOTO_CHECKIN_BUFFER, // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet

            FORBUF_ROBOT_GOTO_FRONTLINE_BUFFER, // ROBOT cho tiến vào vị trí đầu line // vẩn dò đường bằng laser
            FORBUF_ROBOT_FINISED_GOTO_FRONTLINE_BUFFER, // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet
            FORBUF_ROBOT_START_DETECTLINE_TO_LINE_INSIDE_BUFFER, // bắt đầu dò line để đến vị trí line trong buffer
            FORBUF_ROBOT_WAIITNG_DETECTLINE_TO_LINE_INSIDE_BUFFER, // đang đợi dò line để đến vị trí line trong buffer
            FORBUF_ROBOT_CAME_LINE_INSIDE_BUFFER, // đến vị trí line trong buffer

            FORBUF_ROBOT_START_DETECTLINE_DROPDOWN_PALLET_INSIDE_BUFFER, // bắt đầu dò line và vị trí pallet trong buffer
            FORBUF_ROBOT_WAITING_DETECTLINE_DROPDOWN_PALLET_INSIDE_BUFFER, // bắt đầu dò line và vị trí pallet trong buffer
            FORBUF_ROBOT_FINISED_DETECTLINE_DROPDOWN_PALLET_INSIDE_BUFFER, // bắt đầu dò line và vị trí pallet trong buffer // lưu vào cơ sở dữ liệu

            FORBUF_ROBOT_GOBACK_FRONTLINE_BUFFER, // bắt đầu go back vi tri đau line buffer
            FORBUF_ROBOT_WAIITING_GOBACK_FRONTLINE_BUFFER, // đợi
            FORBUF_ROBOT_CAME_GOBACK_FRONTLINE_BUFFER, // đã đến vị trí đầu line
            FORBUF_ROBOT_RELEASED, // trả robot về robotmanagement để nhận quy trình mới
        }
        protected enum BufferToMachine
        {
            BUFMAC_IDLE,
            BUFMAC_ROBOT_GOTO_CHECKIN_BUFFER, // bắt đầu rời khỏi vùng GATE đi đến check in/ đảm bảo check out vùng cổng để robot kế tiếp vào làm việc
            BUFMAC_ROBOT_FINISHED_GOTO_CHECKIN_BUFFER, // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet

            BUFMAC_ROBOT_GOTO_FRONTLINE_BUFFER, // ROBOT cho tiến vào vị trí đầu line // vẩn dò đường bằng laser
            BUFMAC_ROBOT_FINISED_GOTO_FRONTLINE_BUFFER, // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet
            BUFMAC_ROBOT_START_DETECTLINE_TO_LINE_INSIDE_BUFFER, // bắt đầu dò line để đến vị trí line trong buffer
            BUFMAC_ROBOT_WAIITNG_DETECTLINE_TO_LINE_INSIDE_BUFFER, // đang đợi dò line để đến vị trí line trong buffer
            BUFMAC_ROBOT_CAME_LINE_INSIDE_BUFFER, // đến vị trí line trong buffer

            BUFMAC_ROBOT_START_DETECTLINE_PICKUP_PALLET_INSIDE_BUFFER, // bắt đầu dò line và vị trí pallet trong buffer
            BUFMAC_ROBOT_WAITING_DETECTLINE_PICKUP_PALLET_INSIDE_BUFFER, // bắt đầu dò line và vị trí pallet trong buffer
            BUFMAC_ROBOT_FINISED_DETECTLINE_PICKUP_PALLET_INSIDE_BUFFER, // bắt đầu dò line và vị trí pallet trong buffer // lưu vào cơ sở dữ liệu

            BUFMAC_ROBOT_GOBACK_FRONTLINE_BUFFER, // bắt đầu go back vi tri đau line buffer
            BUFMAC_ROBOT_WAIITING_GOBACK_FRONTLINE_BUFFER, // đợi
            BUFMAC_ROBOT_CAME_GOBACK_FRONTLINE_BUFFER, // đã đến vị trí đầu line
           

            BUFMAC_ROBOT_GOTO_CHECKIN_MACHINE, // vị trí check in liệu có quy trình nào tại MACHINE
            BUFMAC_ROBOT_CAME_CHECKIN_MACHINE, // đã đến vị trí
            BUFMAC_ROBOT_ASK_GOINSIDE_PALLETZONE_AT_MACHINE, // robot yêu cầu được phép vào vùng thả pallet
            BUFMAC_ROBOT_WAIITNG_ACCEPT_GOINSIDE_PALLETZONE_AT_MACHINE, // robot yêu cầu được phép vào vùng thả pallet
            BUFMAC_ROBOT_ACCEPTED_GOINSIDE_PALLETZONE_AT_MACHINE, // robot được đồng ý vào vùng thả pallet


            BUFMAC_ROBOT_DETECTLINE_DROPDOWN_PALLET,  // cho phép dò line vàthả pallet
            BUFMAC_ROBOT_WAITING_DETECTLINE_DROPDOWN_PALLET, // đang trong tiến trình dò line và thả pallet
            BUFMAC_ROBOT_FINISED_DETECTLINE_DROPDOWN_PALLET, // hoàn thành dò line và thả pallet

            BUFMAC_ROBOT_GOBACK_FRONTLINE , // quay lại vị trí đầu line
            BUFMAC_ROBOT_CAME_GOBACK_FRONTLINE,
            BUFMAC_ROBOT_RELEASED, // trả robot về robotmanagement để nhận quy trình mới
        }
        protected enum RobotGoToChargeReady
        {
            ROBCHAREA_IDLE,
            ROBCHAREA_ROBOT_GOTO_FRONTLINE_CHARGER_READYSTATION, // ROBOT cho tiến vào vị trí đầu line charge su dung laser
            ROBCHAREA_ROBOT_FINISED_GOTO_CHARGER_READYSTATION, // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet
            ROBCHAREA_ROBOT_START_DETECTLINE_TO_CHARGER_READYSTATION, // bắt đầu dò line để đến vị trí line trong buffer
            ROBCHAREA_ROBOT_WAIITNG_DETECTLINE_TO_CHARGER_READYSTATION, // đang đợi dò line để đến vị trí line trong buffer
            ROBCHAREA_ROBOT_CAME_POSITION_LINE_CHARGER_READYSTATION, // đến vị trí line trong statio charge va ready / nếu là ready thì release robot két thúc quy trình/ nếu là sạc thì chuyển sang trạng thái kế tiếp

            ROBCHAREA_REQUEST_INFORMATION_ROBOT_AND_CHARGER,
            ROBCHAREA_REQUEST_INFORMATION_ERROR, // lỗi thông tin kết nối trạm sạc ...
            ROBCHAREA_ROBOT_APPROACH_POINTCHARGE_CHARGER_READYSTATION, // cho tiếp cận đến vị trí sạc
            ROBCHAREA_ROBOT_APPROACHED_POINTCHARGE_CHARGER_READYSTATION, // đã đến tiếp cận đến vị trí sạc

            ROBCHAREA_CHARGER_CHECKSTATUS, //kiểm tra kết nối và trạng thái sạc
            ROBCHAREA_ROBOT_ALLOW_CUTOFF_POWER_ROBOT, //cho phép cắt nguồn robot
            ROBCHAREA_WAIITNG_CHARGEBATTERY, //dợi charge battery và thông tin giao tiếp server và trạm sạc
            ROBCHAREA_FINISHED_CHARGEBATTERY, //Hoàn Thành charge battery và thông tin giao tiếp server và trạm sạc
            ROBCHAREA_ROBOT_WAITING_RECONNECTING, //Robot mở nguồng và đợi connect lại
            ROBCHAREA_ROBOT_CONNECTED_TO_SERVER, //robot đã connect lại với server
            ROBCHAREA_ROBOT_CHECK_STATUS_OPERATION, //check điều kiện hoạt động
            ROBCHAREA_ROBOT_STATUS_GOOD_OPERATION, //điều kiện hoạt động tốt 
            ROBCHAREA_ROBOT_STATUS_BAD_OPERATION, //điều kiện hoạt động không tốt thông tin về procedure management chuyển sang quy trình xử lý sự cố
            ROBCHAREA_ROBOT_RELEASED, // trả robot về robotmanagement để nhận quy trình mới
        }

    }
}
