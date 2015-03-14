using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.BorderControl
{
    internal class TiltedBorderCollection : BorderCollection
    {
        internal TiltedBorderCollection(ColoBox boundingBox,DXFPoc.Model.LocationStatemachine.Machine machine)
            : base(boundingBox,machine)
        {

        }
        internal override bool FindCollision(ColoBox boxToPlace)
        {
            selectBorderCollection(boxToPlace.IsRotating);
            //bool isVerticalsCrossLine = false;
            //bool isHorizontalsCrossLine = false;
            //double lineX1,lineX2,lineY1,lineY2;
            if (boxToPlace.IsRotating)
                lborders = transformed_borders;
            else
                lborders = borders;


            foreach (var b in lborders)
            {
                LinearLine line = new LinearLine(b);

                if (line.FindCollision(boxToPlace))
                {
                    //calculatedNextLocation = new Vector2 { X = boxToPlace.MinX + boxToPlace.Width, Y = boxToPlace.MinY };
                    return true;
                }
                //else
                //    calculatedNextLocation = null;
              
                


            }
            return false;// isHorizontalsCrossLine;
        }
    }
}
