using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LocationStatemachine
{
    public abstract class StateBase
    {
        protected Machine machine;

        internal StateBase(Machine machine)
        {
            this.machine = machine;
        }

        abstract public Point GetNextLocation();
        abstract internal void MoveNext();
        internal virtual void SetCurrent()
        {
            machine.SetCurrent(this);
        }
    }
}
