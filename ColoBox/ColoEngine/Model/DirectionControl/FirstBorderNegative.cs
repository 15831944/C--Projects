using ColoEngine.Model.BorderControl;
using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.LocationStatemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.DirectionControl
{
    public class FirstBorderNegative : FirstBorderBase
    {
        private ColoEngine.Model.LocationStatemachine.Machine stateMachine;

        public FirstBorderNegative(Machine stateMachine,Border border)
        {            
            this.stateMachine = stateMachine;
            this.FirstBorder = border;
        }
        internal override double GetFirstBorderCoordinate(DirectionBase dc)
        {            
            double minVerticalX = double.MaxValue;
            double minTiltedX;
            double minBoxToPlaceY = stateMachine.CurrentBoxReference.MinY;
            
            //First phase, we need to find the min X coordinate of the vertical border with in the path
            foreach (Border b in stateMachine.BorderVerifier.HorizontalY)
            {
                if (minBoxToPlaceY >= b.Endpoints[0].coordinate1 && minBoxToPlaceY <= b.Endpoints[0].coordinate2)
                {
                    if (b.Coordinate < minVerticalX)
                        minVerticalX = b.Coordinate;
                }
            }

            //second phase, find the first tilted border within the path
            minTiltedX = dc.GetFirstNegativeTiltedBorder(FirstBorder);
            return minTiltedX;
        }
    }
}
