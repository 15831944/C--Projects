using ColoEngine.Model.DirectionControl;
using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.BorderControl;

namespace ColoEngine.Model
{
    public class ColoBox
    {
        public List<Point> Points { get; set; }
        public Dictionary<string,Point> OriginalPoints { set; get; }
        public List<Point> LookAheadPath { set; get; }        
        internal List<RawBorder> Borders { set; get; }
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
        private DirectionBase direction 
        {
            get
            {
                if (LayoutManager != null)
                {
                    return LayoutManager.LocationMachine.direction;
                }
                return null;
            }
        }
        public int BoxId { get; set; }
        public Point NextXLocation { get { return getNextLocation(); } }

        
        public ColoBox(string layer,int boxId)
        {            
            initializeLists();
            this.Layer = layer;
            //this.direction = direction;
            this.BoxId = boxId;
        }
        public ColoBox()
        {
            initializeLists();
            //this.direction = direction;
        }

        private void initializeLists()
        {
            Points = new List<Point>();
            Borders = new List<RawBorder>();
            LookAheadPath = new List<Point>();
        }


        HashSet<string> existingPoints = new HashSet<string>();
        private Point? prevPoint = null;
        public void AddPoint(double x, double y)
        {
            Point v = new Point { X = x, Y = y };
            if (prevPoint.Equals(v))
                return;


            if (prevPoint.HasValue)
            {
                RawBorder rb = new RawBorder(prevPoint.Value, v );
                Borders.Add(rb);
            }
            prevPoint = v;



            string p = string.Format("{0}_{1}", x, y);
            if (!existingPoints.Contains(p))
            {
                
                Points.Add(v);
                calcExtreme();
                existingPoints.Add(p);
            }
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
            ColoBox ret = new ColoBox();
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
                //if (IsRotating)
                //{
                //    newPoints.Add(new Point {  X_Tag = p.X + deltaX, Y_Tag = p.Y + deltaY });
                //}
                //else
                //{
                    newPoints.Add(new Point { X = p.X + deltaX, Y = p.Y + deltaY });
                //}
            }
            Points = newPoints;
            calcExtreme();
           
            LookAheadPath.Add(loc);
        }
        public void Move(double deltaX, double deltaY,bool isRotating)
        {
            for(int i = 0; i<Points.Count;++i)//in Points)
            {
                Point p = Points[i];
                //if (isRotating)
                //{
                //    p.X_Tag += deltaX;
                //    p.Y_Tag += deltaX;
                //}
                //else
                //{
                    p.X += deltaX;
                    p.Y += deltaY;
                //}
            }
            calcExtreme();
        }

