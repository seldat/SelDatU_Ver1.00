using SeldatMRMS;
using SeldatMRMS.Management.RobotManagent;
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
        public MainWindow()
        {
     
            InitializeComponent();
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
            robot2.initialPos(50,50);
            robot3.initialPos(100,100);
        }

        private void sendPose_Click(object sender, RoutedEventArgs e)
        {
            
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
    }
}
