using ColoEngine.Model.BorderControl;
using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.LocationStatemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.DirectionControl
{
    public class FirstBorderPositive : FirstBorderBase
    {
        private ColoEngine.Model.LocationStatemachine.Machine stateMachine;

        public FirstBorderPositive(Machine stateMachine,Border border)
        {            
            this.stateMachine = stateMachine;
            this.FirstBorder = border;
        }

        internal override double GetFirstBorderCoordinate(DirectionBase dc)
        {
            double minVerticalX = double.MaxValue;
            //double minTiltedX;
            

            //First phase, we need to find the min X coordinate of the vertical border with in the path
            foreach (Border b in stateMachine.BorderVerifier.VerticalX)
            {
                double minBoxToPlaceY = dc.GetYPointTouchesBorder(stateMachine.CurrentBoxReference, this.GetType(),b);// stateMachine.CurrentBoxReference.MinY;
                if (minBoxToPlaceY >= b.Endpoints[0].coordinate1 && minBoxToPlaceY <= b.Endpoints[0].coordinate2)
                {
                    if (b.Coordinate < minVerticalX)
                    {
                        stateMachine.CurrentBoxReference.BorderUsedToGetRowStartX = b;
                        minVerticalX = b.Coordinate;
                    }
                }
            }

            //second phase, find the first tilted border within the path
            double firstTildedBorderCoordinate = dc.GetFirstPositiveTiltedBorder(FirstBorder);
           
           
            return minVerticalX;
        }
    }
}
