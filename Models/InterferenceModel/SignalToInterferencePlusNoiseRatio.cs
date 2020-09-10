using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLORA.Dataplane;
using FLORA.Intilization;
using FLORA.Dataplane.PacketRouter;

namespace FLORA.Models.InterferenceModel
{
    /*
     * 
     * Signal-to-interferece-plus-noise ratio https://en.wikipedia.org/wiki/Signal-to-interference-plus-noise_ratio
     * 
     * Mathematical definition
     * 
     *                 Pr
     * SINR(x) = --------------
     *              I  +  N
     *  Pr: the power of the incoming signal of interest
     *  I: the interference power of the other(intering) signals in the network
     *  N: some noise term, which may be a constant or random
     * 
     * 
     * Free-spcae path loss (FSPL) https://en.wikipedia.org/wiki/Free-space_path_loss
     * Pr                      c                   c
     * -- = Dt * Dr * (----------------) * (----------------)
     * Pt               4 * PI * d * f       4 * PI * d * f  
     *   
     *   Dt: the directivity of the tranmitting antenna  (default is 1)
     *   Dr: the directivity of the receiving antenna    (default is 1)
     *   f: the frequency of a radio wave
     *   c: the speed of light
     *   d: the distance between the antenna
     *                         Pt
     * FSPL(dB) = 10 * Log10(------)
     *                         Pr
     */
    public class SignalToInterferencePlusNoiseRatio
    {
        private static double Frequency = 2.4e9;//2.4GHz
        private static double Dt = 1.0;
        private static double Dr = 1.0;
        private static double c = 299792458.0; // speed of light in vacuum
        private static double N = -130; //dBw //  noise
        /// <summary>
        /// 如果需要换算成dB --> 10 * Log10(SINR)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="interferingNeighbors"></param>
        /// <returns>SINR in dimensionsless unit</returns>
        public static double SignalToInterferencePlusNoiseRation(Sensor sender, Sensor receiver)
        {
            double pathLoss = FreeSpacePathLoss(sender, receiver);
            double Pt = PublicParamerters.TransmissionPower; // w (瓦)
            double Pr = Pt / pathLoss; // w
            double I = 0.0;
            foreach (NeighborsTableEntry nei in receiver.NeighborsTable) 
            {
                //判断哪个邻居节点对发包有影响
                //nei.NeiNode.CurrentSensorState == SensorState.Active && (nei.NeiNode.isSendPacket == true || nei.NeiNode.isRetransmitPacket == true)
                if (nei.NeiNode.ID != sender.ID) 
                {
                    bool isNeighborDeliveryPacket = (DateTime.Now.Ticks/10000) > nei.NeiNode.LastSendPacketTime ? false : true;
                    if (nei.NeiNode.CurrentSensorState == SensorState.Active && isNeighborDeliveryPacket == true)
                    {
                        I = I + (Pt / FreeSpacePathLoss(nei.NeiNode, receiver));
                    }
                }  
            }
            return Pr / (I + Math.Pow(10,N/10));
        }
        /// <summary>
        /// return path loss 无单位
        /// </summary>
        /// <returns> return free space path loss
        public static double FreeSpacePathLoss(Sensor sender, Sensor receiver)
        {
            double d = Operations.DistanceBetweenTwoSensors(sender, receiver);
            double pathLoss = (1 / Dt * Dr) * Math.Pow(((4 * Math.PI * d * Frequency) / c), 2);
            return pathLoss;
        }
    }
}
