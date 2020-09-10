using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLORA.FuzzySets
{
    public class TransmissiondistanceFuzzySet
    {
        double transCrisp;
        public TransmissiondistanceFuzzySet(double _transCrisp)
        {
            transCrisp = _transCrisp;
        }
        public double Close
        {
            get
            {
                //缺一个函数；
                double close;
                /*
                double A1 = 0.9761;
                double A2 = 0.00723;
                double x0 = 0.36021;
                double dx = 0.10669;
                close = A2 + (A1 - A2) / (1 + Math.Exp((transCrisp - x0) / (dx)));
                */
                double a1 = 0.60;
                double b1 = 0.34483;
                double a2 = 1.66667;
                double b2 = -3.33333;
                if (transCrisp >= 0.0 && transCrisp < 0.29)
                {
                    close = a1 + b1 * transCrisp;
                }
                else if (transCrisp >= 0.29 && transCrisp < 0.5)
                {
                    close = a2 + b2 * transCrisp;
                }
                else 
                {
                    close = 0.0;
                }
                return close;
            }
        }
        public double Medium
        {
            get
            {
                //缺一个函数；
                double medium;
                /*
                double y0 = 0.23449;
                double xc = 0.47804;
                double w = 0.36869;
                double A = 0.32916;
                medium = y0 + (A / (w * Math.Sqrt(Math.PI / 2.0))) * (Math.Exp(-2 * (Math.Pow(transCrisp - xc, 2)) / (Math.Pow(w, 2))));
                */
                double a2 = -0.01053;
                double b2 = 2.10526;
                double a3 = 0.52368;
                double b3 = 0.26316;
                double a4 = 3.27692;
                double b4 = -3.84615;
                if (transCrisp >= 0.1 && transCrisp < 0.29)
                {
                    medium = a2 + b2 * transCrisp;
                }
                else if (transCrisp >= 0.29 && transCrisp < 0.67)
                {
                    medium = a3 + b3 * transCrisp;
                }
                else if (transCrisp >= 0.67 && transCrisp < 0.8)
                {
                    medium = a4 + b4 * transCrisp;
                }
                else 
                {
                    medium = 0.2;
                }
                return medium;
            }
        }
        public double Far
        {
            get
            {
                //缺一个函数；
                double far;
                /*
                double A1 = 0.01023;
                double A2 = 1.1771;
                double x0 = 0.62611;
                double p = 3.10118;
                far = A2 + (A1 - A2) / (1 + Math.Pow(transCrisp / x0, p));
                */
                double a2 = -0.25532;
                double b2 = 1.2766;
                double a3 = 0.39697;
                double b3 = 0.30303;
                if (transCrisp >= 0.2 && transCrisp < 0.67)
                {
                    far = a2 + b2 * transCrisp;
                }
                else if (transCrisp >= 0.67 && transCrisp <= 1.0)
                {
                    far = a3 + b3 * transCrisp;
                }
                else 
                {
                    far = 0.0;
                }
                return far;
            }
        }
    }
}
