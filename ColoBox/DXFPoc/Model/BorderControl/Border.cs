using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.BorderControl
{
    public class Border : IComparable<Border>
    {        
        internal List<BorderNumberPair> Endpoints { set; get; }
        internal double Coordinate { set; get; }
        internal ColoBox BoxWhichBelongTo { set; get; }        
        public Border()
        {
            Endpoints = new List<BorderNumberPair>();
        }

        internal virtual bool IntersecWith(double minVal, double maxVal)
        {
           
            foreach (BorderNumberPair pair in Endpoints)
            {
                BorderNumberPair bnp = Utility.SortEndpointOrder(pair);

                if ((bnp.coordinate1 <= minVal && bnp.coordinate2 >= minVal) ||
                     (bnp.coordinate1 <= maxVal && bnp.coordinate2 >= maxVal)
                    )
                {

                    return true;
                }
            }


            //foreach (BorderNumberPair pair in Endpoints)
            //{
            //    if ((pair.coordinate1 <= minVal && pair.coordinate2 >= minVal) ||
            //         (pair.coordinate1 <= maxVal && pair.coordinate2 >= maxVal)
            //        )
            //    {

            //        return true;
            //    }
            //}

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (BorderNumberPair bnp in Endpoints)
            {
                string coor2 = bnp.coordinate2 == null ? "null" : bnp.coordinate2.Value.ToString("N0");
                sb.AppendFormat("[{0},{1}({2})]", bnp.coordinate1.Value.ToString("N0"), coor2,bnp.index);

            }
            string ret = string.Format("{0}:[{2}]{1}", Endpoints.Count, sb.ToString(), Coordinate.ToString("N0"));
            if (this is TiltBorder)
            {
                ret = string.Format("{0}[Tilted]", ret);
            }

            return ret;
        }

        public int CompareTo(Border other)
        {
            return (int)(Coordinate - other.Coordinate);
        }

        public override int GetHashCode()
        {
            return (int)(Coordinate * Endpoints[0].coordinate1 * Endpoints[0].coordinate2);
        }

        internal void Rotate(double angle)
        {
            foreach (BorderNumberPair bnp in Endpoints)
            {
                bnp.Rotate(angle);
            }
        }

        internal Border GetNewBorder(bool removeNulls)
        {
            Border b = new Border { Coordinate = this.Coordinate };

            List<BorderNumberPair> newEndpoints = new List<BorderNumberPair>();
            foreach (BorderNumberPair ep in Endpoints)
            {
                newEndpoints.Add(ep);

                if (ep.coordinate2 == null && removeNulls)
                {
                    newEndpoints.Remove(ep);
                }

                if (ep.coordinate2.HasValue && !removeNulls)
                {
                    newEndpoints.Remove(ep);
                }
            }

            b.Endpoints = newEndpoints;

            return b;
        }
    }
}
