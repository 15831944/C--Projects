using DXFPoc.Model;
//using DXFPoc.Model;
//using netDxf;
//using netDxf.Entities;
//using netDxf.Header;
//using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DXFPoc
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //XElement cfg = new XElement("Cfg",
            //                   new XElement("StaticLayers",
            //                       new XElement("Layer", "Layer 4"),
            //                       new XElement("Layer", "pillars")
            //                                ),
            //                   new XElement("ColdWidth", "20"),
            //                   new XElement("SpaceAfterObstacle","4"),
            //                   new XElement("Direction", "R2L") //can be R2L  or L2R
            //                   //new XElement("Direction","L2R") //can be R2L  or L2R
            //                   );
            ////string fileName = @"..\..\Ken-L-Room-One-Pillar.dxf";            
            ////string fileName = @"..\..\Ken-T-Room-One-Pillar-Lots-O-Racks.dxf";
            ////string fileName = @"..\..\Ken-Tough-Room.dxf";
            ////string fileName = @"..\..\32Degree-room.dxf";
            //string fileName = @"..\..\Ken-Tough-Room-Vertical-v2.dxf";
            //ColoOptimizer optimizer = new ColoOptimizer(fileName,cfg);
            //optimizer.Optimize().Save("poc.dxf");            
        }

        

        //private static void CustomHatchPattern(DxfDocument dxf,Layer layer)
        //{
            

        //    LwPolyline poly = new LwPolyline();
        //    poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
        //    poly.Vertexes.Add(new LwPolylineVertex(10, -10));
        //    poly.Vertexes.Add(new LwPolylineVertex(10, 10));
        //    poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
        //    poly.Vertexes[2].Bulge = 1;
        //    poly.IsClosed = true;
        //    poly.Layer = layer;

        //    LwPolyline poly2 = new LwPolyline();
        //    poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
        //    poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
        //    poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
        //    poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
        //    poly2.Vertexes[1].Bulge = -0.25;
        //    poly2.IsClosed = true;

        //    LwPolyline poly3 = new LwPolyline();
        //    poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
        //    poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
        //    poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
        //    poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
        //    poly3.IsClosed = true;

        //    List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
        //                                                                    new HatchBoundaryPath(new List<EntityObject>{poly}),
        //                                                                    new HatchBoundaryPath(new List<EntityObject>{poly2}),
        //                                                                    new HatchBoundaryPath(new List<EntityObject>{poly3}),
        //                                                                  };

        //    HatchPattern pattern = new HatchPattern("MyPattern", "A custom hatch pattern");

        //    HatchPatternLineDefinition line1 = new HatchPatternLineDefinition();
        //    line1.Angle = 45;
        //    line1.Origin = Vector2.Zero;
        //    line1.Delta = new Vector2(4, 4);
        //    line1.DashPattern.Add(12);
        //    line1.DashPattern.Add(-4);
        //    pattern.LineDefinitions.Add(line1);

        //    HatchPatternLineDefinition line2 = new HatchPatternLineDefinition();
        //    line2.Angle = 135;
        //    line2.Origin = new Vector2(2.828427125, 2.828427125);
        //    line2.Delta = new Vector2(4, -4);
        //    line2.DashPattern.Add(12);
        //    line2.DashPattern.Add(-4);
        //    pattern.LineDefinitions.Add(line2);

        //    Hatch hatch = new Hatch(pattern, boundary);
            
        //    hatch.Pattern.Angle = 0;
        //    hatch.Pattern.Scale = 1;
        //    dxf.AddEntity(poly);
        //    dxf.AddEntity(poly2);
        //    dxf.AddEntity(poly3);
        //    dxf.AddEntity(hatch);

        //    dxf.Save("hatchTest.dxf");
        //}
    }
}
