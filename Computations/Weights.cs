using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLORA.Computations
{
    public class Weights
    {
        //ResidualEnergy
        public double ResidualEnergyLow { get; set; }
        public double ResidualEnergyMedium { get; set; }
        public double ResidualEnergyHigh { get; set; }

        //Progress
        /*
        public double ProgressWeak { get; set; }
        public double ProgressMedium { get; set; }
        public double ProgressStrong { get; set; }
        */

        //DirectionAngle
        public double DirectionAngleExtraSmall { get; set; }
        public double DirectionAngleSmall { get; set; }
        public double DirectionAngleMedium { get; set; }
        public double DirectionAngleBig { get; set; }
        public double DirectionAngleSuperLarge { get; set; }

        //TransmissionDistance
        public double TransmissionDistanceClose { get; set; }
        public double TransmissionDistanceMedium { get; set; }
        public double TransmissionDistanceFar { get; set; }
    }

}
