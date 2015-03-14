using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LocationStatemachine
{
    internal class State_HitObstacle : StateBase
    {
        internal State_HitObstacle(Machine machine)
            : base(machine)
        {

        }

        public override Point GetNextLocation()
        {
            throw new NotImplementedException();
        }

        internal override void MoveNext()
        {
            throw new NotImplementedException();
        }
    }
}
