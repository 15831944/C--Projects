using ColoEngine.Model.DirectionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.BorderControl
{
    public class TiltBorder : Border, IComparable<TiltBorder>
    {
        private Border b;        

        public TiltBorder(RawBorder rb, ColoBox box) :base(rb,box)
        {
            
        }

        public TiltBorder()
        {

        }

        public double BoxTouchingBox { set; get; }        
        public DirectionBase Direction { set; get; }
        internal override bool IntersecWith(double minVal, double maxVal)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public int CompareTo(TiltBorder other)
        {
            return (int)(this.BoxTouchingBox - other.BoxTouchingBox);
        }
    }
}
