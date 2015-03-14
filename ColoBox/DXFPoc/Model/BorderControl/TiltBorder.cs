using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.BorderControl
{
    internal class TiltBorder:Border
    {
        internal override bool IntersecWith(double minVal, double maxVal)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
