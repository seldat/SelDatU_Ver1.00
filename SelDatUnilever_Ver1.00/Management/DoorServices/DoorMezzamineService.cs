using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoorControllerService
{
    public class DoorMezzamineService : DoorService
    {
        // 0xAA ID cmd x x 0x0A
        public DoorMezzamineService()
        {

        }
        public bool statusSwitchOnCloseFrontDoor{ get; set; }
        public bool statusSwitchOnOpenFrontDoor { get; set; }
        public bool statusSwitchOnCloseBackDoor { get; set; }
        public bool statusSwitchOnOpenBackDoor { get; set; }
        public virtual void OpenMezzamineDoor(DOORID id) { TransmittedDataPackage(id,SERVER_CMD_OPENDOOR ); }
        public virtual void CloseMezzamineDoor(DOORID id) { TransmittedDataPackage(id,SERVER_CMD_CLOSEDOOR); }
        public void CheckStatusSensor(byte[] data)
        {
            if(data[0]==HEADER_RESPONSE && data[data.Length-1]==TAIL_RESPONSE)
            {
                if(data[1]==(byte)DOORID.DOOR_MEZZAMINE_UP_FRONT)
                {
                    if(data[2]==SWITCHON) 
                    {
                        statusSwitchOnCloseFrontDoor = true;
                    }
                    else
                    {
                        statusSwitchOnCloseFrontDoor = false;
                    }

                    if (data[3] == SWITCHON)
                    {
                        statusSwitchOnOpenFrontDoor = true;
                    }
                    else
                    {
                        statusSwitchOnOpenFrontDoor = false;
                    }
                }
                else if(data[1] == (byte)DOORID.DOOR_MEZZAMINE_UP_BACK)
                {
                    if (data[2] == SWITCHON)
                    {
                        statusSwitchOnCloseFrontDoor = true;
                    }
                    else
                    {
                        statusSwitchOnCloseFrontDoor = false;
                    }

                    if (data[3] == SWITCHON)
                    {
                        statusSwitchOnOpenFrontDoor = true;
                    }
                    else
                    {
                        statusSwitchOnOpenFrontDoor = false;
                    }
                }
            }
        }
        public void ProcedureDoor(DOORID id,PROCEDURETRACKING_MEZZAMINE state)
        {
            switch (state)
            {
                case PROCEDURETRACKING_MEZZAMINE.CHECK_SWITCH_CLOSE:
                    break;
                case PROCEDURETRACKING_MEZZAMINE.CHECK_SWITCH_CLOSE_THEN_OPEN:
                    // kiem tra Switch close nếu đúng open cửa
                    // hoàn thành mở cửa gọi ProcedureDoor(DOORID id,PROCEDURETRACKING_MEZZAMINE.CLOSE_TO_OPEN_FINISH)
                    break;
                case PROCEDURETRACKING_MEZZAMINE.CLOSE_TO_OPEN_FINISH:
                    // trả trạng thái toàn quy trình
                    break;
                case PROCEDURETRACKING_MEZZAMINE.CHECK_SWITCH_OPEN:
                    break;
                case PROCEDURETRACKING_MEZZAMINE.CHECK_SWITCH_OPEN_THEN_CLOSE:
                    // kiem tra Switch Open nếu đúng open cửa
                    // hoàn thành đóng cửa gọi ProcedureDoor(DOORID id,PROCEDURETRACKING_MEZZAMINE.OPEN_TO_CLOSE_FINISH)
                    break;
                case PROCEDURETRACKING_MEZZAMINE.OPEN_TO_CLOSE_FINISH:
                    break;
                case PROCEDURETRACKING_MEZZAMINE.DOOR_ERROR_APPEAR:
                    break;

            }
        }
    }
}
