using DXFPoc.Model.DirectionControl;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXFPoc.Model.BorderControl;

namespace DXFPoc.Model.LocationStatemachine
{
    internal class Machine
    {
        private StateBase currentState;
        internal State_InsideBounding INSIDEBOUNDINGBOX;
        internal State_RowStart ROWSTARTS;
        internal State_OutsideBounding OUTSIDEBOUNDINGBOX;
        internal State_HitObstacle HITOBSTACLE;
        
        private double coldAisleWidth;
        private List<Border> bordersWithinPath=null;
        internal double OriginX { get { return direction.OriginX(boundingBox);}}// boundingBox.Origin.X; } }
        internal double OriginY { get { return direction.OriginY(boundingBox);}}// boundingBox.Origin.Y; } }
        internal double NextRowStart { get { return RowNumber * coldAisleWidth; } }
        internal int RowNumber { set; get; }
        internal ColoBox boundingBox;
        internal bool IsSteppedOutBoundingBox { set; get; }
        public ColoBox CurrentBoxToPlace { get; set; }
        public bool OnRowStart { get { return currentState is State_RowStart; } }
        public Point? OverrideStartPlace { get; set; }
        public bool CurrentlyOutsideTheBox { get { return currentState is State_OutsideBounding; } }
        public int CurrentIndex { get; set; }
        public double SpaceAfterObstacle { get; set; }
        public bool IsRotated { get; set; }
        public DirectionBase direction { get; set; }

        internal Machine(double coldAisleWidth,double spaceAfterObstacle,ColoBox boundingBox)
        {
            INSIDEBOUNDINGBOX = new State_InsideBounding(this);
            OUTSIDEBOUNDINGBOX = new State_OutsideBounding(this);
            ROWSTARTS = new State_RowStart(this);
            HITOBSTACLE = new State_HitObstacle(this);

            currentState = ROWSTARTS;
            RowNumber = 0;
            this.coldAisleWidth = coldAisleWidth;
            this.SpaceAfterObstacle = spaceAfterObstacle;
            this.boundingBox = boundingBox;
            this.direction = direction;
            IsSteppedOutBoundingBox = false;
        }        

        internal void SetCurrent(StateBase stateBase)
        {
            currentState = stateBase;
        }

        public Point NextLocation
        {
            get 
            {
                if (OverrideStartPlace.HasValue)
                {
                    Point ret = OverrideStartPlace.Value;
                    OverrideStartPlace = null;
                    return ret;
                }
                return currentState.GetNextLocation(); 
            }
        }

        internal void NextState()
        {
            currentState.MoveNext();
        }







        public BorderControl.BorderVerifier BorderVerifier { get; set; }

        public double FirstVerticalLine { get; set; }

        public double LastVerticalLine { get; set; }

        public double CurrentBoxWidth { get; set; }
        /// <summary>
        /// This is a flag that checks if the current location of a box is within the bounding box
        /// usually its after the bounding box was advanced into the space without touching the borders directly
        /// since it touched an obsicale border before and exterior border
        /// </summary>
        public bool LocationWithinBoundingBox { get { return locationWithinBoundingBox(); } }

        private bool locationWithinBoundingBox()
        {
            if (bordersWithinPath == null)
            {
                bordersWithinPath = new List<Border>();
                foreach (Border b in BorderVerifier.VerticalX)
                {

                    BorderNumberPair bnp = Utility.SortEndpointOrder(b.Endpoints)
                }
            }
        }
    }
}
