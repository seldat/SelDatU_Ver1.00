using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;

namespace SelDatUnilever_Ver1._00.Management.DeviceManagement
{
    public class ForkLiftToBufferTask
    {
        public ForkLiftToBufferTask()
        {
            checkInPose = new Pose();
            lineAtGate = new LinePose();
            mainLineInBuffer = new LinePose();
            subLineInBuffer = new LinePose();
        }
        public Pose checkInPose;
        public LinePose lineAtGate; // line tại cổng
        public LinePose mainLineInBuffer; // line dò trong buffer đến subLine
        public LinePose subLineInBuffer; // line dò pallet
    }
    public class BufferToMachineTask
    {
        public BufferToMachineTask()
        {
            checkInPoseBuffer = new Pose();
            posePalletAtMachine = new Pose();
            mainLineInBuffer = new LinePose();
            subLineInBuffer = new LinePose();
        }
        public Pose checkInPoseBuffer;
        public Pose posePalletAtMachine;
        public LinePose mainLineInBuffer; // line dò trong buffer đến subLine
        public LinePose subLineInBuffer; // line dò pallet
    }
    public class PalletReturnAtMachine
    {
        public PalletReturnAtMachine()
        {
            checkInPoseBuffer = new Pose();
            checkInPoseReturn = new Pose();
            posePalletAtMachine = new Pose();
            mainLineInReturn = new LinePose();
            subLineInReturn = new LinePose();
        }
        public Pose checkInPoseBuffer;
        public Pose checkInPoseReturn;
        public Pose posePalletAtMachine;
        public LinePose mainLineInReturn; // line dò trong buffer đến subLine
        public LinePose subLineInReturn; // line dò pallet
    }
    public class CollectionDataService
    {
        public ForkLiftToBufferTask ForkLiftToBufferTaskRequested;
        public BufferToMachineTask BufferToMachineTaskRequested;
        public PalletReturnAtMachine PalletReturnAtMachineRequested;
        protected virtual void ReceiveResponseHandler(String msg) { }
        protected BridgeClientRequest clientRequest;
        public CollectionDataService() {
            clientRequest = new BridgeClientRequest();
            clientRequest.ReceiveResponseHandler += ReceiveResponseHandler;
        }
    }
}
