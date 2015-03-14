using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using System.Xml.Linq;
using System.Xml.XPath;
using DXFPoc.Model.DirectionControl;
using System.Collections.ObjectModel;
using netDxf.Blocks;

namespace DXFPoc.Model
{
    class ColoOptimizer
    {
        private string fileName;
        private List<ColoBox> boxes;
        private List<string> staticLayers;
        private double coldWidth;
        private double spaceAfterObstacle;
        private DxfDocument dxf;
        private DirectionBase direction;
        private LayoutManager manager;
        public ColoOptimizer(string fileName, XElement config)
        {
            // TODO: Complete member initialization
            this.fileName = fileName;
            staticLayers = config.XPathSelectElements("//StaticLayers/Layer").Aggregate(new List<string>(), (lst, elem) =>
                {
                    lst.Add(elem.Value);
                    return lst;
                });

            coldWidth = double.Parse(config.XPathSelectElement("//ColdWidth").Value);
            spaceAfterObstacle = double.Parse(config.XPathSelectElement("//SpaceAfterObstacle").Value);
            LockDirection eDirection = (LockDirection)Enum.Parse(typeof(LockDirection), config.XPathSelectElement("//Direction").Value);
            if (eDirection == LockDirection.L2R)
                direction = new DirectionL2R();
            else
                direction = new DirectionR2L();
        }

        internal DxfDocument Optimize()
        {
            dxf = DxfDocument.Load(fileName);
            
            boxes = new List<ColoBox>();

            loadPolylinesToBoxes();
                       
            //CustomHatchPattern(dxf,layer);
            RelocateRacks(dxf);

            return dxf;
        }

        private void loadPolylinesToBoxes()
        {
            List<LwPolyline> polylines = new List<LwPolyline>();
            polylines.AddRange(dxf.LwPolylines);
            polylines.AddRange(getAllPolylines(dxf.Inserts));
            //polylines.AddRange(getAllPolylinesFromBlocks(dxf.Blocks));
               

            //every Polyline from DXF turns into a box (ColoBox)
            foreach (LwPolyline pl in polylines)
            {
                ColoBox box = new ColoBox(pl.Layer.Name, direction);
                boxes.Add(box);
                foreach (var v in pl.Vertexes)
                {
                    box.AddPoint(v.Location.X, v.Location.Y);
                }
            }

            foreach (Spline sp in dxf.Splines)
            {
                ColoBox box = new ColoBox(sp.Layer.Name, direction);
                boxes.Add(box);
                foreach (var v in sp.ControlPoints)
                {
                    box.AddPoint(v.Location.X, v.Location.Y);
                }
            }
        }

        private IEnumerable<LwPolyline> getAllPolylinesFromBlocks(ReadOnlyCollection<Block> blocks)
        {
            List<LwPolyline> lst = new List<LwPolyline>();
            foreach (Block b in blocks)
            {
                List<Insert> insertList = new List<Insert>();
                foreach (var e in b.Entities)
                {                   
                    if (e.GetType() == typeof(Insert))
                    {
                        insertList.Add((Insert)e);
                    }

                    if (e.GetType() == typeof(LwPolyline))
                    {
                        lst.Add((LwPolyline)e);
                    }
                }

                lst.AddRange(getAllPolylines(insertList));
            }

            return lst;
        }

        private IEnumerable<LwPolyline> getAllPolylines(IEnumerable<Insert> inserts)
        {
            List<Insert> foundInserts = new List<Insert>();
            List<LwPolyline> foundPolylines = new List<LwPolyline>();
            foreach (Insert insert in inserts)
            {
                foundInserts.Clear();
                foreach (EntityObject entity in insert.Block.Entities)
                {
                    if (entity is Insert)
                    {
                        foundInserts.Add((Insert)entity);
                    }
                    else if(entity is LwPolyline)
                    {
                        foundPolylines.Add((LwPolyline)entity);
                    }
                }

                if (foundInserts.Count > 0)
                    foundPolylines.AddRange(getAllPolylines(foundInserts));
                
                    
            }
            return foundPolylines;
        }

        private void RelocateRacks(DxfDocument dxf)
        {
            List<ColoBox> boxes2relo = new List<ColoBox>();
            List<ColoBox> staticObjects = new List<ColoBox>();
            manager = new LayoutManager(coldWidth, spaceAfterObstacle,direction);
            loadGraphicsObject(staticObjects,manager,boxes2relo);
            


            //initialize the LayoutManager using the biggest box amoung the fixed boxes
            manager.SetBoundingBox(staticObjects);

            boxes2relo = manager.OrganizeColoBoxes(boxes2relo);

            PopulateDXF(boxes2relo, dxf);

            


        }

        private void loadGraphicsObject(List<ColoBox> staticObjects, LayoutManager manager, List<ColoBox> boxes2relo)
        {
            foreach (ColoBox box in boxes)
            {
                if (staticLayers.Contains(box.Layer))// == "wals")
                {
                    //manager = new LayoutManager(box);
                    staticObjects.Add(box);
                    continue;
                }

                if (box.Layer == "racks")
                {
                    ColoBox cloned = box.Clone();
                    if (manager != null)
                        cloned.LayoutManager = manager;

                    boxes2relo.Add(cloned);
                }
            }
        }

        private void PopulateDXF(List<ColoBox> boxes2relo, DxfDocument dxf)
        {
            //creating the result layer
            Layer layer = new Layer("MyLayer")
            {
                Color = AciColor.Red,
                LineType = LineType.Continuous
            };

            foreach (ColoBox box in boxes2relo)
            {
                LwPolyline poly = new LwPolyline();
                poly.Layer = layer;
                foreach (Point v in box.Points)
                {
                    poly.Vertexes.Add(new LwPolylineVertex(v.X, v.Y));
                }
                poly.IsClosed = true;
                dxf.AddEntity(poly);
            }
        }
            
    }
}
