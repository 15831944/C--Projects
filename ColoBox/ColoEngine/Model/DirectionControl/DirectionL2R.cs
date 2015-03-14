using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.BorderControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.LocationStatemachine;

namespace ColoEngine.Model.DirectionControl
{
    public class DirectionL2R : DirectionBase
    {


        public DirectionL2R(Machine machine)
            : base(machine)
        {
        }
        public DirectionL2R()
            : base(null)
        {
        }
        //gets the coordinate that the current box is going to hit in its path
        public override double GetFirstBorderCoordinate()
        {
            double firstXCoordinate = 0;
            //Code using TDD
            //first call SetRelativeLocation
            LocationMachine.PathSectionBuilder.SetRelativeLocation(this.LocationMachine.CurrentBoxReference);

            //in the case of L2R we are choosing the smallest x value in the PathSection collections, then we choose the bigger
            //between the result of UpperPath and LowerPath
            double minXUpper = double.MaxValue;
            double minXLower = double.MaxValue;
            double valueToCompare = 0;
            foreach (Point p in LocationMachine.PathSectionBuilder.UpperPathSections)
            {
                valueToCompare = getValueToCompare(p); 

                if(valueToCompare < minXUpper)
                {
                    minXUpper = valueToCompare;
                }
            }

            foreach (Point p in LocationMachine.PathSectionBuilder.LowerPathSections)
            {
                valueToCompare = getValueToCompare(p);

                if (valueToCompare < minXLower)
                {
                    minXLower = valueToCompare;
                }
            }
            
            if (minXUpper > minXLower)
                return minXUpper;

            return minXLower;

        }

        private double getValueToCompare(Point p)
        {
            double res = p.X;
            if (res > p.Y)
                res = p.Y;

            return res;
        }


        private bool IsHorizontalLineTouchBox(double newX,double newY,out double newXLocation)
        {
            LocationStatemachine.Machine mch = LayoutManager.LocationMachine;
            newXLocation = -1;
            foreach (Border b in LayoutManager.HorizontalY)
            {
                if(b.BoxWhichBelongTo != mch.boundingBox)
                    continue;

                
                double minx = newX;
                double miny =newY;
                double maxx = minx + mch.CurrentBoxReference.Width;
                double maxy = miny + mch.CurrentBoxReference.Height;
                foreach (BorderNumberPair p in b.Endpoints)
                {
                   // bnp = Utility.SortEndpointOrder(p);
                    if (b.Coordinate <= maxy && b.Coordinate >= miny)
                    {
                        if ( (p.coordinate1 >= minx && p.coordinate1 <= maxx) ||
                              (p.coordinate2 >= minx && p.coordinate2 <= maxx))
                        {
                            if (b.Coordinate != miny && b.Coordinate != maxy)
                                newXLocation = p.coordinate2.Value;
                        }
                    }
                }
            }

            if (newXLocation != -1)
            {
                return true;
            }

            return false;
        }           

        internal override double OriginX(ColoBox boundingBox)
        {
            return boundingBox.Origin.X;
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

        internal override bool IsBoxStillInsideBoundingBox(ColoBox boxToPlace)
        {
            return boxToPlace.MaxX < LocationMachine.boundingBox.MaxX;
        }
        internal override bool IsHittingExteriorBorder(ColoBox boxToPlace)
        {
            double valueToComp = LocationMachine.LastVerticalLine;
            if (valueToComp == 0)
                valueToComp = LocationMachine.boundingBox.MaxX;

            return boxToPlace.MaxX > valueToComp;// locationMachine.LastVerticalLine;
        }

        internal override double NextBoxLocation(double boxOriginIndexX, double boundaryX)
        {
            return boxOriginIndexX + boundaryX;
        }

        internal override double CalculateNextLinearLocation(Border vx, ColoBox boxToPlace, double spaceAfterObstacle)
        {
            return vx.BoxWhichBelongTo.MinX + vx.BoxWhichBelongTo.Width + spaceAfterObstacle;
        }

        internal override double getNextLocationAfterOutside(Point point)
        {
            return point.Y;
        }
        
        internal override bool BoxCrossHorizontalLineOnLineStart(ColoBox boxToPlace, BorderNumberPair bnp)
        {
            return Math.Round(bnp.coordinate1.Value, 2) == Math.Round(boxToPlace.MinY, 2);
        }        

        public override double ClosestX
        {
            get { return LayoutManager.LocationMachine.CurrentBoxToPlace.MinX; }
        }

        public override double NonClosestX
        {
            get
            {
                return LayoutManager.LocationMachine.CurrentBoxToPlace.MaxX;
            }
        }
        internal override double BoxWidthCalculation(double width)
        {
            return width;
        }
        internal override Point setPathCoordinatesOrder(Point point)
        {
            if (point.X > point.Y)
            {
                return new Point { X = point.Y, Y = point.X };
            }
            return point;
        }
        

        internal override void GetOrder(ColoBox boxToPlace, Point section, out RelativeLocation first, out RelativeLocation second)
        {
            if (boxToPlace.MinX <= section.X )//&& boxToPlace.MaxX <= section.Y)
            {
                first = RelativeLocation.Outside;
                second = RelativeLocation.Inside;                
            }
            else 
            {
                first = RelativeLocation.Inside;
                second = RelativeLocation.Outside;
            }
        }

        internal override Point GetMostOutsidePoint(LookAheadPath.CategorizedLocation fromLower, LookAheadPath.CategorizedLocation fromUpper)
        {
            if (fromLower.Point.X >= fromUpper.Point.X)
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

            return upperCheck || lowerCheck;// true;            

        }

        private bool IsBoxOutsideCollection(ColoBox boxToPlace, List<Point> collection)
        {
            //first find the path where minX is live in
            //if that path is "OUTSIDE" return true and leave
            //if that path is "Inside" look for the path of the maxX
            Point thePath = new Point { RelativeLocation = RelativeLocation.Inside };
            foreach (var p in collection)
            {
                if (Utility.GreaterOrEqual(boxToPlace.MinX, p.X) && Utility.SmallerOrEqual(boxToPlace.MinX, p.Y))
                {
                    thePath = p;
                    break;
                }
            }

            if (thePath.RelativeLocation.Value == RelativeLocation.Outside)
                return true;


            if (Utility.GreaterOrEqual(boxToPlace.MaxX, thePath.X) && Utility.SmallerOrEqual(boxToPlace.MaxX, thePath.Y))
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
            foreach (var p in collection)
            {
                if (Utility.GreaterOrEqual(boxToPlace.MinX, p.X) && Utility.SmallerOrEqual(boxToPlace.MinX, p.Y))
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
                        if (Utility.GreaterOrEqual(boxToPlace.MaxX, p.X) && Utility.SmallerOrEqual(boxToPlace.MaxX, p.Y))
                        {
                            //we found a path that the origin of the box is within it

                            //first find if that path is outside
                            if (p.RelativeLocation.Value == RelativeLocation.Outside)
                                maxOutside = true;
                        }
                        else
                        {
                            maxOutside = true;
                        }
                    }
                }
                else
                {
                    minOutside = true;
                }


                collectionFoundOutside.Add(minOutside || maxOutside);
                if (foundPath)
                {                    
                    break;
                }
                
            }

