using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.BorderControl
{
    internal class TiltedBorderCollection : BorderCollection
    {
        internal TiltedBorderCollection(ColoBox boundingBox,ColoEngine.Model.LocationStatemachine.Machine machine)
            : base(boundingBox,machine)
        {

        }
        internal override bool FindCollision(ColoBox boxToPlace)
        {          
            foreach (var b in borders)
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
