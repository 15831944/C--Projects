using ColoEngine.Model;
using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace ColoCanvas
{
    public class MicrosoftWebSockets : WebSocketHandler
    {
        private ColoOptimizer optimizer;
        public override void OnOpen()
        {
            XElement cfg = new XElement("Cfg",
                                new XElement("StaticLayers",
                                    new XElement("Layer", "Layer 4", new XAttribute("IsUniverse", "True")),
                                    new XElement("Layer", "pillars")
                                             ),
                                new XElement("ColdWidth", "20"),
                                new XElement("SpaceAfterObstacle", "4"),
                                new XElement("Direction", "R2L") //can be R2L  or L2R
                //new XElement("Direction","L2R") //can be R2L  or L2R
                                );
            //string fileName = @"..\..\Ken-L-Room-One-Pillar.dxf";            
            //string fileName = @"..\..\Ken-T-Room-One-Pillar-Lots-O-Racks.dxf";
            //string fileName = @"..\..\Ken-Tough-Room.dxf";
            //string fileName = @"..\..\32Degree-room.dxf";
            string fileName = System.Web.HttpContext.Current.Server.MapPath(@"Ken-Tough-Room-Vertical-v2.dxf");
            //optimizer = new ColoOptimizer(fileName, cfg);
            //
        }

        public override void OnMessage(string message)
        {
            switch (message)
            {
                case "Universe":
                    this.Send(optimizer.GetUniverse());
                    break;
            }
        }
    }
}