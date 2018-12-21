using SelDatUnilever_Ver1._00.Management.ComSocket;
using System;
using System.Diagnostics;
using System.Threading;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace DoorControllerService
{
    public class DoorService : TranferData
    {
        private enum CmdDoor
        {
            CMD_GET_ID_DOOR = 0x61,
            RES_GET_ID_DOOR, /*0x62 */
            CMD_SET_ID_DOOR, /*0x63 */
            RES_SET_ID_DOOR, /*0x64 */
            CMD_GET_STATUS_DOOR, /*0x65 */
            RES_GET_STATUS_DOOR, /*0x66 */
            CMD_OPEN_DOOR, /*0x67 */
            RES_OPEN_DOOR, /*0x68 */
            CMD_CLOSE_DOOR, /*0x69 */
            RES_CLOSE_DOOR, /*0x6A */
        }
        public enum DoorId
        {
            DOOR_MEZZAMINE_UP_FRONT = 0x01,
            DOOR_MEZZAMINE_UP_BACK, /* 0x02 */
            DOOR_MEZZAMINE_RETURN_FRONT, /* 0x03 */
            DOOR_MEZZAMINE_RETURN_BACK, /* 0x04 */
            DOOR_ELEVATOR, /* 0x05 */
        }
        public enum DoorStatus
        {
            DOOR_CLOSE = 0x01,
            DOOR_OPEN, /* 0x02 */
            DOOR_CLOSING, /* 0x03 */
            DOOR_OPENING, /* 0x04 */
            DOOR_ERROR /* 0x05 */
        }

        public struct DoorInfoConfig{
           public String ip;
            public Int32 port;
            public DoorId id;   
            public Pose PointFrontLine;
            public PointDetect PointOfPallet;
        }

        public DoorInfoConfig config;
        public DoorService(DoorInfoConfig cf):base(cf.ip,cf.port)
        {
            this.config = cf;
            this.SetId(cf.id);
        }
         public bool GetId(ref DataReceive data)
        {
            bool ret = false;
            byte[] dataSend = new byte[6];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdDoor.CMD_GET_ID_DOOR;
            dataSend[3] = 0x04;
            dataSend[4] = 0x00;
            dataSend[5] = CalChecksum(dataSend,3);
            ret = this.Tranfer(dataSend,ref data);
            return ret;
        }
        public bool SetId(DoorId id)
        {
            bool ret = false;
            byte[] dataSend = new byte[7];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdDoor.CMD_SET_ID_DOOR;
            dataSend[3] = 0x05;
            dataSend[4] = 0x00;
            dataSend[5] = (byte)id;
            dataSend[6] = CalChecksum(dataSend,4);
            ret = this.Tranfer(dataSend);
            return ret;
        }
        public bool GetStatus(ref DataReceive data,DoorId id)
        {
            bool ret = false;
            byte[] dataSend = new byte[7];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdDoor.CMD_GET_STATUS_DOOR;
            dataSend[3] = 0x05;
            dataSend[4] = 0x00;
            dataSend[5] = (byte)id;
            dataSend[6] = CalChecksum(dataSend,4);
            ret = this.Tranfer(dataSend,ref data);
            return ret;
        }
        public bool Open(DoorId id)
        {
            bool ret = false;
            byte[] dataSend = new byte[7];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdDoor.CMD_OPEN_DOOR;
            dataSend[3] = 0x05;
            dataSend[4] = 0x00;
            dataSend[5] = (byte)id;
            dataSend[6] = CalChecksum(dataSend,4);
            ret = this.Tranfer(dataSend);
            return ret;
        }
        public bool Close(DoorId id)
        {
            bool ret = false;
            byte[] dataSend = new byte[7];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdDoor.CMD_CLOSE_DOOR;
            dataSend[3] = 0x05;
            dataSend[4] = 0x00;
            dataSend[5] = (byte)id;
            dataSend[6] = CalChecksum(dataSend,4);
            ret = this.Tranfer(dataSend);
            return ret;
        }

        public bool WaitOpen(DoorId id, UInt32 timeOut)
        {
            bool result = true;
            Stopwatch sw = new Stopwatch();
            DataReceive status = new DataReceive();
            this.Open(id);
            sw.Start();
            do 
            {
                Thread.Sleep(100);
                if (sw.ElapsedMilliseconds > timeOut)
                {
                    result = false;
                    break;
                }
                this.GetStatus(ref status,id);
            } while (status.data[0] != (byte)DoorStatus.DOOR_OPEN);
            sw.Stop();
            return result;
        }

        public bool WaitClose(DoorId id, UInt32 timeOut)
        {
            bool result = true;
            Stopwatch sw = new Stopwatch();
            DataReceive status = new DataReceive();
            this.Close(id);
            sw.Start();
            do 
            {
                Thread.Sleep(100);
                if (sw.ElapsedMilliseconds > timeOut)
                {
                    result = false;
                    break;
                }
                this.GetStatus(ref status,id);
            } while (status.data[0] != (byte)DoorStatus.DOOR_CLOSE);
            sw.Stop();
            return result;
        }
    }
}
