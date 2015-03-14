using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model
{
    public struct Point : IComparable<Point>
    {
        //private double _x;
        //private double _y;
        public double X {set;get;}// { get { if (X_Tag.HasValue) return X_Tag.Value; return _x; } set { _x = value; } }
        public double Y {set;get;}// { get { if (Y_Tag.HasValue) return Y_Tag.Value; return _y; } set { _y = value; } }
        //public double? X_Tag { get; set; }
        //public double? Y_Tag { get; set; }   
        public double OrigX { set; get; }
        public double OrigY { set; get; }
        public RelativeLocation? RelativeLocation { get; set; }
        internal void Rotate(double angle)
        {
            Vector2 transformed = Utility.Rotate(angle, new Vector2 { X = X, Y = Y }, true);
           // X_Tag = transformed.X;
            //Y_Tag = transformed.Y;
        }

        public override string ToString()
        {
            // if (!X_Tag.HasValue)
            string retval = string.Format("[({0},{1})]", Math.Round(X, 2), Math.Round(Y,2));
            if (this.RelativeLocation.HasValue)
            {
                retval = string.Format("{0}[{1}]", retval, RelativeLocation.Value);
            }

            return retval;
            // return string.Format("[({0},{1})]   T[({2},{3})]",X.ToString("N2"),Y.ToString("N2"),X_Tag.Value.ToString("N2"),Y_Tag.Value.ToString("N2"));
        }

        public int CompareTo(Point other)
        {
            return (int)(X - other.X + Y - other.Y);
        }
    }
}
