using DXFPoc.Model.BorderControl;
using DXFPoc.Model.DirectionControl;
using netDxf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFPoc.Model
{
    public class LayoutManager
    {
        public double OriginX { get { return locationMachine.OriginX; } }// boundingBox.Origin.X; } }
        public double OriginY { get { return locationMachine.OriginY; } }// boundingBox.Origin.Y; } }
        private ColoBox boundingBox;
        private BorderVerifier borderVerifier;
        private Point NextLocation;
        private double coldAisle;
        private double spaceAfterObstacle;        
        internal DirectionBase DirectionController { get; set; }
        private int row = 1;
        private DXFPoc.Model.LocationStatemachine.Machine locationMachine;

        public LayoutManager(double coldWidth,double spaceAfterObstacle,DirectionBase direction)
        {
            this.coldAisle = coldWidth;
            this.spaceAfterObstacle = spaceAfterObstacle;

            this.DirectionController = direction;
            direction.LayoutManager = this;
        }
        
        internal void SetBoundingBox(List<ColoBox> staticBoxes)
        {
            boundingBox = GetBiggestBox(staticBoxes);            
            DXFPoc.Model.LocationStatemachine.Machine locationMachine = new LocationStatemachine.Machine(coldAisle, spaceAfterObstacle, boundingBox);
            this.locationMachine = locationMachine;
            this.locationMachine.direction = DirectionController;
            
            borderVerifier = new BorderVerifier(staticBoxes,locationMachine);
            locationMachine.BorderVerifier = borderVerifier;

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
        internal List<ColoBox> OrganizeColoBoxes(List<ColoBox> boxes2relo)
        {
                        
            for (int i = 0; i < 40;++i)// boxes2relo.Count - 1; ++i)
            {
                locationMachine.CurrentIndex = i;
                if (i == 5)
                { }
                
                locationMachine.CurrentBoxWidth = boxes2relo[i].Width;
                NextLocation = borderVerifier.NextLocation;// boxes2relo[i - 1].NextXLocation;
                //boxes2relo[i].MoveOriginTo(NextLocation);

                //running iteration until we found the next valid location
                borderVerifier.Move2NextLocation(NextLocation,boxes2relo[i]);
               
            }

            return boxes2relo;
        }        

        public static ColoBox GetBiggestBox(List<ColoBox> staticObjects)
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

        internal BorderCollection VerticalX { get { return borderVerifier.VerticalX; } }

        public double RowStartY { get { return locationMachine.OriginY + locationMachine.NextRowStart; } }

        public double FirstVerticalLine { get { return locationMachine.FirstVerticalLine; } set { locationMachine.FirstVerticalLine = value; } }

        public double LastVerticalLine { get { return locationMachine.LastVerticalLine; } set { locationMachine.LastVerticalLine = value; } }

        public double BoundingBoxWidth { get { return locationMachine.boundingBox.Width; } }

        public ColoBox CurrentBoxToPlace { get { return locationMachine.CurrentBoxToPlace; } }

        public double CurrentBoxToPlaceWidth { get { return locationMachine.CurrentBoxWidth; } }
    }
}
