using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.BorderControl
{
    //This class holds 2 coordinate number to represent end point of a line.
    //for a vertical line, it will hold the 2 Y coordinates of the points that render the line
    //for a horizontal line, it will hold the 2 X coordinates of the points that render the line
    internal class BorderNumberPair 
    {
        private double? coordinate1_tag;
        private double? coordinate2_tag;

        private double? _coordinate1;
        private double? _coordinate2;
        internal double? coordinate1 { set { _coordinate1 = value; } get { if (coordinate1_tag.HasValue) return coordinate1_tag; return _coordinate1; } }
        internal double? coordinate2 { set { _coordinate2 = value; } get { if (coordinate2_tag.HasValue) return coordinate2_tag; return _coordinate2; } }
        //{
        //    get { return _coordinate2; }
        //    set
        //    {
        //        if (value < coordinate1)
        //        {
        //            _coordinate2 = coordinate1;
        //            coordinate1 = value;
        //            return;
        //        }
        //        _coordinate2 = value;
        //    }
        //}
        internal int index { set; get; }
        public BorderNumberPair(int index)
        {
            this.index = index;
        }

        public BorderNumberPair()
        {
            index = -1;
        }
        internal bool IsFull
        {
            get
            {
                return coordinate1.HasValue && coordinate2.HasValue;
            }
        }
        public override string ToString()
        {
            return string.Format("[{2}]{0}  {1}",coordinate1.Value.ToString("N2"),coordinate2.Value.ToString("N2"),index);
        }



        internal void Rotate(double angle)
        {
            Vector2 transformed = Utility.Rotate(angle, new Vector2 { X = _coordinate1.Value, Y = _coordinate2.Value }, true);
            coordinate1_tag = transformed.X;
            coordinate2_tag = transformed.Y;

            //clockwise
            //coordinate1_tag = _coordinate1 * Math.Cos(angle) + _coordinate2 * Math.Sin(angle);
            //coordinate2_tag = -_coordinate1 * Math.Sin(angle) + _coordinate2 * Math.Cos(angle);


            //counter clockwise
           // _coordinate1 = coordinate1_tag * Math.Cos(angle) - coordinate2_tag * Math.Sin(angle);
           // _coordinate2 = coordinate1_tag * Math.Sin(angle) + coordinate2_tag * Math.Cos(angle);
        }
    }
}
