using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.BorderControl
{
    internal class YBorderCollection : BorderCollection
    {
        internal YBorderCollection(ColoBox boundingBox, ColoEngine.Model.LocationStatemachine.Machine machine)
            : base(boundingBox, machine)
        {
        }
        internal override bool FindCollision(ColoBox boxToPlace)
        {
            
            selectBorderCollection(boxToPlace.IsRotating);
            double min_x, max_x, min_y, max_y;
            min_x = Math.Round(boxToPlace.MinX,2);
            max_x = Math.Round(boxToPlace.MaxX,2);

            min_y = Math.Round(boxToPlace.MinY,2);
            max_y = Math.Round(boxToPlace.MaxY,2);

            //min_x = boxToPlace.MinY;
            //max_x = boxToPlace.MaxY;

            //min_y = boxToPlace.MinX;
            //max_y = boxToPlace.MaxX;

            foreach (var hy in lborders)
            {                
                if (hy.Coordinate < max_y && hy.Coordinate > min_y && Math.Abs(hy.Coordinate - min_y) > 1)
                    
                {
                    //this is the edge case that the beginning of the line cross through a horizontal border
                    if ((min_x > Math.Round(hy.Endpoints[0].coordinate1.Value) && min_x < Math.Round(hy.Endpoints[0].coordinate2.Value) ||
                         max_x > Math.Round(hy.Endpoints[0].coordinate1.Value) && max_x < Math.Round(hy.Endpoints[0].coordinate2.Value)))
                        //if ((min_x >= Math.Round(hy.Endpoints[0].coordinate1.Value) && min_x < Math.Round(hy.Endpoints[0].coordinate2.Value) ||
                        // max_x >= Math.Round(hy.Endpoints[0].coordinate1.Value) && max_x < Math.Round(hy.Endpoints[0].coordinate2.Value)))
                    {
                        locationMachine.OverrideStartPlace = new Point { X = hy.Endpoints[0].coordinate2.Value, Y = boxToPlace.MinY };
                        return true;                    
                    }
                    foreach (var p in hy.Endpoints)
                    {
                        
                        
                        if (locationMachine.direction.BoxCrossHorizontalLineOnLineStart(boxToPlace, p))
                        {
                            
                        }
                        
                        
                        if ((p.coordinate1.Value > min_x && p.coordinate1.Value < max_x) ||
                            (p.coordinate2.Value > min_x && p.coordinate2.Value < max_x))
                          
                        //if(  (min_x >= bnp.coordinate1.Value &&  min_x <= bnp.coordinate2.Value) &&
                        //     (max_x >= bnp.coordinate1.Value &&  max_x <=bnp.coordinate2.Value) )
                        {
                            //if this happen on the begining of the row where the X origin of the room equals to the origin of the 
                            //boxToPlace, force move it to the next location
                            if (p.coordinate1.Value == min_x)
                            {
                                
                                if (locationMachine.OnRowStart)
                                {
                                    locationMachine.OverrideStartPlace = new Point { X = p.coordinate2.Value, Y = boxToPlace.NextXLocation.Y };
                                }
                                else
                                {
                                    locationMachine.OverrideStartPlace = boxToPlace.NextXLocation;
                                }
                                locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                            }


                            if (locationMachine.CurrentlyOutsideTheBox)
                            {
                                locationMachine.PathSectionBuilder.UpdateMachineState(boxToPlace);
                                //locationMachine.INSIDEBOUNDINGBOX.SetCurrent();
                                locationMachine.OverrideStartPlace = new Point { X = p.coordinate2.Value, Y = boxToPlace.MinY };
                            }
                            return true;
                        }

                       
                              
                    }
                }

            }

            return false;
        }
    }
}
