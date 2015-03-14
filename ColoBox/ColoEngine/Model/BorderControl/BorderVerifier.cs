using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.BorderControl;

namespace ColoEngine.Model.BorderControl
{
    public class BorderVerifier
    {
        private BorderCollection verticalX;
        private YBorderCollection horizontalY;
        private BorderCollection tilted;
        internal ColoBox boundingBox;

        internal bool IsRotating { get { if (locationMachine == null) return false; return locationMachine.IsRotated; } }
        internal double Angle { set; get; }
        private ColoEngine.Model.LocationStatemachine.Machine locationMachine;
        private bool isMeetingTiltedWall = false;

        internal BorderCollection VerticalX { get { return verticalX; } }
        internal BorderCollection HorizontalY { get { return horizontalY; } }
        internal BorderCollection Tilted { get { return tilted; } }
        internal bool IsInsideBoundingBox { set; get; }
        internal Point NextLocation { get { return locationMachine.NextLocation; } }
        public BorderVerifier(List<ColoBox> staticBoxes, ColoEngine.Model.LocationStatemachine.Machine machine)
        {
            //string ser = staticBoxes[0].SerializePoints();
            this.boundingBox = machine.boundingBox;
            this.locationMachine = machine;
            verticalX = new BorderCollection (boundingBox,machine);
            horizontalY = new YBorderCollection(boundingBox,machine);
            tilted = new TiltedBorderCollection(boundingBox,machine);

            loadBorderCollections(staticBoxes);
            
            
            


            //if tilted has values and verticalX and horizontalY are empty, rotate the drawing
            if (tilted.Count > 0 && horizontalY.Count == 0 && verticalX.Count == 0)
            {                
                machine.IsRotated = true;
                rotateDrawing();

                //recalculate the borders after the rotation
                //loadBorderCollections(staticBoxes, true);
            }
            
        }
        private void loadBorderCollections(List<ColoBox> staticBoxes)
        {
            foreach (ColoBox box in staticBoxes)
            {
                foreach (RawBorder rb in box.Borders)
                {
                    switch (rb.BorderType)
                    {
                        case Orientation.Horizontal:
                          horizontalY.Add(new Border(rb,box));
                            break;
                        case Orientation.Vertical :
                            verticalX.Add(new Border(rb,box));
                            break;
                        default:
                            tilted.Add(new TiltBorder(rb,box));
                            break;
                    }
                    
                }
            }
        }

        private void loadBorderCollectionsOld(List<ColoBox> staticBoxes,bool isRotating)
        {
            if (isRotating)
            {
                verticalX.IsTransformed = true;
                horizontalY.IsTransformed = true;
                tilted.IsTransformed = true;
            }
            foreach (ColoBox box in staticBoxes)
            {
                int i = 0;
                foreach (Point p in box.Points.Distinct())
                {
                    if (isRotating)
                    {
                        p.Rotate(Angle);                        
                    }
                    
                    //loading the vertical lines
                    verticalX.AddPoint(box, p, true, i);

                    //loading the horizontal lines
                    horizontalY.AddPoint(box, p, false, i);

                    ++i;
                }
            }


            //extracting the point of the angular lines
            List<Border> fromVerX = getAngularEndpoints(verticalX);
            List<Border> fromHorY = getAngularEndpoints(horizontalY);

            if (fromVerX.Count != fromHorY.Count)
                throw new Exception("Failed to find matching point for tilted lines in the bounding box");

            Point fromX;
            Point fromY;
            for (int i = 0; i < fromVerX.Count - 1; ++i)
            {
                fromX = new Point { X = fromVerX[i].Coordinate, Y = fromVerX[i].Endpoints[0].coordinate1.Value };
                fromY = new Point { X = fromHorY[i + 1].Endpoints[0].coordinate1.Value, Y = fromHorY[i + 1].Coordinate };
                tilted.AddTiltedBorder(fromVerX[i].BoxWhichBelongTo, fromX, fromY);
            }

            if (fromVerX.Count > 0)
            {
                fromX = new Point { X = fromVerX[fromVerX.Count - 1].Coordinate, Y = fromVerX[fromVerX.Count - 1].Endpoints[0].coordinate1.Value };
                fromY = new Point { X = fromHorY[0].Endpoints[0].coordinate1.Value, Y = fromHorY[0].Coordinate };
                tilted.AddTiltedBorder(fromVerX[0].BoxWhichBelongTo, fromX, fromY);
            }

        }

        private void rotateDrawing()
        {
            double y2 = tilted[0].Endpoints[1].coordinate2.Value;
            double y1 = tilted[0].Endpoints[0].coordinate2.Value;

            double x2 = tilted[0].Endpoints[1].coordinate1.Value;
            double x1 = tilted[0].Endpoints[0].coordinate1.Value;
            //Angle = Math.Atan2(3, 3) * (180 / Math.PI);
            Angle = Math.Atan2((y2 - y1), (x2 - x1));// *(180 / Math.PI);
            

            //float xDiff = p2.X - p1.X; 
            //float yDiff = p2.Y - p1.Y; 
            //return Math.Atan2(yDiff, xDiff) 

            tilted.Rotate(Angle);
            boundingBox.Rotate(Angle);
        }

        private List<Border> getAngularEndpoints(BorderCollection inputList)
        {
            List<Border> rawList = new List<Border>();            
            foreach (Border b in inputList)
            {                
                foreach (BorderNumberPair bnp in b.Endpoints)
                {                    
                    if (bnp.coordinate2 == null)
                    {
                        rawList.Add(b);                       
                    }
                }
            }


            List<Border> retVal = new List<Border>();
            foreach (Border b in rawList)
            {
                if (b.Endpoints.Count == 1)
                {
                    inputList.Remove(b);
                    retVal.Add(b);
                }
                else
                {
                    //creating new border with endpoint which don't have null values
                    Border nb = b.GetNewBorder(true);
                    inputList.Remove(b);
                    inputList.Add(nb);

                    //getting the border with only the endpoint that have a null value
                    nb = b.GetNewBorder(false);
                    retVal.Add(nb);
                }
            }

            
            return retVal;
        }

        

