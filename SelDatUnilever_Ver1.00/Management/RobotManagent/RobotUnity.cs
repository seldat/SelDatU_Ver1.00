using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SeldatMRMS.Management.RobotManagent
{
    public class RobotUnity : RobotBaseService
    {
        public LoadedConfigureInformation loadConfigureInformation;
        public RobotUnity()
        {

        }
        public void Initialize(DataRow row)
        {
            try
            {
                properties.NameID = row.Field<string>("Name ID");
                properties.URL = row.Field<string>("URL");
                properties.Width = double.Parse(row.Field<string>("Width"));
                properties.Height = double.Parse(row.Field<string>("Height"));
                properties.Length = double.Parse(row.Field<string>("Length"));
                properties.L1 = double.Parse(row.Field<string>("L1"));
                properties.L2 = double.Parse(row.Field<string>("L2"));
                properties.WS = double.Parse(row.Field<string>("WS"));
                properties.DistanceIntersection = double.Parse(row.Field<string>("Distance Intersection"));
                // double oriY = double.Parse(row.Field<string>("ORIGINAL").Split(',')[1]);
                loadConfigureInformation.IsLoadedStatus = true;
            }
            catch
            {
                loadConfigureInformation.IsLoadedStatus = false;
            }
        }

        Canvas canvas;
        Ellipse ep;
        Ellipse ep1;
        Ellipse ep2;
        Ellipse ep3;
        Ellipse ep4;
        Ellipse ep5;
        Ellipse ep6;
        TextBlock textblock;
        double x = 0, y = 0;
        double angle = 0;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public RobotUnity(Canvas canvas)
        {
            #region Timer1
            //dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer.Tick += dispatcherTimer_Tick;
            //dispatcherTimer.Interval = new TimeSpan(5000000);
            #endregion


            this.canvas = canvas;
            properties.L1 = 30;
            properties.L2 = 30;
            properties.WS = 40;//40/2
            textblock = new TextBlock();

            ep = new Ellipse();
            ep.Width = 5;
            ep.Height = 5;
            ep.Name = "hello";
            ep.Fill = new SolidColorBrush(Colors.Red);

            ep1 = new Ellipse();
            ep1.Width = 5;
            ep1.Height = 5;
            ep1.Fill = new SolidColorBrush(Colors.Black);
            ep2 = new Ellipse();
            ep2.Width = 5;
            ep2.Height = 5;
            ep2.Fill = new SolidColorBrush(Colors.Blue);
            ep3 = new Ellipse();
            ep3.Width = 5;
            ep3.Height = 5;
            ep3.Fill = new SolidColorBrush(Colors.Red);
            ep4 = new Ellipse();
            ep4.Width = 5;
            ep4.Height = 5;
            ep4.Fill = new SolidColorBrush(Colors.Yellow);
            ep5 = new Ellipse();
            ep5.Width = 5;
            ep5.Height = 5;
            ep5.Fill = new SolidColorBrush(Colors.White);

            ep6 = new Ellipse();
            ep6.Width = 5;
            ep6.Height = 5;
            ep6.Fill = new SolidColorBrush(Colors.White);

            canvas.Children.Add(ep);
            canvas.Children.Add(ep1);
            canvas.Children.Add(ep2);
            canvas.Children.Add(ep3);
            canvas.Children.Add(ep4);
            canvas.Children.Add(ep5);
            // canvas.Children.Add(ep6);
            canvas.Children.Add(textblock);
            //  dispatcherTimer.Start();

        }

        int state = 1;
        int count = 0;
        Random rnd = new Random();
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{

        //   // try
        //    {
        //        state = rnd.Next(1, 6);
        //        count = 0;
        //       // if (count++ > 20) count = 0;
        //        //if(test == 0)
        //        { switch (state)
        //            {
        //                case 1:
        //                   while(count++ <5)
        //                    this.LeftRobot();
        //                    break;
        //                case 2:
        //                    while (count++ < 5)
        //                        this.RightRobot();
        //                    break;
        //                case 3:
        //                    while (count++ < 5)
        //                        this.UpRobot();
        //                    break;
        //                case 4:
        //                    while (count++ < 5)
        //                        this.DownRobot();
        //                    break;
        //                case 5:
        //                    while (count++ < 5)
        //                        this.RotationLeft();
        //                    break;
        //                case 6:
        //                    while (count++ < 5)
        //                       this.RotationRight();
        //                    break;
        //            }       
        //        }
        //       // count = 0;
        //    }
        //   // catch { }
        //}
        public void initialPos(double xx, double yy)
        {
            x = xx;
            y = yy;
            ep.RenderTransform = new TranslateTransform(xx, yy);
            setConner(new Point(xx, yy), angle);
            textblock.Text = this.properties.NameID;
            textblock.FontSize = 8;
            textblock.RenderTransform = new TranslateTransform(xx + 5, yy);
        }
        public void setConner(Point p, double angle)
        {


            textblock.RenderTransform = new TranslateTransform(p.X + 5, p.Y);
            properties.pose.Position = p;
            properties.pose.AngleW = angle;
            ep1.RenderTransform = new TranslateTransform(TopHeader().X, TopHeader().Y);
            ep2.RenderTransform = new TranslateTransform(BottomHeader().X, BottomHeader().Y);
            ep3.RenderTransform = new TranslateTransform(TopTail().X, TopTail().Y);
            ep4.RenderTransform = new TranslateTransform(BottomTail().X, BottomTail().Y);
            ep5.RenderTransform = new TranslateTransform(MiddleHeader().X, MiddleHeader().Y);
            ep6.RenderTransform = new TranslateTransform(MiddleTail().X, MiddleTail().Y);
            //Canvas.SetLeft(ep1, TopHeader().X);
            // Canvas.SetTop(ep1, TopHeader().Y);
            //Canvas.SetLeft(ep2, BottomHeader().X);
            // Canvas.SetTop(ep2, BottomHeader().Y);
            // Canvas.SetLeft(ep3, TopTail().X);
            // Canvas.SetTop(ep3, TopTail().Y);
            // Canvas.SetLeft(ep4, BottomTail().X);
            // Canvas.SetTop(ep4, BottomTail().Y);



        }
        public void UpRobot()
        {

            if (y > 0)
            {
                y = y - ep.Width;
            }
            ep.RenderTransform = new TranslateTransform(x, y);
            setConner(new Point(x, y), angle);
            SupervisorTraffic();
        }
        public void DownRobot()
        {
            if (y < canvas.Height)
                y = y + ep.Width;
            ep.RenderTransform = new TranslateTransform(x, y);
            setConner(new Point(x, y), angle);
            SupervisorTraffic();
        }
        public void LeftRobot()
        {

            if (x > 0)
            {
                x = x - ep.Width;
            }
            ep.RenderTransform = new TranslateTransform(x, y);
            setConner(new Point(x, y), angle);
            SupervisorTraffic();
        }
        public void RightRobot()
        {
            if (x < canvas.Width)
                x = x + ep.Width;
            ep.RenderTransform = new TranslateTransform(x, y);
            setConner(new Point(x, y), angle);
            SupervisorTraffic();
        }

        public void RotationLeft()
        {
            if (angle > -Math.PI)
            {
                angle = angle - 5 * Math.PI / 180;
            }
            ep.RenderTransform = new TranslateTransform(x, y);
            setConner(new Point(x, y), angle);
            SupervisorTraffic();
        }

        public void RotationRight()
        {
            if (angle < Math.PI)
            {
                angle = angle + 5 * Math.PI / 180;
            }
            ep.RenderTransform = new TranslateTransform(x, y);
            setConner(new Point(x, y), angle);
            SupervisorTraffic();
        }

    }
}
