using FLORA.Dataplane;
using FLORA.Dataplane.PacketRouter;
using FLORA.Intilization;
using FLORA.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FLORA.Dataplane.NOS;
using FLORA.FuzzySets;

namespace FLORA.ControlPlane.NOS.FlowEngin
{
    
    public class MiniFlowTableSorterUpLinkPriority : IComparer<MiniFlowTableEntry>
    {

        public int Compare(MiniFlowTableEntry y, MiniFlowTableEntry x)
        {
            return x.UpLinkPriority.CompareTo(y.UpLinkPriority);
        }
    }

    public class UplinkFlowEnery
    {

        public int CurrentID { get { return Current.ID; } } // ID
        public int NextID { get { return Next.ID; } }
        //
        public double Pr
        {
            get; set;
        }

        // Elementry values:
        public double H { get; set; } // hop to sink
        public double R { get; set; } // riss
        public double L { get; set; } // remian energy
        //
        public double HN { get; set; } // H normalized
        public double RN { get; set; } // R NORMALIZEE value of To. 
        public double LN { get; set; } // L normalized
        //
        public double HP { get; set; } // R normalized
        public double RP { get; set; } // R NORMALIZEE value of To. 
        public double LP { get; set; } // L value of To.

        public double Priority { get; set; }
        public Input Input;

        // return:
        public double Mul
        {
            get
            {
                return RP * LP * HP;
            }
        }

        public Sensor Current { get; set; } // ID
        public Sensor Next { get; set; }
    }

    public class UplinkRouting
    {

        public static void UpdateUplinkFlowEnery(Sensor sender)
        {
            sender.MiniFlowTable.Clear();
            ComputeUplinkFlowEnery(sender);
        }

        public static void ComputeUplinkFlowEnery(Sensor sender)
        {
            List<Point> FourCorners = Operations.PredefinedRoutingZone(sender, PublicParamerters.SinkNode);
            foreach (NeighborsTableEntry can in sender.NeighborsTable)
            {
                if (can.NeiNode.ResidualEnergyPercentage >= 0)
                {
                    MiniFlowTableEntry MiniEntry = new MiniFlowTableEntry();
                    MiniEntry.NeighborEntry = can;
                    MiniEntry.NeighborEntry.D_sn = Operations.DistanceBetweenTwoPositioningErrorsSensors(sender, MiniEntry.NeighborEntry.NeiNode);
                    MiniEntry.NeighborEntry.D_snCrisp = Crisps.TransmissionDistance(sender, MiniEntry.NeighborEntry.NeiNode);
                    MiniEntry.NeighborEntry.theta =Operations.computedAngle(Operations.computedTheta(sender, MiniEntry.NeighborEntry.NeiNode, PublicParamerters.SinkNode));
                    MiniEntry.NeighborEntry.thetaCrisp = Crisps.DirectionAngle(sender, MiniEntry.NeighborEntry.NeiNode, PublicParamerters.SinkNode);
                    MiniEntry.NeighborEntry.residualEnergy = MiniEntry.NeighborEntry.NeiNode.ResidualEnergy;
                    MiniEntry.NeighborEntry.residualEnergyCrisp = Crisps.ResidualEnergy(MiniEntry.NeighborEntry.NeiNode);

                    MiniEntry.NeighborEntry.IsWithinSenderSubZone = Operations.IsNeighborWithinMySubZone(MiniEntry.NeighborEntry.NeiNode, FourCorners);


                    MiniEntry.NeighborEntry.sumEnergyNeiNeiWithinZone = 0.0;
                    MiniEntry.NeighborEntry.numbersNeiNeiWithinZone = 0;

                    Input tmp = new Input(sender, can);
                    tmp.TransmissionDistanceCrisp = MiniEntry.NeighborEntry.D_snCrisp;
                    tmp.ResidualEnergyCrisp = MiniEntry.NeighborEntry.residualEnergyCrisp;
                    tmp.DirectionAngleCrisp = MiniEntry.NeighborEntry.thetaCrisp;
                    MiniEntry.NeighborEntry.priority = tmp.Priority;

                    MiniEntry.NeighborEntry.transDisType = tmp.TransmissionDistance;
                    MiniEntry.NeighborEntry.dAngleType = tmp.DirectionAngle;
                    MiniEntry.NeighborEntry.reEneType = tmp.ResidualEnergy;
                    sender.MiniFlowTable.Add(MiniEntry);
                }
            }


            double sumAll_1 = 0.0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.UpLinkPriority = MiniEntry.NeighborEntry.priority;
                sumAll_1 += MiniEntry.UpLinkPriority;
            }
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.UpLinkPriority = MiniEntry.UpLinkPriority / sumAll_1;
            }
            sender.MiniFlowTable.Sort(new MiniFlowTableSorterUpLinkPriority());


