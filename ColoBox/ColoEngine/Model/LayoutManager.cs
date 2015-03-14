using ColoEngine.Model.BorderControl;
using ColoEngine.Model.DirectionControl;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.Init;
using ColoEngine.Model.LocationStatemachine;

namespace ColoEngine.Model
{
    public class LayoutManager
    {
        public double OriginX { get { return LocationMachine.OriginX; } }// boundingBox.Origin.X; } }
        public double OriginY { get { return LocationMachine.OriginY; } }// boundingBox.Origin.Y; } }
        private ColoBox boundingBox;
        public ColoBox BoundingBox { get { return boundingBox; } set { boundingBox = value; } }
        private BorderVerifier borderVerifier;
        private Point NextLocation;
        private InitParams initParam;     
        internal DirectionBase DirectionController { get; set; }
        private int row = 1;
        private Machine machine;
        public Machine LocationMachine
        {
            set
            {
                machine = value;
                if (initParam == null)
                {
                    DirectionController = new DirectionL2R(value);
                }
                else
                {
                    if (initParam.Direction == LockDirection.L2R)
                        DirectionController = new DirectionL2R(value);
                    else
                        DirectionController = new DirectionR2L(value);
                }

                DirectionController.LayoutManager = this;
            }
            get
            {
                return machine;
            }
        }

        public LayoutManager(InitParams initParams)
        {
           
            this.initParam = initParams;           
        }
        
        internal void SetBoundingBox(List<ColoBox> staticBoxes,bool isCandidate=false)
        {
            boundingBox = GetBiggestBox(staticBoxes,isCandidate);
            boundingBox.LayoutManager = this;
            if (this.LocationMachine == null)
            {
                ColoEngine.Model.LocationStatemachine.Machine locationMachine = new LocationStatemachine.Machine(initParam, boundingBox);
                this.LocationMachine = locationMachine;
                this.LocationMachine.direction = DirectionController;
                
            }
            this.LocationMachine.boundingBox = boundingBox;
            //assign the layout manager to all the static boxes
            foreach (ColoBox box in staticBoxes)
            {
                box.LayoutManager = this;
            }

            borderVerifier = new BorderVerifier(staticBoxes, this.LocationMachine);
            this.LocationMachine.BorderVerifier = borderVerifier;

            //if (borderVerifier.IsRotating)
            //{
            //    double angle = borderVerifier.Angle;               
            //    foreach (ColoBox cb in staticBoxes)
            //    {
            //        cb.Rotate(angle);
            //    }
            //}

            //borderVerifier = new BorderVerifier(staticBoxes, locationMachine);
        }
        internal List<ColoBox> OrganizeColoBoxes(List<ColoBox> boxes2relo,ColoOptimizer optimizer)
        {
            int i = 0;
            for (; i < initParam.Racks; ++i)// boxes2relo.Count - 1; ++i)
            {
                LocationMachine.CurrentIndex = i;
                if (i == initParam.Racks - 1)
                { }

                LocationMachine.CurrentBoxReference = boxes2relo[i];                
                NextLocation = borderVerifier.NextLocation;// boxes2relo[i - 1].NextXLocation;
                //boxes2relo[i].MoveOriginTo(NextLocation);


                //running iteration until we found the next valid location
                try
                {
                    borderVerifier.Move2NextLocation(NextLocation,boxes2relo[i],optimizer,i,optimizer.IndexToStop);
                }
                catch (ExtremeFoundException e)
                {
                    break;
                }

                //notify subscribers that box valid location found
                //optimizer.OnBoxLookAhead(boxes2relo[i].PointsCollection, i,true);
            }

            List<ColoBox> toReturn = new List<ColoBox>();
            for (int j = 0; j < i; ++j)
            {
                toReturn.Add(boxes2relo[j]);
            }
            return toReturn;
        }        

        public ColoBox GetBiggestBox(List<ColoBox> staticObjects,bool isCandidate)
        {
            if (!isCandidate)
            {
                double area = -1;
                ColoBox ret = null;
                foreach (ColoBox b in staticObjects)
                {
                    if (b.Area > area)
                    {
                        area = b.Area;
                        ret = b;
                    }
                }

                return ret;
            }
            else
            {
                double minX, minY, maxX, maxY;
                minX = minY = double.MaxValue;
                maxX = maxY = double.MinValue;
                foreach (ColoBox b in staticObjects)
                {
                    if (b.MinX < minX)
                        minX = b.MinX;
                    if (b.MinY < minY)
                        minY = b.MinY;
                    if (b.MaxX > maxX)
                        maxX = b.MaxX;
                    if (b.MaxY > maxY)
                        maxY = b.MaxY;
                }

                ColoBox ret = new ColoBox();
                ret.AddPoint(minX, minY);
                ret.AddPoint(minX, maxY);
                ret.AddPoint(maxX, maxY);
                ret.AddPoint(maxX, minY);
                ret.LayoutManager = this;
                string x = ret.PointsCollection;
                return ret;
            }
        }

        internal BorderCollection VerticalX { get { return borderVerifier.VerticalX; } }

        internal BorderCollection HorizontalY { get { return borderVerifier.HorizontalY; } }

        public double RowStartY { get { return LocationMachine.OriginY + LocationMachine.NextRowStart; } }

        public double FirstVerticalLine { get { return LocationMachine.FirstVerticalLine; } set { LocationMachine.FirstVerticalLine = value; } }

        public double LastVerticalLine { get { return LocationMachine.LastVerticalLine; } set { LocationMachine.LastVerticalLine = value; } }

        public double BoundingBoxWidth { get { return LocationMachine.boundingBox.Width; } }

        public ColoBox CurrentBoxToPlace { get { return LocationMachine.CurrentBoxToPlace; } }
        

        
    }
}
