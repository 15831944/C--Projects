using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.LocationStatemachine
{
    internal class State_HitObstacle : StateBase
    {
        internal State_HitObstacle(Machine machine)
            : base(machine)
        {

        }

        internal override Point GetNextLocation()
        {
            throw new NotImplementedException();
        }

        internal override void MoveNext()
        {
            throw new NotImplementedException();
        }
    }
}
