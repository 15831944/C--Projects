using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ColoEngine.Model.BorderControl;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using System.Xml.Linq;
using System.Xml.XPath;
using ColoEngine.Model.DirectionControl;
using System.Collections.ObjectModel;
using netDxf.Blocks;
using ColoEngine.Model.Init;
using System.IO;

namespace ColoEngine.Model
{
    public class ColoOptimizer
    {
        #region events
        public delegate void LocationValidDelegate(string list, int boxId,bool isValid);        
        public event LocationValidDelegate BoxLookAhead;

        public delegate void PlacementDoneDelegate(object o);
        public event PlacementDoneDelegate PlacementDone;        
        #endregion events

        private string fileName;
        private List<ColoBox> boxes;        
        private List<string> staticLayers;                
        private DxfDocument dxf;
        private InitParams initParams;
        private DirectionBase direction;
        private LayoutManager manager;
        public List<ColoBox> Boxes2relo;
        private List<ColoBox> staticObjects;
        private string universeLayer;
        public double Factor { get { return manager.DirectionController.factor.Value; } }
        public List<string> LayerNames { get { return dxf.Layers.Select(l => l.Name).ToList(); } }
        public ColoOptimizer(string fileName)
        {
            boxes = new List<ColoBox>();
            try
            {
                dxf = DxfDocument.Load(fileName);
            }
            catch (Exception)
            {

            }            
        }
        public ColoOptimizer(string fileName, InitParams init, ProjectClass project = null)
        {
            
            this.fileName = fileName;            
            this.initParams = init;
                        
            RawBorder.Tolerance = 0.5;//init.Tolerance;
            if (!init.UseExistingData)
            {
                try
                {
                    dxf = DxfDocument.Load(fileName);
                }
                catch (Exception)
                {

                }


                staticLayers = init.StaticLayers.Aggregate(new List<string>(), (lst, entry) => { lst.Add(entry.Name); if (entry.IsUniverse) universeLayer = entry.Name; return lst; });
                boxes = new List<ColoBox>();
                loadPolylinesToBoxes(project);
            }
            else
            {
                //string sJson = File.ReadAllText(init.DataFile);
                Boxes2relo = new List<ColoBox>();
                staticObjects = new List<ColoBox>();
                manager = new LayoutManager(this.initParams);
                //loading all the Boxes2relo                            
                for (int i = 0; i < project.Racks.Length; ++i)
                {
                    Point p1 = new Point { X = 0, Y = 0 };
                    Point p2 = new Point { };
                    for (int j = 0; j < project.Racks[i].Quantity; ++j)
                    {
                        ColoBox box = new ColoBox("racks", i * j);
                        box.LayoutManager = manager;
                        double maxX = project.Racks[i].Width;
                        double maxY = project.Racks[i].Height;
                        box.AddPoint(0, 0);
                        box.AddPoint(maxX, 0);
                        box.AddPoint(maxX, maxY);
                        box.AddPoint(0, maxY);
                        
                        Boxes2relo.Add(box);
                    }
                }

                //loading the universe
                ColoBox universeBox = new ColoBox();
                universeBox.LayoutManager = manager;
                for (int i = 0; i < project.UniverseNodes.Length; ++i)
                {
                    universeBox.AddPoint(project.UniverseNodes[i].X, project.UniverseNodes[i].Y);                    
                }
                staticObjects.Add(universeBox);


                //loading the obstacles
                ColoBox obst = null;
                for (int i = 0; i < project.Obstacles.Length; ++i)
                {
                    obst = new ColoBox();
                    obst.Layer = "pillar";
                    obst.LayoutManager = manager;
                    for (int j = 0; j < project.Obstacles[i].Points.Length; ++j)
                    {
                        obst.AddPoint(project.Obstacles[i].Points[j].X, project.Obstacles[i].Points[j].Y);
                    }
                    staticObjects.Add(obst);
                }
                
            }
        }

        public object Optimize()
        {                                    
                                   
            //CustomHatchPattern(dxf,layer);
            RelocateRacks(dxf);

            return dxf;
        }

        public void SetBoundingBox(bool isCandidate = true)
        {
            manager.LocationMachine = new LocationStatemachine.Machine(this.initParams, new ColoBox());
            manager.DirectionController = new DirectionL2R();
            manager.LocationMachine.direction = manager.DirectionController;
            manager.SetBoundingBox(staticObjects, isCandidate);
        }
        private void loadPolylinesToBoxes(ProjectClass project)
        {
            int id = 0;
            HashSet<string> layerNames = new HashSet<string>();
            List<LwPolyline> polylines = new List<LwPolyline>();


            List<Line> lines = new List<Line>();
            lines.AddRange(dxf.Lines);
            lines.AddRange(getAllBlockLines(dxf.Blocks));
            foreach (var line in lines)
            {
                if (!layerNames.Contains(line.Layer.Name))
                {
                    layerNames.Add(line.Layer.Name);
                }
                ColoBox box = new ColoBox(line.Layer.Name, id++);
                boxes.Add(box);
                box.AddPoint(line.StartPoint.X, line.StartPoint.Y);
                box.AddPoint(line.EndPoint.X, line.EndPoint.Y);
            }


            polylines.AddRange(dxf.LwPolylines);
            polylines.AddRange(getAllPolylines(dxf.Inserts));
            polylines.AddRange(getAllPolylinesFromBlocks(dxf.Blocks));

            

            //every Polyline from DXF turns into a box (ColoBox)
            foreach (LwPolyline pl in polylines)
            {
                if (!layerNames.Contains(pl.Layer.Name))
                {
                    layerNames.Add(pl.Layer.Name);
                }
                if (pl.Layer.Name == "Layer 4")
                { }
                ColoBox box = new ColoBox(pl.Layer.Name,id++);
                boxes.Add(box);
                foreach (var v in pl.Vertexes)
                {
                    box.AddPoint(v.Location.X, v.Location.Y);
                }
            }

            foreach (Spline sp in dxf.Splines)
            {
                if (!layerNames.Contains(sp.Layer.Name))
                {
                    layerNames.Add(sp.Layer.Name);
                }
                if (sp.Layer.Name == "Layer 4")
                { }
                ColoBox box = new ColoBox(sp.Layer.Name,id++);
                boxes.Add(box);
                foreach (var v in sp.ControlPoints)
                {
                    box.AddPoint(v.Location.X, v.Location.Y);
                }
            }

            Boxes2relo = new List<ColoBox>();
            staticObjects = new List<ColoBox>();
            manager = new LayoutManager(this.initParams);

            

            loadGraphicsObject(staticObjects, manager, Boxes2relo);
            if (initParams.Stage == 1 && project != null)
            {
                ColoBox bounding = new ColoBox ();
                double minX = project.BoundingBox[0].X;
                double minY = project.BoundingBox[0].Y;
                double maxX = project.BoundingBox[1].X;
                double maxY = project .BoundingBox [1].Y;
                bounding.AddPoint(minX,minY);
                bounding.AddPoint(minX, maxY);
                bounding.AddPoint(maxX, maxY );
                bounding.AddPoint(maxX, minY);
                staticObjects.Add(bounding);
            }

        }

