using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SeldatMRMS.Management.RobotManagent
{
    public class RobotUnity:RobotBaseService
    {
        public LoadedConfigureInformation loadConfigureInformation;
        public RobotUnity()
        {

        }
        public void Initialize(DataRow row)
        {
            try
            {
                properties.NameID =row.Field<string>("Name ID");
                properties.URL = row.Field<string>("URL");
                properties.Width = double.Parse(row.Field<string>("Width"));
                properties.Height = double.Parse(row.Field<string>("Height"));
                properties.Length = double.Parse(row.Field<string>("Length"));
                properties.L1= double.Parse(row.Field<string>("L1"));
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
        TextBlock textblock;
        double x = 0, y = 0;
        double angle = 0;
        public RobotUnity(Canvas canvas)
        {
            this.canvas = canvas;
            properties.L1 = 30;
            properties.L2 = 30;
            properties.WS = 20;
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
            canvas.Children.Add(ep);
            canvas.Children.Add(ep1);
            canvas.Children.Add(ep2);
            canvas.Children.Add(ep3);
            canvas.Children.Add(ep4);
            canvas.Children.Add(textblock);
        }

        public void initialPos(double xx, double yy)
        {
            x = xx;
            y = yy;
            ep.RenderTransform = new TranslateTransform(xx, yy);
            setConner(new Point(xx, yy), angle);
            textblock.Text = this.properties.NameID;
            textblock.FontSize = 8;
            textblock.RenderTransform = new TranslateTransform(xx+5,yy);
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
