using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLORA.Intilization;
using FLORA.Dataplane;
using System.Windows;
using FLORA.ui;

namespace FLORA.FuzzySets
{
    public static class Crisps
    {
        public static double TransmissionDistance(Sensor sender, Sensor neighbor)
        {
            double dis = Operations.DistanceBetweenTwoPositioningErrorsSensors(sender, neighbor);
            if (dis / PublicParamerters.CommunicationRangeRadius > 1.0)
            {
                return 1.0;
            }
            else 
            {
                return dis / PublicParamerters.CommunicationRangeRadius;
            }     
        }
        public static double ResidualEnergy(Sensor neighbor)
        {
            if (neighbor.ID == PublicParamerters.SinkNode.ID)
            {
                return neighbor.ResidualEnergy / PublicParamerters.BatteryIntialEnergyForSink;
            }
            else
            {
                return neighbor.ResidualEnergy / PublicParamerters.BatteryIntialEnergy;
            }
        }
        public static double DirectionAngle(Sensor sender, Sensor neighbor, Sensor sinkNode)
        {
            double angle_cos = Operations.computedTheta(sender, neighbor, sinkNode);
            double angle = Math.Acos(angle_cos) / Math.PI * 180;
            double angle_n = angle / 180.0;
            return angle_n;
        }
    }
}
