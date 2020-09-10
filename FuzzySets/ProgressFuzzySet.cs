using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLORA.FuzzySets
{
    public class ProgressFuzzySet
    {
        double progressCrisp;
        public ProgressFuzzySet(double _progressCrisp)
        {
            progressCrisp = _progressCrisp;
        }
        public double Weak
        {
            get
            {
                //缺一个函数；
                double weak;
                double var = 13.00007;
                double k = 0.00172;
                double t = 1.6005;
                double x0 = 0.4895;
                weak = k + 1 / (1 + Math.Exp(var * (t * progressCrisp - x0)));
                return weak;
            }
        }
        public double Medium
        {
            get
            {
                //缺一个函数；
                double medium;
                double A = 0.05507;
                double theta = 0.1402;
                double k = 0.01540;
                double x0 = 0.5;
                double cnt1 = A / theta * Math.Sqrt(2 * Math.PI);
                double cnt2 = 1 / Math.Exp(Math.Pow(progressCrisp - x0, 2) / (2 * Math.Pow(theta, 2)));
                medium = cnt1 * cnt2 + k;
                return medium;
            }
        }
        public double Strong
        {
            get
            {
                //缺一个函数；
                double strong;
                double var = 13.00007;
                double k = 0.00381;
                double t = 1.4005;
                double x0 = 0.9725;
                strong = k + 1 / (1 + Math.Exp(-var * (t * progressCrisp - x0)));
                return strong;
            }
        }
    }
}
