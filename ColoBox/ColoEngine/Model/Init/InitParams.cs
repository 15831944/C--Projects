using System.Security.AccessControl;
using ColoEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoEngine.Model.Init
{
    public class InitParams
    {
        public List<LayerName> StaticLayers { get; set; }
        public double ColdAisleWidth { get; set; }
        public double SpaceAfterObstacle { get; set; }
        public int Racks { get; set; }
        public int RacksBeforeOpenning { get; set; }
        public double RacksOpenningSpace { get; set; }
        public LockDirection Direction { get; set; }
        public string[] StaticLayersNames{set;get;}
        public bool[] StaticLayersUniverse{set;get;}
        public string FileName { set; get; }
        public double Tolerance { get; set; }

        public int IndexToStop { get; set; }
        public double OriginX { set; get; }
        public double OriginY { set; get; }
        public bool UseExistingData { get; set; }        
        public List<Offsets> OffsetList { get; set; }
        public int Stage { get; set; }

        public InitParams()
        {
            OffsetList = new List<Offsets>();
        }
    }

    public class Offsets
    {
       public int LineIndex { get; set; }  
       public double Offset { get; set; } 
    }
}
