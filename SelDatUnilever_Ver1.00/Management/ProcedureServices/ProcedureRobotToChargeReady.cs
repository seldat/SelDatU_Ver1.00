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
    class ProcedureRobotToChargeReady : ProcedureControlServices
    {
        private RobotGoToChargeReady StateRobotGoToChargeReady;
        public ProcedureRobotToChargeReady(RobotUnity robot) : base(robot, null) { }
        public void Procedure()
        {
            switch(StateRobotGoToChargeReady)
            {
                case RobotGoToChargeReady.ROBCHAREA_IDLE: break;
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_GOTO_FRONTLINE_CHARGER_READYSTATION: break; // ROBOT cho tiến vào vị trí đầu line charge su dung laser
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_FINISED_GOTO_CHARGER_READYSTATION: break; // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_START_DETECTLINE_TO_CHARGER_READYSTATION: break; // bắt đầu dò line để đến vị trí line trong buffer
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_WAIITNG_DETECTLINE_TO_CHARGER_READYSTATION: break; // đang đợi dò line để đến vị trí line trong buffer
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_CAME_POSITION_LINE_CHARGER_READYSTATION: break; // đến vị trí line trong statio charge va ready / nếu là ready thì release robot két thúc quy trình/ nếu là sạc thì chuyển sang trạng thái kế tiếp

                case RobotGoToChargeReady.ROBCHAREA_REQUEST_INFORMATION_ROBOT_AND_CHARGER: break;
                case RobotGoToChargeReady.ROBCHAREA_REQUEST_INFORMATION_ERROR: break; // lỗi thông tin kết nối trạm sạc ...
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_APPROACH_POINTCHARGE_CHARGER_READYSTATION: break; // cho tiếp cận đến vị trí sạc
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_APPROACHED_POINTCHARGE_CHARGER_READYSTATION: break; // đã đến tiếp cận đến vị trí sạc

                case RobotGoToChargeReady.ROBCHAREA_CHARGER_CHECKSTATUS: break; //kiểm tra kết nối và trạng thái sạc
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_ALLOW_CUTOFF_POWER_ROBOT: break; //cho phép cắt nguồn robot
                case RobotGoToChargeReady.ROBCHAREA_WAIITNG_CHARGEBATTERY: break; //dợi charge battery và thông tin giao tiếp server và trạm sạc
                case RobotGoToChargeReady.ROBCHAREA_FINISHED_CHARGEBATTERY: break; //Hoàn Thành charge battery và thông tin giao tiếp server và trạm sạc
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_WAITING_RECONNECTING: break; //Robot mở nguồng và đợi connect lại
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_CONNECTED_TO_SERVER: break; //robot đã connect lại với server
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_CHECK_STATUS_OPERATION: break; //check điều kiện hoạt động
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_STATUS_GOOD_OPERATION: break; //điều kiện hoạt động tốt 
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_STATUS_BAD_OPERATION: break; //điều kiện hoạt động không tốt thông tin về procedure management chuyển sang quy trình xử lý sự cố
                case RobotGoToChargeReady.ROBCHAREA_ROBOT_RELEASED: break; // trả robot về robotmanagement để nhận quy trình mới
            }
        }
    }
}
