using FLORA.Dataplane;
using System;
using System.Windows;
using System.Collections.Generic;
using FLORA.ui;
using FLORA.Dataplane.PacketRouter;
using FLORA.Forwarding;

namespace FLORA.Intilization
{
    public class Operations
    {
        /*
         * Positioning Errors
         */
        public static double DistanceBetweenTwoPositioningErrorsSensors(Sensor sensor1, Sensor sensor2) 
        {
            double dx = (sensor1.LocationErrors.X - sensor2.LocationErrors.X);
            dx *= dx;
            double dy = (sensor1.LocationErrors.Y - sensor2.LocationErrors.Y);
            dy *= dy;
            return Convert.ToDouble(Math.Sqrt(dx + dy));
        }

        public static double DistanceBetweenTwoSensors(Sensor sensor1, Sensor sensor2)
        {
            double dx = (sensor1.CenterLocation.X - sensor2.CenterLocation.X);
            dx *= dx;
            double dy = (sensor1.CenterLocation.Y - sensor2.CenterLocation.Y);
            dy *= dy;
            return Convert.ToDouble(Math.Sqrt(dx + dy));
        }

        public static double DistanceBetweenTwoPoints(Point p1, Point p2)
        {
            double dx = (p1.X - p2.X);
            dx *= dx;
            double dy = (p1.Y - p2.Y);
            dy *= dy;
            return Math.Sqrt(dx + dy);
        }

        /// <summary>
        /// the communication range is overlapped.
        /// 
        /// </summary>
        /// <param name="sensor1"></param>
        /// <param name="sensor2"></param>
        /// <returns></returns>
        public static bool isOverlapped(Sensor sensor1, Sensor sensor2)
        {
            bool re = false;
            double disttance = DistanceBetweenTwoSensors(sensor1, sensor2);
            if (disttance < (sensor1.ComunicationRangeRadius + sensor2.ComunicationRangeRadius))
            {
                re = true;
            }
            return re;
        }

        /// <summary>
        /// check if j is within the range of i.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static bool isInMySensingRange(Sensor i, Sensor j)
        {
            bool re = false;
            double disttance = DistanceBetweenTwoSensors(i, j);
            if (disttance <= (i.VisualizedRadius))
            {
                re = true;
            }
            return re;
        }

        /// <summary>
        /// commnication=sensing rang*2
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static bool isInMyComunicationRange(Sensor i, Sensor j)
        {
            bool re = false;
            double disttance = DistanceBetweenTwoSensors(i, j);
            if (disttance <= (i.ComunicationRangeRadius))
            {
                re = true;
            }
            return re;
        }

        public static double FindNodeArea(double com_raduos)
        {
            return Math.PI * Math.Pow(com_raduos, 2);
        }

        /// <summary>
        /// n!
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Factorial(int n)
        {
            long i, fact;
            fact = n;
            for (i = n - 1; i >= 1; i--)
            {
                fact = fact * i;
            }
            return fact;
        }

        /// <summary>
        /// combination 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double Combination(int n, int k)
        {
            if (k == 0 || n == k) return 1;
            if (k == 1) return n;
            int dif = n - k;
            int max = Max(dif, k);
            int min = Min(dif, k);

            long i, bast;
            bast = n;
            for (i = n - 1; i > max; i--)
            {
                bast = bast * i;
            }
            double mack = Factorial(min);
            double x = bast / mack;
            return x;
        }

        //修改的
        //有点尴尬，忘记这个函数是干嘛的来着。。。。。。

