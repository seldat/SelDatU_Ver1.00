using Newtonsoft.Json.Linq;
using SeldatMRMS;
using SeldatMRMS.Management;
using SeldatMRMS.Management.DoorServices;
using SeldatMRMS.Management.RobotManagent;
using SeldatMRMS.Management.TrafficManager;
using SeldatMRMS.Management.UnityService;
using SelDatUnilever_Ver1._00.Communication.HttpBridge;
using SelDatUnilever_Ver1._00.Management.ChargerCtrl;
using SelDatUnilever_Ver1._00.Management.DeviceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static SeldatMRMS.Management.RobotManagent.RobotUnityControl;
using static SeldatMRMS.Management.TrafficRobotUnity;
using static SeldatMRMS.ProcedureForkLiftToBuffer;
using static SelDatUnilever_Ver1._00.Management.ChargerCtrl.ChargerCtrl;
using static SelDatUnilever_Ver1._00.Management.ComSocket.RouterComPort;

namespace SelDatUnilever_Ver1._00
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, RobotUnity> robotlist = new Dictionary<string, RobotUnity>();
        List<RobotUnity> robottrafficlist = new List<RobotUnity>();
        String namerobot = "robot1";
        DeviceRegistrationService deviceRegistrationService;

        /* Chau test */
        RobotUnity robot;
        DoorManagementService door;
        ChargerManagementService charger;
        TrafficManagementService traffiicService;

        /*End*/

        public MainWindow()
        {
            InitializeComponent();
            UnityManagementService unityService = new UnityManagementService();
            unityService.Initialize();
           // door = new DoorManagementService();
           // ProcedureForkLiftToBuffer pr = new ProcedureForkLiftToBuffer(robot, door, traffiicService);
          //  pr.GetInfoOfPalletBuffer();
            /*Chau test*/

            //ChargerInfoConfig cf;
            //cf.id = ChargerId.CHARGER_ID_1;
            //cf.ip = "127.0.0.1";
            //cf.port = 10001;
            //ChargerCtrl chargerCtrl = new ChargerCtrl(cf);
            //DataReceive id = new DataReceive();
            //chargerCtrl.GetId(ref id);
            //Console.WriteLine("id :{0}", id.data);

            //chargerCtrl.SetId(ChargerId.CHARGER_ID_2);

            //chargerCtrl.StartCharge();

            //DataReceive state = new DataReceive();
            //chargerCtrl.GetState(ref state);

            //DataReceive batLevel = new DataReceive();
            //chargerCtrl.GetBatteryLevel(ref batLevel);

            //chargerCtrl.StopCharge();

            /*  robot = new RobotUnity();
              robot.Start("ws://192.168.109.128:9090");
              robot.properties.pose = new Pose(new System.Windows.Point(30, 8), 0);
              door = new DoorManagementService();
              // ProcedureBufferToMachine pr = new ProcedureBufferToMachine(robot);

              TrafficManagementService traffiicService = new TrafficManagementService();
              traffiicService.LoadConfigureZone();
              ProcedureForkLiftToBuffer pr = new ProcedureForkLiftToBuffer(robot, door, traffiicService);
              pr.RegistrationTranfficService(traffiicService);
              pr.Start();*/
            /*end*/
           RobotUnity robot1 = new RobotUnity(canvas);
               robot1.properties.NameID = "robot1";
               RobotUnity robot2 = new RobotUnity(canvas);
               robot2.properties.NameID = "robot2";
               RobotUnity robot3 = new RobotUnity(canvas);
               robot3.properties.NameID = "robot3";
               robottrafficlist.Add(robot1);
               robottrafficlist.Add(robot2);
               robottrafficlist.Add(robot3);

               robot1.RegisteRobotInAvailable(robottrafficlist);
               robot2.RegisteRobotInAvailable(robottrafficlist);
               robot3.RegisteRobotInAvailable(robottrafficlist);

               robotlist.Add("robot1", robot1);
               robotlist.Add("robot2", robot2);
               robotlist.Add("robot3", robot3);

               robot1.initialPos(0,0);
               robot2.initialPos(300,300);
               robot3.initialPos(400,400);
              /* TrafficManagementService traffic = new TrafficManagementService();
               traffic.LoadConfigureZone();*/

            //  deviceRegistrationService=new DeviceRegistrationService(9000);
            //  deviceRegistrationService.listen();
            string text = System.IO.File.ReadAllText("C:\\Users\\luat.tran\\Desktop\\datajson.json");
         
        /*    JArray results = JArray.Parse(text);
            foreach (var result in results)
            {
                int temp_productDetailID = (int)result["productDetailId"];
                if (temp_productDetailID ==1)
                {
                    var bufferResults = result["buffers"];
                    var palletResults = bufferResults[0]["pallets"];
                    var dataPalletItemResults = palletResults[0]["dataPallet"];
                   // bool dataPalletItem_hasBranch = (bool)dataPalletItemResults["hasBranch"];
                    var ppalets = dataPalletItemResults["pallet"];
                    var pppoints = ppalets["point"];
                    double pX = (double)pppoints["X"];
                    double pY = (double)pppoints["Y"];
                    int pDBmvDir = (int)ppalets["mvDir"];
                    MessageBox.Show(pX.ToString());
                    break;
                }
            }*/

            /*  string text = System.IO.File.ReadAllText("C:\\Users\\luat.tran\\Desktop\\datajson.json");

              JArray results = JArray.Parse(text);
              foreach (var result in results)
              {
                  int temp_productDetailID = (int)result["productDetailId"];
                  if (temp_productDetailID == 1)
                  {
                      var bufferResults = result["buffers"];


                      var palletResults = bufferResults[0]["pallets"];
                      var palletItemResults = palletResults[0]["dataPallet"];
                      int d = 5;
                      bool dataPalletItem_hasMainLine = (bool)palletItemResults["hasMainLine"];
                      double dataPalletItem_rot = (double)palletItemResults["rot"];
                      double dataPalletItem_mainThreshold = (double)palletItemResults["mainThreshold"];
                      double dataPalletItem_subThreshold = (double)palletItemResults["subThreshold"];
                    //  dataPalletItem = new DataPallet() { hasMainLine = dataPalletItem_hasMainLine, rot = dataPalletItem_rot, ThresholdDetectsMaker_MainLine = dataPalletItem_mainThreshold, ThresholdDetectsMaker_SubLine = dataPalletItem_subThreshold };
                  }
              }*/
            /* string text = System.IO.File.ReadAllText("C:\\Users\\luat.tran\\source\\repos\\TestServer\\TestServer\\HttpServerRounter\\datajson.json");
             JArray results = JArray.Parse(text);
             foreach (var result in results)
             {
                 int temp_productDetailID = (int)result["productDetailId"];
                 var bufferResults = result["buffers"];
                 var checkinResults = bufferResults[0]["bufferCheckIn"];
                     foreach (var checkinResult in checkinResults)
                     {
                         double x = (int)checkinResult["X"];
                         double y = (int)checkinResult["Y"];
                         double angle = (int)checkinResult["A"];
                         Pose poseTemp = new Pose(x, y, angle * Math.PI / 180.0);
                     }
                     //var palletResults = bufferResults[0]["pallets"];


             }*/

            /* String data = "[{\"a\":3,\"b\":[{\"e\":10},{\"d\":10}] }]";
             JArray stff = JArray.Parse(data);
             foreach(var result in stff)
             {
                 var bb = result["b"];
                 int ee = (int)bb[0]["e"];
                 MessageBox.Show("" + ee);
             }*/
        }

        private void sendPose_Click(object sender, RoutedEventArgs e)
        {
            BridgeClientRequest pp = new BridgeClientRequest();
            pp.PostCallAPI("",null);
        }

        private void canvas_KeyUp(object sender, KeyEventArgs e)
        {
           
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void main_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
          
            if (e.Key == Key.Left)
            {
                robotlist[namerobot].LeftRobot();
            }
            if (e.Key == Key.Right)
            {
                robotlist[namerobot].RightRobot();
            }
            if (e.Key == Key.Up)
            {
                robotlist[namerobot].UpRobot();
            }
            if (e.Key == Key.Down)
            {
                robotlist[namerobot].DownRobot();
            }
            if (e.Key == Key.Next)
            {
                robotlist[namerobot].RotationLeft();
            }
            if (e.Key == Key.End)
            {
                robotlist[namerobot].RotationRight();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void selRobot1_Checked(object sender, RoutedEventArgs e)
        {
            namerobot = "robot1";
        }

        private void selRobot2_Checked(object sender, RoutedEventArgs e)
        {
            namerobot = "robot2";
        }

        private void selRobot3_Checked(object sender, RoutedEventArgs e)
        {
            namerobot = "robot3";
        }
        /*Chau test*/
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            robot.FinishedStatesPublish(2000);
        }
        /*End*/
    }
}
