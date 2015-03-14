using ColoEngine.Model.BorderControl;
using ColoEngine.Model.LocationStatemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LookAheadPath
{
    public class PathSection : IPathBase
    {
        public List<Point> UpperPathSections = null;
        public List<Point> LowerPathSections = null;
        public Machine machine { get; set; }

        public PathSection(Machine machine)
        {
            this.machine = machine;
        }

        #region public
        public void ResetRelativeLocation()
        {
            UpperPathSections = null;
            LowerPathSections = null;
        }

        public void AssertBoxLocation()
        {
            if (UpperPathSections == null)
                return;

            bool isInside = true;           
            foreach (Point kvp in UpperPathSections)
            {                
                if(boxInOutsidePath(kvp))
                {
                   if(kvp.RelativeLocation == RelativeLocation.Outside)
                       isInside = false;
                }
            }

            foreach (Point kvp in LowerPathSections)
            {                  
                if(boxInOutsidePath(kvp))
                {
                   if(kvp.RelativeLocation == RelativeLocation.Outside)
                       isInside = false;
                }
            }                        


            if(isInside)
                machine.INSIDEBOUNDINGBOX.SetCurrent();
            else
                machine.OUTSIDEBOUNDINGBOX.SetCurrent();


        }


        private bool boxInOutsidePath(Point pathSection)
        {
            return machine.direction.BoxInOutsidePaTh(pathSection,machine.CurrentBoxToPlace);            
        }

        public void SetRelativeLocation(ColoBox boxToPlace)
        {            
            if (UpperPathSections == null || LowerPathSections == null)
            {
                machine.CurrentBoxToPlace = boxToPlace;
                UpperPathSections = createPathDictionary(boxToPlace,boxToPlace.MaxY);
                LowerPathSections = createPathDictionary(boxToPlace,boxToPlace.MinY);
                
                //making sure that if an inside renders as outside we might be able to convert from outside to inside
                if(UpperPathSections.Count != LowerPathSections.Count )
                {                    
                    reverseOrder();                    
                }
            }
        }

        

        private List<Point> copySections(List<Point> pathSections)
        {
            List<Point> lst = new List<Point>();
            foreach (Point p in pathSections)
            {
                lst.Add(p);
            }
            return lst;
        }
        

        //shifting the location by one unit (up for lowerPath and down for upper path) if the borderFirstCoordinate of the new box equals to the
        //last pathCoordinate - we need to reverse
        private bool needRevers(Point path,int dir,bool isLower=true)
        {
            double origFirstBorder;
            double shiftedFirstBorder;
            if (isLower)
            {
                machine.CurrentBoxToPlace.MoveOriginTo(new Point { X = machine.CurrentBoxToPlace.MinX, Y = machine.CurrentBoxToPlace.MinY + dir });
                origFirstBorder = machine.direction.GetFirstBorderForLower();
                machine.CurrentBoxToPlace.MoveOriginTo(new Point { X = machine.CurrentBoxToPlace.MinX, Y = machine.CurrentBoxToPlace.MinY - dir });
                shiftedFirstBorder = machine.direction.GetFirstBorderForLower();
            }
            else
            {
                machine.CurrentBoxToPlace.MoveOriginTo(new Point { X = machine.CurrentBoxToPlace.MinX, Y = machine.CurrentBoxToPlace.MinY + dir });
                origFirstBorder = machine.direction.GetFirstBorderForUpper();
                machine.CurrentBoxToPlace.MoveOriginTo(new Point { X = machine.CurrentBoxToPlace.MinX, Y = machine.CurrentBoxToPlace.MinY - dir });
                shiftedFirstBorder = machine.direction.GetFirstBorderForUpper();
            }

            return origFirstBorder != shiftedFirstBorder;
        }

        private void reverseOrder()
        {
            string whichLevelToChange = "";
            bool needReverseFlag = false;
            List<Point> toReverse = null;
            int sectionIndexToCheck = -1;
            if (UpperPathSections.Count > LowerPathSections.Count)
            {
                toReverse = UpperPathSections;
                whichLevelToChange = "upper";
                sectionIndexToCheck = machine.direction.GetSectionIndexToCheck(toReverse);
                needReverseFlag = needRevers(UpperPathSections[sectionIndexToCheck], -2);

                if(!needReverseFlag)
                    needReverseFlag = needRevers(UpperPathSections[sectionIndexToCheck], -2,false);
            }
            else
            {
                toReverse = LowerPathSections;
                whichLevelToChange = "lower";
                sectionIndexToCheck = machine.direction.GetSectionIndexToCheck(toReverse);
                needReverseFlag = needRevers(LowerPathSections[sectionIndexToCheck], 2);
            }

            

            if (!needReverseFlag)
                return;

            

            bool needToReverse = false;
            foreach (Border b in machine.BorderVerifier.HorizontalY.borders)
            {
                if (toReverse[sectionIndexToCheck].X == b.Endpoints[0].coordinate1.Value &&
                    toReverse[sectionIndexToCheck].Y == b.Endpoints[0].coordinate2.Value)
                {
                    needToReverse = true;
                    break;
                }
            }

            if (needToReverse)
            {
                PerformRevers(toReverse,whichLevelToChange);
            }
        }

        private void PerformRevers(List<Point> toReverse,string whichLevelToChange)
        {
            List<Point> reversed = new List<Point>();
            Point newpoint;
            foreach (Point p in toReverse)
            {
                newpoint = new Point { X = p.X, Y = p.Y };

                if (p.RelativeLocation.Value == RelativeLocation.Inside)
                {
                    newpoint.RelativeLocation = RelativeLocation.Outside;
                    reversed.Add(newpoint);
                }
                else
                {
                    newpoint.RelativeLocation = RelativeLocation.Inside;
                    reversed.Add(newpoint);
                }
            }

            if (whichLevelToChange == "upper")
            {
                UpperPathSections = reversed;
            }
            else
            {
                LowerPathSections = reversed;
            }

            
        }


        public Point? UpdateMachineState(ColoBox boxToPlace)
        {
            //The state "RowStarts" prepare the pathSections down stream of the current row so we can compare if the box is inside or outside the bounding box
            SetRelativeLocation(boxToPlace);
            Point outsidePath = new Point {X = double.MinValue, RelativeLocation = RelativeLocation.Outside };
            bool isOutside = false;

            //analyzing the upper
            foreach (var p in UpperPathSections)
            {


                if (machine.direction.ClosestX >= p.X && machine.direction.ClosestX <= p.Y)
                {
                    
                    if (p.RelativeLocation == RelativeLocation.Outside)
                    {//test path 1
                        isOutside = true;
                        outsidePath = p;
                        break;
                    }


                    //if (machine.direction.ClosestX + machine.direction.BoxWidthCalculation(boxToPlace.Width) >= p.Y)
                    if (machine.direction.FurtherSideOutsideSection(p))
                    {
                        if (p.RelativeLocation == RelativeLocation.Inside)
                        {//test path 2
                            isOutside = true;
                            //outsidePath = p;
                            break;
                        }
                    }
                }
            }

            //only in case that we didn't find "OUTSIDE" in the Upper, try look into the lower
            if (!isOutside)
            {
                foreach (var p in LowerPathSections)
                {

                    if (machine.direction.ClosestX >= p.X && machine.direction.ClosestX <= p.Y)
                    {
                        if (p.RelativeLocation == RelativeLocation.Outside)
                        {//test path 3
                            isOutside = true;
                            outsidePath = p;
                            break;
                        }


                        //if (machine.direction.ClosestX + machine.direction.BoxWidthCalculation(boxToPlace.Width) >= p.Y)
                        if (machine.direction.FurtherSideOutsideSection(p))
                        {
                            if (p.RelativeLocation == RelativeLocation.Inside)
                            {//test path 4
                                isOutside = true;
                                //outsidePath = p;
                                break;
                            }
                        }
                    }
                }
            }


            if (isOutside)
            {
                machine.OUTSIDEBOUNDINGBOX.SetCurrent();
                if (outsidePath.X == double.MinValue)
                    return null;

               //throw new NotImplementedException();
                //In this case we found an "outside" within the path so we need to provide the points which is the first point
                //inside after we pass that "outside" path
                machine.INSIDEBOUNDINGBOX.SetCurrent();
                return new Point {X = machine.direction.XAfterOutsidePath(outsidePath), Y = boxToPlace.MinY }; //first point of the next "inside" path
            }
            


            //we are looking to find an "outside" path 
            if (machine.direction.IsBoxOutside(boxToPlace, this))
            {
                machine.OUTSIDEBOUNDINGBOX.SetCurrent();
                return null;
            }


            if (machine.ROWSTARTS.IsAdvancedFromFirstPositionInALine)
            {
                machine.INSIDEBOUNDINGBOX.SetCurrent();
            }

            return null;


            //CategorizedLocation fromLower = null;
            //CategorizedLocation fromUpper = null;

            //fromLower = getCategorizedLocation(LowerPathSections, boxToPlace);
            //fromUpper = getCategorizedLocation(UpperPathSections, boxToPlace);


            //if (fromLower == null && fromUpper == null)
            //    return null;


            ////if both locations upperPath and lowerPath are outside, get the most further one
            //if (fromLower.Location == RelativeLocation.Outside && fromUpper.Location == RelativeLocation.Outside)
            //{
            //    return machine.direction.GetMostOutsidePoint(fromLower,fromUpper);
            //}
            //else if (fromLower.Location == RelativeLocation.Outside)
            //{
            //    return fromLower.Point;
            //}
            //else if (fromUpper.Location == RelativeLocation.Outside)
            //{
            //    return fromUpper.Point;
            //}


            //foreach (KeyValuePair<Point, string> el in LowerPathSections)
            //{
            //    if (boxToPlace.MaxX >= el.Key.X && boxToPlace.MaxX <= el.Key.Y)
            //    {
            //        //if we set the state to OUTSIDE, we are going to set the return point to be the next location after the outside
            //        //ret = setInsideOutsideState(value, machine.direction.getNextLocationAfterOutside(el.Key, machine), boxToPlace.MinY);
            //        if (setInsideOutsideState(el.Value))
            //        {
            //            double x = machine.direction.getNextLocationAfterOutside(el.Key, machine);
            //            ret = new Point { X = x, Y = boxToPlace.MinY };
            //            fromLower = new CategorizedLocation { Point = ret.Value, Location = "Outside" };
            //            break;
            //        }
            //    }
            //}



            //foreach (KeyValuePair<Point, string> el in pathSections)
            //{
            //    if (boxToPlace.MaxX >= el.Key.X && boxToPlace.MaxX <= el.Key.Y)
            //    {
            //        //this can be "Inside" or "Outside"
            //        string value = el.Value;

            //        //if we set the state to OUTSIDE, we are going to set the return point to be the next location after the outside
            //        //ret = setInsideOutsideState(value, machine.direction.getNextLocationAfterOutside(el.Key, machine), boxToPlace.MinY);
            //        if (setInsideOutsideState(value))
            //        {
            //            double x = machine.direction.getNextLocationAfterOutside(el.Key, machine);
            //            ret = new Point { X = x, Y = boxToPlace.MinY };
            //        }
            //    }
            //}

            return null;
        }

        private CategorizedLocation getCategorizedLocation(List<Point> pathSections, ColoBox boxToPlace)
        {
            foreach (Point el in UpperPathSections)
            {
                if (boxToPlace.MaxX >= el.X && boxToPlace.MaxX <= el.Y)
                {
                    //if we set the state to OUTSIDE, we are going to set the return point to be the next location after the outside
                    //ret = setInsideOutsideState(value, machine.direction.getNextLocationAfterOutside(el.Key, machine), boxToPlace.MinY);
                    if (setInsideOutsideState(el.RelativeLocation.Value))
                    {
                        double x = machine.direction.getNextLocationAfterOutside(el);                        
                        return new CategorizedLocation { Point = new Point { X = x, Y = boxToPlace.MinY }, Location =  RelativeLocation.Outside };                        
                    }
                }
            }

            return null;
        }
        #endregion

        #region private
        private bool setInsideOutsideState(RelativeLocation insideOutsideValue)
        {
            if (insideOutsideValue == RelativeLocation.Inside)
            {
                machine.INSIDEBOUNDINGBOX.SetCurrent();
                return false;
            }
            else
            {
                machine.OUTSIDEBOUNDINGBOX.SetCurrent();
                return true;
            }
        }

        private List<Point> createPathDictionary(ColoBox boxToPlace, double y)
        {
            List<Border> borderInPath = new List<Border>();
            
            double lowY, highY;
            foreach (Border b in machine.BorderVerifier.VerticalX)
            {
                if (b.BoxWhichBelongTo != machine.boundingBox)
                    continue;

                //make sure that we sorted out and have the lower point of the border as "lowY" and the upper as "highY"
                Utility.BorderEndpoints(b,machine.InitParam.Tolerance,y, out lowY, out highY,out y);


                //setting up the tolerance correction before comparing
                

                if (y >= lowY && y <= highY)
                {
                    if (!borderInPath.Contains(b))
                        borderInPath.Add(b);
                }
            }

            borderInPath.AddRange(machine.GetAngledBordersInPath(boxToPlace,y));
            if (borderInPath.Count == 3)
            { }
            borderInPath = setDistinctBorders(borderInPath);

            List<Point> pathSections = new List<Point>();
            RelativeLocation first = RelativeLocation.Inside;
            RelativeLocation second = RelativeLocation.Outside;
            bool isOrderSet = false;
            for (int i = 0; i < borderInPath.Count - 1; ++i)
            {
                Point p;
                TiltBorder tb = getTiltedBorder(borderInPath[i]); //(TiltBorder)borderInPath[i];
                TiltBorder tb2 = getTiltedBorder(borderInPath[i + 1]); //(TiltBorder)borderInPath[i + 1];
                p = new Point { X = tb.Coordinate, Y = tb2.Coordinate };
                p = this.machine.direction.setPathCoordinatesOrder(p);


                //if (!isOrderSet)
                //{
                //    isOrderSet = true;
                //    machine.direction.GetOrder(boxToPlace,p,out first, out second);                    
                //}

                if (i % 2 == 0)
                //
                {
                    //pathSections.Add(p, "Inside");
                    p.RelativeLocation = first;
                    pathSections.Add(p);
                }
                else
                {
                    //pathSections.Add(p, "Outside");
                    p.RelativeLocation = second;
                    pathSections.Add(p);
                }
            }


            //at this point, if we found that on section is shorter than a box width, just merge that section
            //double boxWidth = boxToPlace.Width;
            //List<Point> newPathSection = null;
            //for (int i = 0; i < pathSections.Count - 1; ++i)
            //{
            //    if (Math.Abs(pathSections[i].X - pathSections[i].Y) > boxWidth)
            //    {
            //        if (newPathSection == null)
            //            newPathSection = new List<Point>();

            //        newPathSection.Add(new Point { X = pathSections[i].X, Y = pathSections[i + 1].Y , RelativeLocation = RelativeLocation.Inside });
            //    }
            //}

            //if (newPathSection != null)
            //    return newPathSection;

            //before we create path section 
            return pathSections;
        }

        private List<Border> setDistinctBorders(List<Border> borderInPath)
        {
            List<Border> distinctRetVal = new List<Border>();
            List<double> foundKey = new List<double>();
            foreach (Border tb in borderInPath)
            {
                bool keyFound = false;
                foreach (double d in foundKey)
                {
                    if (Math.Abs(Math.Abs(d) - Math.Abs(tb.Coordinate)) < 2 * machine.InitParam.Tolerance)
                    {
                        keyFound = true;
                    }
                }
                if (!keyFound)
                {
                    distinctRetVal.Add(tb);
                    foundKey.Add(tb.Coordinate);
                }
            }

            return machine.direction.SortLookaheadPath(distinctRetVal);
        }

        private TiltBorder getTiltedBorder(Border border)
        {
            if (border is TiltBorder)
                return (TiltBorder)border;

            return new TiltBorder{ Coordinate = border.Coordinate};
        }
        #endregion
    }
}
