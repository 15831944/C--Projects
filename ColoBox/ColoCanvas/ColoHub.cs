using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using ColoEngine.Model;
using System.Threading;
using ColoEngine.Model.Init;

namespace ColoCanvas
{
    public class ColoHub : Hub
    {
        public static string ConnectionId;
        private ColoOptimizer optimizer;
        public void Send(string message)
        {
            //if (message == "init")
            //{
            //    startColoOptimizer();
            //    Clients.Caller.drawUniverse(optimizer.GetUniverse());
            //    Clients.Caller.drawObstacles(optimizer.GetObstaclesPoints());
                
            //    //start the operation that raises events which communicate back with the client
            //    optimizer.Optimize();
            //}


        }

        public void BeginOptimization(InitParams init)
        {
            startColoOptimizer(init);
            Clients.Caller.drawUniverse(optimizer.GetUniverse());
            Clients.Caller.drawObstacles(optimizer.GetObstaclesPoints());

            //start the operation that raises events which communicate back with the client
            optimizer.Optimize();
        }
        
        private void startColoOptimizer(InitParams init)
        {
            //XElement cfg = new XElement("Cfg",
            //                   new XElement("StaticLayers",
            //                       new XElement("Layer", "Layer 4", new XAttribute("IsUniverse", "True")),
            //                       new XElement("Layer", "pillars")
            //                                ),
            //                   new XElement("ColdWidth", "22"),
            //                   new XElement("SpaceAfterObstacle", "4"),
            //                   //new XElement("Direction", "R2L") //can be R2L  or L2R
            //                   new XElement("Direction","L2R") //can be R2L  or L2R
                               //);
            //string fileName = @"..\..\Ken-L-Room-One-Pillar.dxf";            
            //string fileName = @"..\..\Ken-T-Room-One-Pillar-Lots-O-Racks.dxf";
            //string fileName = @"..\..\Ken-Tough-Room.dxf";
            //string fileName = @"..\..\32Degree-room.dxf";
            string fileName = System.Web.HttpContext.Current.Server.MapPath(@"..\api\Ken-Tough-Room-Vertical-v2.dxf");
            optimizer = new ColoOptimizer(fileName, init);

            optimizer.BoxLookAhead+=optimizer_BoxLookAhead;
            optimizer.PlacementDone += optimizer_PlacementDone;
        }

        void optimizer_PlacementDone(object o)
        {
            optimizer.BoxLookAhead -= optimizer_BoxLookAhead;
            optimizer.PlacementDone -= optimizer_PlacementDone;
            optimizer = null;
            Clients.Caller.placementDone();
        }

        void optimizer_BoxLookAhead(string pointList, int boxId, bool isValid)
        {
            //Thread.Sleep(50);
            Clients.Caller.drawBox(pointList, boxId, isValid);
        }
    }
}
