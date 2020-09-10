using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLORA.Dataplane;
using FLORA.FuzzySets;

namespace FLORA.Intilization
{
    public class WeightsComputations
    {
        public static void computeWeights(List<Sensor> myNetWork,int length)
        {
            foreach (Sensor currentNode in myNetWork)
            {
                PairwiseComparisonMatrixComputeMethods cmp = new PairwiseComparisonMatrixComputeMethods();
                cmp.distance= Convert.ToDouble(currentNode.HopsToSink);
                double[,] A_tmp = new double[11,11]
                {
                    {1.0,cmp.FunWithTCAndTM(),cmp.FunWithTCAndTF(),cmp.FunWithTCAndRL(),cmp.FunWithTCAndRM(),cmp.FunWithTCAndRH(),cmp.FunWithTCAndDES(),cmp.FunWithTCAndDS(),cmp.FunWithTCAndDM(),cmp.FunWithTCAndDB(),cmp.FunWithTCAndDSL()},
                    { 1/cmp.FunWithTCAndTM(),1.0,cmp.FunWithTMAndTF(),cmp.FunWithTMAndRL(),cmp.FunWithTMAndRM(),cmp.FunWithTMAndRH(),cmp.FunWithTMAndDES(),cmp.FunWithTMAndDS(),cmp.FunWithTMAndDM(),cmp.FunWithTMAndDB(),cmp.FunWithTMAndDSL() },
                    { 1/cmp.FunWithTCAndTF(),1/cmp.FunWithTMAndTF(),1.0,cmp.FunWithTFAndRL(),cmp.FunWithTFAndRM(),cmp.FunWithTFAndRH(),cmp.FunWithTFAndDES(),cmp.FunWithTFAndDS(),cmp.FunWithTFAndDM(),cmp.FunWithTFAndDB(),cmp.FunWithTFAndDSL() },
                    { 1/cmp.FunWithTCAndRL(),1/cmp.FunWithTMAndRL(),1/cmp.FunWithTFAndRL(),1.0,cmp.FunWithRLAndRM(),cmp.FunWithRLAndRH(),cmp.FunWithRLAndDES(),cmp.FunWithRLAndDS(),cmp.FunWithRLAndDM(),cmp.FunWithRLAndDB(),cmp.FunWithRLAndDSL() },
                    { 1/cmp.FunWithTCAndRM(),1/cmp.FunWithTMAndRM(),1/cmp.FunWithTFAndRM(),1/cmp.FunWithRLAndRM(),1.0,cmp.FunWithRMAndRH(),cmp.FunWithRMAndDES(),cmp.FunWithRMAndDS(),cmp.FunWithRMAndDM(),cmp.FunWithRMAndDB(),cmp.FunWithRMAndDSL()},
                    { 1/cmp.FunWithTCAndRH(),1/cmp.FunWithTMAndRH(),1/cmp.FunWithTFAndRH(),1/cmp.FunWithRLAndRH(),1/cmp.FunWithRMAndRH(),1.0,cmp.FunWithRHAndDES(),cmp.FunWithRHAndDS(),cmp.FunWithRHAndDM(),cmp.FunWithRHAndDB(),cmp.FunWithRHAndDSL()},
                    { 1/cmp.FunWithTCAndDES(),1/cmp.FunWithTMAndDES(),1/cmp.FunWithTFAndDES(),1/cmp.FunWithRLAndDES(),1/cmp.FunWithRMAndDES(),1/cmp.FunWithRHAndDES(),1.0,cmp.FunWithDESAndDS(),cmp.FunWithDESAndDM(),cmp.FunWithDESAndDB(),cmp.FunWithDESAndDSL()},
                    { 1/cmp.FunWithTCAndDS(),1/cmp.FunWithTMAndDS(),1/cmp.FunWithTFAndDS(),1/cmp.FunWithRLAndDS(),1/cmp.FunWithRMAndDS(),1/cmp.FunWithRHAndDS(),1/cmp.FunWithDESAndDS(),1.0,cmp.FunWithDSAndDM(),cmp.FunWithDSAndDB(),cmp.FunWithDSAndDSL()},
                    { 1/cmp.FunWithTCAndDM(),1/cmp.FunWithTMAndDM(),1/cmp.FunWithTFAndDM(),1/cmp.FunWithRLAndDM(),1/cmp.FunWithRMAndDM(),1/cmp.FunWithRHAndDM(),1/cmp.FunWithDESAndDM(),1/cmp.FunWithDSAndDM(),1.0,cmp.FunWithDMAndDB(),cmp.FunWithDMAndDSL()},
                    { 1/cmp.FunWithTCAndDB(),1/cmp.FunWithTMAndDB(),1/cmp.FunWithTFAndDB(),1/cmp.FunWithRLAndDB(),1/cmp.FunWithRMAndDB(),1/cmp.FunWithRHAndDB(),1/cmp.FunWithDESAndDB(),1/cmp.FunWithDSAndDB(),1/cmp.FunWithDMAndDB(),1.0,cmp.FunWithDBAndDSL()},
                    { 1/cmp.FunWithTCAndDSL(),1/cmp.FunWithTMAndDSL(),1/cmp.FunWithTFAndDSL(),1/cmp.FunWithRLAndDSL(),1/cmp.FunWithRMAndDSL(),1/cmp.FunWithRHAndDSL(),1/cmp.FunWithDESAndDSL(),1/cmp.FunWithDSAndDSL(),1/cmp.FunWithDMAndDSL(),1/cmp.FunWithDBAndDSL(),1.0}
                };
                PairwiseComparisonMatrix matrix = new PairwiseComparisonMatrix();
                matrix.A = A_tmp;
                double[,] matrix_nor = new double[length, length];
                matrix_nor = matrix.getA_nor();
                double[] weights = new double[length];
                for (int i = 0; i < length; i++)
                {
                    double sum = 0.0;
                    for (int j = 0; j < length; j++)
                    {
                        sum += matrix_nor[i, j];
                    }
                    weights[i] = sum / Convert.ToDouble(length);
                }

                currentNode.Weights.TransmissionDistanceClose = weights[0];
                currentNode.Weights.TransmissionDistanceMedium = weights[1];
                currentNode.Weights.TransmissionDistanceFar = weights[2];

                currentNode.Weights.ResidualEnergyLow = weights[3];
                currentNode.Weights.ResidualEnergyMedium = weights[4];
                currentNode.Weights.ResidualEnergyHigh = weights[5];

                currentNode.Weights.DirectionAngleExtraSmall = weights[6];
                currentNode.Weights.DirectionAngleSmall = weights[7];
                currentNode.Weights.DirectionAngleMedium = weights[8];
                currentNode.Weights.DirectionAngleBig = weights[9];
                currentNode.Weights.DirectionAngleSuperLarge = weights[10];

            }
            
        }

        public static double[] ComputeFeedBackWeights(double[,] A,int length)
        {
            double[,] A_nor = new double[length, length];
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < length; k++)
                {
                    double sum1 = 0.0;
                    for (int l = 0; l < length; l++)
                    {
                        sum1 += A[l, k];
                    }
                    A_nor[j, k] = A[j, k] / sum1;
                }
            }
            double[] weights = new double[length];
            for (int j = 0; j < length; j++)
            {
                double sum2 = 0.0;
                for (int l = 0; l < length; l++)
                {
                    sum2 += A_nor[j, l];
                }
                weights[j] = sum2 / Convert.ToDouble(length);
            }
            return weights;
        }
    }
}
