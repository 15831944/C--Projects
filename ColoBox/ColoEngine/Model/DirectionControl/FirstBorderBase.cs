using ColoEngine.Model.BorderControl;
using ColoEngine.Model.DirectionControl;
using ColoEngine.Model.LocationStatemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.DirectionControl
{
    public abstract class FirstBorderBase
    {
        protected Machine stateMachine;
        protected Border FirstBorder { set; get; }
        //find out if the first border to hit in the current path is vertical or tilted
        internal static FirstBorderBase GetFirstBorderToHit(Machine stateMachine,DirectionBase dir)
        {

            if (dir is DirectionL2R)
            {
                return GetFirstBorderL2R(stateMachine);
            }
            else
            {
                return GetFirstBorderR2L(stateMachine);
            }
        }

        private static FirstBorderBase GetFirstBorderR2L(Machine stateMachine)
        {
            //first finding the largest VerticalX border
            double maxX = double.MinValue;
            foreach (Border b in stateMachine.BorderVerifier.VerticalX)
            {
                if (b.Coordinate > maxX)
                {
                    maxX = b.Coordinate;
                }
            }


            //finding the smallest x of a tilted border, once found, we need to calculate if its a negative or positive one
            Border tilted = null;
            double yToCompare = stateMachine.OriginY + stateMachine.NextRowStart;
            foreach (Border b in stateMachine.BorderVerifier.Tilted)
            {
                double? xToCompare = null;
                //this is the case of Endpoint[0] is higher
                if (b.Endpoints[0].coordinate2 > b.Endpoints[1].coordinate2)
                {
                    if (b.Endpoints[0].coordinate2 >= yToCompare && b.Endpoints[1].coordinate2 <= yToCompare)
                        xToCompare = b.Endpoints[1].coordinate1;
                }
                //this is the case that Endpoint[1] is higher
                else if (b.Endpoints[1].coordinate2 > b.Endpoints[0].coordinate2)
                {
                    int epCoordinate0 = (int) b.Endpoints[0].coordinate2.Value;
                    int epCoordinate1 = (int) b.Endpoints[1].coordinate2.Value;
                    int yInt = (int) yToCompare;
                    if ( epCoordinate1>= yInt && epCoordinate0 <= yInt)
                        xToCompare = b.Endpoints[0].coordinate1;
                }


                if (xToCompare.HasValue)
                {
                    if (xToCompare.Value > maxX)
                    {
                        maxX = xToCompare.Value;
                        tilted = b;
                    }
                }
            }


            if (tilted == null)
                return null;

            //x1y1 on top left (negative)
            if (tilted.Endpoints[0].coordinate2 < tilted.Endpoints[1].coordinate2)
            {
                if (tilted.Endpoints[0].coordinate1 < tilted.Endpoints[1].coordinate1)
                    return new FirstBorderNegative(stateMachine, tilted);
            }

            //x1y1 on top right (positive)
            if (tilted.Endpoints[0].coordinate2 > tilted.Endpoints[1].coordinate2)
                if (tilted.Endpoints[1].coordinate1 < tilted.Endpoints[0].coordinate1)
                    return new FirstBorderPositive(stateMachine, tilted);


            //x1,y1 on bottom right (negative)
            if (tilted.Endpoints[0].coordinate2 < tilted.Endpoints[1].coordinate2)
                if (tilted.Endpoints[0].coordinate1 > tilted.Endpoints[1].coordinate1)
                    return new FirstBorderNegative(stateMachine, tilted);

            //x1,y1 on bottom left (positive)
            if (tilted.Endpoints[0].coordinate2 < tilted.Endpoints[1].coordinate2)
                if (tilted.Endpoints[0].coordinate1 < tilted.Endpoints[1].coordinate1)
                    return new FirstBorderPositive(stateMachine, tilted);
            return null;
        }

        private static FirstBorderBase GetFirstBorderL2R(Machine stateMachine)
        {
            //finding the smallest x of a tilted border, once found, we need to calculate if its a negative or positive one
            Border tilted = null;
            double yToCompare = stateMachine.OriginY + stateMachine.NextRowStart;

             //first finding the smallest VerticalX border that the yToCompare is within that border
            double minX = double.MaxValue;
            //foreach (Border b in stateMachine.BorderVerifier.VerticalX)
            //{
            //    double highY, lowY;
            //    Utility.BorderEndpoints(b,out lowY,out highY);
            //    if (yToCompare >= lowY && yToCompare <= highY)
            //    {
            //        if (b.Coordinate < minX)
            //        {
            //            minX = b.Coordinate;
            //        }
            //    }

            //}

           
            foreach (Border b in stateMachine.BorderVerifier.Tilted)
            {
                double? xToCompare = null;
                //this is the case of Endpoint[0] is higher
                if (b.Endpoints[0].coordinate2 > b.Endpoints[1].coordinate2)
                {
                    if (b.Endpoints[0].coordinate2 >= yToCompare && b.Endpoints[1].coordinate2 <= yToCompare)
                        xToCompare = b.Endpoints[1].coordinate1;
                }
                    //this is the case that Endpoint[1] is higher
                else if(b.Endpoints[1].coordinate2 > b.Endpoints[0].coordinate2)
                {
                    if (b.Endpoints[1].coordinate2 >= yToCompare && b.Endpoints[0].coordinate2 <= yToCompare)
                        xToCompare = b.Endpoints[0].coordinate1;
                }


                if (xToCompare.HasValue)
                {
                    if (xToCompare.Value < minX)
                    {
                        minX = xToCompare.Value;
                        tilted = b;                        
                    }
                }
            }


            if (tilted == null)
                return null;

            //x1y1 on top left (negative)
            if (tilted.Endpoints[0].coordinate2 > tilted.Endpoints[1].coordinate2)
            {
                if(tilted.Endpoints[0].coordinate1 < tilted.Endpoints[1].coordinate1)
                    return new FirstBorderNegative(stateMachine,tilted);
            }

            //x1y1 on top right (positive)
            if(tilted.Endpoints[0].coordinate2 > tilted.Endpoints[1].coordinate2)
                if (tilted.Endpoints[1].coordinate1 < tilted.Endpoints[0].coordinate1)
                    return new FirstBorderPositive(stateMachine,tilted);

            
            //x1,y1 on bottom right (negative)
            if(tilted.Endpoints[0].coordinate2 < tilted.Endpoints[1].coordinate2)
                if(tilted.Endpoints[0].coordinate1 > tilted.Endpoints[1].coordinate1)
                    return new FirstBorderNegative(stateMachine,tilted);

            //x1,y1 on bottom left (positive)
            if (tilted.Endpoints[0].coordinate2 < tilted.Endpoints[1].coordinate2)
                if (tilted.Endpoints[0].coordinate1 < tilted.Endpoints[1].coordinate1)
                    return new FirstBorderPositive(stateMachine,tilted);

            return null;
        }

        internal abstract double GetFirstBorderCoordinate(DirectionBase dc);
    }
}
