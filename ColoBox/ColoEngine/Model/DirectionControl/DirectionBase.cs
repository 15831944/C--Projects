using ColoEngine.Model.BorderControl;
using ColoEngine.Model.LocationStatemachine;
using ColoEngine.Model.LookAheadPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.DirectionControl
{
    public abstract class DirectionBase
    {
        public abstract double GetFirstBorderCoordinate();
        public LayoutManager LayoutManager { get; set; }
        internal double? factor;
        internal FirstBorderBase firstBorder;
        public Machine LocationMachine { get; set; }

        public DirectionBase(Machine machine)
        {
            this.LocationMachine = machine;
        }
        internal virtual double OriginY(ColoBox boundingBox)
        {
            return boundingBox.Origin.Y;
        }

        internal abstract double OriginX(ColoBox boundingBox);

        protected bool IsBorderInVector(Border b, double nextYCoordinate)
        {
            bool borderInVector = false;
            
            foreach (var p in b.Endpoints)
            {
                //sep = Utility.SortEndpointOrder(ep);
                if (p.coordinate1 <= nextYCoordinate && p.coordinate2 >= nextYCoordinate)
                {
                    borderInVector = true;
                }
            }

            return borderInVector;
        }

        

        internal abstract bool IsTouchingOriginLines( Border vx);

        internal abstract bool IsHittingExteriorBorder(ColoBox boxToPlace);

        internal abstract double NextBoxLocation(double p, double boundaryX);

        internal abstract double CalculateNextLinearLocation(Border vx, ColoBox boxToPlace, double spaceAfterObstacle);
        
        internal abstract double getNextLocationAfterOutside(Point point);

        internal abstract bool BoxCrossHorizontalLineOnLineStart(ColoBox boxToPlace, BorderNumberPair bnp);        

        internal abstract void GetOrder(ColoBox boxToPlace, Point section, out RelativeLocation first, out RelativeLocation second);

        internal abstract Point GetMostOutsidePoint(CategorizedLocation fromLower, CategorizedLocation fromUpper);

        public abstract bool IsBoxOutside(ColoBox boxToPlace, PathSection pathSection);

        internal abstract bool BoxInOutsidePaTh(Point pathSection, ColoBox boxToPlace);

        internal abstract double BoxWidthCalculation(double width);
        public abstract List<Border> SortLookaheadPath(List<Border> borders);

        public double minX { get; set; }

        public double minY { get; set; }

        public double YOffset { get; set; }


        public abstract double ClosestX { get; }
        public abstract double NonClosestX { get; }
        public abstract double ExtremeX { get; }
        public abstract bool FurtherSideOutsideSection(Point further);
        public abstract double XAfterOutsidePath(Point p);


        internal double GetFirstPositiveTiltedBorder(Border firstBorder)
        {
            LocationStatemachine.Machine m = LayoutManager.LocationMachine;

            double boxNewNextY = GetYPointTouchesBorder(m.CurrentBoxReference, typeof(FirstBorderPositive), firstBorder);
            //if (this.GetType() == typeof (DirectionL2R))
            //{            
            //    boxNewNextY = m.OriginY + LayoutManager.LocationMachine.NextRowStart + m.CurrentBoxToPlace.Height;
            //}
            //else
            //{
            //    boxNewNextY = m.OriginY + LayoutManager.LocationMachine.NextRowStart;
            //}


            LinearLine line = new LinearLine(firstBorder);
            return line.GetXIntersect(boxNewNextY);
        }

        internal double GetFirstNegativeTiltedBorder(Border firstBorder)
        {
            double boxNewNextY = LayoutManager.LocationMachine.OriginY + LayoutManager.LocationMachine.NextRowStart;

            LinearLine line = new LinearLine(firstBorder);
            return line.GetXIntersect(boxNewNextY);
        }




        internal abstract bool IsBoxStillInsideBoundingBox(ColoBox boxToPlace);

        internal abstract Point setPathCoordinatesOrder(Point point);


        public abstract double GetYPointTouchesBorder(ColoBox currentBoxReference, Type type, Border b);
        public abstract double GetFirstBorderForLower();
        public abstract double GetFirstBorderForUpper();
        public abstract double GetNextRackLocationAfterSpace(double RacksOpenningSpace);
        internal abstract int GetSectionIndexToCheck(List<Point> toReverse);

        
        
    }
}
