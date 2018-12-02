using SeldatMRMS.Management;
using SeldatMRMS.Management.RobotManagent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;

namespace SeldatMRMS
{
    public class ProcedureBufferToMachine : ProcedureControlServices
    {
        private BufferToMachine StateBufferToMachine;
        RobotUnity robot;
        public ProcedureBufferToMachine(RobotUnity robot):base(robot,null) {
            StateBufferToMachine = BufferToMachine.BUFMAC_IDLE;
            this.robot = robot;
        }
        public void Procedure()
        {
            switch(StateBufferToMachine)
            {
                case BufferToMachine.BUFMAC_IDLE: break;
                case BufferToMachine.BUFMAC_ROBOT_GOTO_CHECKIN_BUFFER: break; // bắt đầu rời khỏi vùng GATE đi đến check in/ đảm bảo check out vùng cổng để robot kế tiếp vào làm việc
                   //gửi toa độ laser 
                    // Pose position = new Pose(new System.Windows.Point(10,10),0);
                  //  this.robot.SendPoseStamped(position);
                case BufferToMachine.BUFMAC_ROBOT_FINISHED_GOTO_CHECKIN_BUFFER: break; // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet

                case BufferToMachine.BUFMAC_ROBOT_GOTO_FRONTLINE_BUFFER: break; // ROBOT cho tiến vào vị trí đầu line // vẩn dò đường bằng laser
                case BufferToMachine.BUFMAC_ROBOT_FINISED_GOTO_FRONTLINE_BUFFER: break; // hoàn thành đến vùng check in/ kiểm tra có robot đang làm việc vùng này và lấy vị trí line và pallet
                case BufferToMachine.BUFMAC_ROBOT_START_DETECTLINE_TO_LINE_INSIDE_BUFFER: break; // bắt đầu dò line để đến vị trí line trong buffer
                case BufferToMachine.BUFMAC_ROBOT_WAIITNG_DETECTLINE_TO_LINE_INSIDE_BUFFER: break; // đang đợi dò line để đến vị trí line trong buffer
                case BufferToMachine.BUFMAC_ROBOT_CAME_LINE_INSIDE_BUFFER: break; // đến vị trí line trong buffer

                case BufferToMachine.BUFMAC_ROBOT_START_DETECTLINE_PICKUP_PALLET_INSIDE_BUFFER: break; // bắt đầu dò line và vị trí pallet trong buffer
                case BufferToMachine.BUFMAC_ROBOT_WAITING_DETECTLINE_PICKUP_PALLET_INSIDE_BUFFER: break; // bắt đầu dò line và vị trí pallet trong buffer
                case BufferToMachine.BUFMAC_ROBOT_FINISED_DETECTLINE_PICKUP_PALLET_INSIDE_BUFFER: break; // bắt đầu dò line và vị trí pallet trong buffer // lưu vào cơ sở dữ liệu

                case BufferToMachine.BUFMAC_ROBOT_GOBACK_FRONTLINE_BUFFER: break; // bắt đầu go back vi tri đau line buffer
                case BufferToMachine.BUFMAC_ROBOT_WAIITING_GOBACK_FRONTLINE_BUFFER: break; // đợi
                case BufferToMachine.BUFMAC_ROBOT_CAME_GOBACK_FRONTLINE_BUFFER: break; // đã đến vị trí đầu line


                case BufferToMachine.BUFMAC_ROBOT_GOTO_CHECKIN_MACHINE: break; // vị trí check in liệu có quy trình nào tại MACHINE
                case BufferToMachine.BUFMAC_ROBOT_CAME_CHECKIN_MACHINE: break; // đã đến vị trí
                case BufferToMachine.BUFMAC_ROBOT_ASK_GOINSIDE_PALLETZONE_AT_MACHINE: break; // robot yêu cầu được phép vào vùng thả pallet
                case BufferToMachine.BUFMAC_ROBOT_WAIITNG_ACCEPT_GOINSIDE_PALLETZONE_AT_MACHINE: break; // robot yêu cầu được phép vào vùng thả pallet
                case BufferToMachine.BUFMAC_ROBOT_ACCEPTED_GOINSIDE_PALLETZONE_AT_MACHINE: break; // robot được đồng ý vào vùng thả pallet


                case BufferToMachine.BUFMAC_ROBOT_DETECTLINE_DROPDOWN_PALLET: break;  // cho phép dò line vàthả pallet
                case BufferToMachine.BUFMAC_ROBOT_WAITING_DETECTLINE_DROPDOWN_PALLET: break; // đang trong tiến trình dò line và thả pallet
                case BufferToMachine.BUFMAC_ROBOT_FINISED_DETECTLINE_DROPDOWN_PALLET: break; // hoàn thành dò line và thả pallet

                case BufferToMachine.BUFMAC_ROBOT_GOBACK_FRONTLINE: break; // quay lại vị trí đầu line
                case BufferToMachine.BUFMAC_ROBOT_CAME_GOBACK_FRONTLINE: break;
                case BufferToMachine.BUFMAC_ROBOT_RELEASED: break; // trả robot về robotmanagement để nhận quy trình mới

            }

    }
        public override void FinishStatesCallBack(Int32 message) { }
    }
}
