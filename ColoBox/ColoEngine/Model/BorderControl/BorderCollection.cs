using netDxf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.BorderControl
{
    internal class BorderCollection : IEnumerable<Border>
    {
        protected double? previousValue;
        internal List<Border> borders;
        internal List<Border> transformed_borders;
        internal List<Border> lborders;
        internal ColoBox boundingBox;
        internal bool IsTransformed { set; get; }
        internal int Count { get { return borders.Count; } }
        protected ColoEngine.Model.LocationStatemachine.Machine locationMachine;
        internal BorderCollection(ColoBox boundingBox, ColoEngine.Model.LocationStatemachine.Machine machine)
        {
            this.boundingBox = boundingBox;
            borders = new List<Border>();
            transformed_borders = new List<Border>();
            this.locationMachine = machine;
        }

        internal Border this[int i]
        {
            get
            {
                if (IsTransformed)
                    return transformed_borders[1];

                return borders[i];
            }
        }
        //Adding point for Horizontal/Vertical lines
        internal void AddPoint(ColoBox boxBelongsTo,Point p1, bool isVertical,int index)
        {
            
            if (isVertical)
            {
                if (previousValue.HasValue)
                {
                    if (p1.Y == previousValue)
                    {
                        //we found that this is a horizontal line, so we are not interested, first reset the previous value 
                        previousValue = null;
                        //return;
                    }
                }
                //looking for the X coordinate within the borders collection
                updateCoordinates(boxBelongsTo, p1.X, p1.Y,index);


                //keeping the previous value so the next function call we can verify that this is coordinates of a vertical line, if the 2 Y coordinates
                //are equals, it means that its a part of a horizontal line, so we are not going to be interested                
                previousValue = p1.Y;
                
                return;
            }


            /////////////  Horizontal line

            if (previousValue.HasValue)
            {
                if (p1.X == previousValue)
                {
                    //we found that this is a vartical line, so we are not interested, first reset the previous value and then return
                    previousValue = null;
                    //return;
                }
            }

            //updating horizontal lines
            updateCoordinates(boxBelongsTo, p1.Y, p1.X,index);

            //keeping the previous value so the next function call we can verify that this is coordinates of a horizontal line, if the 2 X coordinates
            //are equals, it means that its a part of a vertical line, so we are not going to be interested                
             previousValue = p1.X;

        }

        //Adding points that render a tilted line
        internal void AddTiltedBorder(ColoBox boxBelongsTo,Point fromX,Point fromY)
        {
            TiltBorder border = new TiltBorder();
            border.BoxWhichBelongTo = boxBelongsTo;
            border.Endpoints.Add(new BorderNumberPair { coordinate1 = fromX.X, coordinate2 = fromX.Y });
            border.Endpoints.Add(new BorderNumberPair { coordinate1 = fromY.X, coordinate2 = fromY.Y });
            borders.Add(border);
        }
        //This method update the second value of the endpoint to an existing border
        private void updateCoordinates(ColoBox boxBelongTo, double mainCoordinate, double endPoint,int index)
        {
            List<Border> localBorders;

            if (IsTransformed)
                localBorders = transformed_borders;
            else
                localBorders = borders;

            Border borderToUpdate = null;
            BorderNumberPair pair = null;
            foreach (Border b in localBorders)
            {
                if (b.Coordinate == mainCoordinate)
                {
                    borderToUpdate = b;
                    break;
                }
            }

            if (borderToUpdate == null)
            {
                borderToUpdate = new Border();
                borderToUpdate.BoxWhichBelongTo = boxBelongTo;
                localBorders.Add(borderToUpdate);
                pair = new BorderNumberPair(index);
                borderToUpdate.Endpoints.Add(pair);
                borderToUpdate.Coordinate = mainCoordinate;
            }

            //this is the case with existing borderToUpdate, so we need to extract the last pair
            if (pair == null)
            {
                //first we looking for a pair with a null value on coordinate2
                pair = borderToUpdate.Endpoints.FirstOrDefault(e => !e.coordinate2.HasValue);

                if(pair == null)
                   pair = borderToUpdate.Endpoints.FirstOrDefault(e => e.coordinate1 == endPoint || e.coordinate2 == endPoint);


                int indexChange = 0;
                if(pair != null)
                 indexChange = pair.index - index;

                //this is a protection not to connect dots that are way far apart (incase they happen to have the same vertical or horizontal)
                if (indexChange < -5)
                    pair = null;

                //checking if this pair is full, if it is, create a new pair
                if (pair == null)
                {
                    pair = new BorderNumberPair(index);
                    borderToUpdate.Endpoints.Add(pair);
                }
            }

            if (!pair.coordinate1.HasValue)
                pair.coordinate1 = endPoint;
            else if (!pair.coordinate2.HasValue)
                pair.coordinate2 = endPoint;
            else
            {
                BorderNumberPair newPair = new BorderNumberPair(index);
                newPair.coordinate1 = endPoint;
                borderToUpdate.Endpoints.Add(newPair);
            }
        }





        internal virtual bool FindCollisionOrig(ColoBox boxToPlace)
        {

            return true;
        }

        //finds collision between a box and the fixed border of the drawing
        internal virtual bool FindCollision(ColoBox boxToPlace)
        {
            selectBorderCollection(boxToPlace.IsRotating);

            double min_x, max_x, min_y, max_y;
            min_x = boxToPlace.MinX;
            max_x = boxToPlace.MaxX;

            min_y = boxToPlace.MinY;
            max_y = boxToPlace.MaxY;

            

            

            foreach (var vx in lborders)
            {
                //indicates if the borders that the box is touching is part of the origin. in that case we are not render that as a collision.
                bool isTouchingOriginLines = locationMachine.direction.IsTouchingOriginLines(vx); 
                if (vx.Coordinate >= min_x && vx.Coordinate <= max_x && !isTouchingOriginLines)
                {
                    if (vx.IntersecWith(min_y, max_y))
                    {
                        //we found a wall in the a box which is not the bounding box we return the next
                        if (vx.BoxWhichBelongTo != boundingBox)
                        {
                            locationMachine.OverrideStartPlace = CalculateNextLinearLocation(vx, boxToPlace);
                            return true;
                        }

                        //stepping out of the boundingbox localy
                        if (vx.BoxWhichBelongTo == boundingBox && min_x < boundingBox.MaxX)
                        {
                            if (!locationMachine.CurrentlyOutsideTheBox)
                                locationMachine.OUTSIDEBOUNDINGBOX.SetCurrent();
                            else
                                locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                        }

                        //Found that a verticalX line intersect
                        return true;
                    }
                }
            }

            return false;
        }

        protected void selectBorderCollection(bool isRotating)
        {
            lborders = borders;
            if (isRotating)
                lborders = transformed_borders;
        }

        protected Point CalculateNextLinearLocation(Border vx, ColoBox boxToPlace)
        {
            double x = locationMachine.direction.CalculateNextLinearLocation(vx, boxToPlace, locationMachine.InitParam.SpaceAfterObstacle);
            return new Point { X = x, Y = boxToPlace.MinY /* to keep the box on the same baseline of the current row*/};
        }
        public IEnumerator<Border> GetEnumerator()
        {
            return borders.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return borders.GetEnumerator();
        }

        internal void RemoveRange(List<Border> retVal)
        {
            foreach(Border b in retVal)
            {
                borders.Remove(b);
            }
        }

        //internal bool BorderNotAlignWithBoundingBox(ColoBox boxToPlace)
        //{
        //    double horizontalY1 = boxToPlace.MinY;
        //    double horizontalY2 = boxToPlace.MaxY;
        //    List<Border> bordersToExam = new List<Border> ();
        //    foreach (Border b in borders)
        //    {
        //        foreach (BorderNumberPair x in b.Endpoints)
        //        {
        //            if (
        //                (x.coordinate1 > horizontalY1 && x.coordinate2 < horizontalY1) ||
        //                (x.coordinate1 > horizontalY2 && x.coordinate2 < horizontalY2)
        //              )
        //            {
        //                bordersToExam.Add(b);
        //            }                                             
        //        }
        //    }

        //    bordersToExam.Sort();

        //    foreach (Border b in bordersToExam)
        //    {
        //        if (Math.Round(b.Coordinate) > Math.Round(boxToPlace.MinX))
        //        {
        //            locationMachine.OverrideStartPlace = new Point { X = b.Coordinate, Y = boxToPlace.MinY };
        //            locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        public override string ToString()
        {
            return string.Format("[{0}] [(T){1}]", borders.Count, transformed_borders.Count);
        }

        internal void Rotate(double angle)
        {
            List<Border> newBorders = new List<Border>();
            foreach (Border b in borders)
            {
                b.Rotate(angle);
                if (b.Endpoints.Count == 2)
                {
                    if (Math.Round(b.Endpoints[0].coordinate2.Value,2) == Math.Round(b.Endpoints[1].coordinate2.Value,2))
                    {
                        Border nb = new Border { Coordinate = b.Endpoints[0].coordinate2.Value };
                        nb.Endpoints.Add(new BorderNumberPair { coordinate1 = b.Endpoints[0].coordinate1, coordinate2 = b.Endpoints[1].coordinate1 });
                    }
                }
            }
        }

        internal void Remove(Border b)
        {
            borders.Remove(b);
        }

        internal void Add(Border nb)
        {
            borders.Add(nb);
        }

    }
}
