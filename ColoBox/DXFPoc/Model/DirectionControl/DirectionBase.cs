using DXFPoc.Model.BorderControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.DirectionControl
{
    public abstract class DirectionBase
    {
        internal abstract double GetFirstBorderCoordinate();      
        internal LayoutManager LayoutManager { get; set; }

        internal virtual double OriginY(ColoBox boundingBox)
        {
            return boundingBox.Origin.Y;
        }

        internal abstract double OriginX(ColoBox boundingBox);

        protected bool IsBorderInVector(Border b, double nextYCoordinate)
        {
            bool borderInVector = false;
            BorderNumberPair sep = null;
            foreach (var ep in b.Endpoints)
            {
                sep = Utility.SortEndpointOrder(ep);
                if (sep.coordinate1 <= nextYCoordinate && sep.coordinate2 >= nextYCoordinate)
                {
                    borderInVector = true;
                }
            }

            return borderInVector;
        }


        internal abstract bool IsTouchingOriginLines(LocationStatemachine.Machine locationMachine, Border vx);

        internal abstract bool IsHittingExteriorBorder(ColoBox boxToPlace, LocationStatemachine.Machine locationMachine);

        internal abstract double NextBoxLocation(double p, double boundaryX);

        internal abstract double CalculateNextLinearLocation(Border vx, ColoBox boxToPlace, double spaceAfterObstacle);
    }
}