        internal void Move2NextLocation(Point nextLocation,ColoBox boxToPlace,ColoOptimizer optimizer,int boxId,int indexToStop)
        {
            if (boxId == 47)
            { }
            bool isValid = false;            
            Point calcNextLocation = nextLocation;
            //isInsideBoundingBox = true;
            locationMachine.CurrentBoxToPlace = boxToPlace;

            //reseting the flag indicates we are hitting a tilted wall
            isMeetingTiltedWall = false;

           int i = 0;
            while (!isValid)
            {

                //opening a rack space if it required
                if (locationMachine.NeedOpenRackSpace)
                {
                    calcNextLocation.X += locationMachine.direction.GetNextRackLocationAfterSpace(locationMachine.InitParam.RacksOpenningSpace);
                    locationMachine.RacksCountBeforeSpace = 0;
                }


                boxToPlace.MoveOriginTo(calcNextLocation);                

                //if (locationMachine.ROWSTARTS.IsAdvancedFromFirstPositionInALine)
                //{
                //    locationMachine.AssertBoxLocation();
                //}

                

                //optimizer.OnBoxLookAhead(boxToPlace.PointsCollection, boxId,false);

                //this call makes sure that if a box cross a line either to outside the bounding box, or back inside, we are updating the state accordingly
                Point? nextAfterGap = locationMachine.UpdateMachineState(boxToPlace);
                if(nextAfterGap.HasValue)
                {
                    locationMachine.RacksCountBeforeSpace = 0;
                    boxToPlace.MoveOriginTo(nextAfterGap.Value);
                    locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                    if (nextSpaceValid(boxToPlace))
                    {
                        locationMachine.ResetOverride();
                        break;
                    }

                    locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                    
                }


                if (locationMachine.currentState == locationMachine.ROWSTARTS)
                {
                    locationMachine.ROWSTARTS.IsAdvancedFromFirstPositionInALine = true;
                }

                //if we are in a situation that we are placing outside the bounding box (and there is more space to place inside)
                //we need to verify if we crossed back into the internal side of the bounding box. if we did, switch the stateMachine to indicate
                //that state (inside the box)
                //if (locationMachine.CurrentlyOutsideTheBox)
                //{
                //    if (locationMachine.LocationWithinBoundingBox)
                //    {
                //        locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                //    }
                //}
                ++i;
                if (i > indexToStop)
                {
                   // break;
                }
                
                if (nextSpaceValid(boxToPlace))
                {                   
                    locationMachine.NextState();


                    //if we are out of the bounding box, the break should not hit until we either step out of the outer bounding box or until
                    //we got back in
                    if (!locationMachine.IsSteppedOutBoundingBox)
                        break;
                }
                calcNextLocation = locationMachine.NextLocation;
                if (calcNextLocation.X == locationMachine.direction.ExtremeX)
                    throw new ExtremeFoundException();

                locationMachine.ResetOverride();
                
            }
            locationMachine.RacksCountBeforeSpace++;

        }

        private bool nextSpaceValid(ColoBox boxToPlace)
        {
            if (tilted.FindCollision(boxToPlace) && locationMachine.IsInsideBoundingBox)
            {
                if (locationMachine.IsInsideBoundingBox)
                    isMeetingTiltedWall = true;
                return false;
            }
              
            if (verticalX.FindCollision(boxToPlace))
            {                
                return false;
            }

            if(horizontalY.FindCollision(boxToPlace))
            {
                return false;
            }

            //if (tilted.FindCollision(boxToPlace))
            //{
            //    isMeetingTiltedWall = true;
            //    return false;
            //}


            //we found a local wall, keep looking inside the bounding box to find potential space before the exterior wall (in case this is not exterior)
            if (/* boxToPlace.MaxX < boundingBox.MaxX */ locationMachine.direction.IsBoxStillInsideBoundingBox(boxToPlace) &&  isMeetingTiltedWall)
            {
                locationMachine.IsSteppedOutBoundingBox = true;
                locationMachine.OUTSIDEBOUNDINGBOX.SetCurrent();
                return false;
            }

            //when we hit the exterior region of the bounding box of the main space
            if (locationMachine.direction.IsHittingExteriorBorder(boxToPlace))// boxToPlace.MaxX > locationMachine.LastVerticalLine )
            {
                if(locationMachine.IsSteppedOutBoundingBox)
                {
                    locationMachine.IsSteppedOutBoundingBox = false;
                }

                if (!locationMachine.IsRowStart)
                {
                    locationMachine.RowNumber++;
                    locationMachine.ROWSTARTS.IsAdvancedFromFirstPositionInALine = false;
                }

                locationMachine.ROWSTARTS.SetCurrent();
               
                isMeetingTiltedWall = false;

                return false;
            }


            ////This is a special case that we are trying to locate a new row starting at the "ORIGIN" coordinates of the next row
            ////sometimes the wall on certain location doesn't start on the ORIGIN of the bounding box. so we need to verify that
            //if (locationMachine.OnRowStart)
            //{
            //    //first find vertical lines that have y-coordinates which include at least one box horizontal line                
            //    if (verticalX.BorderNotAlignWithBoundingBox(boxToPlace))
            //    {
            //        return false;
            //    }

            //}

            if (locationMachine.CurrentlyOutsideTheBox)
                return false;

            return true;                                  
        }





        internal void CalculateBordersProjecting()
        {
            
        }

    }
}
