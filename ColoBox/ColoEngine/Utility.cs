using ColoEngine.Model;
using ColoEngine.Model.BorderControl;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine
{
    public static class Utility
    {
        public static double tolerance = 0.5;

        public static Vector2 Rotate(double angle, Vector2 input,bool clockwise)
        {
            Vector2 ret = new Vector2();
            if (clockwise)
            {
                //clockwise
                ret.X = input.X * Math.Cos(angle) + input.Y * Math.Sin(angle);
                ret.Y = -input.X * Math.Sin(angle) + input.Y * Math.Cos(angle);
            }
            else
            {
                //counter clockwise
                ret.X = input.X * Math.Cos(angle) - input.Y * Math.Sin(angle);
                ret.Y = input.X * Math.Sin(angle) + input.Y * Math.Cos(angle);
            }


            return ret;
        }

        internal static BorderNumberPair SortEndpointOrderDelMe(Model.BorderControl.BorderNumberPair ep)
        {
            BorderNumberPair ret = new BorderNumberPair();

            ret.coordinate1 = ep.coordinate1;
            if (ep.coordinate2 < ret.coordinate1)
            {
                ret.coordinate1 = ep.coordinate2;
                ret.coordinate2 = ep.coordinate1;
            }
            else
                ret.coordinate2 = ep.coordinate2;

            return ret;
        }

        internal static void BorderEndpoints(Border b,double tolerance,double yIn, out double lowY, out double highY,out double y)
        {

            y = yIn;

            if (b.BorderType == Orientation.Vertical)
            {
                lowY = b.Endpoints[0].coordinate1.Value;
                highY = b.Endpoints[0].coordinate2.Value;
                if (lowY > highY)
                {
                    highY = lowY;
                    lowY = b.Endpoints[0].coordinate2.Value;                                       
                }

                //setting up the tolerance window
                if (Math.Abs(Math.Abs(lowY) - Math.Abs(y)) < tolerance)
                    y = lowY;

                if (Math.Abs(Math.Abs(highY) - Math.Abs(y)) < tolerance)
                    y = highY;

                return; 
            }
            lowY = b.Endpoints[0].coordinate2.Value;
            if (b.Endpoints[1].coordinate2.Value < lowY)
            {
                lowY = Math.Round( b.Endpoints[1].coordinate2.Value);
                highY = Math.Round(b.Endpoints[0].coordinate2.Value);
            }
            else
            {
                highY = Math.Round(b.Endpoints[1].coordinate2.Value);
            }

            //setting up the tolerance window
            if (Math.Abs(Math.Abs(lowY) - Math.Abs(y)) < tolerance)
                y = lowY;

            if (Math.Abs(Math.Abs(highY) - Math.Abs(y)) < tolerance)
                y = highY;
        }

        internal static bool GreaterOrEqual(double p1, double p2)
        {
            
            double absP1 = Math.Abs(p1);
            double absP2 = Math.Abs(p2);
            double result = Math.Abs(absP1 - absP2);

            if (result <= tolerance)
                return true;

            return p1 > p2;

        }

        internal static bool Greater(double p1, double p2)
        {
            double absP1 = Math.Abs(p1);
            double absP2 = Math.Abs(p2);
            double result = Math.Abs(absP1 - absP2);

            if (result <= tolerance)
                return false;

            return p1 > p2;
        }

        internal static bool SmallerOrEqual(double p1, double p2)
        {
            double absP1 = Math.Abs(p1);
            double absP2 = Math.Abs(p2);
            double result = Math.Abs(absP1 - absP2);

            if (result <= tolerance)
                return true;

            return p1 < p2;
        }

        internal static bool Smaller(double p1, double p2)
        {
            double absP1 = Math.Abs(p1);
            double absP2 = Math.Abs(p2);
            double result = Math.Abs(absP1 - absP2);

            if (result <= tolerance)
                return false;

            return p1 < p2;
        }
    }
}
