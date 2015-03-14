using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LocationStatemachine
{
    public class State_RowStart : StateBase
    {
        //this is the index of the current row. we use that value to trigger a calculation of the collection of all the vertical borders that are on the projecting course of that row
        private int currentRowNumber = -1;
        public bool IsAdvancedFromFirstPositionInALine = false;
        public State_RowStart(Machine machine)
            : base(machine)
        {
        }

        public override Point GetNextLocation()
        {
            double nextYCoordinate = machine.InitParam.OriginY + machine.OriginY + machine.NextRowStart;
            machine.CurrentBoxReference.MoveOriginTo(new Point { X = machine.CurrentBoxReference.MinX,Y=nextYCoordinate});

            //verifying that the vertical border of the beginning of the row is infact starts at the origin. if not, just it to the first vertical row that matches
            double nextXCoordinate = machine.direction.GetFirstBorderCoordinate() + machine.InitParam.OriginX;


            Point nextLocation = new Point { X = nextXCoordinate, Y = nextYCoordinate };
            //machine.INSIDEBOUNDINGBOX.SetCurrent();
           return nextLocation;
        }

        internal override void MoveNext()
        {            
            machine.IsSteppedOutBoundingBox = false;
            machine.INSIDEBOUNDINGBOX.SetCurrent();
        }

        internal override void SetCurrent()
        {
            base.SetCurrent();
            //sets the list of the locations vs. inside/outside the box, so we can compare and change the state based on the current location of the moving box
            machine.ResetRelativeLocation();

            machine.RacksCountBeforeSpace = 0;
        }
    }
}
