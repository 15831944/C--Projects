using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.Init
{
    public class ProjectClass
    {
        public double Factor { get; set; }
        public Point[] UniverseNodes { get; set; }
        public RackDefinition[] Racks { get; set; }
        public Obstacle[] Obstacles{ get; set; }
        public string[] Floor { set; get; }
        public Point[] BoundingBox { set; get; }
    }
}
