using ColoEngine.Model.BorderControl;
using ColoEngine.Model.LocationStatemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.LookAheadPath
{
    internal class PathSectionBuilderoo : IPathBase
    {
        private Dictionary<Point, string> pathSections = null;
        //private List<Dictionary<Point, string>> pathSections = null;
        public Machine machine { get; set; }


        public PathSectionBuilderoo(Machine machine)
        {
            this.machine = machine;
        }        

        private Point? setInsideOutsideState(string p, double x, double y)
        {
            if (p == "Inside")
            {
                machine.INSIDEBOUNDINGBOX.SetCurrent();
                return null;
            }
            else
            {
                machine.OUTSIDEBOUNDINGBOX.SetCurrent();
                return new Point { X = x, Y = y };
            }
        }

        #region public
        internal void ResetRelativeLocation()
        {
            pathSections = null;
        }

        internal void AssertBoxLocation(ColoBox boxToPlace)
        {
            if (pathSections == null)
                return;

            // if (pathSections.Count < 2)
            //     return;

            Point lastPoint = new Point { X = double.MaxValue, Y = double.MaxValue };
            foreach (var p in pathSections)
            {
                lastPoint = p.Key;
            }

            if (machine.direction.ClosestX > lastPoint.Y && machine.IsInsideBoundingBox)
                machine.OUTSIDEBOUNDINGBOX.SetCurrent();
        }

        internal void SetRelativeLocation(ColoBox boxToPlace)
        {
            if (pathSections == null)
            {
                //machine.NextRowStart
                //currentBoxToPlace.Height
                List<Border> borderInPath = new List<Border>();
                foreach (Border b in machine.BorderVerifier.VerticalX)
                {
                    if (b.BoxWhichBelongTo != machine.boundingBox)
                        continue;

                    foreach (var p in b.Endpoints)
                    {

                        if (p.coordinate1 >= boxToPlace.MinY && p.coordinate1 <= boxToPlace.MaxY ||
                          p.coordinate2 >= boxToPlace.MinY && p.coordinate2 <= boxToPlace.MaxY)
                        {
                            if (!borderInPath.Contains(b))
                                borderInPath.Add(b);
                        }

                    }
                }


                borderInPath.AddRange(machine.GetAngledBordersInPath(boxToPlace,0));


                borderInPath.Sort();

                pathSections = new Dictionary<Point, string>();
                string first = string.Empty;
                string second = string.Empty;
                bool isOrderSet = false;
                for (int i = 0; i < borderInPath.Count - 1; ++i)
                {
                    Point p = new Point { X = borderInPath[i].Coordinate, Y = borderInPath[i + 1].Coordinate };

                    //this is a case of tilted border
                    if (borderInPath[i].Endpoints.Count > 1)
                    {
                        p = getTiltedBorderEnds(borderInPath, i);
                    }



                    if (!isOrderSet)
                    {
                        isOrderSet = true;


                        if (boxToPlace.MinX >= p.X && boxToPlace.MaxX <= p.Y)
                        {
                            first = "Inside";
                            second = "Outside";
                        }
                        else
                        {
                            first = "Outside";
                            second = "Inside";
                        }
                    }


                    if (i % 2 == 0)
                    //
                    {
                        //pathSections.Add(p, "Inside");
                        pathSections.Add(p, first);
                    }
                    else
                    {
                        //pathSections.Add(p, "Outside");
                        pathSections.Add(p, second);
                    }
                }
            }

        }

        /// <summary>
        /// This method is looking to find out if the box is currently inside or outside the main shape
        /// </summary>
        /// <param name="boxToPlace"></param>
        /// <returns></returns>
        internal Point? UpdateMachineState(ColoBox boxToPlace)
        {
            //The state "RowStarts" prepare the pathSections down stream of the current row so we can compare if the box is inside or outside the bounding box
            SetRelativeLocation(boxToPlace);
            Point? ret = null;
            foreach (KeyValuePair<Point, string> el in pathSections)
            {
                if (boxToPlace.MaxX >= el.Key.X && boxToPlace.MaxX <= el.Key.Y)
                {
                    ret = setInsideOutsideState(el.Value, machine.direction.getNextLocationAfterOutside(el.Key), boxToPlace.MinY);
                }
            }

            return ret;
        }
        #endregion public
        //this is the method that finds the 
        private Point getTiltedBorderEnds(List<Border> borderInPath, int i)
        {
            Border b1 = borderInPath[i];
            Border b2 = borderInPath[i + 1];
           

            return new Point { X = b1.PointWithinBoxLimits.X, Y = b2.PointWithinBoxLimits.X };
        }



        void IPathBase.ResetRelativeLocation()
        {
            throw new NotImplementedException();
        }

        void IPathBase.AssertBoxLocation()
        {
            throw new NotImplementedException();
        }

        void IPathBase.SetRelativeLocation(ColoBox boxToPlace)
        {
            throw new NotImplementedException();
        }

        Point? IPathBase.UpdateMachineState(ColoBox boxToPlace)
        {
            throw new NotImplementedException();
        }
    }
}
