using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FLORA.Dataplane;
using FLORA.Computations;

namespace FLORA.FuzzySets
{
    public class PairwiseComparisonMatrix
    {
        public double[,] A = new double[11, 11];

        public double[,] getA_nor()
        {
            int length = 11;
            double[,] A_nor = new double[length, length];
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < length; k++)
                {
                    double sum = 0.0;
                    for (int l = 0; l < length; l++)
                    {
                        sum += A[l, k];
                    }
                    A_nor[j, k] = A[j, k] / sum;
                }
            }
            return A_nor;
        }

    }

    public class PairwiseComparisonMatrixComputeMethods
    {
        public double distance { get; set; }

        //TC
        //1
        public double FunWithTCAndTM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0) 
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 4.0;
            }
            else
            {
                return 1.0 / 3.0;
            }
        }
        //2
        public double FunWithTCAndTF()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 1.0 / 4.0;
            }
        }
        //3
        public double FunWithTCAndRL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0;
            }
            else
            {
                return 2.0;
            }
        }
        //4
        public double FunWithTCAndRM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 4.0;
            }
            else
            {
                return 2.0;
            }
        }
        //5
        public double FunWithTCAndRH()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 4.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0 / 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 2.0;
            }
        }
        //6
        public double FunWithTCAndDES()
        {
            if (distance <= 1.0)
            {
                return 1.0 / 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 1.0 / 5.0;
            }
        }
        //7
        public double FunWithTCAndDS()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 1.0 / 5.0;
            }
        }
        //8
        public double FunWithTCAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 9.0 / 12.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 4.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //9
        public double FunWithTCAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 5.0 / 8.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 1.0;
            }
        }
        //10
        public double FunWithTCAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 14.0 / 15.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 2.0;
            }
        }
        //TM
        //1
        public double FunWithTMAndTF()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0;
            };
        }
        //2
        public double FunWithTMAndRL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 4.0;
            }
            else
            {
                return 3.0;
            }
        }
        //3
        public double FunWithTMAndRM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0;
            }
            else
            {
                return 3.0;
            }
        }
        //4
        public double FunWithTMAndRH()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 4.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0 / 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 3.0;
            }
        }
        //5
        public double FunWithTMAndDES()
        {
            if (distance <= 1.0)
            {
                return 1.0 / 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 3.0;
            }
            else
            {
                return 1.0 / 3.0;
            }
        }
        //6
        public double FunWithTMAndDS()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 3.0;
            }
            else
            {
                return 1.0 / 3.0;
            }
        }
        //7
        public double FunWithTMAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 9.0 / 12.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //8
        public double FunWithTMAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 5.0 / 8.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 3.0;
            }
            else
            {
                return 2.0;
            }
        }
        //9
        public double FunWithTMAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 14.0 / 15.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 3.0;
            }
            else
            {
                return 3.0;
            }
        }

        //TF
        //1
        public double FunWithTFAndRL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 5.0;
            }
            else
            {
                return 3.0;
            }
        }
        //2
        public double FunWithTFAndRM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 3.0;
            }
        }
        //3
        public double FunWithTFAndRH()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 4.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0 / 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0;
            }
            else
            {
                return 3.0;
            }
        }
        //4
        public double FunWithTFAndDES()
        {
            if (distance <= 1.0)
            {
                return 1.0 / 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //5
        public double FunWithTFAndDS()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //6
        public double FunWithTFAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 9.0 / 12.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0 / 3.0;
            }
            else
            {
                return 1.0;
            }
        }
        //7
        public double FunWithTFAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 5.0 / 8.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 4.0;
            }
            else
            {
                return 3.0;
            }
        }
        //8
        public double FunWithTFAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 14.0 / 15.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 4.0;
            }
            else
            {
                return 5.0;
            }
        }

        //RL
        //1
        public double FunWithRLAndRM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 4.0;
            }
            else
            {
                return 1.0;
            }
        }
        //2
        public double FunWithRLAndRH()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 4.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 4.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0 / 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 1.0;
            }
        }
        //3
        public double FunWithRLAndDES()
        {
            if (distance <= 1.0)
            {
                return 1.0 / 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 5.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 5.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 1.0 / 6.0;
            }
        }
        //4
        public double FunWithRLAndDS()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 5.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 5.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 5.0;
            }
            else
            {
                return 1.0 / 6.0;
            }
        }
        //5
        public double FunWithRLAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 9.0 / 12.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 4.0;
            }
            else
            {
                return 1.0 / 3.0;
            }
        }
        //6
        public double FunWithRLAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 5.0 / 8.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //7
        public double FunWithRLAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 14.0 / 15.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 1.0;
            }
        }

        //RM
        //1
        public double FunWithRMAndRH()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0 / 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0;
            }
        }
        //2
        public double FunWithRMAndDES()
        {
            if (distance <= 1.0)
            {
                return 1.0 / 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 3.0;
            }
            else
            {
                return 1.0 / 6.0;
            }
        }
        //3
        public double FunWithRMAndDS()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 3.0;
            }
            else
            {
                return 1.0 / 6.0;
            }
        }
        //4
        public double FunWithRMAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 9.0 / 12.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0 / 3.0;
            }
        }
        //5
        public double FunWithRMAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 5.0 / 8.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 3.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //6
        public double FunWithRMAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 14.0 / 15.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 3.0;
            }
            else
            {
                return 1.0;
            }
        }

        //RH
        //1
        public double FunWithRHAndDES()
        {
            if (distance <= 1.0)
            {
                return 1.0 / 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0 / 6.0;
            }
        }
        //2
        public double FunWithRHAndDS()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0 / 2.0;
            }
            else
            {
                return 1.0 / 6.0;
            }
        }
        //3
        public double FunWithRHAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 9.0 / 12.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0 / 3.0;
            }
            else
            {
                return 1.0 / 3.0;
            }
        }
        //4
        public double FunWithRHAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 3.0 / 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 5.0 / 8.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 4.0;
            }
            else
            {
                return 1.0 / 2.0;
            }
        }
        //5
        public double FunWithRHAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 5.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 14.0 / 15.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 4.0;
            }
            else
            {
                return 1.0;
            }
        }
        //DES
        //1
        public double FunWithDESAndDS()
        {
            if (distance <= 1.0)
            {
                return 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 1.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 1.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 1.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0;
            }
            else
            {
                return 1.0;
            }
        }
        //2
        public double FunWithDESAndDM()
        {
            if (distance <= 1.0)
            {
                return 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 3.0;
            }
            else
            {
                return 3.0;
            }
        }
        //3
        public double FunWithDESAndDB()
        {
            if (distance <= 1.0)
            {
                return 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 5.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 5.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 5.0;
            }
            else
            {
                return 5.0;
            }
        }
        //4
        public double FunWithDESAndDSL()
        {
            if (distance <= 1.0)
            {
                return 9.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 5.0;
            }
            else
            {
                return 6.0;
            }
        }


        //DS
        //1
        public double FunWithDSAndDM()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 3.0;
            }
            else
            {
                return 3.0;
            }
        }
        //2
        public double FunWithDSAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 5.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 5.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 5.0;
            }
            else
            {
                return 5.0;
            }
        }
        //3
        public double FunWithDSAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 6.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 7.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 4.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 5.0;
            }
            else
            {
                return 6.0;
            }
        }

        //DM
        //1
        public double FunWithDMAndDB()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 3.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 3.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 3.0;
            }
        }
        //2
        public double FunWithDMAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 5.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 5.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 3.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 2.0;
            }
            else
            {
                return 5.0;
            }
        }

        //DB
        //1
        public double FunWithDBAndDSL()
        {
            if (distance <= 1.0)
            {
                return 1.0;
            }
            else if (distance > 1.0 && distance <= 2.0)
            {
                return 2.0;
            }
            else if (distance > 2.0 && distance <= 3.0)
            {
                return 2.0;
            }
            else if (distance > 3.0 && distance <= 4.0)
            {
                return 2.0;
            }
            else if (distance > 4.0 && distance <= 5.0)
            {
                return 1.0;
            }
            else
            {
                return 2.0;
            }
        }
    }

}
