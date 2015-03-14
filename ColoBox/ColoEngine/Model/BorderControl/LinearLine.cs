using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.BorderControl
{
    internal class LinearLine
    {
        double lineX1;
        double lineX2;
        double lineY1;
        double lineY2;

        double a;
        double b;
        public LinearLine(Border border)
        {
            lineX1 = border.Endpoints[0].coordinate1.Value;
            lineX2 = border.Endpoints[1].coordinate1.Value;
            lineY1 = border.Endpoints[0].coordinate2.Value;
            lineY2 = border.Endpoints[1].coordinate2.Value;


            //making sure that always y2 is greater than y1
            if (lineY1 > lineY2)
            {
                //switching them
                double x1 = lineX1;
                double y1 = lineY1;
                lineX1 = lineX2;
                lineY1 = lineY2;

                lineX2 = x1;
                lineY2 = y1;
            }

            a = (lineY2 - lineY1) / (lineX2 - lineX1);
            b = lineY1 - a * lineX1;
        }

        public double GetXIntersect(double y)
        {
            double x = (y - b) / a;
            return x;
        }

        private double getIntersect(double x)
        {
            double y = a * x + b;
            return y;
        }

        internal bool FindCollision(ColoBox boxToPlace)
        {


            //bool isNegative = false;
            double minY = boxToPlace.MinY;
            double maxY = boxToPlace.MaxY;
            double minX = boxToPlace.MinX;
            double maxX = boxToPlace.MaxX;


            //edge case that a corner of the box is on the line but not crossing it
            if (Math.Round(minY,2) == Math.Round(a * minX + b,2) ||
               Math.Round(maxY,2) == Math.Round(a * maxX + b,2))
                return false;

            bool isVerticalLineCollide = IsVerticalCollide(boxToPlace);
            bool isHorizontalLineCollide = IsHorizontalCollide(boxToPlace);

            return isVerticalLineCollide || isHorizontalLineCollide;

        }

        private bool IsHorizontalCollide(ColoBox boxToPlace)
        {
            double minX = boxToPlace.MinX;
            double maxX = boxToPlace.MaxX;
            double minY = boxToPlace.MinY;
            double maxY = boxToPlace.MaxY;

            double minCrossX = this.GetXIntersect(minY);
            double maxCrossX = this.GetXIntersect(maxY);

            bool minCross = false;
            bool maxCross = false;

            if (minX < minCrossX && minCrossX < maxX)
            {
                //double minCrossY = this.getIntersect(minCrossX);
                if (getSmaller("Y") < minY && minY < getBigger("Y"))
                {
                    minCross = true;
                }
            }

            if (minY < maxCrossX && maxCrossX < maxY)
            {
                if (getSmaller("Y") < maxY && maxY < getBigger("Y"))
                {
                    maxCross = true;
                }
            }


            return minCross || maxCross;
        }

        private bool IsVerticalCollide(ColoBox boxToPlace)
        {
            double minX = boxToPlace.MinX;
            double maxX = boxToPlace.MaxX;
            double minY = boxToPlace.MinY;
            double maxY = boxToPlace.MaxY;

            double minCrossY = this.getIntersect(minX);
            double maxCrossY = this.getIntersect(maxX);

            bool minCross = false;
            bool maxCross = false;

            if (minY < minCrossY && minCrossY < maxY)
            {
                //double minCrossX = this.GetXIntersect(minCrossY);
                if (getSmaller("X") < minX && minX < getBigger("X"))
                {
                    minCross = true;
                }
                
            }

            if (minY < maxCrossY && maxCrossY < maxY)
            {
                //double maxCrossX = this.GetXIntersect(maxCrossY);
                if (getSmaller("X") < maxX && maxX < getBigger("X"))
                {
                    maxCross = true;
                }
                
            }


            return minCross || maxCross;
        }

        private double getBigger(string axis)
        {
            if (axis == "X")
            {
                if (lineX1 > lineX2)
                    return lineX1;

                return lineX2;
            }

            if (lineY1 > lineY2)
                return lineY1;

            return lineY2;
        }

        private double getSmaller(string axis)
        {
            if (axis == "X")
            {
                if (lineX1 < lineX2)
                    return lineX1;
                else
                    return lineX2;
            }


            if (lineY1 < lineY2)
                return lineY1;
            else
                return lineY2;
        }

        private bool findIntersection(double minX, double minY, double maxX, double maxY)
        {

            if (minY > getSmaller("Y") && minY < getBigger("Y"))
            {
                //negative pitch
                if (lineX2 < lineX1)
                {
                    if (minX > lineX2 && minX < lineX1)
                    {
                        //double yOfX = this.getIntersect(minX);
                        //if (yOfX > minY && yOfX < maxY)
                        return true;
                    }
                }

                //positive pitch
                if (lineX1 < lineX2)
                {
                    if (minX > lineX1 && minX < lineX2)
                    {
                        //double yOfX = this.getIntersect(minX);
                        //if (yOfX > minY && yOfX < maxY)
                        return true;
                    }
                }
            }


            if (minY > getSmaller("Y") && minY < getBigger("Y"))
            {
                //negative pitch
                if (lineX2 < lineX1)
                {
                    if (minX > lineX2 && minX < lineX1)
                    {
                        // double yOfX = this.getIntersect(minX);
                        // if (yOfX > minY && yOfX < maxY)
                        return true;
                    }
                }

                //positive pitch
                if (lineX1 < lineX2)
                {
                    if (minX > lineX1 && minX < lineX2)
                    {
                        // double yOfX = this.getIntersect(minX);
                        //  if (yOfX > minY && yOfX < maxY)
                        return true;
                    }
                }
            }


            return false;
        }
    }
}