            return collectionFoundOutside.Any(a => a == true);
            
            //Point lastPoint = new Point { X = double.MinValue, Y = 0 };
            //foreach (var p in collection)
            //{
            //    if (p.X >= lastPoint.X)
            //    {
            //        lastPoint = p;
            //    }
            //}

            //if (boxToPlace.MaxX >= lastPoint.X )
            //{
            //    return true;
            //}

            //return false;
        }

        internal override bool BoxInOutsidePaTh(Point pathSection,ColoBox boxToPlace)
        {
            return boxToPlace.MaxX > pathSection.Y;
        }

        public override double GetYPointTouchesBorder(ColoBox currentBoxReference,Type type,Border b)
        {
            double lowY, highY;
            if (b is TiltBorder)
            {
                double yOut;
                Utility.BorderEndpoints(b,LocationMachine.InitParam.Tolerance,currentBoxReference.MaxY, out lowY, out highY,out yOut);
                if (type == typeof (FirstBorderPositive))
                {
                    if (yOut >= lowY && yOut <= highY)
                    {
                        return yOut;
                    }


                    //even though it makes sense to return the maxY for a positive border when we move L2R, if the maxY is not within the border vertical values, we will have to return the
                    //minY 
                    return currentBoxReference.MinY;
                }
                else
                {
                    //First Border is negative
                    if (currentBoxReference.MinY >= lowY && currentBoxReference.MinY <= highY )
                    {
                        return currentBoxReference.MinY;
                    } 
                    return currentBoxReference.MaxY;
                }
            }

            //for a vertical border, it doesn't matter minY or maxY
            return currentBoxReference.MinY;
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
            for (int i = 0; i < lst.Count; ++i)
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
            get { return double.MaxValue; }
        }

        public override bool FurtherSideOutsideSection(Point further)
        {
            return ClosestX + BoxWidthCalculation(LocationMachine.CurrentBoxToPlace.Width) > further.Y;
        }

        public override double XAfterOutsidePath(Point p)
        {
            return p.Y;
        }

        internal override int GetSectionIndexToCheck(List<Point> toReverse)
        {
            return 0;
        }

        public override double GetFirstBorderForLower()
        {
            double y = LocationMachine.CurrentBoxToPlace.MinY;
            double firstBorder = double.MaxValue;
            foreach (Border b in LocationMachine.BorderVerifier.VerticalX.borders)
            {
                double upper;
                double lower;
                Utility.BorderEndpoints(b, 0.5, y, out lower, out upper, out y);
                if (y >= lower && y <= upper)
                {
                    if (b.Coordinate < firstBorder)
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
            double firstBorder = double.MaxValue;
            foreach (Border b in LocationMachine.BorderVerifier.VerticalX.borders)
            {
                double upper;
                double lower;
                Utility.BorderEndpoints(b, 0.5, y, out lower, out upper, out y);
                if (y >= lower && y <= upper)
                {
                    if (b.Coordinate < firstBorder)
                    {
                        firstBorder = b.Coordinate;
                    }
                }

            }

            return firstBorder;
        }

        public override double GetNextRackLocationAfterSpace(double RacksOpenningSpace)
        {
            return RacksOpenningSpace;
        }
    }
}
