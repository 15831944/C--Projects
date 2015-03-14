using DXFPoc.Model.BorderControl;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc
{
    public static class Utility
    {
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

        internal static BorderNumberPair SortEndpointOrder(Model.BorderControl.BorderNumberPair ep)
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
    }
}
