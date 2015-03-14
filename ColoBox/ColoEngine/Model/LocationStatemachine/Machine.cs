using ColoEngine.Model.DirectionControl;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.BorderControl;
using ColoEngine.Model.Init;
using ColoEngine.Model.LookAheadPath;

namespace ColoEngine.Model.LocationStatemachine
{
    public class Machine
    {
        internal StateBase currentState;
        internal State_InsideBounding INSIDEBOUNDINGBOX;
        public State_RowStart ROWSTARTS;
        internal State_OutsideBounding OUTSIDEBOUNDINGBOX;
        internal State_HitObstacle HITOBSTACLE;
        
        
        private List<Border> bordersWithinPath=null;

        public InitParams InitParam { set; get; }
        internal double OriginX { get { return direction.OriginX(boundingBox);}}// boundingBox.Origin.X; } }
        internal double OriginY { get { return direction.OriginY(boundingBox);}}// boundingBox.Origin.Y; } }
        internal double NextRowStart 
        { 
            get 
            { 
                double ret = RowNumber * InitParam.ColdAisleWidth;
                if(this.LineOffsets.ContainsKey(RowNumber))
                {
                    ret+= LineOffsets[RowNumber];
                }
                return ret;
            } 
        }
        public int RowNumber { set; get; }
        public ColoBox boundingBox;
        //internal bool IsSteppedOutBoundingBox { get { return currentState is State_OutsideBounding; } }
        internal bool IsSteppedOutBoundingBox { set; get; }
        public PathSection PathSectionBuilder { set; get; }
        public ColoBox CurrentBoxToPlace { get; set; }
        public ColoBox CurrentBoxReference { get; set; }
        public bool OnRowStart { get { return currentState is State_RowStart; } }

        private Point? overrideStartPlace;

        public Point? OverrideStartPlace
        {
            get
            {
                return overrideStartPlace;
            }
            set
            {
                if (previousOverrideStartPlaces == null)
                {
                    previousOverrideStartPlaces = new HashSet<Point>();
                }
                if (value != null)
                {
                    if (!previousOverrideStartPlaces.Contains(value.Value))
                    {
                        previousOverrideStartPlaces.Add(value.Value);
                        overrideStartPlace = value;
                        return;
                    }
                }
                overrideStartPlace = null;
            }
        }

        private HashSet<Point> previousOverrideStartPlaces;
        public bool CurrentlyOutsideTheBox { get { return currentState is State_OutsideBounding; } }
        public int CurrentIndex { get; set; }        
        public bool IsRotated { get; set; }
        public DirectionBase direction { get; set; }
        public Dictionary<int,double> LineOffsets { get; set; }

        public Machine(InitParams initParam, ColoBox boundingBox)
        {
            INSIDEBOUNDINGBOX = new State_InsideBounding(this);
            OUTSIDEBOUNDINGBOX = new State_OutsideBounding(this);
            ROWSTARTS = new State_RowStart(this);
            HITOBSTACLE = new State_HitObstacle(this);

            currentState = ROWSTARTS;
            RowNumber = 0;
            this.InitParam = initParam;
            this.InitParam.Tolerance = 0.5;    
            this.boundingBox = boundingBox;
            this.direction = direction;
            IsSteppedOutBoundingBox = false;
            //pathSectionBuilder = new PathSectionBuilder(this);
            PathSectionBuilder = new PathSection(this);

            LineOffsets = new Dictionary<int, double>();
            foreach (var offset in initParam.OffsetList)
            {
                LineOffsets.Add(offset.LineIndex, offset.Offset);
            }
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
                    return ret;
                }
                return currentState.GetNextLocation(); 
            }
        }

        public void ResetOverride()
        {
            OverrideStartPlace = null;
        }
        internal void NextState()
        {
            currentState.MoveNext();
        }

        public IEnumerable<TiltBorder> GetAngledBordersInPath(ColoBox boxToPlace, double y)
        {
            //1. collect all the angled borders in the path            
            //3. create virtual borders as vertical in the correct position so we can consider that point as the valid next point to place a given box
            //4. order the borders (sort)
            HashSet<string> foundPositive = new HashSet<string>();
            HashSet<string> foundNegative = new HashSet<string>();
            List<TiltBorder> retVal = new List<TiltBorder>();
            List<TiltBorder> bordersWithinPath = new List<TiltBorder>();



            foreach (TiltBorder b in BorderVerifier.Tilted)
            {
                if (b.BoxWhichBelongTo != boundingBox)
                    continue;

                LinearLine line = new LinearLine(b);
                double lowY, highY;
                double rY = Math.Round(y);
                double yOut;
                Utility.BorderEndpoints(b,InitParam.Tolerance,y, out lowY, out highY,out yOut);

                string pKey = string.Format("{0}p",yOut);
                string nKey = string.Format("{0}n",yOut);

                if (b.BorderType == Orientation.Positive)
                {

                    if (yOut >= lowY && yOut <= highY )//&& ! foundNegative.Contains(nKey))
                    {
                       foundPositive.Add(pKey);
                        //minY is applicaple for both path (upper and lower)
                        b.BoxTouchingBox = line.GetXIntersect(y);
                        bordersWithinPath.Add(b);
                    }
                }

                if (b.BorderType == Orientation.Negative)// && !foundPositive.Contains(pKey))
                {
                    if (yOut >= lowY && yOut <= highY)
                    {
                        foundNegative.Add(nKey);
                        //minY is applicaple for both path (upper and lower)
                        b.BoxTouchingBox = line.GetXIntersect(y);
                        bordersWithinPath.Add(b);
                    }
                }


            }
            bordersWithinPath.Sort();

            foreach (TiltBorder b in bordersWithinPath)
            {
                TiltBorder nb = new TiltBorder { Coordinate = b.BoxTouchingBox };
                retVal.Add(nb);
            }

            return retVal;
           
        }





        public BorderControl.BorderVerifier BorderVerifier { get; set; }

        public double FirstVerticalLine { get; set; }

        public double LastVerticalLine { get; set; }
        

        
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

                    //BorderNumberPair bnp = Utility.SortEndpointOrder(b.Endpoints)
                }
            }
            return false;
        }                       

        internal void ResetRelativeLocation()
        {
            PathSectionBuilder.ResetRelativeLocation();
        }

        internal Point? UpdateMachineState(ColoBox boxToPlace)
        {
            return PathSectionBuilder.UpdateMachineState(boxToPlace);
        }
        public int RacksCountBeforeSpace { get; set; }

        public bool NeedOpenRackSpace { get { return RacksCountBeforeSpace > InitParam.RacksBeforeOpenning; } }

        internal void AssertBoxLocation()
        {
            PathSectionBuilder.AssertBoxLocation();
        }

        public bool IsInsideBoundingBox { get { return currentState is State_InsideBounding; } }

        public bool IsRowStart { get { return currentState == this.ROWSTARTS; } }
    }
}
