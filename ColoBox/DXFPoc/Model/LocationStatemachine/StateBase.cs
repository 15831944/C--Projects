using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.LocationStatemachine
{
    internal abstract class StateBase
    {
        protected Machine machine;

        internal StateBase(Machine machine)
        {
            this.machine = machine;
        }

        abstract internal Point GetNextLocation();
        abstract internal void MoveNext();
        internal void SetCurrent()
        {
            machine.SetCurrent(this);
        }
    }
}
