using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLORA.FuzzySets
{
    public class DirectionAngleFuzzySet
    {
        double angCrisp;
        public DirectionAngleFuzzySet(double _angCrisp)
        {
            angCrisp = _angCrisp;
        }
        public double ExtraSmall 
        {
            get 
            {
                //缺一个函数;
                double extraSmall;
                double a1 = 1.0;
                double b1 = -3.6036;
                double a2 = 0.89989;
                double b2 = -1.79978;
                if (angCrisp >= 0 && angCrisp < 0.0555)
                {
                    extraSmall = a1 + b1 * angCrisp;
                }
                else if (angCrisp >= 0.0555 && angCrisp < 0.5)
                {
                    extraSmall = a2 + b2 * angCrisp;
                }
                else 
                {
                    extraSmall = 0.0;
                }
                return extraSmall;
            }
        }
        public double Small
        {
            get
            {
                //缺一个函数；
                /*
                double A1 = 1.0147;
                double A2 = 0.02905;
                double x0 = 0.2718;
                double dx = 0.07058;
                double small = A2 + (A1 - A2) / (1 + Math.Exp((angCrisp - x0) / (dx)));
                */
                double small;
                double a1 = 0.6;
                double b1 = 9.00901;
                double a2 = 0.70232;
                double b2 = -0.20929;
                double a3 = 1.07857;
                double b3 = -1.71429;
                if (angCrisp >= 0 && angCrisp < 0.0111)
                {
                    small = a1 + b1 * angCrisp;
                }
                else if (angCrisp >= 0.0111 && angCrisp < 0.25)
                {
                    small = a2 + b2 * angCrisp;
                }
                else if (angCrisp >= 0.25 && angCrisp < 0.6) 
                {
                    small = a3 + b3 * angCrisp;
                }
                else
                {
                    small = 0.05;
                }
                return small;
            }
        }
        public double Medium
        {
            get
            {
                //缺一个函数;
                /*
                double medium;
                double y0 = 0.15991;
                double xc = 0.24875;
                double w = 0.3882;
                double A = 0.38814;
                medium = y0 + (A / (w * Math.Sqrt(Math.PI / 2.0))) * (Math.Exp(-2 * (Math.Pow(angCrisp - xc, 2)) / (Math.Pow(w, 2))));
                */
                double medium;
                double a1 = 0.5;
                double b1 = 1.2;
                double a2 = 0.95;
                double b2 = -0.6;
                double a3 = 1.56667;
                double b3 = -1.83333;
                if (angCrisp >= 0 && angCrisp < 0.25)
                {
                    medium = a1 + b1 * angCrisp;
                }
                else if (angCrisp >= 0.25 && angCrisp < 0.5)
                {
                    medium = a2 + b2 * angCrisp;
                }
                else if (angCrisp >= 0.5 && angCrisp < 0.8)
                {
                    medium = a3 + b3 * angCrisp;
                }
                else 
                {
                    medium = 0.1;
                }
                return medium;
            }
        }
        public double Big
        {
            get
            {
                //缺一个函数；
                /*
                double A1 = 0;
                double A2 = 0.95;
                double x0 = 0.48871;
                double p = 3.0;
                double big = A2 + (A1 - A2) / (1 + Math.Pow(angCrisp / x0, p));
                */
                double big;
                double a1 = 0.3;
                double b1 = 1.0;
                double a2 = 1.1;
                double b2 = -0.6;
                double a3 = 4.4;
                double b3 = -5.0;
                if (angCrisp >= 0 && angCrisp < 0.5)
                {
                    big = a1 + b1 * angCrisp;
                }
                else if (angCrisp >= 0.5 && angCrisp < 0.75)
                {
                    big = a2 + b2 * angCrisp;
                }
                else if (angCrisp >= 0.75 && angCrisp < 0.85)
                {
                    big = a3 + b3 * angCrisp;
                }
                else
                {
                    big = 0.15;
                }
                return big;
            }
        }
        public double SuperLarge 
        {
            get 
            {
                //缺一个函数；
                double superLarge;
                /*
                double A1 = 0;
                double A2 = 0.95;
                double x0 = 0.48871;
                double p = 3.0;
                superLarge = A2 + (A1 - A2) / (1 + Math.Pow(angCrisp / x0, p));
                */
                double a2 = -0.53333;
                double b2 = 1.77778;
                double a3 = 1.1;
                double b3 = -0.4;
                if (angCrisp >= 0.3 && angCrisp < 0.75)
                {
                    superLarge = a2 + b2 * angCrisp;
                }
                else if (angCrisp >= 0.75 && angCrisp <= 1.0)
                {
                    superLarge = a3 + b3 * angCrisp;
                }
                else 
                {
                    superLarge = 0.0;
                }
                return superLarge;
            }
        }
    }
}