        public static void computeCoordinateA1(Sensor sender, Sensor sinkNode, ref Point A1)
        {
            if (sender.LocationErrors.X == sinkNode.LocationErrors.X)
            {
                A1.Y = sinkNode.LocationErrors.Y;
                A1.X = sinkNode.LocationErrors.X + PublicParamerters.CommunicationRangeRadius;
            }
            else
            {
                A1.Y = sinkNode.LocationErrors.Y - Math.Sqrt((Math.Pow(PublicParamerters.CommunicationRangeRadius, 2) * Math.Pow(sender.LocationErrors.X - sinkNode.LocationErrors.X, 2)) / (Math.Pow(DistanceBetweenTwoPositioningErrorsSensors(sender, sinkNode), 2)));
                A1.X = sinkNode.LocationErrors.X - ((A1.Y - sinkNode.LocationErrors.Y) * (sender.LocationErrors.Y - sinkNode.LocationErrors.Y)) / (sender.LocationErrors.X - sinkNode.LocationErrors.X);
            }
        }
        public static void computeCoordinateA2(Sensor sender, Sensor sinkNode, ref Point A2)
        {
            if (sender.LocationErrors.X == sinkNode.LocationErrors.X)
            {
                A2.Y = sinkNode.LocationErrors.Y;
                A2.X = sinkNode.LocationErrors.X - PublicParamerters.CommunicationRangeRadius;
            }
            else
            {
                A2.Y = sinkNode.LocationErrors.Y + Math.Sqrt((Math.Pow(PublicParamerters.CommunicationRangeRadius, 2) * Math.Pow(sender.LocationErrors.X - sinkNode.LocationErrors.X, 2)) / (Math.Pow(DistanceBetweenTwoPositioningErrorsSensors(sender, sinkNode), 2)));
                A2.X = sinkNode.LocationErrors.X - ((A2.Y - sinkNode.LocationErrors.Y) * (sender.LocationErrors.Y - sinkNode.LocationErrors.Y)) / (sender.LocationErrors.X - sinkNode.LocationErrors.X);
            }
        }

        //计算夹角
        public static double computedTheta(Sensor source, Sensor neighbor, Sensor sinkNode)
        {
            Point s = new Point(source.LocationErrors.X, source.LocationErrors.Y);
            Point n = new Point(neighbor.LocationErrors.X, neighbor.LocationErrors.Y);
            Point d = new Point(sinkNode.LocationErrors.X, sinkNode.LocationErrors.Y);
            double molecule = (n.X - s.X) * (d.X - s.X) + (n.Y - s.Y) * (d.Y - s.Y);
            double denominator = DistanceBetweenTwoPositioningErrorsSensors(source, sinkNode) * DistanceBetweenTwoPositioningErrorsSensors(source, neighbor);
            if (denominator == 0.0)
            {
                return 1.0;
            }
            else
            {
                //处理double自身的精度问题
                double tmp = molecule / denominator;
                if (tmp <= 1)
                {
                    return tmp;
                }
                else
                {
                    return 1.0;
                }
            }
        }
        public static double computedAngle(double theta)
        {
            double angle = Math.Acos(theta) / Math.PI * 180;
            return angle;
        }

        //计算四个角坐标
        public static List<Point> computedFourPoint(Sensor source, Sensor sinkNode, double width, double distance)
        {
            List<Point> point = new List<Point>();
            //compute s1,s3;
            Point s1 = new Point();
            Point s3 = new Point();
            //compute d2,d4;
            Point d2 = new Point();
            Point d4 = new Point();
            if (source.LocationErrors.X == sinkNode.LocationErrors.X)
            {
                s1.Y = source.LocationErrors.Y;
                s3.Y = source.LocationErrors.Y;
                s1.X = source.LocationErrors.X + width;
                s3.X = source.LocationErrors.X - width;
                d2.Y = sinkNode.LocationErrors.Y;
                d4.Y = sinkNode.LocationErrors.Y;
                d2.X = sinkNode.LocationErrors.X + width;
                d4.X = sinkNode.LocationErrors.X - width;
            }
            else
            {
                s1.Y = source.LocationErrors.Y - (width / distance) * (sinkNode.LocationErrors.X - source.LocationErrors.X);
                s3.Y = source.LocationErrors.Y + (width / distance) * (sinkNode.LocationErrors.X - source.LocationErrors.X);
                s1.X = source.LocationErrors.X - (s1.Y - source.LocationErrors.Y) * (sinkNode.LocationErrors.Y - source.LocationErrors.Y) / (sinkNode.LocationErrors.X - source.LocationErrors.X);
                s3.X = source.LocationErrors.X - (s3.Y - source.LocationErrors.Y) * (sinkNode.LocationErrors.Y - source.LocationErrors.Y) / (sinkNode.LocationErrors.X - source.LocationErrors.X);
                d2.Y = sinkNode.LocationErrors.Y - (width / distance) * (sinkNode.LocationErrors.X - source.LocationErrors.X);
                d4.Y = sinkNode.LocationErrors.Y + (width / distance) * (sinkNode.LocationErrors.X - source.LocationErrors.X);
                d2.X = sinkNode.LocationErrors.X - (s1.Y - source.LocationErrors.Y) * (sinkNode.LocationErrors.Y - source.LocationErrors.Y) / (sinkNode.LocationErrors.X - source.LocationErrors.X);
                d4.X = sinkNode.LocationErrors.X - (s3.Y - source.LocationErrors.Y) * (sinkNode.LocationErrors.Y - source.LocationErrors.Y) / (sinkNode.LocationErrors.X - source.LocationErrors.X);
            }
            point.Add(s1);
            point.Add(d2);
            point.Add(s3);
            point.Add(d4);
            return point;
        }



