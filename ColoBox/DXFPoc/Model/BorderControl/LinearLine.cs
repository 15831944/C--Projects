using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model.BorderControl
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

            a = (lineY2 - lineY1) / (lineX2 - lineX1);
            b = lineY1 - a * lineX1;
        }

        private double getIntersect(double x)
        {
            double y = a * x + b;
            return y;
        }

        internal bool FindCollision(ColoBox boxToPlace)
        {            
            Dictionary<double, List<double>> lines = new Dictionary<double, List<double>>();

            //creating 2 vertical lines of the boxToPlace
            foreach (Point point in boxToPlace.Points.Distinct())
            {
                if (!lines.ContainsKey(point.X))
                {
                    List<double> lst = new List<double>();
                    lst.Add(point.Y);
                    lines.Add(point.X, lst);
                }
                else
                {
                    
                    lines[point.X].Add(point.Y);
                }
            }

            foreach(double x in lines.Keys)
            {
                double y = getIntersect(x);
                List<double> ys = lines[x];
                ys.Sort();

                if (y > ys[0] && y < ys[1])
                {
                    if (x > lineX1 && x < lineX2)
                        return true;
                }
            }

            return false;
        }
    }
}
