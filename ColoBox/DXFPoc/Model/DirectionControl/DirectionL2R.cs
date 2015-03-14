using DXFPoc.Model.BorderControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.DirectionControl
{
    internal class DirectionL2R : DirectionBase
    {
        internal override double GetFirstBorderCoordinate()
        {
           
            double nextYCoordinate = LayoutManager.RowStartY;
            Border minBorder = null;
            Border maxBorder = null;
            foreach (Border b in LayoutManager.VerticalX)
            {               
                if (IsBorderInVector(b,nextYCoordinate))
                {
                    if (minBorder == null)
                    {
                        minBorder = b;                        
                    }

                    if (maxBorder == null)
                    {
                        maxBorder = b;
                    }


                    if (b.Coordinate < minBorder.Coordinate)
                    {
                        minBorder = b;
                    }

                    if (b.Coordinate > maxBorder.Coordinate)
                    {
                        maxBorder = b;
                    }
                }
            }

            if (minBorder == null || maxBorder == null)
            {
                LayoutManager.FirstVerticalLine = LayoutManager.OriginX;
                LayoutManager.LastVerticalLine = LayoutManager.OriginX + LayoutManager.BoundingBoxWidth;
                return LayoutManager.OriginX;
            }

            LayoutManager.FirstVerticalLine = minBorder.Coordinate;
            LayoutManager.LastVerticalLine = maxBorder.Coordinate;

            return minBorder.Coordinate;
        }           

        internal override double OriginX(ColoBox boundingBox)
        {
            return boundingBox.Origin.X;
        }

        internal override bool IsTouchingOriginLines(LocationStatemachine.Machine locationMachine, Border vx)
        {
            return locationMachine.FirstVerticalLine == vx.Coordinate;
        }

        internal override bool IsHittingExteriorBorder(ColoBox boxToPlace, LocationStatemachine.Machine locationMachine)
        {
            return boxToPlace.MaxX > locationMachine.LastVerticalLine;
        }

        internal override double NextBoxLocation(double boxOriginIndexX, double boundaryX)
        {
            return boxOriginIndexX + boundaryX;
        }

        internal override double CalculateNextLinearLocation(Border vx, ColoBox boxToPlace, double spaceAfterObstacle)
        {
            return vx.BoxWhichBelongTo.MinX + vx.BoxWhichBelongTo.Width + spaceAfterObstacle;
        }
    }
}