        //判断是否在转发子区域内
        public static bool IsNeighborWithinMySubZone(Sensor neighbor, List<Point> FourCorners)
        {
            Parallelogram Zone = new Parallelogram();
            if (FourCorners != null)
            {
                Zone.P1 = FourCorners[0];
                Zone.P2 = FourCorners[1];
                Zone.P3 = FourCorners[2];
                Zone.P4 = FourCorners[3];
            }
            Geomtric Geo = new Intilization.Geomtric();
            if (PublicParamerters.SinkNode.ID == neighbor.ID)
            {
                return true;
            }
            else
            {
                Point point = neighbor.LocationErrors;
                if (Geo.PointTestParallelogram(Zone, point))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //ZPR
        public static List<Point> PredefinedRoutingZone(Sensor source, Sensor sinkNode)
        {
            List<Point> zone = new List<Point>();
            //double Ws = 2 * PublicParamerters.CommunicationRangeRadius / Math.Sqrt(PublicParamerters.Density);
            //double Ws = (PublicParamerters.CommunicationRangeRadius * Math.Sqrt(2 * PublicParamerters.CommunicationRangeRadius)) / PublicParamerters.Density;
            //double Ws = (PublicParamerters.CommunicationRangeRadius * Math.Sqrt(3 * PublicParamerters.CommunicationRangeRadius)) / PublicParamerters.Density;
            double Ws =2 * PublicParamerters.CommunicationRangeRadius;
            double Xb = PublicParamerters.SinkNode.LocationErrors.X;
            double Yb = PublicParamerters.SinkNode.LocationErrors.Y;
            double Xs = source.LocationErrors.X;
            double Ys = source.LocationErrors.Y;
            double D_sb = Operations.DistanceBetweenTwoPositioningErrorsSensors(source, sinkNode);
            Point c1 = new Point();
            c1.X = Xb - Ws * (Yb - Ys) / (2 * D_sb);
            c1.Y = Yb - Ws * (Xs - Xb) / (2 * D_sb);
            Point c2 = new Point();
            c2.X = Xb + Ws * (Yb - Ys) / (2 * D_sb);
            c2.Y = Yb + Ws * (Xs - Xb) / (2 * D_sb);
            Point c3 = new Point();
            c3.X = Xs - Ws * (Yb - Ys) / (2 * D_sb);
            c3.Y = Ys - Ws * (Xs - Xb) / (2 * D_sb);
            Point c4 = new Point();
            c4.X = Xs + Ws * (Yb - Ys) / (2 * D_sb);
            c4.Y = Ys + Ws * (Xs - Xb) / (2 * D_sb);
            zone.Add(c1);
            zone.Add(c2);
            zone.Add(c3);
            zone.Add(c4);
            return zone;

        }
        public static bool IsDeliverySuccessInCaseOfInterference() 
        {
            double linkErrorRatio = UnformRandomNumberGenerator.GetUniform(PublicParamerters.MaximumErrorRatio);
            double successRatio = UnformRandomNumberGenerator.GetUniform(1.0);
            if (linkErrorRatio < successRatio)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private static int Max(int n1,int n2) { if (n1 > n2) return n1; else return n2; }
        private static int Min(int n1, int n2) { if (n1 < n2) return n1; else return n2; } 
    }
}
