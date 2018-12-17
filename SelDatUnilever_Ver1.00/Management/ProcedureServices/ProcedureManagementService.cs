using SeldatMRMS.Management.RobotManagent;

namespace SeldatMRMS
{
    public class ProcedureManagementService:RegisterProcedureService
    {
        public ProcedureManagementService()
        {

        }
        public void Register(ProcedureItemSelected ProcedureItem, RobotUnity robot)
        {
            switch(ProcedureItem)
            {
                case ProcedureItemSelected.PROCEDURE_FORLIFT_TO_BUFFER:
                    break;
                case ProcedureItemSelected.PROCEDURE_BUFFER_TO_MACHINE: break;
                case ProcedureItemSelected.PROCEDURE_BUFFER_TO_HOPPER: break;
            }
        }
    }
}
