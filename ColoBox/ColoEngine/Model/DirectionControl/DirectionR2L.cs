using System.IO;
using ColoEngine.Model.BorderControl;
using ColoEngine.Model.LocationStatemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.DirectionControl
{
    public class DirectionR2L : DirectionBase
    {
        public DirectionR2L(Machine machine)
            : base(machine)
        {

        }

        public DirectionR2L()
            : base(null)
        {

        }
        public override double GetFirstBorderCoordinate()
        {
            double firstXCoordinate = 0;
            //Code using TDD
            //first call SetRelativeLocation
            if (this.LocationMachine.CurrentBoxReference == null)
                this.LocationMachine.CurrentBoxReference = LocationMachine.CurrentBoxToPlace;
            LocationMachine.PathSectionBuilder.SetRelativeLocation(this.LocationMachine.CurrentBoxReference);

            //in the case of R2L we are choosing the highest x value in the PathSection collections, then we choose the smaller
            //between the result of UpperPath and LowerPath
            double maxXUpper = double.MinValue;
            double maxXLower = double.MinValue;
            double valueToCompare = 0;
            foreach (Point p in LocationMachine.PathSectionBuilder.UpperPathSections)
            {
                valueToCompare = getValueToCompare(p);

                if (valueToCompare > maxXUpper)
                {
                    maxXUpper = valueToCompare;
                }
            }

            foreach (Point p in LocationMachine.PathSectionBuilder.LowerPathSections)
            {
                valueToCompare = getValueToCompare(p);

                if (valueToCompare > maxXLower)
                {
                    maxXLower = valueToCompare;
                }
            }

            if (maxXUpper > maxXLower)
                return maxXLower - LocationMachine.CurrentBoxReference.Width;

            return maxXUpper - LocationMachine.CurrentBoxReference.Width;
        }

        private double getValueToCompare(Point p)
        {
            double res = p.Y;
            if (res < p.X)
                res = p.X;

            return res;
        }
        internal override double OriginX(ColoBox boundingBox)
        {
            return boundingBox.Origin.X + boundingBox.Width;
        }

        internal override bool IsTouchingOriginLines(Border vx)
        {
            foreach (Border b in LocationMachine.BorderVerifier.VerticalX)
            {
                if (b.BoxWhichBelongTo == LocationMachine.boundingBox)
                {
                    if (b.Coordinate == LocationMachine.CurrentBoxToPlace.MinX || b.Coordinate == LocationMachine.CurrentBoxToPlace.MaxX)
                        return true;
                }
            }
            //return locationMachine.FirstVerticalLine == vx.Coordinate;
            return false;
        }

        internal override bool IsHittingExteriorBorder(ColoBox boxToPlace)
        {
            return boxToPlace.MinX < LocationMachine.boundingBox.MinX;//.LastVerticalLine;
        }

        internal override double NextBoxLocation(double boxOriginIndexX, double boundaryX)
        {
            return boxOriginIndexX - boundaryX;
        }

        internal override double CalculateNextLinearLocation(Border vx, ColoBox boxToPlace, double spaceAfterObstacle)
        {
            return vx.BoxWhichBelongTo.MinX - boxToPlace.Width - spaceAfterObstacle;
        }

        internal override double getNextLocationAfterOutside(Point point)
        {
            return point.X - LocationMachine.CurrentBoxToPlace.Width;
        }

        internal override bool BoxCrossHorizontalLineOnLineStart(ColoBox boxToPlace, BorderNumberPair bnp)
        {
            return bnp.coordinate2 == boxToPlace.MaxY;
        }

        public override double ClosestX
        {
            get
            {
                return LayoutManager.LocationMachine.CurrentBoxToPlace.MaxX;
            }
        }

        public override double NonClosestX
        {
            get
            {
                return LayoutManager.LocationMachine.CurrentBoxToPlace.MinX;
            }
        }
        internal override double BoxWidthCalculation(double width)
        {
            return width * -1;
        }
        internal override Point setPathCoordinatesOrder(Point point)
        {           
            return new Point{ X = point.Y,Y = point.X};
        }
        //internal override IEnumerable<TiltBorder> GetAngledBordersInPath(ColoBox boxToPlace, double y)
        //{
        //    //1. collect all the angled borders in the path            
        //    //3. create virtual borders as vertical in the correct position so we can consider that point as the valid next point to place a given box
        //    //4. order the borders (sort)
        //    Machine machine = LayoutManager.LocationMachine;
        //    List<TiltBorder> retVal = new List<TiltBorder>();
        //    List<TiltBorder> bordersWithinPath = new List<TiltBorder>();



        //    foreach (TiltBorder b in machine.BorderVerifier.Tilted)
        //    {
        //        if (b.BoxWhichBelongTo != machine.boundingBox)
        //            continue;

        //        LinearLine line = new LinearLine(b);
        //        double lowY, highY;
        //        double rY = Math.Round(y);
        //        Utility.BorderEndpoints(b, out lowY, out highY);



        //        if (b.BorderType == Orientation.Positive)
        //        {
        //            if (Math.Round(y) >= lowY && Math.Round(y) <= highY)
        //            {
        //                //minY is applicaple for both path (upper and lower)
        //                b.BoxTouchingBox = line.GetXIntersect(boxToPlace.MinY);
        //                bordersWithinPath.Add(b);
        //            }
        //        }

        //        if (b.BorderType == Orientation.Negative)
        //        {
        //            if (Math.Round(y) >= lowY && Math.Round(y) <= highY)
        //            {
        //                //minY is applicaple for both path (upper and lower)
        //                b.BoxTouchingBox = line.GetXIntersect(boxToPlace.MinY);
        //                bordersWithinPath.Add(b);
        //            }
        //        }


        //    }
        //    bordersWithinPath.Reverse();

        //    foreach (TiltBorder b in bordersWithinPath)
        //    {
        //        TiltBorder nb = new TiltBorder { Coordinate = b.BoxTouchingBox };
        //        retVal.Add(nb);
        //    }


        //    retVal.Reverse();
        //    return retVal;
        //}

        internal override void GetOrder(ColoBox boxToPlace, Point section, out RelativeLocation first, out RelativeLocation second)
        {
            if (true)//boxToPlace.MaxX >= section.Y)//&& boxToPlace.MaxX <= section.Y)
            {                
                first = RelativeLocation.Inside;
                second = RelativeLocation.Outside;
            }
            else
            {
                first = RelativeLocation.Outside;
                second = RelativeLocation.Inside;
            }
        }

        internal override Point GetMostOutsidePoint(LookAheadPath.CategorizedLocation fromLower, LookAheadPath.CategorizedLocation fromUpper)
        {
            if (fromLower.Point.X <= fromUpper.Point.X)
                return fromLower.Point;

            return fromUpper.Point;
        }

        public override bool IsBoxOutside(ColoBox boxToPlace, LookAheadPath.PathSection pathSection)
        {
            bool upperCheck = false;
            bool lowerCheck = false;
            if (IsBoxOutsideCollection(boxToPlace, pathSection.UpperPathSections))
                upperCheck = true;

            if (IsBoxOutsideCollection(boxToPlace, pathSection.LowerPathSections))
                lowerCheck = true;

            return upperCheck || lowerCheck;// false;  
        }

        internal override bool IsBoxStillInsideBoundingBox(ColoBox boxToPlace)
        {
            return boxToPlace.MinX > LocationMachine.boundingBox.MinX;
        }
        internal override bool BoxInOutsidePaTh(Point pathSection, ColoBox boxToPlace)
        {
            double minX = pathSection.X;
            if (minX > pathSection.Y)
                minX = pathSection.Y;
            return boxToPlace.MinX < minX;
        }

        public override double GetYPointTouchesBorder(ColoBox currentBoxReference, Type type, Border b)
        {
            double lowY, highY;
            if (b is TiltBorder)
            {
                double yOut;
                Utility.BorderEndpoints(b, LocationMachine.InitParam.Tolerance, currentBoxReference.MinY, out lowY, out highY, out yOut);
                if (type == typeof (FirstBorderPositive))
                {
                    if (yOut >= lowY && yOut <= highY)
                    {
                        return yOut;
                    }


                    //even though it makes sense to return the maxY for a positive border when we move L2R, if the maxY is not within the border vertical values, we will have to return the
                    //minY 
                    return currentBoxReference.MaxY;
                }
                else
                {
                    //First Border is negative
                    if (currentBoxReference.MaxY >= lowY && currentBoxReference.MaxY <= highY)
                    {
                        return currentBoxReference.MaxY;
                    }
                    return currentBoxReference.MinY;
                }
            }

            //for a vertical border, it doesn't matter minY or maxY
            return currentBoxReference.MinY;
        }

        private bool IsBoxOutsideCollection(ColoBox boxToPlace, List<Point> collection)
        {
            //first find the path where maxX is live in
            //if that path is "OUTSIDE" return true and leave
            //if that path is "Inside" look for the path of the maxX
            Point thePath = new Point { RelativeLocation = RelativeLocation.Inside };
            foreach (var p in collection)
            {
                if (Utility.GreaterOrEqual(boxToPlace.MaxX, p.X) && Utility.SmallerOrEqual(boxToPlace.MaxX, p.Y))
                {
                    thePath = p;
                    break;
                }
            }

            if (thePath.RelativeLocation.Value == RelativeLocation.Outside)
                return true;


            if (Utility.GreaterOrEqual(boxToPlace.MinX, thePath.X) && Utility.SmallerOrEqual(boxToPlace.MinX, thePath.Y))
            {
                return false;
            }

            return true;










            bool minOutside = false;
            bool maxOutside = false;
            List<bool> collectionFoundOutside = new List<bool>();
            bool foundPath = false;
            //first find the path that the minX (L2R) or maxX(R2L) are with in the path
            //if the path is already "OUTSIDE", return true
            //if the path is inside, check the maxX(L2R) or minX(R2L) that is not outside that path - if it is return true otherwise false
            foreach(var p in collection)
            {
                if (!(Utility.GreaterOrEqual(boxToPlace.MaxX, p.X) && Utility.SmallerOrEqual(boxToPlace.MaxX, p.Y)))
                {
                    continue;
                }
                if (Utility.GreaterOrEqual(boxToPlace.MaxX, p.X) && Utility.SmallerOrEqual(boxToPlace.MaxX, p.Y))
                {
                    foundPath = true;
                    //we found a path that the origin of the box is within it

                    //first find if that path is outside
                    if (p.RelativeLocation.Value == RelativeLocation.Outside)
                    {
                        minOutside = true;
                    }
                    else
                    {
                        if (Utility.GreaterOrEqual(boxToPlace.MinX, p.X) && Utility.SmallerOrEqual(boxToPlace.MinX, p.Y))
                        {
                            //we found a path that the origin of the box is within it

                            //first find if that path is outside
                            if (p.RelativeLocation.Value == RelativeLocation.Outside)
                                minOutside = true;
                        }
                        else
                        {
                            minOutside = true;
                        }
                    }
                }
                else
                {                    
                    maxOutside = true;                    
                }

                collectionFoundOutside.Add(minOutside || maxOutside);
                if (foundPath)
                {                    
                    break;
                }

            }

            return collectionFoundOutside.Any(a => a == true);
        }



        public override List<Border> SortLookaheadPath(List<Border> borders)
        {
            List<double> lst = new List<double>();
            foreach (Border b in borders)
            {
                lst.Add(b.Coordinate);
            }

            lst.Sort();
            List<Border> sorted = new List<Border>();
            for (int i = lst.Count - 1; i > -1; --i)
            {
                foreach (Border b in borders)
                {
                    if (b.Coordinate == lst[i])
                    {
                        sorted.Add(b);
                    }
                }
            }
            return sorted;
        }

        public override double ExtremeX
        {
            get { return double.MinValue; }
        }



        public override bool FurtherSideOutsideSection(Point further)
        {
            return ClosestX + BoxWidthCalculation(LocationMachine.CurrentBoxToPlace.Width) < further.X;
        }

        public override double XAfterOutsidePath(Point p)
        {
            return p.X - LocationMachine.CurrentBoxToPlace.Width;
        }

        internal override int GetSectionIndexToCheck(List<Point> toReverse)
        {
            return toReverse.Count - 1;
        }

        public override double GetFirstBorderForLower()
        {
            double y = LocationMachine.CurrentBoxToPlace.MinY;
            double firstBorder = double.MinValue;
            foreach (Border b in LocationMachine.BorderVerifier.VerticalX.borders)
            {
                double upper;
                double lower;
                Utility.BorderEndpoints(b, 0.5, y, out lower, out upper, out y);
                if (y >= lower && y <= upper)
                {
                    if (b.Coordinate > firstBorder)
                    {
                        firstBorder = b.Coordinate;
                    }
                }

            }

            return firstBorder;
        }

        public override double GetFirstBorderForUpper()
        {
            double y = LocationMachine.CurrentBoxToPlace.MaxY;
            double firstBorder = double.MinValue;
            foreach (Border b in LocationMachine.BorderVerifier.VerticalX.borders)
            {
                double upper;
                double lower;
                Utility.BorderEndpoints(b, 0.5, y, out lower, out upper, out y);
                if (y >= lower && y <= upper)
                {
                    if (b.Coordinate > firstBorder)
                    {
                        firstBorder = b.Coordinate;
                    }
                }

            }

            return firstBorder;
        }

        public override double GetNextRackLocationAfterSpace(double RacksOpenningSpace)
        {
            return -RacksOpenningSpace - LayoutManager.CurrentBoxToPlace.Width;
        }
    }
}
