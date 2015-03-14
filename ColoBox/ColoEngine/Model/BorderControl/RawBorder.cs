using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model;

namespace ColoEngine.Model.BorderControl
{
    public class RawBorder
    {
        public static double Tolerance { set; get; }

        public RawBorder(Point point1, Point point2)
        {
            if (point1.X < point2.X)
            {
                Point1 = point1;
                Point2 = point2;
            }
            else
            {
                Point2 = point1;
                Point1 = point2;
            }
        }
        public Point Point1 { set; get; }
        public Point Point2 { set; get; }
        public Orientation BorderType {
            get
            {
                if (Math.Abs(Point1.X - Point2.X) <= Tolerance && Point1.X != Point2.X)
                {
                    //Point1.X = Point2.X;
                    Point1 = new Point {X = Point2.X, Y = Point1.Y};
                }

                if (Math.Abs(Point1.Y - Point2.Y) <= Tolerance && Point1.Y != Point2.Y)
                {
                    //Point1.Y = Point2.Y;
                    Point1 = new Point {X = Point1.X, Y = Point2.Y};
                }

                if (Math.Round(Point1.X,2) == Math.Round(Point2.X,2))
                    return Orientation.Vertical;

                if (Math.Round(Point1.Y) == Math.Round(Point2.Y))
                    return Orientation.Horizontal;


                if (Point1.Y < Point2.Y)
                {
                    return Orientation.Positive;
                }


                return Orientation.Negative;
            }
        }


        public override string ToString()
        {
            return string.Format("[{0},{1}][{2},{3}]{4}",Point1.X.ToString("N0"),Point1.Y.ToString("N0"),Point2.X.ToString("N0"),Point2.Y.ToString("N0"),BorderType);
        }
    }
}
