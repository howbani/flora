using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLORA.FuzzySets
{
    public class ResidualEnergyFuzzySet
    {
        double enerCrisp;
        public ResidualEnergyFuzzySet(double _enerCrisp)
        {
            enerCrisp = _enerCrisp;
        }
        public double Low
        {
            get
            {
                //缺一个函数；
                double low;
                /*
                double A1 = 0.96276;
                double A2 = 0.0328;
                double x0 = 0.40557;
                double dx = 0.05975;
                low = A2 + (A1 - A2) / (1 + Math.Exp((enerCrisp - x0) / (dx)));
                */
                double a1 = 0.75;
                double b1 = 0.5;
                double a2 = 1.575;
                double b2 = -2.25;
                if (enerCrisp >= 0.0 && enerCrisp < 0.3)
                {
                    low = a1 + b1 * enerCrisp;
                }
                else if (enerCrisp >= 0.3 && enerCrisp < 0.7)
                {
                    low = a2 + b2 * enerCrisp;
                }
                else
                {
                    low = 0.0;
                }
                return low;
            }
        }
        public double Medium
        {
            get
            {
                //缺一个函数；
                double medium;
                /*
                double y0 = -0.21604;
                double xc = 0.70434;
                double w = 0.96835;
                double A = 1.46083;
                medium = y0 + (A / (w * Math.Sqrt(Math.PI / 2.0))) * (Math.Exp(-2 * (Math.Pow(enerCrisp - xc, 2)) / (Math.Pow(w, 2))));
                */
                double a1 = -0.075;
                double b1 = 2.75;
                //double a2 = 0.6375;
                //double b2 = 0.375;
                double a2 = 0.725;
                double b2 = 0.25;
                double a3 = 3.35;
                double b3 = -3.5;
                if (enerCrisp >= 0.1 && enerCrisp < 0.3)
                {
                    medium = a1 + b1 * enerCrisp;
                }
                else if (enerCrisp >= 0.3 && enerCrisp < 0.7)
                {
                    medium = a2 + b2 * enerCrisp;
                }
                else if (enerCrisp >= 0.7 && enerCrisp < 0.9)
                {
                    medium = a3 + b3 * enerCrisp;
                }
                else
                {
                    medium = 0.2;
                }
                return medium;
            }
        }
        public double High
        {
            get
            {
                //缺一个函数；
                double high;
                /*
                double A1 = 0.04391;
                double A2 = 1.01215;
                double x0 = 0.65326;
                double p = 8.6196;
                high = A2 + (A1 - A2) / (1 + Math.Pow(enerCrisp  / x0, p));
                */
                double a2 = -1.875;
                double b2 = 3.75;
                double a3 = 0.4;
                double b3 = 0.5;
                if (enerCrisp >= 0.5 && enerCrisp < 0.7)
                {
                    high = a2 + b2 * enerCrisp;
                }
                else if (enerCrisp >= 0.7 && enerCrisp <= 1)
                {
                    high = a3 + b3 * enerCrisp;
                }
                else
                {
                    high = 0.0;
                }
                return high;
            }
        }
        /*
        public double Low
        {
            get
            {
                //缺一个函数；
                double low;
                double a = 0.25;
                double b = 0.5;
                if (enerCrisp <= a)
                {
                    low = 1;
                }
                else if (enerCrisp > a && enerCrisp <= b)
                {
                    low = (b - enerCrisp) / (b - a);
                }
                else
                {
                    low = 0;
                }
                return low;
            }
        }
        public double Medium
        {
            get
            {
                //缺一个函数；
                double medium ;
                double a = 0.25;
                double b = 0.35;
                double c = 0.65;
                double d = 0.75;
                if (enerCrisp <= a)
                    medium = 0;
                else if (enerCrisp > a && enerCrisp <= b)
                    medium = (enerCrisp - a) / (b - a);
                else if (enerCrisp > b && enerCrisp <= c)
                    medium = 1;
                else if (enerCrisp > c && enerCrisp <= d)
                    medium = (d - enerCrisp) / (d - c);
                else
                    medium = 0;
                return medium;
            }
        }
        public double High
        {
            get
            {
                //缺一个函数；
                double high;
                double a = 0.5;
                double b = 0.75;
                if (enerCrisp <= a)
                    high = 0;
                else if (enerCrisp > a && enerCrisp <= b)
                    high = (enerCrisp - a) / (b - a);
                else
                    high = 1;
                return high;
            }
        }
        */
    }
}