        public override string ToString()
        {
            return string.Format("[{0:0},{1:0}][{2:0},{3:0}]({5}){4}",minX,minY,maxX,maxY,Layer,Borders.Count );
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
            double x = this.LayoutManager.LocationMachine.direction.NextBoxLocation(boxOriginIndex.X, boundaryX);//   this.boxOriginIndex.X + boundaryX;
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

        
        public string SerializePoints()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Point p in Points)
            {
                sb.AppendLine(p.ToString());
            }
            return sb.ToString();
        }
        public string PointsCollection {
            get
            {
                calculateFactor();
                Dictionary<string, Point> origPoints;
                string ret = buildPointCollection(Points,out origPoints);
                this.OriginalPoints = origPoints;
                return ret;
            }
        }

        public string BorderCollection
        {
            get
            {
                calculateFactor();
                List<Point> points = new List<Point>();
                foreach (RawBorder rb in Borders)
                {
                    points.Add(rb.Point1);
                    points.Add(rb.Point2);
                }
                Dictionary<string,Point> tempOrigList;
                string res = buildPointCollection(points, out tempOrigList, false);
                OriginalPoints = tempOrigList;
                return res;
            }
        }

        private void calculateFactor()
        {
            if (!this.LayoutManager.LocationMachine.direction.factor.HasValue)
            {
                calcExtreme();
                direction.minX = minX;
                direction.minY = minY;
                if (Width >= Height)
                {
                    if (Width != 0)
                        this.LayoutManager.LocationMachine.direction.factor = 700 / Width;

                }
                else
                {
                    if (Height != 0)
                        this.LayoutManager.LocationMachine.direction.factor = 700 / Height;
                }

                this.LayoutManager.LocationMachine.direction.YOffset = getYOffset(this.LayoutManager.LocationMachine.direction.factor.Value);
            }
        }
        private string buildPointCollectionWithOriginal(List<Point> points, out Dictionary<string, Point> origList, bool closeShape = true)
        {
            StringBuilder retList = new StringBuilder("[");
            origList = new Dictionary<string, Point>();
            HashSet<string> alreadyUsed = new HashSet<string>();
            string fp = null;
            Point ofp = new Point();
            foreach (var p in points)
            {

                string token = string.Format("{{ \"point\": {{ \"render\": {{ \"x\":{0:0.00},\"y\" :{1:0.00} }}, \"orig\": {{\"x\":{2:0.00},\"y\" :{3:0.00} }} }}", 
                                            (p.X - direction.minX) * direction.factor, 
                                            (2000 - p.Y) * direction.factor - direction.YOffset,
                                            p.X,
                                            p.Y);
                Point origPoint = new Point() { X = (p.X - direction.minX), Y = (2000 - p.Y) };
                if (fp == null)
                {
                    fp = token;
                    ofp = origPoint;
                }
                if (!alreadyUsed.Contains(token))
                {
                    alreadyUsed.Add(token);
                    retList.AppendFormat("{0},", token);
                    origList.Add(token, origPoint);
                }
            }

            if (closeShape)
            {
                retList.AppendFormat("{0},", fp);
                // origList.Add(fp, ofp);
            }
            return retList.ToString();
        }
        private string buildPointCollection(List<Point> points,out Dictionary<string,Point> origList,bool closeShape=true)
        {
            StringBuilder retList = new StringBuilder();
            origList = new Dictionary<string, Point>();
            HashSet<string> alreadyUsed = new HashSet<string>();
            string fp = null;
            Point ofp = new Point ();
            foreach (var p in points)
            {

                string token = string.Format("{0:0.00},{1:0.00}", (p.X - direction.minX) * direction.factor, (2000 - p.Y) * direction.factor - direction.YOffset);
                Point origPoint = new Point (){ X = (p.X - direction.minX), Y = (2000 - p.Y) };
                if (fp == null)
                {
                    fp = token;
                    ofp = origPoint;
                }
                if (!alreadyUsed.Contains(token))
                {
                    alreadyUsed.Add(token);
                    retList.AppendFormat("{0} ", token);
                    origList.Add(token,origPoint);
                }
            }

            if (closeShape)
            {
                retList.AppendFormat("{0} ", fp);
               // origList.Add(fp, ofp);
            }
            return retList.ToString();


            // return "531.67,531.67 507.86,507.86";


            //return "256,84.75 209,100.75 230.75,157.5 142.5,194.25 160.75,248.5 221.75,223.75 256,241 ";
        }
        public List<string> ShadowPath {
            get
            {
                return LookAheadPath.Aggregate(new List<string>(), (lst, point) =>
                    {
                        List<Point> input = new List<Point>();
                        input.Add(point);

                        //second: right-left
                        input.Add(new Point { X = point.X + Width, Y = point.Y });

                        //third: right-top
                        input.Add(new Point { X = point.X + Width, Y = point.Y + Height });

                        //forth: left - top
                        input.Add(new Point { X = point.X, Y = point.Y + Height });

                        Dictionary<string, Point> tempPoints;
                        lst.Add(buildPointCollection(input,out tempPoints));
                        this.OriginalPoints = tempPoints;
                        return lst;
                    });                
            }
        }
        private double getYOffset(double factor)
        {
            double min = double.MaxValue;
            double calc;
            foreach (Point p in Points)
            {
                calc = (2000 - p.Y) * factor;
                if (calc < min)
                    min = calc;
            }

            return min;
        }

        public Border BorderUsedToGetRowStartX { get; set; }

        
    }
}