        private IEnumerable<Line> getAllBlockLines(ReadOnlyCollection<Block> blocks)
        {
            List<Line> lines = new List<Line>();
            foreach (Block b in blocks)
            {
                foreach (var e in b.Entities)
                {
                    if (e.GetType() == typeof(Line))
                    {
                        lines.Add((Line)e);
                    }
                }
            }

            return lines;
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
            //List<ColoBox> boxes2relo = new List<ColoBox>();
            //List<ColoBox> staticObjects = new List<ColoBox>();
            //manager = new LayoutManager(coldWidth, spaceAfterObstacle,direction);
            //loadGraphicsObject(staticObjects,manager,boxes2relo);
            


            //initialize the LayoutManager using the biggest box amoung the fixed boxes
            manager.SetBoundingBox(staticObjects);

            Boxes2relo = manager.OrganizeColoBoxes(Boxes2relo,this);

            //signal to subsribers that placement is done
            OnPlacementIsDone(null);

            if (dxf != null)
            {
                PopulateDXF(Boxes2relo, dxf);
            }
            


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


        public string GetUniverse()
        {
           ColoBox universe = staticObjects.First(s => s.Layer == universeLayer);
           return universe.PointsCollection;// 
        }

        public string GetUniverseCandidate()
        {
            List<ColoBox> lines = staticObjects.Where(s => s.Layer == universeLayer).ToList();

            ColoBox universe = new ColoBox();
            double minX, minY, maxX, maxY;
            minX = minY = double.MaxValue;
            maxX = maxY = double.MinValue;
            foreach (ColoBox line in lines)
            {
                if (line.MinX < minX)
                    minX = line.MinX;

                if (line.MinY < minY)
                    minY = line.MinY;

                if (line.MaxX > maxX)
                    maxX = line.MaxX;

                if (line.MaxY > maxY)
                    maxY = line.MaxY;

                Point point1 = new Point (){ X = minX,Y = minY};
                Point point2 = new Point (){ X = maxX,Y = maxY};
                universe.Borders.Add(new RawBorder(point1, point2));
            }
            
            universe.AddPoint(minX, minY);
            universe.AddPoint(maxX,maxY);
            return universe.PointsCollection;
        }
        
        internal void OnBoxLookAhead(string list, int boxIndex,bool isValidLocation)
        {
            if (BoxLookAhead != null)
                BoxLookAhead(list, boxIndex, isValidLocation);
        }

        internal void OnPlacementIsDone(object o)
        {
            if (PlacementDone != null)
                PlacementDone(null);
        }

        public string[] GetObstaclesPoints()
        {
            string[] ret = staticObjects.Where(s => s.Layer != universeLayer).Aggregate(new List<string>(), (lst, box) =>
            {
                lst.Add(box.PointsCollection);
                return lst;
            }).ToArray();

            return ret;
        }
        public string[] GetObstaclesPointsAsUniverseCandidate()
        {
            string[] ret = staticObjects.Where(s => s.Layer == universeLayer).Aggregate(new List<string>(), (lst, box) =>
            {
                lst.Add(box.BorderCollection);                
                return lst;
            }).ToArray();

            return ret;
        }

        public PointDictionary[] GetObstaclesDictionary()
        {
            // this.po
            List<PointDictionary> ret = staticObjects.Where(s => s.Layer == universeLayer).Aggregate(new List<PointDictionary>(), (lst, box) =>
                    {
                        foreach (KeyValuePair<String, Point> kvp in box.OriginalPoints)
                        {

                            lst.Add(new PointDictionary { Key = kvp.Key, Value = string.Format("{0:0.00},{1:0.00}", kvp.Value.X, kvp.Value.Y) });

                        }
                        return lst;
                    });
            return ret.ToArray();
        }

        public int IndexToStop {
            get { return initParams.IndexToStop; }
        }




        public void AddUniverse(ColoBox universe)
        {
            staticObjects.Add(universe);
        }

        public Point[] getBoudingBox()
        {
            List<Point> points = new List<Point>();
            points.Add(new Point { X = manager.BoundingBox.MinX, Y = manager.BoundingBox.MinY});
            points.Add(new Point { X = manager.BoundingBox.MaxX, Y = manager.BoundingBox.MaxY });

            return points.ToArray();
        }
    }

    public class PointDictionary
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
