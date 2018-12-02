using DoorControllerService;
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
    public class ControlService:DBProcedureService
    {
       public ControlService(RobotUnity robot,DoorService doorService)
       {
            if (robot != null)
            {
                robot.ZoneCallBack += ZoneCallBack;
                robot.FinishStatesCallBack += FinishStatesCallBack;
                robot.PoseCallBack+= PoseCallBack;
  
            }
            if (doorService != null)
                doorService.ReceiveRounterEvent += ReceiveRounterEvent;
        }
       // robot control
       public virtual void ZoneCallBack(Communication.Message message) { }
       public virtual void FinishStatesCallBack(Int32 message) { }
       public virtual void PoseCallBack(Pose p,Object obj) { }
       public virtual void CtrlRobotSpeed() { }
       public virtual void MoveBaseGoal() { }
       public virtual void AcceptDoSomething() { }
        // door control
       public virtual void ReceiveRounterEvent(String message) { }
       public virtual void CtrlDoor(DoorService.DOORID id, byte cmd) { }
    }
}
