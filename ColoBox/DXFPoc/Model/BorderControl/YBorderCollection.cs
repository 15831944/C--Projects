using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.BorderControl
{
    internal class YBorderCollection : BorderCollection
    {
        internal YBorderCollection(ColoBox boundingBox, DXFPoc.Model.LocationStatemachine.Machine machine)
            : base(boundingBox, machine)
        {
        }
        internal override bool FindCollision(ColoBox boxToPlace)
        {
            selectBorderCollection(boxToPlace.IsRotating);
            double min_x, max_x, min_y, max_y;
            min_x = Math.Round(boxToPlace.MinX);
            max_x = Math.Round(boxToPlace.MaxX);

            min_y = Math.Round(boxToPlace.MinY);
            max_y = Math.Round(boxToPlace.MaxY);

            //min_x = boxToPlace.MinY;
            //max_x = boxToPlace.MaxY;

            //min_y = boxToPlace.MinX;
            //max_y = boxToPlace.MaxX;

            foreach (var hy in lborders)
            {
                if (hy.Coordinate <= max_y && hy.Coordinate >= min_y)
                {
                    BorderNumberPair bnp;
                    foreach (var ep in hy.Endpoints)
                    {
                        bnp = Utility.SortEndpointOrder(ep);


                        if ((bnp.coordinate1.Value > min_x && bnp.coordinate1.Value < max_x) ||
                            (bnp.coordinate2.Value > min_x && bnp.coordinate2.Value < max_x))
                          
                        {
                            //if this happen on the begining of the row where the X origin of the room equals to the origin of the 
                            //boxToPlace, force move it to the next location
                            if (bnp.coordinate1.Value == min_x)
                            {
                                
                                if (locationMachine.OnRowStart)
                                {
                                    locationMachine.OverrideStartPlace = new Point { X = bnp.coordinate2.Value, Y = boxToPlace.NextXLocation.Y };
                                }
                                else
                                {
                                    locationMachine.OverrideStartPlace = boxToPlace.NextXLocation;
                                }
                                locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                            }


                            if (locationMachine.CurrentlyOutsideTheBox)
                            {
                                locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                                locationMachine.OverrideStartPlace = new Point { X = bnp.coordinate2.Value, Y = boxToPlace.MinY };
                            }
                            return true;
                        }

                       
                              
                    }
                }




                //if (hy.Coordinate > min_x && hy.Coordinate < max_x)
                //{
                //    if (hy.IntersecWith(min_y, max_y))
                //    {
                //        //we found a wall in the a box which is not the bounding box we return the next
                //        if (hy.BoxWhichBelongTo != boundingBox)
                //        {
                //            // calculatedNextLocation = CalculateNextLinearLocation(vx, boxToPlace);
                //        }

                //        //stepping out of the boundingbox localy
                //        if (hy.BoxWhichBelongTo == boundingBox && min_x < boundingBox.MaxX)
                //        {
                //            locationMachine.OUTSIDEBOUNDINGBOX.SetCurrent();
                //        }

                //        //Found that a verticalX line intersect
                //        return true;
                //    }
                //}
            }

            return false;
        }
    }
}
