using DXFPoc.Model.DirectionControl;
using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model
{
    public class ColoBox
    {
        public List<Point> Points { get; set; }
        public Point Origin { get; set; }
        public string Layer { get; set; }

        private double minX = double.MaxValue;
        public double MinX { get { return minX; } }

        private double maxX = double.MinValue;
        public double MaxX { get { return maxX; } }

        private double minY = double.MaxValue;
        public double MinY { get { return minY; } }

        private double maxY = double.MinValue;
        public double MaxY { get { return maxY; } }

        public double Area { get { return boundaryX * boundaryY; } }
        public double Width { get { return boundaryX; } }
        public double Height { get { return boundaryY; } }
        private double boundaryX;
        private double boundaryY;
        private Point boxOriginIndex;
        public bool IsRotating { set; get; }
        public LayoutManager LayoutManager { get; set; }
        private DirectionBase direction { set; get; }
        
        public Point NextXLocation { get { return getNextLocation(); } }

        
        public ColoBox(string layer,DirectionBase direction)
        {            
            initializeLists();
            this.Layer = layer;
            this.direction = direction;
        }
        public ColoBox(DirectionBase direction)
        {
            initializeLists();
            this.direction = direction;
        }

        private void initializeLists()
        {
            Points = new List<Point>();            
        }

        

        public void AddPoint(double x, double y)
        {
            Point v = new Point { X = x, Y = y };
            Points.Add(v);
            calcExtreme();
        }

        /// <summary>
        /// Find the min and the max for each box
        /// </summary>
        private void calcExtreme()
        {
            minX = double.MaxValue;
            maxX = double.MinValue;
            minY = double.MaxValue;
            maxY = double.MinValue;
            for (int i = 0; i < Points.Count - 1; ++i)
            {
                Point v = Points[i];
                //finding min x
                if (v.X < minX)
                {
                    minX = v.X;
                    boxOriginIndex = v;                    
                }

                //finding max x
                if (v.X > maxX)
                    maxX = v.X;

                //finding min y
                if (v.Y < minY)
                {
                    minY = v.Y;
                    //this is the case that we found a point with a minimum Y and we need to make sure that it's X is still the minumim, otherwise 
                    if (v.X != boxOriginIndex.X && v.Y != boxOriginIndex.Y)
                    {
                        if (v.X < boxOriginIndex.X)
                        {
                            boxOriginIndex = v;
                        }
                    }
                }

                //finding max y
                if (v.Y > maxY)
                    maxY = v.Y;
               
            }

            boundaryX = maxX - minX;
            boundaryY = maxY - minY;

            Origin = new Point
            {
                X = minX,
                Y = minY
            };

        }

        public ColoBox Clone()
        {
            ColoBox ret = new ColoBox(direction);
            foreach (Point v in Points)
            {
                ret.AddPoint(v.X, v.Y);
            }

            return ret;
        }
        public void MoveOriginTo(Point loc)
        {
            double deltaX = loc.X - Origin.X;
            double deltaY = loc.Y - Origin.Y;

            List<Point> newPoints = new List<Point>();
            for (int i = 0; i < Points.Count; ++i)//in Points)
            {
                Point p = Points[i];
                if (IsRotating)
                {
                    newPoints.Add(new Point {  X_Tag = p.X + deltaX, Y_Tag = p.Y + deltaY });
                }
                else
                {
                    newPoints.Add(new Point { X = p.X + deltaX, Y = p.Y + deltaY });
                }
            }
            Points = newPoints;
            calcExtreme();
        }
        public void Move(double deltaX, double deltaY,bool isRotating)
        {
            for(int i = 0; i<Points.Count;++i)//in Points)
            {
                Point p = Points[i];
                if (isRotating)
                {
                    p.X_Tag += deltaX;
                    p.Y_Tag += deltaX;
                }
                else
                {
                    p.X += deltaX;
                    p.Y += deltaY;
                }
            }
            calcExtreme();
        }

        public override string ToString()
        {
            return Layer;
        }

        internal void MoveToOrigin()
        {
            double deltaX = boxOriginIndex.X;
            double deltaY = boxOriginIndex.Y;

            //if we have a layoutManager, we need to shift the delta using the origin values of the layoutManager
            if (LayoutManager != null)
            {
                deltaX -= LayoutManager.OriginX;
                deltaY -= LayoutManager.OriginY;
            }

            List<Point> newPoints = new List<Point>();
            for (int i = 0; i < Points.Count - 1; ++i)
            {
                Point v = new Point
                {
                    X = Points[i].X - deltaX,
                    Y = Points[i].Y - deltaY
                };
                newPoints.Add(v);
            }

            Points = newPoints;
            calcExtreme();
        }


        private Point getNextLocation()
        {
            double x =  direction.NextBoxLocation(boxOriginIndex.X, boundaryX);//   this.boxOriginIndex.X + boundaryX;
            return new Point { X = x, Y = boxOriginIndex.Y };
        }


        internal void Rotate(double Angle)
        {
            List<Point> transformedPoints = new List<Point>();
            foreach (Point p in Points)
            {
                p.Rotate(Angle);
                transformedPoints.Add(p);
            }
            calcExtreme();
            Points = transformedPoints;
            IsRotating = true;
        }
    }
}
