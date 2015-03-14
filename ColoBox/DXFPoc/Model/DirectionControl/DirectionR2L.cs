using DXFPoc.Model.BorderControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.DirectionControl
{
    internal class DirectionR2L : DirectionBase
    {
        internal override double GetFirstBorderCoordinate()
        {
            double nextYCoordinate = LayoutManager.RowStartY;
            Border minBorder = null;
            Border maxBorder = null;
            foreach (Border b in LayoutManager.VerticalX)
            {
                if (IsBorderInVector(b, nextYCoordinate))
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
                LayoutManager.FirstVerticalLine = LayoutManager.OriginX + LayoutManager.BoundingBoxWidth;// -LayoutManager.CurrentBoxToPlace.Width;// LayoutManager.OriginX;
                LayoutManager.LastVerticalLine = LayoutManager.OriginX - LayoutManager.BoundingBoxWidth;
                return LayoutManager.LastVerticalLine;
            }

            LayoutManager.FirstVerticalLine = maxBorder.Coordinate - LayoutManager.CurrentBoxToPlaceWidth;
            LayoutManager.LastVerticalLine = minBorder.Coordinate;

            return LayoutManager.FirstVerticalLine;// maxBorder.Coordinate;
        }        

        internal override double OriginX(ColoBox boundingBox)
        {
           return boundingBox.Origin.X + boundingBox.Width;
        }

        internal override bool IsTouchingOriginLines(LocationStatemachine.Machine locationMachine, Border vx)
        {
            return locationMachine.FirstVerticalLine == vx.Coordinate - locationMachine.CurrentBoxWidth;
        }

        internal override bool IsHittingExteriorBorder(ColoBox boxToPlace, LocationStatemachine.Machine locationMachine)
        {
            return boxToPlace.MinX < locationMachine.LastVerticalLine;
        }

        internal override double NextBoxLocation(double boxOriginIndexX, double boundaryX)
        {
            return boxOriginIndexX - boundaryX;
        }

        internal override double CalculateNextLinearLocation(Border vx, ColoBox boxToPlace, double spaceAfterObstacle)
        {
            return vx.BoxWhichBelongTo.MinX -boxToPlace.Width  - spaceAfterObstacle;
        }
    }
}
