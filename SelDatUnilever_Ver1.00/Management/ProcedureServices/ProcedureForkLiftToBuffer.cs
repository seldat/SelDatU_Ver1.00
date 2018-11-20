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
    class ProcedureForkLiftToBuffer : ProcedureControlServices
    {
        ForkLiftToBuffer StateForkLiftToBuffer;
        public ProcedureForkLiftToBuffer(RobotUnity robot,DoorService doorservice):base(robot,doorservice)
        {
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_IDLE;
        }
        public void Start(String content)
        {
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_POSITION;
        }
        public void Destroy()
        {
            StateForkLiftToBuffer = ForkLiftToBuffer.FORBUF_IDLE;
        }
        public void Procedure()
        {
            switch(StateForkLiftToBuffer)
            {
                case ForkLiftToBuffer.FORBUF_IDLE: break;
                case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_POSITION: break; // vị trí check in liệu có quy trình nào tại cổng
                case ForkLiftToBuffer.FORBUF_ROBOT_CAME_CHECKIN_POSITION: break; // đã đến vị trí
                case ForkLiftToBuffer.FORBUF_ROBOT_GOINTO_WAITINGZONE: break; // vào vùng chờ
                case ForkLiftToBuffer.FORBUF_ROBOT_CAME_WAITINGZONE: break;  // Waiitng tại vùng chờ
                case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_READYZONE_GATE: break; // đến vùng sẵng sáng trước cổng
                case ForkLiftToBuffer.FORBUF_ROBOT_CAME_READYZONE_GATE: break; // đến vùng trước cổng
                case ForkLiftToBuffer.FORBUF_ROBOT_GATE_CHECKSENSOR: break;  // kiểm tra trạng thái sensors

                case ForkLiftToBuffer.FORBUF_GATE_READYOPEN: break; // Cổng làm việc tốt / và gửi yêu cầu Mở cổng
                case ForkLiftToBuffer.FORBUF_GATE_FINISHED_READYOPEN: break; // Hoàn Thành Mở cổng
                case ForkLiftToBuffer.FORBUF_GATE_ERRORSENSOR_OPEN: break;

                case ForkLiftToBuffer.FORBUF_GATE_READYCLOSE: break; // Cổng làm việc tốt / và gửi yêu cầu đóng cổng
                case ForkLiftToBuffer.FORBUF_GATE_FINISHED_READYCLOSE: break; // Hoàn Thành đóng cổng
                case ForkLiftToBuffer.FORBUF_GATE_ERRORSENSOR_CLOSE: break;

                case ForkLiftToBuffer.FORBUF_ROBOT_DETECTLINE_PICKUP_PALLET: break;  // cho phép dò line và lấy pallet
                case ForkLiftToBuffer.FORBUF_ROBOT_WAITING_DETECTLINE_PICKUP_PALLET: break; // đang trong tiến trình dò line và pick up pallet
                case ForkLiftToBuffer.FORBUF_ROBOT_FINISED_DETECTLINE_PICKUP_PALLET: break; // hoàn thành dò line và pick up pallet

                case ForkLiftToBuffer.FORBUF_ROBOT_GOBACK_READYZONE_GATE: break;
                case ForkLiftToBuffer.FORBUF_ROBOT_CAME_GOBACK_READYZONE_GATE: break;

                case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_CHECKIN_BUFFER: break; // bắt đầu rời khỏi vùng GATE đi đến check in/ đảm bảo check out vùng cổng để robot kế tiếp vào làm việc
                case ForkLiftToBuffer.FORBUF_ROBOT_FINISHED_GOTO_CHECKIN_BUFFER: break; // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet

                case ForkLiftToBuffer.FORBUF_ROBOT_GOTO_FRONTLINE_BUFFER: break; // ROBOT cho tiến vào vị trí đầu line // vẩn dò đường bằng laser
                case ForkLiftToBuffer.FORBUF_ROBOT_FINISED_GOTO_FRONTLINE_BUFFER: break; // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet
                case ForkLiftToBuffer.FORBUF_ROBOT_START_DETECTLINE_TO_LINE_INSIDE_BUFFER: break; // bắt đầu dò line để đến vị trí line trong buffer
                case ForkLiftToBuffer.FORBUF_ROBOT_WAIITNG_DETECTLINE_TO_LINE_INSIDE_BUFFER: break; // đang đợi dò line để đến vị trí line trong buffer
                case ForkLiftToBuffer.FORBUF_ROBOT_CAME_LINE_INSIDE_BUFFER: break; // đến vị trí line trong buffer

                case ForkLiftToBuffer.FORBUF_ROBOT_START_DETECTLINE_DROPDOWN_PALLET_INSIDE_BUFFER: break; // bắt đầu dò line và vị trí pallet trong buffer
                case ForkLiftToBuffer.FORBUF_ROBOT_WAITING_DETECTLINE_DROPDOWN_PALLET_INSIDE_BUFFER: break; // bắt đầu dò line và vị trí pallet trong buffer
                case ForkLiftToBuffer.FORBUF_ROBOT_FINISED_DETECTLINE_DROPDOWN_PALLET_INSIDE_BUFFER: break; // bắt đầu dò line và vị trí pallet trong buffer // lưu vào cơ sở dữ liệu

                case ForkLiftToBuffer.FORBUF_ROBOT_GOBACK_FRONTLINE_BUFFER: break; // bắt đầu go back vi tri đau line buffer
                case ForkLiftToBuffer.FORBUF_ROBOT_WAIITING_GOBACK_FRONTLINE_BUFFER: break; // đợi
                case ForkLiftToBuffer.FORBUF_ROBOT_CAME_GOBACK_FRONTLINE_BUFFER: break; // đã đến vị trí đầu line
                case ForkLiftToBuffer.FORBUF_ROBOT_RELEASED: break; // trả robot về robotmanagement để nhận quy trình mới
            }
        }
    }
}
