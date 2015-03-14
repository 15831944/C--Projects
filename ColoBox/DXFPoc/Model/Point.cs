using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model
{
    public struct Point
    {
        private double _x;
        private double _y;
        public double X { get { if (X_Tag.HasValue) return X_Tag.Value; return _x; } set { _x = value; } }
        public double Y { get { if (Y_Tag.HasValue) return Y_Tag.Value; return _y; } set { _y = value; } }
        public double? X_Tag { get; set; }
        public double? Y_Tag { get; set; }        
        internal void Rotate(double angle)
        {
            Vector2 transformed = Utility.Rotate(angle, new Vector2 { X = X, Y = Y }, true);
            X_Tag = transformed.X;
            Y_Tag = transformed.Y;
        }

        public override string ToString()
        {
            if (!X_Tag.HasValue)
                return string.Format("[({0},{1})]", _x, _y);
            return string.Format("[({0},{1})]   T[({2},{3})]",_x.ToString("N2"),_y.ToString("N2"),X_Tag.Value.ToString("N2"),Y_Tag.Value.ToString("N2"));
        }
    }
}
