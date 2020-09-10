using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLORA.Dataplane;
using FLORA.Computations;
using FLORA.Dataplane.PacketRouter;

namespace FLORA.FuzzySets
{
    //ITEM STETS
    public enum TransmissionDistance { Close, Medium, Far }
    public enum ResidualEnergy { Low, Medium, High }
    public enum DirectionAngle {ExtraSmall, Small, Medium, Big, SuperLarge }

    public class Input
    {
        public Sensor currentNode;
        public NeighborsTableEntry nei;
        public Input(Sensor _currentNode,NeighborsTableEntry _nei)
        {
            currentNode = _currentNode;
            nei = _nei;
        }
        
        
        public TransmissionDistance TransmissionDistance
        {
            get
            {
                if (TransmissionDistanceCrisp >= 0.0 && TransmissionDistanceCrisp <= 0.29)
                {
                    return TransmissionDistance.Close;
                }
                else if (TransmissionDistanceCrisp > 0.29 && TransmissionDistanceCrisp <= 0.67)
                {
                    return TransmissionDistance.Medium;
                }
                else
                {
                    return TransmissionDistance.Far;
                }
            }
        }
        public ResidualEnergy ResidualEnergy
        {
            get
            {
                if (ResidualEnergyCrisp >= 0.0 && ResidualEnergyCrisp <= 0.30)
                {
                    return ResidualEnergy.Low;
                }
                else if (ResidualEnergyCrisp > 0.30 && ResidualEnergyCrisp <= 0.70)
                {
                    return ResidualEnergy.Medium;
                }
                else
                {
                    return ResidualEnergy.High;
                }
            }
        }
        //已经经过余弦函数计算了
        public DirectionAngle DirectionAngle
        {
            get
            {
                if (DirectionAngleCrisp >= 0 && DirectionAngleCrisp < (1.6 / 180.0)) 
                {
                    return DirectionAngle.ExtraSmall;
                }
                else if (DirectionAngleCrisp >= (1.6 / 180.0) && DirectionAngleCrisp < (60.0 / 180.0))
                {
                    return DirectionAngle.Small;
                }
                else if (DirectionAngleCrisp >= (60.0 / 180.0) && DirectionAngleCrisp < (86.0 / 180.0))
                {
                    return DirectionAngle.Medium;
                }
                else if (DirectionAngleCrisp >= (86.0 / 180.0) && DirectionAngleCrisp < (135.0 / 180.0)) 
                {
                    return DirectionAngle.Big;
                }
                else
                {
                    return DirectionAngle.SuperLarge;
                }
            }
        }
        public double TransmissionDistanceCrisp { get; set; }
        public double ResidualEnergyCrisp { get; set; }
        public double DirectionAngleCrisp { get; set; }
        public double Priority
        {
            get
            {
                return RuleBase.Aggregate(this,currentNode,nei);
            }
        }
    }

    public static class RuleBase
    {
        public static double Aggregate(Input i,Sensor currentNode,NeighborsTableEntry nei)
        {
            TransmissiondistanceFuzzySet tdf = new TransmissiondistanceFuzzySet(i.TransmissionDistanceCrisp);
            ResidualEnergyFuzzySet re = new ResidualEnergyFuzzySet(i.ResidualEnergyCrisp);
            DirectionAngleFuzzySet daf = new DirectionAngleFuzzySet(i.DirectionAngleCrisp);

            //1
            string str = "";
            if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")" + "=" + tmp;
                nei.priorityStr=str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //2
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Big)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")"  + "="+tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //3
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //4
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //5
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //6
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //7
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //8
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //9
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //10
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //next TransmissionDistance.Medium && ResidualEnergy.High && DirectionAngle.small
            //11
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //12
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //13
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //14
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //15
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")"  + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //16
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //17
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.High&& i.DirectionAngle == DirectionAngle.Small)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleSmall * daf.Small);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSmall + "*" + daf.Small + "=" + currentNode.Weights.DirectionAngleSmall * daf.Small + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleSmall;
                return tmp;
            }
            //18
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Big)
            {
                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleBig * daf.Big);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleBig + "*" + daf.Big + "=" + currentNode.Weights.DirectionAngleBig * daf.Big + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleBig;
                return tmp;
            }
            //TransmissionDistace+ResidualEnergy+DirectionAngleMedium
            //改8处
            //19
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //20
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //21
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //22
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //23
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //24
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //25
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //26
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }
            //27
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.Medium)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleMedium * daf.Medium);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleMedium + "*" + daf.Medium + "=" + currentNode.Weights.DirectionAngleMedium * daf.Medium + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleMedium;
                return tmp;
            }

            //TransmissionDistace+ResidualEnergy+DirectionAngleSuperLarge
            //改8处
            //28
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //29
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //30
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //31
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //32
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //33
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //34
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //35
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }
            //36
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.SuperLarge)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleSuperLarge + "*" + daf.SuperLarge + "=" + currentNode.Weights.DirectionAngleSuperLarge * daf.SuperLarge + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleSuperLarge;
                return tmp;
            }

            //TransmissionDistace+ResidualEnergy+DirectionAngleExtraSmall
            //改8处
            //37
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //38
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //39
            else if (i.TransmissionDistance == TransmissionDistance.Close && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceClose * tdf.Close) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceClose + "*" + tdf.Close + "=" + currentNode.Weights.TransmissionDistanceClose * tdf.Close + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceClose + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //40
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //41
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //42
            else if (i.TransmissionDistance == TransmissionDistance.Medium && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceMedium * tdf.Medium) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceMedium + "*" + tdf.Medium + "=" + currentNode.Weights.TransmissionDistanceMedium * tdf.Medium + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceMedium + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //43
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Low && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyLow * re.Low) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyLow + "*" + re.Low + "=" + currentNode.Weights.ResidualEnergyLow * re.Low + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyLow + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //44
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.Medium && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyMedium * re.Medium) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyMedium + "*" + re.Medium + "=" + currentNode.Weights.ResidualEnergyMedium * re.Medium + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyMedium + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }
            //45
            else if (i.TransmissionDistance == TransmissionDistance.Far && i.ResidualEnergy == ResidualEnergy.High && i.DirectionAngle == DirectionAngle.ExtraSmall)
            {

                double tmp = (currentNode.Weights.TransmissionDistanceFar * tdf.Far) + (currentNode.Weights.ResidualEnergyHigh * re.High) + (currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall);
                str = "(" + currentNode.Weights.TransmissionDistanceFar + "*" + tdf.Far + "=" + currentNode.Weights.TransmissionDistanceFar * tdf.Far + ")" + "+" + "(" + currentNode.Weights.ResidualEnergyHigh + "*" + re.High + "=" + currentNode.Weights.ResidualEnergyHigh * re.High + ")" + "+" + "(" + currentNode.Weights.DirectionAngleExtraSmall + "*" + daf.ExtraSmall + "=" + currentNode.Weights.DirectionAngleExtraSmall * daf.ExtraSmall + ")" + "=" + tmp;
                nei.priorityStr = str;
                double sum = currentNode.Weights.TransmissionDistanceFar + currentNode.Weights.ResidualEnergyHigh + currentNode.Weights.DirectionAngleExtraSmall;
                return tmp;
            }


            return 0.0;
        }
    }
}