            int forwardersCount = 0;
            double cvThreshold = 1 / Convert.ToDouble(sender.MiniFlowTable.Count);
            double n = Convert.ToDouble(sender.MiniFlowTable.Count) + 1;
            int Ftheashoeld = Convert.ToInt16(Math.Ceiling(Math.Sqrt(Math.Sqrt(n))));
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                if (forwardersCount <= Ftheashoeld && MiniEntry.UpLinkPriority >= cvThreshold && MiniEntry.IsWithinSenderSubZone)
                {
                    MiniEntry.UpLinkAction = FlowAction.Forward;
                    forwardersCount++;
                }
                else MiniEntry.UpLinkAction = FlowAction.Drop;
            }
           

            /********
             * 
             * 
             * 分界线
             * 
             * ***********/


            /*
            double n =  Convert.ToDouble(sender.NeighborsTable.Count) + 1;

            double LControl = Settings.Default.ExpoLCnt * Math.Sqrt(n);
            double HControl = Settings.Default.ExpoHCnt * Math.Sqrt(n);
            double EControl = Settings.Default.ExpoECnt * Math.Sqrt(n);
            double RControl = Settings.Default.ExpoRCnt * Math.Sqrt(n);


            double HSum = 0; // sum of h value.
            double RSum = 0;
            foreach (NeighborsTableEntry can in sender.NeighborsTable)
            {
                HSum += can.H;
                RSum += can.R;
            }

            // normalized values.
            foreach (NeighborsTableEntry neiEntry in sender.NeighborsTable)
            {
                if (neiEntry.NeiNode.ResidualEnergyPercentage >= 0) // the node is a live.
                {
                    MiniFlowTableEntry MiniEntry = new MiniFlowTableEntry();
                    MiniEntry.NeighborEntry = neiEntry;
                    MiniEntry.NeighborEntry.HN = 1.0 / (Math.Pow((Convert.ToDouble(MiniEntry.NeighborEntry.H) + 1.0), HControl));
                    MiniEntry.NeighborEntry.RN = 1 - (Math.Pow(MiniEntry.NeighborEntry.R, RControl) / RSum);
                    MiniEntry.NeighborEntry.LN = Math.Pow(MiniEntry.NeighborEntry.L / 100, LControl);

                    MiniEntry.NeighborEntry.E = Operations.DistanceBetweenTwoPoints(PublicParamerters.SinkNode.CenterLocation, MiniEntry.NeighborEntry.CenterLocation);
                    MiniEntry.NeighborEntry.EN = (MiniEntry.NeighborEntry.E / (Operations.DistanceBetweenTwoPoints(PublicParamerters.SinkNode.CenterLocation, sender.CenterLocation) + sender.ComunicationRangeRadius));

                    sender.MiniFlowTable.Add(MiniEntry);
                }
            }

            // pro sum
            double HpSum = 0; // sum of h value.
            double LpSum = 0;
            double RpSum = 0;
            double EpSum = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                HpSum += (1 - Math.Exp(MiniEntry.NeighborEntry.HN));
                RpSum += Math.Exp(MiniEntry.NeighborEntry.RN);
                LpSum += (1 - Math.Exp(-MiniEntry.NeighborEntry.LN));
                EpSum += (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.EN)), EControl));
            }

            double sumAll = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.NeighborEntry.HP = (1 - Math.Exp(MiniEntry.NeighborEntry.HN)) / HpSum;
                MiniEntry.NeighborEntry.RP = Math.Exp(MiniEntry.NeighborEntry.RN) / RpSum;
                MiniEntry.NeighborEntry.LP = (1 - Math.Exp(-MiniEntry.NeighborEntry.LN)) / LpSum;
                MiniEntry.NeighborEntry.EP = (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.EN)), EControl)) / EpSum;

                MiniEntry.UpLinkPriority = (MiniEntry.NeighborEntry.EP + MiniEntry.NeighborEntry.HP + MiniEntry.NeighborEntry.RP + MiniEntry.NeighborEntry.LP) / 4;
                sumAll += MiniEntry.UpLinkPriority;
            }

            // normalized:
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.UpLinkPriority = (MiniEntry.UpLinkPriority / sumAll);
            }
            // sort:
            sender.MiniFlowTable.Sort(new MiniFlowTableSorterUpLinkPriority());

            // action:
            double average = 1 / Convert.ToDouble(sender.MiniFlowTable.Count);
            int Ftheashoeld = Convert.ToInt16(Math.Ceiling(Math.Sqrt(Math.Sqrt(n)))); // theshold.
            int forwardersCount = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                if (MiniEntry.UpLinkPriority >= average && forwardersCount <= Ftheashoeld)
                {
                    MiniEntry.UpLinkAction = FlowAction.Forward;
                    forwardersCount++;
                }
                else MiniEntry.UpLinkAction = FlowAction.Drop;
            }
            */

        }
    }
}
