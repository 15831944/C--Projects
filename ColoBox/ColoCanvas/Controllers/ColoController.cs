using ColoEngine.Model.Init;
using ColoEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.IO;
using System.Web.Script.Serialization;

namespace ColoCanvas.Controllers
{
    public class ColoController : Controller
    {
        private const string OPTIMIZER = "OPTIMIZER";
        private ColoOptimizer optimizer = null;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Spa()
        {                       
            return View();
        }
        public ActionResult Universe()
        {
            return View();
        }

        public ActionResult GetDxfFiles()
        {
            string path = Server.MapPath("/Api");
            DirectoryInfo di = new DirectoryInfo(path);
            List<object> files = new List<object>();
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.FullName.ToLower().EndsWith(".dxf"))
                {
                    files.Add(new { Name = fi.Name, Id = fi.Name });
                }
            }

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    files = files
                }
            };
        }

        public ActionResult GetLayers(string fileName)
        {
            InitParams ip = new InitParams() { FileName = fileName.Replace(".dxf","") };
            string fullFileName = getFileName(ip);
            optimizer = new ColoOptimizer(fullFileName);
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    layers = optimizer.LayerNames
                }
            };
        }
        public ActionResult GetResults(InitParams init)
        {
            //string fileName = System.Web.HttpContext.Current.Server.MapPath(@"..\api\Ken-Tough-Room-Vertical-v2.dxf");
            //string fileName = System.Web.HttpContext.Current.Server.MapPath(@"..\api\Ken-Crazy-Colo.dxf");
            string fileName = getFileName(init); System.Web.HttpContext.Current.Server.MapPath(string.Format(@"..\api\{0}.dxf", init.FileName));

                        
            ProjectClass project = getProject();

            createOptimizer(fileName, init,project);
            
            //start the operation that raises events which communicate back with the client
            optimizer.Optimize();


            JsonResult js = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    universePoints = optimizer.GetUniverse(),
                    floor = project.Floor,
                    obstaclePointArr = optimizer.GetObstaclesPoints(),
                    boxArr = optimizer.Boxes2relo.Select(b => new { points = b.PointsCollection, boxId = b.BoxId, shadowArr = b.ShadowPath.ToArray() }).ToArray()
                }
            };


            return js;

        }
        public ActionResult ApplyObstacles(ProjectClass definedSpace)
        {
            
            ProjectClass project = getProject();
            List<Obstacle> obsLst = new List<Obstacle>();
            foreach (Obstacle o in definedSpace.Obstacles)
            {
                List<Point > lst =new List<Point> ();
                foreach (Point p in o.Points)
                {
                    lst.Add(new Point { X = p.X / definedSpace.Factor, Y = -1 * (p.Y / definedSpace.Factor) });
                }
                obsLst.Add(new Obstacle { Id = o.Id, Points = lst.ToArray() });
            }


            project.Obstacles = obsLst.ToArray();
            saveProject(project);
            return null;
        }
        public ActionResult GetUniverse(InitParams init)
        {
            string fileName = getFileName(init);// System.Web.HttpContext.Current.Server.MapPath(string.Format(@"..\api\{0}.dxf", init.FileName));
            createOptimizer(fileName, init);
            //this stage means that we already have the 
            bool isCandidate = true;
            ColoBox universe = null;
            //if (init.Stage == 1)
            //{
            //    ProjectClass project = getProject();
            //    universe = new ColoBox();
            //    universe.Layer = init.StaticLayers.Where(l => l.IsUniverse).First ().Name;
            //    foreach (Point p in project.UniverseNodes)
            //    {
            //        universe.AddPoint(p.X, p.Y);
            //    }
               
            //    optimizer.AddUniverse(universe);
            //    isCandidate = false;
            //}
            optimizer.SetBoundingBox(isCandidate);

            //saving the bounding box into the project
            if (init.Stage == 0)
            {
                ProjectClass project = getProject();                
                project.BoundingBox = optimizer.getBoudingBox();
                saveProject(project);
            }
            if (universe != null)
            {
                var pc = universe.PointsCollection;
            }
            //string[] points = optimizer.GetObstaclesPointsAsUniverseCandidate();
            //ProjectClass prj = getProject();
            //prj.Floor = points;
            //saveProject(prj);
            

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {                    
                    obstaclePointArr = optimizer.GetObstaclesPointsAsUniverseCandidate(),
                    factor = optimizer.Factor                    
                }
            };
        }

       

        public ActionResult UploadSelectedUniversePoints(ProjectClass definedSpace)
        {
            

            //applying the factor
            List<Point> lst = new List<Point>();
            foreach(Point p in definedSpace.UniverseNodes)
            {
                lst.Add(new Point { X = p.X / definedSpace.Factor, Y = -1 * (p.Y / definedSpace.Factor) });
            }

            ProjectClass prj = getProject();
            prj.UniverseNodes = lst.ToArray();
            saveProject(prj);

            
            
            return null;
        }
        #region private methods
        private ProjectClass getProject()
        {            
            System.Web.Script.Serialization.JavaScriptSerializer jss = new JavaScriptSerializer();
            var path = Server.MapPath("/Projects");

            string sJson = System.IO.File.ReadAllText(Path.Combine(path, "data.json"));
            return (ProjectClass)jss.Deserialize(sJson, typeof(ProjectClass));
        }

        private void saveProject(ProjectClass project)
        {
            var jss = new JavaScriptSerializer();
            var sJson = jss.Serialize(project);
            var path = Server.MapPath("/Projects");
            System.IO.File.WriteAllText(Path.Combine(path, "data.json"), sJson);
        }
        private void createOptimizer(string fileName, InitParams init, ProjectClass project = null)
        {
            //if (Session[OPTIMIZER] == null)
            //{
            if (init.Stage == 1)
            {
                project = getProject();
            }
                optimizer = new ColoOptimizer(fileName, init,project);
                Session[OPTIMIZER] = optimizer;
           // }
           // else
           // {
           //     optimizer = Session[OPTIMIZER] as ColoOptimizer;
           // }
        }

        private string getFileName(InitParams init)
        {
            return  System.Web.HttpContext.Current.Server.MapPath(string.Format(@"..\api\{0}.dxf",init.FileName));
        }
        #endregion
    }
}
