using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.LocationStatemachine
{
    internal class State_OutsideBounding : StateBase
    {
        internal State_OutsideBounding(Machine machine)
            : base(machine)
        {

        }

        internal override Point GetNextLocation()
        {
            return machine.CurrentBoxToPlace.NextXLocation;
            //if (false)//!isInsideBoundingBox)
            //{
            //    //this is the case that we found a local obstacle and we need to keep moving in the space to look for
            //    //more available location before we exiting the bounding box
            //    if (boundingBox.MaxX > boxToPlace.MinX)
            //    {
            //        calculatedNextLocation = new Vector2 { X = boxToPlace.MaxX, Y = boxToPlace.MinY };
            //        return false;
            //    }
            //    else
            //    {
            //        calculatedNextLocation = new Vector2 { X = boundingBox.MinX, Y = ++row * boxToPlace.Height + coldAisle };
            //        return false;
            //    }
            //}
        }

        internal override void MoveNext()
        {
            if (machine.IsSteppedOutBoundingBox)
                return;

            machine.ROWSTARTS.SetCurrent();
        }
    }
}
