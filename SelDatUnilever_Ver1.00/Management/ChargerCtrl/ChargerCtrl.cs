using System;
using System.Diagnostics;
using System.Threading;
using SelDatUnilever_Ver1._00.Management.ComSocket;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;

namespace SelDatUnilever_Ver1._00.Management.ChargerCtrl
{
    public class ChargerCtrl : TranferData
    {
        public enum ChargerId
        {
            CHARGER_ID_1 = 1,
            CHARGER_ID_2,
            CHARGER_ID_3,
        }

        public enum ChargerState
        {
            ST_READY = 0x01,
            ST_CHARGING,/* 02 */
            ST_ERROR, /* 03 */
            ST_CHARGE_FULL, /* 04 */
            ST_PUSH_PISTON, /* 05 */
            ST_CONTACT_GOOD, /* 06 */
            ST_CONTACT_FAIL /* 07 */
        }

        private enum CmdCharge
        {
            CMD_GET_ID = 0x01,
            RES_GET_ID, /*0x02 */
            CMD_SET_ID, /*0x03 */
            RES_SET_ID, /*0x04 */
            CMD_GET_STATUS, /*0x05 */
            RES_GET_STATUS, /*0x06 */
            CMD_GET_BAT_LEVEL, /*0x07 */
            RES_GET_BAT_LEVEL, /*0x08 */
            CMD_START_CHARGE, /*0x09 */
            RES_START_CHARGE, /*0x0A */
            CMD_STOP_CHARGE, /*0x0B */
            RES_STOP_CHARGE, /*0x0C */
        }

        public struct ChargerInfoConfig
        {
            public String ip;
            public Int32 port;
            public ChargerId id;
            public Pose PointFrontLine;
            public PointDetect PointOfPallet;
        }
        public ChargerCtrl(ChargerInfoConfig cf) : base(cf.ip, cf.port)
        {
            this.SetId(cf.id);
        }
        public bool GetId(ref DataReceive data)
        {
            bool ret = false;
            byte[] dataSend = new byte[6];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdCharge.CMD_GET_ID;
            dataSend[3] = 0x04;
            dataSend[4] = 0x00;
            dataSend[5] = CalChecksum(dataSend, 3);
            ret = this.Tranfer(dataSend, ref data);
            return ret;
        }
        public bool SetId(ChargerId id)
        {
            bool ret = false;
            byte[] dataSend = new byte[7];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdCharge.CMD_SET_ID;
            dataSend[3] = 0x05;
            dataSend[4] = 0x00;
            dataSend[5] = (byte)id;
            dataSend[6] = CalChecksum(dataSend, 4);
            ret = this.Tranfer(dataSend);
            return ret;
        }

        public bool GetState(ref DataReceive data)
        {
            bool ret = false;
            byte[] dataSend = new byte[6];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdCharge.CMD_GET_STATUS;
            dataSend[3] = 0x04;
            dataSend[4] = 0x00;
            dataSend[5] = CalChecksum(dataSend, 3);
            ret = this.Tranfer(dataSend, ref data);
            return ret;
        }

        public bool GetBatteryLevel(ref DataReceive data)
        {
            bool ret = false;
            byte[] dataSend = new byte[6];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdCharge.CMD_GET_BAT_LEVEL;
            dataSend[3] = 0x04;
            dataSend[4] = 0x00;
            dataSend[5] = CalChecksum(dataSend, 3);
            ret = this.Tranfer(dataSend, ref data);
            return ret;
        }
        public bool StartCharge()
        {
            bool ret = false;
            byte[] dataSend = new byte[6];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdCharge.CMD_START_CHARGE;
            dataSend[3] = 0x04;
            dataSend[4] = 0x00;
            dataSend[5] = CalChecksum(dataSend, 3);
            ret = this.Tranfer(dataSend);
            return ret;
        }
        public bool StopCharge()
        {
            bool ret = false;
            byte[] dataSend = new byte[6];

            dataSend[0] = 0xFA;
            dataSend[1] = 0x55;
            dataSend[2] = (byte)CmdCharge.CMD_STOP_CHARGE;
            dataSend[3] = 0x04;
            dataSend[4] = 0x00;
            dataSend[5] = CalChecksum(dataSend, 3);
            ret = this.Tranfer(dataSend);
            return ret;
        }

        public bool WaitState(ChargerState status, UInt32 timeOut)
        {
            bool result = true;
            Stopwatch sw = new Stopwatch();
            DataReceive st = new DataReceive();
            sw.Start();
            do
            {
                Thread.Sleep(1000);
                if (sw.ElapsedMilliseconds > timeOut)
                {
                    result = false;
                    break;
                }
                this.GetState(ref st);
            } while (st.data[0] != (byte)status);
            sw.Stop();
            return result;
        }
        public bool WaitChargeFull(ref DataReceive batLevel,ref DataReceive status)
        {
            bool result = true;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            do
            {
                Thread.Sleep(8000);
                this.GetState(ref status);
                Thread.Sleep(2000);
                this.GetBatteryLevel(ref batLevel);
            } while (batLevel.data[0] == 100);
            sw.Stop();
            return result;
        }
    }
}
