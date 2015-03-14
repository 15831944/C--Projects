using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.BorderControl;

namespace ColoEngine.Model.BorderControl
{
    public class Border : IComparable<Border>
    {        
        internal List<BorderNumberPair> Endpoints { set; get; }
        public double Coordinate { set; get; }
        internal ColoBox BoxWhichBelongTo { set; get; }
        public Orientation BorderType { set; get; }

        /// <summary>
        /// this is the point that found (within a tilted border) to fall between the MinY and MaxY of the currentBox that we placed.
        /// We are using this value when we build the pathSections collection for "Inside"/"Outside" list
        /// </summary>
        internal Point PointWithinBoxLimits { set; get; }
        public Border()
        {
            Endpoints = new List<BorderNumberPair>();
        }

        public Border(RawBorder b, ColoBox box)
        {
            BoxWhichBelongTo = box;
            Endpoints = new List<BorderNumberPair>();
            BorderNumberPair bnp = new BorderNumberPair();
            BorderType = b.BorderType;
            Endpoints.Add(bnp);
            switch (b.BorderType)
            {
                case Orientation.Horizontal:
                    Coordinate = b.Point1.Y;                    
                    bnp.coordinate1 = b.Point1.X;
                    if (b.Point2.X < b.Point1.X)
                    {
                        bnp.coordinate1 = b.Point2.X;
                        bnp.coordinate2 = b.Point1.X;
                    }
                    else
                    {
                        bnp.coordinate2 = b.Point2.X;
                    }
                    break;
                case Orientation.Vertical:
                    Coordinate = b.Point1.X;
                    bnp.coordinate1 = b.Point1.Y;
                    if (b.Point2.Y < b.Point1.Y)
                    {
                        bnp.coordinate1 = b.Point2.Y;
                        bnp.coordinate2 = b.Point1.Y;
                    }
                    else
                    {
                        bnp.coordinate2 = b.Point2.Y;
                    }
                    break;
                default:
                    Coordinate = 0;
                    bnp.coordinate1 = b.Point1.X;
                    bnp.coordinate2 = b.Point1.Y;

                    bnp = new BorderNumberPair();
                    bnp.coordinate1 = b.Point2.X;
                    bnp.coordinate2 = b.Point2.Y;
                    Endpoints.Add(bnp);
                    break;
            }
        }
        internal virtual bool IntersecWith(double minVal, double maxVal)
        {
           
            foreach (BorderNumberPair p in Endpoints)
            {
                //BorderNumberPair bnp = Utility.SortEndpointOrder(pair);

                //if ((bnp.coordinate1 <= minVal && bnp.coordinate2 >= minVal) ||
                //     (bnp.coordinate1 <= maxVal && bnp.coordinate2 >= maxVal)
                //    )
                if ((p.coordinate1 < minVal && p.coordinate2 > minVal) ||
                     (p.coordinate1 < maxVal && p.coordinate2 > maxVal)
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
            //this is a case of tilted borders
            if (Coordinate == other.Coordinate &&
               Coordinate == 0)
            {
                return (int)(PointWithinBoxLimits.X - other.PointWithinBoxLimits.X);
            }
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
