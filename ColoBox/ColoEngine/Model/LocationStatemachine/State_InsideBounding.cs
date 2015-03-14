using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LocationStatemachine
{
    internal class State_InsideBounding : StateBase
    {
        internal State_InsideBounding(Machine machine)
            : base(machine)
        {

        }


        public override Point GetNextLocation()
        {
            return machine.CurrentBoxToPlace.NextXLocation;
        }

        internal override void MoveNext()
        {
            if (!machine.IsSteppedOutBoundingBox)
                return;

            machine.OUTSIDEBOUNDINGBOX.SetCurrent();
        }
    }
}