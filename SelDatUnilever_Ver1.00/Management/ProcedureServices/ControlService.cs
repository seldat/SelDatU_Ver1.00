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
    public class ControlService:DBProcedureService
    {
       public ControlService(RobotUnity robot,DoorService doorService)
       {
            if (robot != null)
            {
                robot.ZoneHandler += ZoneHandler;
                robot.FinishStatesHandler += FinishStatesHandler;
                robot.AmclPoseHandler += AmclPoseHandler;
                doorService.ReceiveRounterEvent += ReceiveRounterEvent;
            }
            if(doorService!=null)
            {

            }
       }
       // robot control
       public virtual void ZoneHandler(Communication.Message message) { }
       public virtual void FinishStatesHandler(Communication.Message message) { }
       public virtual void AmclPoseHandler(Communication.Message message) { }
       public virtual void CtrlRobotSpeed() { }
       public virtual void MoveBaseGoal() { }
       public virtual void AcceptDoSomething() { }
        // door control
       public virtual void ReceiveRounterEvent(String message) { }
       public virtual void CtrlDoor(DoorService.DOORID id, byte cmd) { }
    }
}
