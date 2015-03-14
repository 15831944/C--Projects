using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.LocationStatemachine
{
    internal class State_RowStart : StateBase
    {
        //this is the index of the current row. we use that value to trigger a calculation of the collection of all the vertical borders that are on the projecting course of that row
        private int currentRowNumber = -1;
        internal State_RowStart(Machine machine)
            : base(machine)
        {
        }

        internal override Point GetNextLocation()
        {
            double nextYCoordinate = machine.OriginY + machine.NextRowStart;
            //if (currentRowNumber != machine.RowNumber)
            //{
            //    //calculating the collection of the vertical border onthe projecting course
            //    machine.BorderVerifier.CalculateBordersProjecting();


            //    //updating to match the machine rowNumber
            //    currentRowNumber = machine.RowNumber;
            //}


            //verifying that the vertical border of the beginning of the row is infact starts at the origin. if not, just it to the first vertical row that matches
            double nextXCoordinate = machine.direction.GetFirstBorderCoordinate();


            Point nextLocation = new Point { X = nextXCoordinate, Y = nextYCoordinate };
           
           return nextLocation;
        }

        internal override void MoveNext()
        {
            machine.IsSteppedOutBoundingBox = false;
            machine.INSIDEBOUNDINGBOX.SetCurrent();
        }
    }
}
